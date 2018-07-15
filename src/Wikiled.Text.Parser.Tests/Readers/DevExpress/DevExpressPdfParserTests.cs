using System;
using System.IO;
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
        public void Parse()
        {
            var result = instance.Parse();
            Assert.IsNotNull(result);
            Assert.AreEqual(23330, result.Length);
        }
    }
}