using Wikiled.Text.Parser.Result;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class NullTextParser : ITextParser
    {
        private NullTextParser()
        {}

        public static NullTextParser Instance { get; } = new NullTextParser();

        public DocumentResult Parse()
        {
            return new DocumentResult
                   {
                       Pages = new PageItem[0]
                   };
        }
    }
}
