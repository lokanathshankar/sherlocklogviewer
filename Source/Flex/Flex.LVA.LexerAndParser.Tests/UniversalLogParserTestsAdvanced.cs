using Flex.LVA.Core;
using Flex.LVA.Shared;

using System.Diagnostics;
using System.Text;

namespace Flex.LVA.LexerAndParser.Tests
{
    using Flex.LVA.Core.Interfaces;
    using Flex.LVA.Shared.Containers;
    using Moq;

    [TestClass]
    public class UniversalLogParserTestsAdvanced
    {
        private int myEngineTestId = 20000;

        [TestMethod]
        [DataRow]
        [TestCategory("Basic")]
        public void TestForSingleLineAndMultiLineSyntheses()
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            string aLogLine =
                @"----------------------------------------
Message: Some single line trace.
ID: RealId:125 RealValue:Proc1
Severity: Warning
Timestamp: 2023-03-10T12:38:40.175+05:30
Process Name: ProcessName.exe

----------------------------------------
----------------------------------------
Message: MultiLineTrace,
Line2,
Line3
Line4
ID: RealId:127 RealValue:Proc3
Severity: Warning
Timestamp: 2023-03-10T12:39:40.187+05:30
Process Name: ProcessName2

----------------------------------------";
            aDef = new LogDefinition();
            aDef = SerilizationUtils.DeSerilizeFromJson<LogDefinition>(File.ReadAllText(
                        $"{TestUtils.TestFolder}/DefaultNestedLineDefinition.yaml"));
            aDef.ConvertEnglishMarkersToCSharp();
            UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId());
            aParser.Parse(aDef, aLogLine);
            Assert.AreEqual(
                File.ReadAllText($"{TestUtils.TestFolder}/{nameof(UniversalLogParserTests)}.{nameof(TestForSingleLineAndMultiLineSyntheses)}.json"), 
                SerilizationUtils.SerilizeToJsonReadable(aParser.LogGraph.SemanticLogs));
        }

        [TestMethod]
        [DataRow]
        [TestCategory("TODO")]
        public void TestForSingleLineAndMultiLineSynthesesDuplicate()
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            string aLogLine =
                @"----------------------------------------
Message: Some single line trace.
ID: RealId:125 RealValue:Proc1
Severity: Warning
Timestamp: 2023-03-10T12:38:40.175+05:30
Process Name: ProcessName.exe
ID: RealId:135 RealValue:Proc4

----------------------------------------
----------------------------------------
Message: MultiLineTrace,
Line2,
Line3
Line4
ID: RealId:127 RealValue:Proc3
Severity: Warning
Timestamp: 2023-03-10T12:39:40.187+05:30
Process Name: ProcessName2
ID: RealId:157 RealValue:Proc7

----------------------------------------";
            aDef = new LogDefinition();
            aDef = SerilizationUtils.DeSerilizeFromJson<LogDefinition>(File.ReadAllText(
                        $"{TestUtils.TestFolder}/DefaultNestedLineDefinitionDuplicates.yaml"));
            aDef.ConvertEnglishMarkersToCSharp();
            UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId());
            aParser.Parse(aDef, aLogLine);
            Assert.AreEqual(
                File.ReadAllText($"{TestUtils.TestFolder}/{nameof(UniversalLogParserTests)}.{nameof(TestForSingleLineAndMultiLineSyntheses)}.json"),
                SerilizationUtils.SerilizeToJsonReadable(aParser.LogGraph.SemanticLogs));
        }
    }
}
