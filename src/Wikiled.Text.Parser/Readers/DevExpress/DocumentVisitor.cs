using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraRichEdit.API.Native;
using Wikiled.Text.Parser.Result;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class DocumentVisitor : DocumentVisitorBase
    {
        private CurrentLayoutVisitor layoutVisitor;

        private readonly List<PageItem> pages = new List<PageItem>();

        private readonly List<TrackingBlockItem> blocks = new List<TrackingBlockItem>();

        private PageItem activePage;

        private TrackingBlockItem activeBlock;

        public DocumentVisitor(CurrentLayoutVisitor layoutVisitor)
        {
            this.layoutVisitor = layoutVisitor;
            VisitPage();
        }

        public DocumentResult GenerateResult()
        {
            ExtractBlocks();
            var result = new DocumentResult();
            result.Pages = pages.ToArray();
            return result;
        }

        private void VisitPage()
        {
            ExtractBlocks();
            blocks.Clear();
            activePage = new PageItem();
            pages.Add(activePage);
        }

        private void ExtractBlocks()
        {
            if (activePage != null)
            {
                activePage.Blocks = blocks.Select(item => item.Construct()).Where(item => item != null).ToArray();
                blocks.Clear();
                activePage = null;
            }
        }

        public override void Visit(DocumentText text)
        {
            if (activeBlock == null)
            {
                throw new InvalidOperationException("No active block");
            }

            var page = layoutVisitor.GetPage(text.Position);
            if (page >= pages.Count)
            {
                VisitPage();
            }

            activeBlock.AddText(text.Text);
        }

        public override void Visit(DocumentParagraphStart paragraphStart)
        {
            activeBlock = new TrackingBlockItem();
            blocks.Add(activeBlock);
            base.Visit(paragraphStart);
        }
    }
}
