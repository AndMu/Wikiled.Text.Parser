using System;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using Wikiled.Text.Parser.Ocr;
using Wikiled.Text.Parser.Readers.DevExpress;

namespace Wikiled.Text.Parser.Tests.Readers.DevExpress
{
    [TestFixture]
    public class DevExpressOcrParserTests
    {
        private IOcrImageParser ocrImageParser;
        private FileInfo fileInfo;

        private DevExpressOcrParser instance;

        [SetUp]
        public void SetUp()
        {
            ocrImageParser = new OcrImageParser();
            fileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "data", "non.pdf"));
            instance = CreateDevExpressOcrParser();
        }

        [TearDown]
        public void TearDown()
        {
            ocrImageParser.Dispose();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new DevExpressOcrParser(
                ocrImageParser,
                fileInfo,
                10));
        }

        [Test]
        public async Task Parse()
        {
            var text = await instance.Parse().ConfigureAwait(false);
            Assert.AreEqual(10, text.Pages);
        }

        private DevExpressOcrParser CreateDevExpressOcrParser()
        {
            return new DevExpressOcrParser(
                ocrImageParser,
                fileInfo,
                10);
        }
    }
}
