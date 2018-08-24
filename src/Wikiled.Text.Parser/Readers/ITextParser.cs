using System.Threading.Tasks;
using Wikiled.Text.Analysis.Structure.Raw;

namespace Wikiled.Text.Parser.Readers
{
    public interface ITextParser
    {
        Task<RawDocument> Parse();
    }
}
