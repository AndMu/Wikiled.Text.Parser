using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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

        public ParsingType Type => ParsingType.Any;

        public Task<ParsingResult> Parse(ParsingRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            logger.LogDebug("Parsing [{0}]", request.File.FullName);
            var sourceImage = Image.FromFile(request.File.FullName);
            using (var byteStream = new MemoryStream())
            {
                sourceImage.Save(byteStream, ImageFormat.Tiff);
                var data = byteStream.ToArray();
                var document = new RawDocument();
                document.Pages = new[] {new RawPage()};
                document.Pages[0].Blocks = ocrImageParser.Parse(data).Take(request.MaxPages).ToArray();
                return Task.FromResult(new ParsingResult(document, request, ParsingType.OCR));
            }
        }
    }
}
