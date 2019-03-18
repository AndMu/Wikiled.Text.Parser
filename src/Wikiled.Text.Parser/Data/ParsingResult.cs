using Wikiled.Text.Analysis.Structure.Raw;

namespace Wikiled.Text.Parser.Data
{
    public class ParsingResult
    {
        public static readonly ParsingResult Error =
            new ParsingResult(new RawDocument { Pages = new RawPage[0] }, ParsingType.Failed);

        public ParsingResult(RawDocument document, ParsingType type)
        {
            Document = document;
            Type = type;
        }

        public RawDocument Document { get; }

        public ParsingType Type { get; }
    }
}
