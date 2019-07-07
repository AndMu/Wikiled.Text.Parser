﻿using System;
using System.Collections.Generic;
using System.Text;
using Tesseract;
using Wikiled.Text.Analysis.Structure.Raw;

namespace Wikiled.Text.Parser.Ocr
{
    public class OcrImageParser : IOcrImageParser
    {
        private readonly string location;

        public OcrImageParser(string location = @"./tessdata")
        {
            this.location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public IEnumerable<TextBlockItem> Parse(byte[] data)
        {
            using (var engine = new TesseractEngine(location, "eng", EngineMode.Default))
            using (var pix = Pix.LoadTiffFromMemory(data))
            using (var page = engine.Process(pix))
            using (var iter = page.GetIterator())
            {
                iter.Begin();
                do
                {
                    foreach (var blockItem in ExtractPage(iter))
                    {
                        yield return blockItem;
                    }
                }
                while (iter.Next(PageIteratorLevel.Block));
            }
        }

        private static IEnumerable<TextBlockItem> ExtractPage(ResultIterator iter)
        {
            do
            {
                yield return ExtractParaphraph(iter);
            }
            while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
        }

        private static TextBlockItem ExtractParaphraph(ResultIterator iter)
        {
            var text = new StringBuilder();
            do
            {
                do
                {
                    text.Append(iter.GetText(PageIteratorLevel.Word));
                    text.Append(" ");

                    if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                    {
                        text.Append(Environment.NewLine);
                    }
                }
                while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
                {
                    text.Append(Environment.NewLine);
                }
            }
            while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));

            return new TextBlockItem
                   {
                       Text = text.ToString()
                   };
        }
    }
}
