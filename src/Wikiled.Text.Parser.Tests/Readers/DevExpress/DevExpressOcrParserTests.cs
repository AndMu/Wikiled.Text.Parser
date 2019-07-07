using System;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Wikiled.Text.Parser.Data;
using Wikiled.Text.Parser.Ocr;
using Wikiled.Text.Parser.Readers;
using Wikiled.Text.Parser.Readers.DevExpress;

namespace Wikiled.Text.Parser.Tests.Readers.DevExpress
{
    [TestFixture]
    public class DevExpressOcrParserTests
    {
        private IOcrImageParser ocrImageParser;

        private FileInfo fileInfo;

        private DevExpressPdfOcrParser instance;

        [SetUp]
        public void SetUp()
        {
            ocrImageParser = new OcrImageParser();
            fileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "data", "non.pdf"));
            instance = CreateDevExpressOcrParser();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new DevExpressPdfOcrParser(new NullLogger<DevExpressPdfOcrParser>(), null));
            Assert.Throws<ArgumentNullException>(() => new DevExpressPdfOcrParser(null, ocrImageParser));
        }

        [Test]
        public async Task Parse()
        {
            var result = await instance.Parse(new ParsingRequest(fileInfo, ParsingType.OCR, 10)).ConfigureAwait(false);
            Assert.AreEqual(ParsingType.OCR, result.ProcessedAs);
            Assert.AreEqual(10, result.Document.Pages.Length);
            Assert.Greater(result.Document.Pages[0].Blocks[0].Text.Length, 10);
            Assert.AreEqual(19317, result.Document.Build().Length);
        }

        private DevExpressPdfOcrParser CreateDevExpressOcrParser()
        {
            return new DevExpressPdfOcrParser(new NullLogger<DevExpressPdfOcrParser>(),  ocrImageParser);
        }
    }
}
