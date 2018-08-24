using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure.Raw;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class NullTextParser : ITextParser
    {
        private NullTextParser()
        {}

        public static NullTextParser Instance { get; } = new NullTextParser();

        public Task<RawDocument> Parse()
        {
            return Task.FromResult(new RawDocument
            {
                Pages = new RawPage[0]
            });
        }
    }
}
