using DevExpress.Pdf;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure.Raw;
using Wikiled.Text.Parser.Data;
using Wikiled.Text.Parser.Ocr;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class DevExpressPdfOcrParser : ITextParser
    {
        private readonly ILogger<DevExpressPdfOcrParser> logger;

        private readonly IOcrImageParser ocrImageParser;

        public DevExpressPdfOcrParser(ILogger<DevExpressPdfOcrParser> logger, IOcrImageParser ocrImageParser)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.ocrImageParser = ocrImageParser ?? throw new ArgumentNullException(nameof(ocrImageParser));
        }

        public ParsingType Type => ParsingType.OCR;

        public Task<ParsingResult> Parse(ParsingRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            logger.LogDebug("Parsing [{0}]", request.File.FullName);
            var document = new RawDocument();
            using (var documentProcessor = new PdfDocumentProcessor())
            {
                documentProcessor.LoadDocument(request.File.FullName);
                var pages = request.MaxPages > documentProcessor.Document.Pages.Count ? documentProcessor.Document.Pages.Count : request.MaxPages;
                var pagesList = new List<RawPage>();
                document.Pages = new RawPage[pages];
                for (var i = 1; i <= pages; i++)
                {
                    var page = new RawPage();

                    using (var memory = new MemoryStream())
                    {
                        documentProcessor.CreateTiff(memory, 1024 * 5, new []{i});
                        var data = memory.ToArray();
                        page.Blocks = ocrImageParser.Parse(data).ToArray();
                    }

                    document.Pages[i - 1] = page;
                }
            }

            return Task.FromResult(new ParsingResult(document, request, ParsingType.OCR));
        }
    }
}
