using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Wikiled.Server.Core.ActionFilters;
using Wikiled.Server.Core.Helpers;
using Wikiled.Text.Parser.Readers;
using Wikiled.Text.Parser.Service.Logic;

namespace Wikiled.Text.Parser.Service.Controllers
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(RequestValidationAttribute))]
    public class ParserController : Controller
    {
        private static readonly object syncRoot = new object();

        private readonly ILogger<ParserController> logger;

        private readonly IIpResolve resolve;

        private readonly ITextParserFactory parserFactory;

        private readonly IHostingEnvironment hostingEnvironment;

        private readonly DocumentsConfig config;

        public ParserController(ILogger<ParserController> logger, IIpResolve resolve, ITextParserFactory parser, DocumentsConfig config, IHostingEnvironment hostingEnvironment)
        {
            this.resolve = resolve ?? throw new ArgumentNullException(nameof(resolve));
            parserFactory = parser ?? throw new ArgumentNullException(nameof(parser));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Route("version")]
        [HttpGet]
        public string ServerVersion()
        {
            var version = $"Version: [{Assembly.GetExecutingAssembly().GetName().Version}]";
            logger.LogInformation("Version request: {0}", version);
            return version;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("processfile")]
        public ActionResult<string> ProcessFile(IFormFile file)
        {
            if (file.Length > 0)
            {
                string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Value;
                string fullPath = GetFileName(fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var parser = parserFactory.ConstructParsers(new FileInfo(fullPath));
                var result = parser.Parse();
                return Ok(result);
            }

            return StatusCode(500, "Failed processing");
        }

        private string GetFileName(string name)
        {
            var path = config.Save;
            if (!Path.IsPathRooted(path))
            {
                string webRootPath = hostingEnvironment.ContentRootPath;
                path = Path.Combine(webRootPath, path);
            }

            path = Path.Combine(path, DateTime.Today.ToString("MMddyyyy"));
            if (!Directory.Exists(path))
            {
                lock (syncRoot)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
            }

            return Path.Combine(path, name);
        }
    }
}
