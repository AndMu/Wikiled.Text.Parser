using System;
using System.IO;
using System.Text;
using DevExpress.Pdf;
using Wikiled.Text.Parser.Result;

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

        public DocumentResult Parse()
        {
            DocumentResult document = new DocumentResult();
            using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
            {
                documentProcessor.LoadDocument(file.FullName);
                int pages = maxPages > documentProcessor.Document.Pages.Count ? documentProcessor.Document.Pages.Count : maxPages;
                document.Pages = new PageItem[pages];
                for (int i = 1; i <= pages; i++)
                {
                    PageItem page = new PageItem();
                    page.Blocks = new[] {new TextBlockItem()};
                    page.Blocks[0].Text = documentProcessor.GetPageText(i);
                    document.Pages[i - 1] = page;
                }
            }

            return document;
        }
    }
}
