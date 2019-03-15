using System;
using Tesseract;

namespace Wikiled.Text.Parser.Ocr
{
    public class OcrFileParser : IDisposable
    {
        private readonly TesseractEngine engine;

        public OcrFileParser()
        {
            engine = new TesseractEngine(@"OCR", "eng", EngineMode.TesseractAndCube);
        }

        public void Parse(byte[] data)
        {
            var pix = Pix.LoadTiffFromMemory(data);
            engine.Process(pix);
        }

        public void Dispose()
        {
            engine?.Dispose();
        }
    }
}
