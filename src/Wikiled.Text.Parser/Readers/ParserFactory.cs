using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Wikiled.Text.Parser.Ocr;
using Wikiled.Text.Parser.Readers.DevExpress;
using Wikiled.Text.Parser.Readers.Other;

namespace Wikiled.Text.Parser.Readers
{
    public class ParserFactory : ITextParserFactory
    {
        private readonly ILoggerFactory loggerFactory;

        private readonly ILogger<ParserFactory> logger;

        public ParserFactory(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger < ParserFactory>();
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public string [] Supported { get; } = { "pdf", "doc", "docx", "rtf", "txt", "tif", "png", "jpg", "bmp", "gif" };

        public ITextParser ConstructParsers(FileInfo file)
        {
            logger.LogDebug("ConstructParsers {0}", file);
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (string.Compare(file.Extension, ".pdf", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return new CombinedParser(
                    new DevExpressPdfParser(loggerFactory.CreateLogger<DevExpressPdfParser>()),
                    new DevExpressPdfOcrParser(loggerFactory.CreateLogger<DevExpressPdfOcrParser>(), new OcrImageParser()));
            }

            if (string.Compare(file.Extension, ".doc", StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(file.Extension, ".docx", StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(file.Extension, ".rtf", StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(file.Extension, ".txt", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return new RichDocumentParser(loggerFactory.CreateLogger<RichDocumentParser>());
            }

            if (string.Compare(file.Extension, ".gif", StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(file.Extension, ".tif", StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(file.Extension, ".png", StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(file.Extension, ".jpg", StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(file.Extension, ".bmp", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return new ImageTextParser(loggerFactory.CreateLogger<ImageTextParser>(), new OcrImageParser());
            }

            return NullTextParser.Instance;
        }
    }
}
