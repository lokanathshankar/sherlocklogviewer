namespace Flex.LVA.LexerAndParser.Tests
{
    using Flex.LVA.Core.EngineManagement;
    using Flex.LVA.Core.IntellisenseLogs;
    using Flex.LVA.Core.Interfaces;
    using Flex.LVA.Shared;
    using Moq;
    using System.Globalization;
    using System.Security.Cryptography;

    [TestClass]
    public class DateTimeFormatTests
    {
        //// 22 60 55 888888 PM 5:30 GST
        [TestMethod]
        [TestCategory(CategoryConstants.Time)]
        [DataRow("22:21:55", "HH:mm:ss")]
        [DataRow("22:21:55.1", "HH:mm:ss.f")]
        [DataRow("22:21:55.12", "HH:mm:ss.ff")]
        [DataRow("22/21/55.123", "HH/mm/ss.fff")]
        [DataRow("22_21:55.1234", "HH_mm:ss.ffff")]
        [DataRow("22,21:55.12345", "HH,mm:ss.fffff")]
        [DataRow("22.21:55.123456", "HH.mm:ss.ffffff")]
        [DataRow("22:21:55.1234567", "HH:mm:ss.fffffff")]
        [DataRow("22:21:55.1234567 GMT", "HH:mm:ss.fffffff GMT")]
        [DataRow("             22:21:55.1234567              GMT      ", "HH:mm:ss.fffffff GMT")]
        [DataRow("11:05:5/55", "HH:mm:s/ff")]
        [DataRow("11:05:5 PM", "hh:mm:s tt")]
        [DataRow("11:05:5/55 A", "hh:mm:s/ff t")]
        [DataRow("11:05:5/55 A GMT", "hh:mm:s/ff t GMT")]
        [DataRow("11:05:5/55 +5:30 A", "hh:mm:s/ff zzz t")]
        [DataRow("11:05:5/55 -12 A", "hh:mm:s/ff zz t")]
        [DataRow("11:05:5/55 +5 A", "hh:mm:s/ff z t")]
        [DataRow("+5 11:05:5/55 A", "z hh:mm:s/ff t")]
        [DataRow("     GMT A 11:05:5/55    ", "     GMT t hh:mm:s/ff    ")]
        [DataRow("-12 GMT PM 11:05:5/9876", "zz GMT tt hh:mm:s/ffff")]
        [DataRow("+5:30 GMT PM 11:05:5/98796", "zzz GMT tt hh:mm:s/fffff")]
        [DataRow("2023-06-29T18:45:26.7425581+05:30", "yyyy-MM-ddTHH:mm:ss.fffffffzzz")]
        public void TestDateFormats(string theInput, string theExpectedFormat)
        {
            string aActual = TimeFormatParser.PredictFormat(theInput);
            Assert.AreEqual(theExpectedFormat, aActual);
            if (!DateTimeOffset.TryParseExact(theInput, theExpectedFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                Assert.Inconclusive();
            }
        }

        [TestMethod]
        [TestCategory(CategoryConstants.Time)]
        [DataRow("55:55:55.555555 AM +5:30 GST")]
        [DataRow("55:55:55")]
        [DataRow("+5:30 GMT PM 11:05:5/98796 IST")]
        public void TestDateFormatsUnParsable(string theInput)
        {
            Assert.ThrowsException<ArgumentException>(() => Console.WriteLine(TimeFormatParser.PredictFormat(theInput)));
        }
    }
}
