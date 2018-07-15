using System.IO;

namespace Wikiled.Text.Parser.Readers
{
    public interface ITextParserFactory
    {
        ITextParser ConstructParsers(FileInfo file);

        string[] Supported { get; }
    }
}
