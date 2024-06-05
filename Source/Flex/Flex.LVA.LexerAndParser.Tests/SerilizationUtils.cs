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
    public class SerilizationUtilsTests
    {
        //// 22 60 55 888888 PM 5:30 GST
        [TestMethod]
        public void SerilizationValidationDefault()
        {
            LogHeader aHeader = new LogHeader();
            aHeader.Add("TestCol0", LogElementType.String);
            aHeader.Add("TestCol1", LogElementType.String);
            aHeader.Add("TestCol2", LogElementType.String);
            aHeader.Add("TestCol3", LogElementType.String);
            aHeader.Add("TestCol4", LogElementType.String);
            aHeader.Add("TestCol5", LogElementType.String);
            string aExpectedBytes = SerilizationUtils.SerilizeToJson(aHeader);
            Assert.AreEqual(aExpectedBytes, SerilizationUtils.SerilizeToJson(aHeader));
        }

        [TestMethod]
        public void SerilizationValidationDefault2()
        {
            LogHeader aHeader = new LogHeader();
            aHeader.LogDefinition = new LogDefinition();
            aHeader.Add("TestCol0", LogElementType.String);
            aHeader.Add("TestCol1", LogElementType.String);
            aHeader.Add("TestCol2", LogElementType.String);
            aHeader.Add("TestCol3", LogElementType.String);
            aHeader.Add("TestCol4", LogElementType.String);
            aHeader.Add("TestCol5", LogElementType.String);
            string aExpectedBytes = SerilizationUtils.SerilizeToJson(aHeader);
            Assert.AreEqual(aExpectedBytes, SerilizationUtils.SerilizeToJson(aHeader));
        }
    }
}
