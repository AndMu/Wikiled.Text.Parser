using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Pdf;
using Wikiled.Text.Analysis.Structure.Raw;
using Wikiled.Text.Parser.Ocr;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class DevExpressOcrParser : ITextParser
    {
        private readonly int maxPages;

        private readonly FileInfo file;

        private readonly IOcrImageParser ocrImageParser;

        public DevExpressOcrParser(IOcrImageParser ocrImageParser, FileInfo file, int maxPages)
        {
            if (maxPages <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxPages));
            }

            this.maxPages = maxPages;
            this.ocrImageParser = ocrImageParser ?? throw new ArgumentNullException(nameof(ocrImageParser));
            this.file = file ?? throw new ArgumentNullException(nameof(file));
        }

        public Task<RawDocument> Parse()
        {
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
                        documentProcessor.CreateTiff(memory, 1024, new []{i});
                        var data = memory.ToArray();
                        page.Blocks[0].Text = ocrImageParser.Parse(data);
                    }

                    document.Pages[i - 1] = page;
                }
            }

            return Task.FromResult(document);
        }
    }
}
