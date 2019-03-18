using System.IO;
using System.Threading.Tasks;
using Wikiled.Text.Parser.Data;

namespace Wikiled.Text.Parser.Readers
{
    public interface ITextParser
    {
        Task<ParsingResult> Parse(FileInfo file, int maxPages);
    }
}
