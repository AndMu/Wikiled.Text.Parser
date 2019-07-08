using System;
using Wikiled.Text.Analysis.Structure.Raw;

namespace Wikiled.Text.Parser.Data
{
    public class ParsingResult
    {
        public ParsingResult(RawDocument document, ParsingRequest request, ParsingType? processedAs)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
            Request = request ?? throw new ArgumentNullException(nameof(request));
            ProcessedAs = processedAs;
        }

        public RawDocument Document { get; }

        public ParsingRequest Request { get; }

        public ParsingType? ProcessedAs { get; }

        public bool Succeeded => ProcessedAs != null;

        public static ParsingResult ConstructError(ParsingRequest request)
        {
            return new ParsingResult(new RawDocument {Pages = new RawPage[0]}, request, null);
        }
    }
}
