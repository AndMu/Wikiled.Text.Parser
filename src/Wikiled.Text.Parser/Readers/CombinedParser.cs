using System;
using System.Linq;
using System.Threading.Tasks;
using Wikiled.Text.Parser.Data;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class CombinedParser : ITextParser
    {
        private readonly ITextParser[] inner;

        public CombinedParser(params ITextParser[] inner)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public ParsingType Type => ParsingType.Any;

        public async Task<ParsingResult> Parse(ParsingRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            foreach (var parser in inner.Where(item => item.Type == request.Type || request.Type == ParsingType.Any))
            {
                var result = await parser.Parse(request).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return result;
                }
            }

            return ParsingResult.ConstructError(request);
        }
    }
}
