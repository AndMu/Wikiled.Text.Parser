using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Text.Parser.Readers.DevExpress;

namespace Wikiled.Text.Parser.Tests.Readers.DevExpress
{
    [TestFixture]
    public class DevExpressPdfParserTests
    {
        private FileInfo fileHowTo;

        private FileInfo fileOcr;

        [SetUp]
        public void Setup()
        {
            fileHowTo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, @".\Data\HowTo.pdf"));
            fileOcr = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, @".\Data\non.pdf"));
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DevExpressPdfParser(fileHowTo, 0));
            Assert.Throws<ArgumentNullException>(() => new DevExpressPdfParser(null, 10));
        }

        [Test]
        public async Task Parse()
        {
            var instance = new DevExpressPdfParser(fileHowTo, 10);
            var result = await instance.Parse().ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Pages.Length);
            Assert.AreEqual(6004, result.Pages[0].Blocks[0].Text.Length);
        }

        [Test]
        public async Task ParseUnreadable()
        {
            var instance = new DevExpressPdfParser(fileOcr, 10);
            var result = await instance.Parse().ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Pages.Length);
            Assert.AreEqual(6004, result.Pages[0].Blocks[0].Text.Length);
        }
    }
}