using System.Threading.Tasks;
using Wikiled.Text.Parser.Data;

namespace Wikiled.Text.Parser.Readers
{
    public interface ITextParser
    {
        Task<ParsingResult> Parse(ParsingRequest request);

        ParsingType Type { get; }
    }
}
