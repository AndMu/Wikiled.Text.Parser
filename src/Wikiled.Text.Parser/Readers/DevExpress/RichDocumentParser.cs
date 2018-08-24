using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Layout;
using DevExpress.XtraRichEdit.API.Native;
using Wikiled.Text.Analysis.Structure.Raw;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class RichDocumentParser : ITextParser
    {
        private readonly FileInfo file;

        public RichDocumentParser(FileInfo file)
        {
            this.file = file ?? throw new ArgumentNullException(nameof(file));
        }

        public async Task<RawDocument> Parse()
        {
            using (var documentProcessor = new RichEditDocumentServer())
            {
                documentProcessor.LayoutCalculationMode = CalculationModeType.Automatic;
                documentProcessor.LayoutUnit = DocumentLayoutUnit.Document;
                Section vSection = documentProcessor.Document.Sections[0];

                //vSection.Page.Landscape = false;
                //vSection.Page.PaperKind = System.Drawing.Printing.PaperKind.A4;
                //vSection.Margins.Left = 15;
                //vSection.Margins.Top = 15;
                //vSection.Margins.Right = 15;
                //vSection.Margins.Bottom = 15;

                var loaded = Observable.FromEventPattern<EventHandler, EventArgs>(
                        h => documentProcessor.DocumentLayout.DocumentFormatted += h,
                        h => documentProcessor.DocumentLayout.DocumentFormatted -= h)
                    .FirstOrDefaultAsync()
                    .GetAwaiter();

                documentProcessor.LoadDocument(file.FullName);
                await loaded;
                
                DocumentIterator iterator = new DocumentIterator(documentProcessor.Document);
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

                return visitor.GenerateResult();
            }
        }
    }
}
