using DevExpress.Pdf;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure.Raw;
using Wikiled.Text.Parser.Data;
using Wikiled.Text.Parser.Ocr;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class DevExpressPdfOcrParser : ITextParser
    {
        private readonly ILogger<DevExpressPdfOcrParser> logger;

        private readonly IOcrImageParser ocrImageParser;

        public DevExpressPdfOcrParser(ILogger<DevExpressPdfOcrParser> logger, IOcrImageParser ocrImageParser)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.ocrImageParser = ocrImageParser ?? throw new ArgumentNullException(nameof(ocrImageParser));
        }

        public ParsingType Type => ParsingType.OCR;

        public Task<ParsingResult> Parse(ParsingRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            logger.LogDebug("Parsing [{0}]", request.File.FullName);
            var document = new RawDocument();
            using (var documentProcessor = new PdfDocumentProcessor())
            {
                documentProcessor.LoadDocument(request.File.FullName);
                var pages = request.MaxPages > documentProcessor.Document.Pages.Count ? documentProcessor.Document.Pages.Count : request.MaxPages;
                var pagesList = new List<RawPage>();
                document.Pages = new RawPage[pages];
                for (var i = 1; i <= pages; i++)
                {
                    var page = new RawPage();

                    using (var memory = new MemoryStream())
                    {
                        documentProcessor.CreateTiff(memory, 1024 * 5, new[] { i });
#if DEBUG
                        var image = Image.FromStream(memory);
                        image.Save($"origingal_{request.File.Name}.jpeg", ImageFormat.Jpeg);
#endif
                        image = GetBlackAndWhiteImage(image);
#if DEBUG
                        image.Save($"BW_{request.File.Name}.jpeg", ImageFormat.Jpeg);
                        using (var bwStream = new MemoryStream())
                        {
                            image.Save(bwStream, ImageFormat.Tiff);
                            var data = bwStream.ToArray();
                            page.Blocks = ocrImageParser.Parse(data).ToArray();
                        }
#endif
                    }

                    document.Pages[i - 1] = page;
                }
            }

            return Task.FromResult(new ParsingResult(document, request, ParsingType.OCR));
        }

        public static Image GetBlackAndWhiteImage(Image img)
        {

            Bitmap bmp = new Bitmap(img.Width, img.Height);

            var grayMatrix = new ColorMatrix(
                new[]
                {
                    new float[] { 0.299f, 0.299f, 0.299f, 0, 0  },
                    new float[] { 0.587f, 0.587f, 0.587f, 0, 0  },
                    new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1}
                });

            using (var g = Graphics.FromImage(bmp))
            {
                using (var ia = new ImageAttributes())
                {

                    ia.SetColorMatrix(grayMatrix);
                    ia.SetThreshold(0.5f);
                    g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
                }
            }

            return bmp;

        }
    }
}
