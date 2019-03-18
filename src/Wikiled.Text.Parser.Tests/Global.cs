using System;
using System.IO;
using NUnit.Framework;

namespace Wikiled.Text.Parser.Tests
{
    [SetUpFixture]
    public class Global
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        [OneTimeTearDown]
        public void Clean()
        {
        }
    }
}
