using System;
using System.IO;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class DevExpressParserFactory : ITextParserFactory
    {
        private readonly int maxPages;

        public DevExpressParserFactory(int maxPages=100)
        {
            if (maxPages <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxPages));
            }

            this.maxPages = maxPages;
        }

        public string[] Supported { get; } = { "pdf", "doc", "docx", "rtf", "txt" };

        public ITextParser ConstructParsers(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (string.Compare(file.Extension, ".pdf", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return new DevExpressPdfParser(file, maxPages);
            }

            if (string.Compare(file.Extension, ".doc", StringComparison.OrdinalIgnoreCase) == 0 ||
               string.Compare(file.Extension, ".docx", StringComparison.OrdinalIgnoreCase) == 0 ||
               string.Compare(file.Extension, ".rtf", StringComparison.OrdinalIgnoreCase) == 0 ||
               string.Compare(file.Extension, ".txt", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return new RichDocumentParser(file);
            }

            return NullTextParser.Instance;
        }
    }
}
