using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Wikiled.Text.Parser.Data;
using Wikiled.Text.Parser.Readers.DevExpress;

namespace Wikiled.Text.Parser.Tests.Readers.DevExpress
{
    [TestFixture]
    public class DevExpressPdfParserTests
    {
        private FileInfo fileHowTo;

        private FileInfo fileOcr;

        private DevExpressPdfParser instance;

        [SetUp]
        public void Setup()
        {
            instance = Create();
            fileHowTo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, @".\Data\HowTo.pdf"));
            fileOcr = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, @".\Data\non.pdf"));
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new DevExpressPdfParser(null));
        }

        [Test]
        public async Task Parse()
        {
            var result = await instance.Parse(fileHowTo, 10).ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Document.Pages.Length);
            Assert.AreEqual(6004, result.Document.Pages[0].Blocks[0].Text.Length);
        }

        [Test]
        public async Task ParseUnreadable()
        {
            var result = await instance.Parse(fileOcr, 10).ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Document.Pages.Length);
            Assert.AreEqual(ParsingType.Failed, result.Type);
        }

        private DevExpressPdfParser Create()
        {
            return new DevExpressPdfParser(new NullLogger<DevExpressPdfParser>());
        }
    }
}