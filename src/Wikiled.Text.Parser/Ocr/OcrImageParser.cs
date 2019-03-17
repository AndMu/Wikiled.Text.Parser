using Tesseract;

namespace Wikiled.Text.Parser.Ocr
{
    public class OcrImageParser : IOcrImageParser
    {
        private readonly TesseractEngine engine;

        public OcrImageParser()
        {
            engine = new TesseractEngine(@"Resources", "eng", EngineMode.TesseractAndCube);
        }

        public string Parse(byte[] data)
        {
            var pix = Pix.LoadTiffFromMemory(data);
            var page  = engine.Process(pix);
            return page.GetText();
        }

        public void Dispose()
        {
            engine?.Dispose();
        }
    }
}
