using System.Collections.Generic;
using Wikiled.Text.Analysis.Structure.Raw;

namespace Wikiled.Text.Parser.Ocr
{
    public interface IOcrImageParser
    {
        IEnumerable<TextBlockItem> Parse(byte[] data);
    }
}