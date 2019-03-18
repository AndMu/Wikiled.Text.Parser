using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wikiled.Text.Parser.Data;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class DevExpressCombinedParser : ITextParser
    {
        private readonly ITextParser[] inner;

        public DevExpressCombinedParser(params ITextParser[] inner)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public async Task<ParsingResult> Parse(FileInfo file, int maxPages)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (maxPages <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxPages));
            }

            foreach (var parser in inner)
            {
                var result = await parser.Parse(file, maxPages).ConfigureAwait(false);
                if (result.Type != ParsingType.Failed)
                {
                    return result;
                }
            }

            return ParsingResult.Error;
        }
    }
}
