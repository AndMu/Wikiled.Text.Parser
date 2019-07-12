using System.Drawing;
using System.Drawing.Imaging;

namespace Wikiled.Text.Parser.Helpers
{
    public static class ImageExtensions
    {
        public static Image GetBlackAndWhiteImage(this Image image, float threshold = 0.5f)
        {
            var result = new Bitmap(image.Width, image.Height);

            var grayMatrix = new ColorMatrix(
                new[]
                {
                    new float[] { 0.299f, 0.299f, 0.299f, 0, 0  },
                    new float[] { 0.587f, 0.587f, 0.587f, 0, 0  },
                    new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1}
                });

            using (var g = Graphics.FromImage(result))
            {
                using (var ia = new ImageAttributes())
                {

                    ia.SetColorMatrix(grayMatrix);
                    ia.SetThreshold(threshold);
                    g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia);
                }
            }

            return result;
        }
    }
}
