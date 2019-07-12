using DevExpress.Pdf;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure.Raw;
using Wikiled.Text.Parser.Data;
using Wikiled.Text.Parser.Helpers;
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
                    var data = GetImage(request, documentProcessor, i);
                    page.Blocks = ocrImageParser.Parse(data).ToArray(); 
                    document.Pages[i - 1] = page;
                }
            }

            return Task.FromResult(new ParsingResult(document, request, ParsingType.OCR));
        }

        private byte[] GetImage(ParsingRequest request, PdfDocumentProcessor documentProcessor, int i)
        {
            using (var memory = new MemoryStream())
            {
                documentProcessor.CreateTiff(memory, 1024 * 5, new[] { i });
                using (var image = Image.FromStream(memory))
                {
#if DEBUG
                    image.Save($"{request.File.Name}.jpeg", ImageFormat.Jpeg);
#endif
                    if (request.BwThreshold == null)
                    {
                        return memory.ToArray();
                    }

                    using (var bwImage = image.GetBlackAndWhiteImage(request.BwThreshold.Value))
                    {
#if DEBUG
                        bwImage.Save($"{request.File.Name}_BW.jpeg", ImageFormat.Jpeg);
#endif
                        using (var bwStream = new MemoryStream())
                        {
                            bwImage.Save(bwStream, ImageFormat.Tiff);
                            return bwStream.ToArray();
                        }
                    }
                }
            }
        }
    }
}
