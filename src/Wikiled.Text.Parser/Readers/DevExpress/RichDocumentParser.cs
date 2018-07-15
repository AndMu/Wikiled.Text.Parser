using System;
using System.IO;
using DevExpress.XtraRichEdit;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class RichDocumentParser : ITextParser
    {
        private readonly FileInfo file;

        public RichDocumentParser(FileInfo file)
        {
            this.file = file ?? throw new ArgumentNullException(nameof(file));
        }

        public string Parse()
        {
            using (var documentProcessor = new RichEditDocumentServer())
            {
                documentProcessor.LoadDocument(file.FullName);
                return documentProcessor.Text;
            }
        }
    }
}
