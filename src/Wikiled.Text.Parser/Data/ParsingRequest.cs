using System;
using System.IO;
using Wikiled.Text.Parser.Data;

namespace Wikiled.Text.Parser.Readers
{
    public class ParsingRequest
    {
        public ParsingRequest(FileInfo file, ParsingType type, int maxPages)
        {
            if (maxPages <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxPages));
            }

            Type = type;
            MaxPages = maxPages;
            File = file ?? throw new ArgumentNullException(nameof(file));
        }

        public FileInfo File { get; }

        public ParsingType Type { get; }

        public int MaxPages { get; }
    }
}
