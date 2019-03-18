namespace Wikiled.Text.Parser.Ocr
{
    public interface IOcrImageParser
    {
        string Parse(byte[] data);
    }
}