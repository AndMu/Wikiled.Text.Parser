using System;
using System.IO;
using System.Threading.Tasks;
using DevExpress.Pdf;
using Wikiled.Text.Analysis.Structure.Raw;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class DevExpressPdfParser : ITextParser
    {
        private readonly int maxPages;

        private readonly FileInfo file;

        public DevExpressPdfParser(FileInfo file, int maxPages)
        {
            if (maxPages <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxPages));
            }

            this.maxPages = maxPages;
            this.file = file ?? throw new ArgumentNullException(nameof(file));
        }

        public Task<RawDocument> Parse()
        {
            RawDocument document = new RawDocument();
            using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
            {
                documentProcessor.LoadDocument(file.FullName);                
                int pages = maxPages > documentProcessor.Document.Pages.Count ? documentProcessor.Document.Pages.Count : maxPages;
                document.Pages = new RawPage[pages];
                for (int i = 1; i <= pages; i++)
                {
                    var page = new RawPage();
                    page.Blocks = new[] {new TextBlockItem()};
                    page.Blocks[0].Text = documentProcessor.GetPageText(i).Replace(Environment.NewLine, " ");
                    document.Pages[i - 1] = page;
                }
            }

            return Task.FromResult(document);
        }
    }
}
