using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using Wikiled.Text.Parser.Data;
using Wikiled.Text.Parser.Readers;
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
            instance = new RichDocumentParser(new NullLogger<RichDocumentParser>());
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new RichDocumentParser(null));
        }

        [Test, Ignore("Temporary")]
        public async Task Parse()
        {
            var resultTask = instance.Parse(new ParsingRequest(file, ParsingType.Any, 10));
            if (await Task.WhenAny(resultTask, Task.Delay(TimeSpan.FromMinutes(4))).ConfigureAwait(false) != resultTask)
            {
                Assert.Fail("Timeout");
            }

            var result = await resultTask.ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Document.Pages.Length);
            Assert.AreEqual(56, result.Document.Pages[0].Blocks[0].Text.Length);
        }
    }
}