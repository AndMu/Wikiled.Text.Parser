using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Layout;
using DevExpress.XtraRichEdit.API.Native;
using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
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

        public ParsingType Type => ParsingType.Extract;

        public async Task<ParsingResult> Parse(ParsingRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            logger.LogDebug("Parsing [{0}]", request.File.FullName);
            using (var documentProcessor = new RichEditDocumentServer())
            {
                documentProcessor.LayoutCalculationMode = CalculationModeType.Automatic;
                documentProcessor.LayoutUnit = DocumentLayoutUnit.Document;
                var loaded = Observable.FromEventPattern<EventHandler, EventArgs>(
                        h => documentProcessor.DocumentLayout.DocumentFormatted += h,
                        h => documentProcessor.DocumentLayout.DocumentFormatted -= h)
                    .FirstOrDefaultAsync()
                    .GetAwaiter();

                documentProcessor.LoadDocument(request.File.FullName);
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

                return new ParsingResult(visitor.GenerateResult(request.MaxPages), request, ParsingType.Extract);
            }
        }
    }
}
