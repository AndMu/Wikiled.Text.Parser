using Wikiled.Text.Parser.Result;

namespace Wikiled.Text.Parser.Readers
{
    public interface ITextParser
    {
        DocumentResult Parse();
    }
}
