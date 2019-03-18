using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure.Raw;
using Wikiled.Text.Parser.Data;
using Wikiled.Text.Parser.Ocr;

namespace Wikiled.Text.Parser.Readers.Other
{
    public class ImageTextParser : ITextParser
    {
        private readonly ILogger<ImageTextParser> logger;

        private readonly IOcrImageParser ocrImageParser;

        public ImageTextParser(ILogger<ImageTextParser> logger, IOcrImageParser ocrImageParser)
        {
            this.ocrImageParser = ocrImageParser ?? throw new ArgumentNullException(nameof(ocrImageParser));
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
            var sourceImage = Image.FromFile(file.FullName);
            using (var byteStream = new MemoryStream())
            {
                sourceImage.Save(byteStream, ImageFormat.Tiff);
                var data = byteStream.ToArray();
                var text = ocrImageParser.Parse(data);
                var document = new RawDocument();
                document.Pages = new RawPage[1];
                document.Pages[0] = new RawPage
                {
                    Blocks = new[] { new TextBlockItem() }
                };

                document.Pages[0].Blocks[0].Text = text;
                return Task.FromResult(new ParsingResult(document, ParsingType.OCR));
            }
        }
    }
}
