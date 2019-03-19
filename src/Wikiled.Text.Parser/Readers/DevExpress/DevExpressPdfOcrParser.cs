using DevExpress.Pdf;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

        public Task<ParsingResult> Parse(FileInfo file, int maxPages)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (maxPages <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxPages));
            }

            logger.LogDebug("Parsing [{0}]", file.FullName);
            var document = new RawDocument();
            using (var documentProcessor = new PdfDocumentProcessor())
            {
                documentProcessor.LoadDocument(file.FullName);
                var pages = maxPages > documentProcessor.Document.Pages.Count ? documentProcessor.Document.Pages.Count : maxPages;
                document.Pages = new RawPage[pages];
                for (var i = 1; i <= pages; i++)
                {
                    var page = new RawPage
                               {
                                   Blocks = new[] { new TextBlockItem() }
                               };

                    using (var memory = new MemoryStream())
                    {
                        documentProcessor.CreateTiff(memory, 1024 * 5, new []{i});
                        var data = memory.ToArray();
                        page.Blocks[0].Text = ocrImageParser.Parse(data);
                    }

                    document.Pages[i - 1] = page;
                }
            }

            return Task.FromResult(new ParsingResult(document, ParsingType.OCR));
        }
    }
}
