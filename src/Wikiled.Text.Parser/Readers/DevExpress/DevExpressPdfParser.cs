using System;
using System.IO;
using System.Text;
using DevExpress.Pdf;

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

        public string Parse()
        {
            StringBuilder builder = new StringBuilder();
            using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
            {
                documentProcessor.LoadDocument(file.FullName);
                int pages = maxPages > documentProcessor.Document.Pages.Count ? documentProcessor.Document.Pages.Count : maxPages;
                for (int i = 1; i <= pages; i++)
                {
                    if (i > 1)
                    {
                        builder.Append(" ");
                    }

                    builder.Append(documentProcessor.GetPageText(i));
                }
            }

            return builder.ToString();
        }
    }
}
