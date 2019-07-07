using System.IO;
using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure.Raw;
using Wikiled.Text.Parser.Data;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class NullTextParser : ITextParser
    {
        private NullTextParser()
        {}

        public ParsingType Type => ParsingType.Any;

        public static NullTextParser Instance { get; } = new NullTextParser();

        public Task<ParsingResult> Parse(ParsingRequest request)
        {
            return Task.FromResult(ParsingResult.ConstructError(request));
        }
    }
}
