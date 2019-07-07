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

        [Test]
        public void Arguments()
        {
            Assert.Throws<ArgumentNullException>(() => instance.ConstructParsers(null));
        }

        [TestCase("HowTo.pdf", ParsingType.Any, ParsingType.Extract, 5, 6133)]
        [TestCase("HowTo.pdf", ParsingType.OCR, ParsingType.OCR, 5, 2362)]
        [TestCase("non.pdf", ParsingType.Any, ParsingType.OCR, 10, 213)]
        [TestCase("phototest.bmp", ParsingType.Any, ParsingType.OCR, 1, 307)]
        [TestCase("phototest.gif", ParsingType.Any, ParsingType.OCR, 1, 307)]
        [TestCase("phototest.jpg", ParsingType.Any, ParsingType.OCR, 1, 307)]
        [TestCase("phototest.png", ParsingType.Any, ParsingType.OCR, 1, 307)]
        [TestCase("phototest.tif", ParsingType.Any, ParsingType.OCR, 1, 307)]
        [TestCase("dbs.doc", ParsingType.Any, ParsingType.Extract, 10, 1855)]
        public async Task ConstructParsers(string fileName, ParsingType requesting, ParsingType type, int pages, int textLength)
        {
            var file = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, $@".\Data\{fileName}"));
            var parser = instance.ConstructParsers(file);
            Assert.IsNotNull(parser);
            var result = await parser.Parse(new ParsingRequest(file, requesting,10)).ConfigureAwait(false);
            Assert.AreEqual(type, result.ProcessedAs);
            Assert.AreEqual(pages, result.Document.Pages.Length);
            Assert.AreEqual(textLength, result.Document.Pages[0].Build().Length);
        }
    }
}
