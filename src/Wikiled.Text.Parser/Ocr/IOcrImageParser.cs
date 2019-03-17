using System;

namespace Wikiled.Text.Parser.Ocr
{
    public interface IOcrImageParser
        : IDisposable
    {
        string Parse(byte[] data);
    }
}