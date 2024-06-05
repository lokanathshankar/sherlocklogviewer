
namespace Flex.LVA.LexerAndParser.Tests
{
    using Flex.LVA.Core;
    using Flex.LVA.Core.IntellisenseLogs;
    using Flex.LVA.Shared;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    [TestClass]
    public class IntellisenseLogTests
    {
        [TestMethod]
        public void TestIntelliSense()
        {
            string logLine = "      SegmentServerIVS    25308 92                               0        0   S 23:59:49,962 INFO         11us AX.ImgSys.LinkManager.SrioNetLink.SenderChannel SenderChannel::SendStatusToParent() - NETLINK #Info# SenderChannel IVS << 20 >> (SS_CnC): SendStatusToParent -Hca Status 0  SenderOpen 0  PingStatus 0  Sending 0";
            string traceLine = "- NETLINK #Info# SenderChannel IVS << 20 >> (SS_CnC): SendStatusToParent -Hca Status 0  SenderOpen 0  PingStatus 0  Sending 0";
            IntellisenseLogs aIntellisenseLog = new IntellisenseLogs();
            aIntellisenseLog.DetectLogDefinition(logLine, traceLine, out LogDefinition theDef);
            Assert.IsNotNull(theDef);
            Assert.AreEqual("AutoDetect_Space", theDef.Name);
            Assert.AreEqual(11, theDef.Syntaxes[0].Elements.Count);
            Assert.AreEqual("AutoDetect_Space", theDef.Name);
        }



        [TestMethod]
        [DataRow("|", "Pipe")]
        public void TestIntelliSenseDelimitedRanged(string theSep, string theRealName)
        {
            IntellisenseLogs aIntellisenseLog = new IntellisenseLogs();
            aIntellisenseLog.RangedDetect(new List<string>() {
                string.Format("Col1{0} Col2{0} Col{0}", theSep),
                string.Format("Col1{0} Col2{0} Col{0}", theSep),
                string.Format("Col1{0} Col2{0} Col{0}", theSep),
                string.Format("Col1{0} Col2{0} Col{0}", theSep)
            }, out LogDefinition aDef, out string aEstimatedSep);
            Assert.AreEqual(theSep, aEstimatedSep);
            Assert.IsNotNull(aDef);
            Assert.AreEqual($"AutoDetect_{theRealName}_Delimited", aDef.Name);
            Assert.AreEqual(3, aDef.Syntaxes[0].Elements.Count);
            Assert.AreEqual(theSep, aDef.Syntaxes[0].Elements[0].EndSeparator);
            Assert.AreEqual("Column 1", aDef.Syntaxes[0].Elements[0].Name);
            Assert.AreEqual(theSep, aDef.Syntaxes[0].Elements[1].EndSeparator);
            Assert.AreEqual("Column 2", aDef.Syntaxes[0].Elements[1].Name);
            Assert.AreEqual(null, aDef.Syntaxes[0].Elements[2].EndSeparator);
            Assert.AreEqual("Column 3", aDef.Syntaxes[0].Elements[2].Name);
            string aTestLogLine = string.Format("Col1{0} Col2{0} Col3\nCol1{0} Col2{0} Col3", theSep);
            using (var aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
            {
                aParser.Reset();
                aParser.Parse(aDef, aTestLogLine);
                Assert.AreEqual(2, aParser.LogGraph.SemanticLogs.Count);
                foreach (var aItem in aParser.LogGraph.SemanticLogs.Values)
                {
                    Assert.AreEqual("Col1", aItem[2].ToString());
                    Assert.AreEqual("Col2", aItem[3].ToString());
                    Assert.AreEqual(" Col3", aItem[4].ToString());
                }
            }
        }

        [TestMethod]
        [DataRow("|", "Pipe")]
        [DataRow(" ", "Space")]
        [DataRow("\t", "Tab")]
        [DataRow(",", "Comma")]
        [DataRow("/", "Slash")]
        public void TestIntelliSenseDelimitedFiles(string theSep, string theRealName)
        {
            string logLine = string.Format("Col1{0} Col2{0} Col{0}", theSep);
            IntellisenseLogs aIntellisenseLog = new IntellisenseLogs();
            aIntellisenseLog.DetectLogDefinitionUniform(logLine, out LogDefinition theDef, out string aEstimatedSep);
            Assert.AreEqual(theSep, aEstimatedSep);
            Assert.IsNotNull(theDef);
            Assert.AreEqual($"AutoDetect_{theRealName}_Delimited", theDef.Name);
            Assert.AreEqual(3, theDef.Syntaxes[0].Elements.Count);
            Assert.AreEqual(theSep, theDef.Syntaxes[0].Elements[0].EndSeparator);
            Assert.AreEqual("Column 1", theDef.Syntaxes[0].Elements[0].Name);
            Assert.AreEqual(theSep, theDef.Syntaxes[0].Elements[1].EndSeparator);
            Assert.AreEqual("Column 2", theDef.Syntaxes[0].Elements[1].Name);
            Assert.AreEqual(null, theDef.Syntaxes[0].Elements[2].EndSeparator);
            Assert.AreEqual("Column 3", theDef.Syntaxes[0].Elements[2].Name);
            string aTestLogLine = string.Format("Col1{0} Col2{0} Col3\nCol1{0} Col2{0} Col3", theSep);
            using (var aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
            {
                aParser.Reset();
                aParser.Parse(theDef, aTestLogLine);
                Assert.AreEqual(2, aParser.LogGraph.SemanticLogs.Count);
                foreach (var aItem in aParser.LogGraph.SemanticLogs.Values)
                {
                    Assert.AreEqual("Col1", aItem[2].ToString());
                    Assert.AreEqual("Col2", aItem[3].ToString());
                    Assert.AreEqual(" Col3", aItem[4].ToString());
                }
            }
        }
    }
}
