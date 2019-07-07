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

        public ParsingType Type => ParsingType.Extract;

        public Task<ParsingResult> Parse(ParsingRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            logger.LogDebug("Parsing [{0}]", request.File.FullName);
            var document = new RawDocument();
            bool containsText = false;
            using (var documentProcessor = new PdfDocumentProcessor())
            {
                documentProcessor.LoadDocument(request.File.FullName);
                var pages = request.MaxPages > documentProcessor.Document.Pages.Count ? documentProcessor.Document.Pages.Count : request.MaxPages;
                document.Pages = new RawPage[pages];
                for (var i = 1; i <= pages; i++)
                {
                    var page = new RawPage
                    {
                        Blocks = new[] { new TextBlockItem() }
                    };

                    page.Blocks[0].Text = documentProcessor.GetPageText(i);
                    if (!string.IsNullOrWhiteSpace(page.Blocks[0].Text))
                    {
                        containsText = true;
                    }

                    document.Pages[i - 1] = page;
                }
            }

            if (!containsText)
            {
                logger.LogInformation("Failed to find text in: [{0}]", request.File.FullName);
                return Task.FromResult(ParsingResult.ConstructError(request));
            }

            return Task.FromResult(new ParsingResult(document, request, ParsingType.Extract));
        }
    }
}
