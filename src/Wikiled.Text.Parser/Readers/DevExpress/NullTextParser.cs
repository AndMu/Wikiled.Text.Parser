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

        public static NullTextParser Instance { get; } = new NullTextParser();

        public Task<ParsingResult> Parse(FileInfo file, int maxPages)
        {
            return Task.FromResult(
                new ParsingResult(
                    new RawDocument { Pages = new RawPage[0] },
                    ParsingType.Failed));
        }
    }
}
