namespace Flex.LVA.LexerAndParser.Tests
{
    using System.Collections.Generic;
    using System.Reflection.Metadata;
    using System.Text;
    using Flex.LVA.Core;
    using Flex.LVA.Core.EngineManagement;
    using Flex.LVA.Core.Interfaces;
    using Flex.LVA.Shared;
    using Flex.LVA.Shared.Containers;
    using Moq;
    using Newtonsoft.Json.Linq;
    using Serilog;

    [TestClass]
    public class RawStreamProviderTests
    {

        [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        public void StreamGetStringFromIdTest()
        {
            StringBuilder aBuilder = new StringBuilder();
            aBuilder.Append($"TestPorc 124 12 22:04:55,333 Domain Function10() - Trace Goes here 10.");
            aBuilder.Append($"TestPorc 124 12 22:04:55,333 Domain Function11() - Trace Goes here 11.");
            Console.WriteLine(aBuilder.Length);
            using (RawLogStreamer aStreamer = new RawLogStreamer(1))
            {
                aStreamer.Writer.Write(Encoding.UTF8.GetBytes(aBuilder.ToString()));
                aStreamer.Writer.Flush();
                aStreamer.LoadStore(aBuilder.ToString());
                aStreamer.StoreRange(1, 0, 70);
                aStreamer.StoreRange(2, 70, 140);
                Assert.IsTrue(aStreamer.GetString(2, out string aTwo));
                Assert.IsTrue(aStreamer.GetString(1, out string aOne));
                Assert.AreEqual("TestPorc 124 12 22:04:55,333 Domain Function10() - Trace Goes here 10.", aOne);
                Assert.AreEqual("TestPorc 124 12 22:04:55,333 Domain Function11() - Trace Goes here 11.", aTwo);
            }
        }

        [TestMethod]
        [TestCategory(CategoryConstants.Todo)]
        public void RawLogStreamerMustPreserveSeperators()
        {

        }

        [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        public void StreamGetWithReset()
        {
            StringBuilder aBuilder = new StringBuilder();
            aBuilder.Append($"TestPorc 124 12 22:04:55,333 Domain Function10() - Trace Goes here 10.");
            aBuilder.Append($"TestPorc 124 12 22:04:55,333 Domain Function11() - Trace Goes here 11.");
            Console.WriteLine(aBuilder.Length);
            using (RawLogStreamer aStreamer = new RawLogStreamer(1))
            {
                {
                    aStreamer.Writer.Write(Encoding.UTF8.GetBytes(aBuilder.ToString()));
                    aStreamer.Writer.Flush();
                    aStreamer.LoadStore(aBuilder.ToString());
                    aStreamer.StoreRange(1, 0, 70);
                    aStreamer.StoreRange(2, 70, 140);
                    Assert.IsTrue(aStreamer.GetString(2, out string aTwo));
                    Assert.IsTrue(aStreamer.GetString(1, out string aOne));
                    Assert.AreEqual("TestPorc 124 12 22:04:55,333 Domain Function10() - Trace Goes here 10.", aOne);
                    Assert.AreEqual("TestPorc 124 12 22:04:55,333 Domain Function11() - Trace Goes here 11.", aTwo);
                }

                aStreamer.ResetStore();
                {
                    aStreamer.Writer.Write(Encoding.UTF8.GetBytes(aBuilder.ToString()));
                    aStreamer.Writer.Flush();
                    aStreamer.LoadStore(aBuilder.ToString());
                    aStreamer.StoreRange(1, 0, 70);
                    aStreamer.StoreRange(2, 70, 140);
                    Assert.IsTrue(aStreamer.GetString(2, out string aTwo));
                    Assert.IsTrue(aStreamer.GetString(1, out string aOne));
                    Assert.AreEqual("TestPorc 124 12 22:04:55,333 Domain Function10() - Trace Goes here 10.", aOne);
                    Assert.AreEqual("TestPorc 124 12 22:04:55,333 Domain Function11() - Trace Goes here 11.", aTwo);
                }
            }
        }

        [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        public void StoreRangeWorksWithoutStepStoreToFinish()
        {
            StringBuilder aBuilder = new StringBuilder();
            aBuilder.Append($"TestPorc 124 12 22:04:55,333 Domain Function10() - Trace Goes here 10.");
            aBuilder.Append($"TestPorc 124 12 22:04:55,333 Domain Function11() - Trace Goes here 11.");
            Console.WriteLine(aBuilder.Length);
            using (RawLogStreamer aStreamer = new RawLogStreamer(2))
            {
                aStreamer.Writer.Write(Encoding.UTF8.GetBytes(aBuilder.ToString()));
                aStreamer.Writer.Flush();
                aStreamer.LoadStore(aBuilder.ToString());
                Assert.IsTrue(aStreamer.StoreRange(1, 0, 70));
                Assert.IsTrue(aStreamer.StoreRange(2, 70, 140));
                Assert.IsTrue(aStreamer.GetString(1, out string aOne));
                Assert.IsTrue(aStreamer.GetString(2, out string aTwo));
                Assert.AreEqual("TestPorc 124 12 22:04:55,333 Domain Function10() - Trace Goes here 10.", aOne);
                Assert.AreEqual("TestPorc 124 12 22:04:55,333 Domain Function11() - Trace Goes here 11.", aTwo);
            }
        }

            [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        [TestCategory(CategoryConstants.Negative)]
        public void StreamGetStringFromNegative()
        {
            StringBuilder aBuilder = new StringBuilder();
            aBuilder.AppendLine($"TestPorc 124 12 22:04:55,333 Domain Function10() - Trace Goes here 10.");
            aBuilder.AppendLine($"TestPorc 124 12 22:04:55,333 Domain Function11() - Trace Goes here 11.");
            using (RawLogStreamer aStreamer = new RawLogStreamer(3))
            {
                aStreamer.Writer.Write(Encoding.UTF8.GetBytes(aBuilder.ToString()));
                aStreamer.Writer.Flush();
                aStreamer.StoreRange(1, 0, 70);
                aStreamer.StoreRange(2, 70, 140);
                Assert.IsFalse(aStreamer.GetString(3, out string aOne));
                Assert.IsFalse(aStreamer.GetString(4, out string aTwo));
                Assert.AreEqual(RawLogStreamer.ErrorConstant, aOne);
                Assert.AreEqual(RawLogStreamer.ErrorConstant, aTwo);
            }
        }

        [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        [TestCategory(CategoryConstants.Negative)]
        public void StreamGetStringOutOfRange()
        {
            StringBuilder aBuilder = new StringBuilder();
            aBuilder.AppendLine($"TestPorc 124 12 22:04:55,333 Domain Function10() - Trace Goes here 10.");
            aBuilder.AppendLine($"TestPorc 124 12 22:04:55,333 Domain Function11() - Trace Goes here 11.");
            using (RawLogStreamer aStreamer = new RawLogStreamer(4))
            {
                aStreamer.Writer.Write(Encoding.UTF8.GetBytes(aBuilder.ToString()));
                aStreamer.Writer.Flush();
                aStreamer.LoadStore(aBuilder.ToString());
                Assert.IsFalse(aStreamer.StoreRange(1, 0, 150));
                Assert.IsFalse(aStreamer.GetString(3, out _));
            }
        }
    }
}
