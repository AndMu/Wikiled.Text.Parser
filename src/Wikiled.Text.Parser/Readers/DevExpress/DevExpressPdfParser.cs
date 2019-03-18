using DevExpress.Pdf;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Text.Analysis.Structure.Raw;
using Wikiled.Text.Parser.Data;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class DevExpressPdfParser : ITextParser
    {
        private readonly ILogger<DevExpressPdfParser> logger;

        public DevExpressPdfParser(ILogger<DevExpressPdfParser> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            bool containsText = false;
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

                    page.Blocks[0].Text = documentProcessor.GetPageText(i).Replace(Environment.NewLine, " ");
                    if (!string.IsNullOrWhiteSpace(page.Blocks[0].Text))
                    {
                        containsText = true;
                    }

                    document.Pages[i - 1] = page;
                }
            }

            if (!containsText)
            {
                logger.LogInformation("Failed to find text in: [{0}]", file.FullName);
                return Task.FromResult(ParsingResult.Error);
            }

            return Task.FromResult(new ParsingResult(document, ParsingType.Extract));
        }
    }
}
