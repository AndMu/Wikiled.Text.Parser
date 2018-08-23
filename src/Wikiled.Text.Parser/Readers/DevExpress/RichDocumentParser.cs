using System;
using System.IO;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Layout;
using DevExpress.XtraRichEdit.API.Native;
using Wikiled.Text.Parser.Result;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class RichDocumentParser : ITextParser
    {
        private readonly FileInfo file;

        public RichDocumentParser(FileInfo file)
        {
            this.file = file ?? throw new ArgumentNullException(nameof(file));
        }

        public DocumentResult Parse()
        {
            DocumentResult documentResult = new DocumentResult();
            using (var documentProcessor = new RichEditDocumentServer())
            {
                documentProcessor.LoadDocument(file.FullName);
                DocumentIterator iterator = new DocumentIterator(documentProcessor.Document);
                var isCompleted = documentProcessor.DocumentLayout.IsDocumentFormattingCompleted;
                var total = documentProcessor.DocumentLayout.GetPageCount();

                var pageLayout = new CurrentLayoutVisitor();
                DocumentVisitor visitor = new DocumentVisitor(pageLayout);
                
                LayoutIterator layoutIterator = new LayoutIterator(documentProcessor.DocumentLayout);
                while (layoutIterator.MoveNext(LayoutLevel.Page))
                {
                    layoutIterator.Current.Accept(pageLayout);
                }
                
                while (iterator.MoveNext())
                {
                    iterator.Current.Accept(visitor);
                }

                var result = visitor.GenerateResult();
            }

            return documentResult;
        }
    }
}
