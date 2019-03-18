using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Layout;
using DevExpress.XtraRichEdit.API.Native;
using Microsoft.Extensions.Logging;
using Wikiled.Text.Analysis.Structure.Raw;
using Wikiled.Text.Parser.Data;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class RichDocumentParser : ITextParser
    {
        private readonly ILogger<RichDocumentParser> logger;

        public RichDocumentParser(ILogger<RichDocumentParser> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ParsingResult> Parse(FileInfo file, int maxPages)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (maxPages <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxPages));
            }

            logger.LogDebug("Parsing [{0}]", file.FullName);
            using (var documentProcessor = new RichEditDocumentServer())
            {
                documentProcessor.LayoutCalculationMode = CalculationModeType.Automatic;
                documentProcessor.LayoutUnit = DocumentLayoutUnit.Document;
                var loaded = Observable.FromEventPattern<EventHandler, EventArgs>(
                        h => documentProcessor.DocumentLayout.DocumentFormatted += h,
                        h => documentProcessor.DocumentLayout.DocumentFormatted -= h)
                    .FirstOrDefaultAsync()
                    .GetAwaiter();

                documentProcessor.LoadDocument(file.FullName);
                await loaded;
                
                var iterator = new DocumentIterator(documentProcessor.Document);
                var pageLayout = new CurrentLayoutVisitor();
                var visitor = new DocumentVisitor(pageLayout);
                
                var layoutIterator = new LayoutIterator(documentProcessor.DocumentLayout);
                while (layoutIterator.MoveNext(LayoutLevel.Page))
                {
                    layoutIterator.Current.Accept(pageLayout);
                }
                
                while (iterator.MoveNext())
                {
                    iterator.Current.Accept(visitor);
                }

                return new ParsingResult(visitor.GenerateResult(maxPages), ParsingType.Extract);
            }
        }
    }
}
