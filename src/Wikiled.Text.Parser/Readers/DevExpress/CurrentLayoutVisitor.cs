using DevExpress.XtraRichEdit.API.Layout;
using DevExpress.XtraRichEdit.Utils;

namespace Wikiled.Text.Parser.Readers.DevExpress
{
    public class CurrentLayoutVisitor : LayoutVisitor
    {
        private readonly SortedList<int> startPositions = new SortedList<int>();

        protected override void VisitPage(LayoutPage page)
        {
            base.VisitPage(page);
            startPositions.Add(page.MainContentRange.Start);
        }

        public int GetPage(int position)
        {
            for (var i = 0; i < startPositions.Count; i++)
            {
                if (startPositions[i] > position)
                {
                    return i - 1;
                }
            }

            return startPositions.Count - 1;
        }
    }
}
