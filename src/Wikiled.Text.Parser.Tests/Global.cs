using NUnit.Framework;

namespace Wikiled.Text.Parser.Tests
{
    [SetUpFixture]
    public class Global
    {
        [OneTimeSetUp]
        public void Setup()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        [OneTimeTearDown]
        public void Clean()
        {
        }
    }
}
