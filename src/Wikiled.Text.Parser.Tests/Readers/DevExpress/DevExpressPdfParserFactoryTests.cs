using System;
using System.IO;
using NUnit.Framework;
using Wikiled.Text.Parser.Readers.DevExpress;

namespace Wikiled.Text.Parser.Tests.Readers.DevExpress
{
    [TestFixture]
    public class DevExpressPdfParserFactoryTests
    {
         private DevExpressParserFactory instance;

        [SetUp]
        public void Setup()
        {
            instance = new DevExpressParserFactory(10);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DevExpressParserFactory(0));
        }

        [Test]
        public void ConstructParsers()
        {
            var file = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, @".\Data\HowTo.pdf"));
            Assert.Throws<ArgumentNullException>(() => instance.ConstructParsers(null));
            var parser = instance.ConstructParsers(file);
            Assert.IsNotNull(parser);
        }
    }
}
