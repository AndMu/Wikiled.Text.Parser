using Tesseract;

namespace Wikiled.Text.Parser.Ocr
{
    public class OcrImageParser : IOcrImageParser
    {
        public string Parse(byte[] data)
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            using (var pix = Pix.LoadTiffFromMemory(data))
            using (var page = engine.Process(pix))
            {
                return page.GetText();
            }
        }
    }
}
