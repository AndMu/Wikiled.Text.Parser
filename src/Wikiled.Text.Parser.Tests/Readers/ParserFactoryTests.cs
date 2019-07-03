using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Wikiled.Text.Parser.Data;
using Wikiled.Text.Parser.Readers;

namespace Wikiled.Text.Parser.Tests.Readers.DevExpress
{
    [TestFixture]
    public class ParserFactoryTests
    {
         private ParserFactory instance;

        [SetUp]
        public void Setup()
        {
            instance = new ParserFactory(new NullLoggerFactory());
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new ParserFactory(null));
        }

        [TestCase("HowTo.pdf", ParsingType.Extract, 5, 6133)]
        [TestCase("non.pdf", ParsingType.OCR, 10, 179)]
        [TestCase("phototest.bmp", ParsingType.OCR, 1, 287)]
        [TestCase("phototest.gif", ParsingType.OCR, 1, 287)]
        [TestCase("phototest.jpg", ParsingType.OCR, 1, 287)]
        [TestCase("phototest.png", ParsingType.OCR, 1, 287)]
        [TestCase("phototest.tif", ParsingType.OCR, 1, 287)]
        [TestCase("dbs.doc", ParsingType.Extract, 10, 56)]
        public async Task ConstructParsers(string fileName, ParsingType type, int pages, int textLength)
        {
            var file = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, $@".\Data\{fileName}"));
            Assert.Throws<ArgumentNullException>(() => instance.ConstructParsers(null));
            var parser = instance.ConstructParsers(file);
            Assert.IsNotNull(parser);
            var result = await parser.Parse(file, 10).ConfigureAwait(false);
            Assert.AreEqual(type, result.Type);
            Assert.AreEqual(pages, result.Document.Pages.Length);
            Assert.AreEqual(textLength, result.Document.Pages[0].Blocks[0].Text.Length);
        }
    }
}
