using System;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using Wikiled.Text.Parser.Readers.DevExpress;

namespace Wikiled.Text.Parser.Tests.Readers.DevExpress
{
    [TestFixture]
    public class RichDocumentParserTests
    {
        private RichDocumentParser instance;

        private FileInfo file;

        [SetUp]
        public void Setup()
        {
            file = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, @".\Data\dbs.doc"));
            instance = new RichDocumentParser(file);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new RichDocumentParser(null));
        }

        [Test]
        public async Task Parse()
        {
            var result = await instance.Parse().ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.AreEqual(11, result.Pages.Length);
            Assert.AreEqual(56, result.Pages[0].Blocks[0].Text.Length);
        }
    }
}