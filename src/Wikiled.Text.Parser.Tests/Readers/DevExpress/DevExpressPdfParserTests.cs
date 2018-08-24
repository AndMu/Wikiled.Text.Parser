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
        private DevExpressPdfParser instance;

        private FileInfo file;

        [SetUp]
        public void Setup()
        {
            file = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, @".\Data\HowTo.pdf"));
            instance = new DevExpressPdfParser(file, 10);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DevExpressPdfParser(file, 0));
            Assert.Throws<ArgumentNullException>(() => new DevExpressPdfParser(null, 10));
        }

        [Test]
        public async Task Parse()
        {
            var result = await instance.Parse().ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Pages.Length);
            Assert.AreEqual(6133, result.Pages[0].Blocks[0].Text.Length);
        }
    }
}