using Flex.LVA.Core;
using Flex.LVA.Core.EngineManagement;
using Flex.LVA.Core.Interfaces;
using Flex.LVA.Shared;
using Flex.LVA.Shared.Containers;
using Moq;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Flex.LVA.LexerAndParser.Tests
{
    [TestClass]
    public class StructruredParserTests
    {
        [TestMethod]
        public void BasicJsonTestWithProgressAndDataValidation()
        {
            string aTestData = "[{\r\n\"timestamp\": \"2022-12-23T12:34:56Z\",\r\n\"level\": \"warning\",\r\n\"message\": \"Low memory warning\",\r\n\"free_memory\": 456,\r\n\"total_memory\": 1024\r\n},        {\r\n\"timestamp\": \"2022-12-23T12:34:56Z\",\r\n\"level\": \"warning\",\r\n\"message\": \"Low memory warning\",\r\n\"free_memory\": 456,\r\n\"total_memory\": 1024\r\n},{\r\n\"timestamp\": \"2022-12-23T12:34:56Z\",\r\n\"level\": \"warning\",\r\n\"message\": \"Low memory warning\",\r\n\"free_memory\": 456,\r\n\"total_memory\": 1024\r\n},{\r\n\"timestamp\": \"2022-12-23T12:34:56Z\",\r\n\"level\": \"warning\",\r\n\"message\": \"Low memory warning\",\r\n\"free_memory\": 456,\r\n\"total_memory\": 1024\r\n},{\r\n\"timestamp\": \"2022-12-23T12:34:56Z\",\r\n\"level\": \"warning\",\r\n\"message\": \"Low memory warning\",\r\n\"free_memory\": 456,\r\n\"total_memory\": 1024\r\n},{\r\n\"timestamp\": \"2022-12-23T12:34:56Z\",\r\n\"level\": \"warning\",\r\n\"message\": \"Low memory warning\",\r\n\"free_memory\": 456,\r\n\"total_memory\": 1024\r\n},{\r\n\"timestamp\": \"2022-12-23T12:34:56Z\",\r\n\"level\": \"warning\",\r\n\"message\": \"Low memory warning\",\r\n\"free_memory\": 456,\r\n\"total_memory\": 1024\r\n},{\r\n\"timestamp\": \"2022-12-23T12:34:56Z\",\r\n\"level\": \"warning\",\r\n\"message\": \"Low memory warning\",\r\n\"free_memory\": 456,\r\n\"total_memory\": 1024\r\n},{\r\n\"timestamp\": \"2022-12-23T12:34:56Z\",\r\n\"level\": \"warning\",\r\n\"message\": \"Low memory warning\",\r\n\"free_memory\": 456,\r\n\"total_memory\": 1024\r\n},{\r\n\"timestamp\": \"2022-12-23T12:34:56Z\",\r\n\"level\": \"warning\",\r\n\"message\": \"Low memory warning\",\r\n\"free_memory\": 456,\r\n\"total_memory\": 1024\r\n}]";
            using (JSONLogParser aParser = new JSONLogParser(GeneralUtils.GetTestEngineId()))
            {
                int aTotalElements = 0;
                int aStart = 1;
                int aEnd = 2;
                bool aHeaderCalled = false;
                bool aCompCalled = false;
                void AParserLogChunkReady(object theSender, ProgressData theE)
                {
                    if (theE.LineRange.End.Value == theE.LineRange.Start.Value)
                    {
                        return;
                    }

                    aTotalElements += theE.LineRange.End.Value - theE.LineRange.Start.Value;
                    Console.WriteLine(theE.LineRange);
                    Assert.AreEqual(aStart, theE.LineRange.Start.Value);
                    Assert.AreEqual(aEnd, theE.LineRange.End.Value);
                    aStart++;
                    aEnd++;
                }

                void HeaderReady(object theSender, ILogHeader theE)
                {
                    Assert.AreEqual(Constants.Header, theE.ColumnNames[0]);
                    Assert.AreEqual(LogElementType.Bool, theE.ColumnTypes[0]);

                    Assert.AreEqual(Constants.Id, theE.ColumnNames[1]);
                    Assert.AreEqual(LogElementType.Number, theE.ColumnTypes[1]);

                    Assert.AreEqual("timestamp", theE.ColumnNames[2]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[2]);

                    Assert.AreEqual("level", theE.ColumnNames[3]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[3]);

                    Assert.AreEqual("message", theE.ColumnNames[4]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[4]);

                    Assert.AreEqual("free_memory", theE.ColumnNames[5]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[5]);

                    Assert.AreEqual("total_memory", theE.ColumnNames[6]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[6]);
                    aHeaderCalled = true;
                }

                void JobDone(object theSender, EventArgs theE)
                {
                    aCompCalled = true;
                }

                aParser.LogChunkReady += AParserLogChunkReady;
                aParser.HeaderReady += HeaderReady;
                aParser.JobFinished += JobDone;
                aParser.Parse(
                    new LogDefinition() { LogFileType = LogDataType.Json }, aTestData);
                aParser.LogChunkReady -= AParserLogChunkReady;
                aParser.HeaderReady -= HeaderReady;
                Assert.AreEqual(10, aParser.LogGraph.SemanticLogs.Count);
                Assert.AreEqual(aParser.LogGraph.SemanticLogs.Count, aTotalElements);
                int aIndex = 0;
                foreach (List<object> aLog in aParser.LogGraph.SemanticLogs.Values)
                {
                    aIndex++;
                    Assert.AreEqual(true, (aLog[0] as LogBoolContainer).ValueTyped);
                    Assert.AreEqual(aIndex, (aLog[1] as LogIntContainer).ValueTyped);
                    Assert.AreEqual("2022-12-23T12:34:56Z", aLog[2].ToString());
                    Assert.AreEqual("warning", aLog[3].ToString());
                    Assert.AreEqual("Low memory warning", aLog[4].ToString());
                    Assert.AreEqual("456", aLog[5].ToString());
                    Assert.AreEqual("1024", aLog[6].ToString());
                }

                Assert.IsTrue(aHeaderCalled);
                Assert.IsTrue(aCompCalled);
                Assert.IsTrue(aParser.LogStore.GetString(1, out string aRawString));
                Assert.AreEqual("{\r\n\"timestamp\": \"2022-12-23T12:34:56Z\",\r\n\"level\": \"warning\",\r\n\"message\": \"Low memory warning\",\r\n\"free_memory\": 456,\r\n\"total_memory\": 1024\r\n}", aRawString);
            }
        }

        [TestMethod]
        [DataRow(",")]
        [DataRow("\t")]
        [DataRow("|")]
        [DataRow(";")]
        public void BasicDelimetedTestWithProgressAndDataValidation(string theSep)
        {
            string aTestData = $"==========================================\r\ntimestamp{theSep}level{theSep}message{theSep}free_memory{theSep}total_memory\r\n2022-12-23T12:34:56Z{theSep}warning{theSep}Low memory warning{theSep}456{theSep}1024";
            using (ILogParser aParser = new DelimitedLogParser(GeneralUtils.GetTestEngineId()))
            {
                int aTotalElements = 0;
                int aStart = 1;
                int aEnd = 2;
                bool aHeaderCalled = false;
                bool aCompCalled = false;
                void AParserLogChunkReady(object theSender, ProgressData theE)
                {
                    if (theE.LineRange.End.Value == theE.LineRange.Start.Value)
                    {
                        return;
                    }

                    aTotalElements += theE.LineRange.End.Value - theE.LineRange.Start.Value;
                    Console.WriteLine(theE.LineRange);
                    Assert.AreEqual(aStart, theE.LineRange.Start.Value);
                    Assert.AreEqual(aEnd, theE.LineRange.End.Value);
                    aStart++;
                    aEnd++;
                }

                void HeaderReady(object theSender, ILogHeader theE)
                {
                    Assert.AreEqual(Constants.Header, theE.ColumnNames[0]);
                    Assert.AreEqual(LogElementType.Bool, theE.ColumnTypes[0]);

                    Assert.AreEqual(Constants.Id, theE.ColumnNames[1]);
                    Assert.AreEqual(LogElementType.Number, theE.ColumnTypes[1]);

                    Assert.AreEqual("timestamp", theE.ColumnNames[2]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[2]);

                    Assert.AreEqual("level", theE.ColumnNames[3]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[3]);

                    Assert.AreEqual("message", theE.ColumnNames[4]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[4]);

                    Assert.AreEqual("free_memory", theE.ColumnNames[5]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[5]);

                    Assert.AreEqual("total_memory", theE.ColumnNames[6]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[6]);

                    aHeaderCalled = true;
                }

                void JobDone(object theSender, EventArgs theE)
                {
                    aCompCalled = true;
                }

                aParser.LogChunkReady += AParserLogChunkReady;
                aParser.HeaderReady += HeaderReady;
                aParser.JobFinished += JobDone;
                aParser.Parse(
                    new LogDefinition() { LogFileType = LogDataType.Json, HeaderLineCount = 1}, aTestData);
                aParser.LogChunkReady -= AParserLogChunkReady;
                aParser.HeaderReady -= HeaderReady;
                Assert.AreEqual(1, aParser.LogGraph.SemanticLogs.Count);
                Assert.AreEqual(aParser.LogGraph.SemanticLogs.Count, aTotalElements);
                int aIndex = 0;
                foreach (List<object> aLog in aParser.LogGraph.SemanticLogs.Values)
                {
                    aIndex++;
                    Assert.AreEqual(true, (aLog[0] as LogBoolContainer).ValueTyped);
                    Assert.AreEqual(aIndex, (aLog[1] as LogIntContainer).ValueTyped);
                    Assert.AreEqual("2022-12-23T12:34:56Z", aLog[2].ToString());
                    Assert.AreEqual("warning", aLog[3].ToString());
                    Assert.AreEqual("Low memory warning", aLog[4].ToString());
                    Assert.AreEqual("456", aLog[5].ToString());
                    Assert.AreEqual("1024", aLog[6].ToString());
                }

                Assert.IsTrue(aHeaderCalled);
                Assert.IsTrue(aCompCalled);
                Assert.IsTrue(aParser.LogStore.GetString(1, out string aRawString));
                Assert.AreEqual($"2022-12-23T12:34:56Z{theSep}warning{theSep}Low memory warning{theSep}456{theSep}1024", aRawString);
            }
        }

        [TestMethod]
        public void BasicXmlTestWithProgressAndDataValidation()
        {
            string aTestData = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n<root>\r\n  <row>\r\n    <timestamp>2022-12-23T12:34:56Z</timestamp>\r\n    <level>warning</level>\r\n    <message>Low memory warning</message>\r\n    <free_memory>456</free_memory>\r\n    <total_memory>1024</total_memory>\r\n  </row>\r\n  <row>\r\n    <timestamp>2022-12-23T12:34:56Z</timestamp>\r\n    <level>warning</level>\r\n    <message>Low memory warning</message>\r\n    <free_memory>456</free_memory>\r\n    <total_memory>1024</total_memory>\r\n  </row>\r\n  <row>\r\n    <timestamp>2022-12-23T12:34:56Z</timestamp>\r\n    <level>warning</level>\r\n    <message>Low memory warning</message>\r\n    <free_memory>456</free_memory>\r\n    <total_memory>1024</total_memory>\r\n  </row>\r\n  <row>\r\n    <timestamp>2022-12-23T12:34:56Z</timestamp>\r\n    <level>warning</level>\r\n    <message>Low memory warning</message>\r\n    <free_memory>456</free_memory>\r\n    <total_memory>1024</total_memory>\r\n  </row>\r\n  <row>\r\n    <timestamp>2022-12-23T12:34:56Z</timestamp>\r\n    <level>warning</level>\r\n    <message>Low memory warning</message>\r\n    <free_memory>456</free_memory>\r\n    <total_memory>1024</total_memory>\r\n  </row>\r\n  <row>\r\n    <timestamp>2022-12-23T12:34:56Z</timestamp>\r\n    <level>warning</level>\r\n    <message>Low memory warning</message>\r\n    <free_memory>456</free_memory>\r\n    <total_memory>1024</total_memory>\r\n  </row>\r\n  <row>\r\n    <timestamp>2022-12-23T12:34:56Z</timestamp>\r\n    <level>warning</level>\r\n    <message>Low memory warning</message>\r\n    <free_memory>456</free_memory>\r\n    <total_memory>1024</total_memory>\r\n  </row>\r\n  <row>\r\n    <timestamp>2022-12-23T12:34:56Z</timestamp>\r\n    <level>warning</level>\r\n    <message>Low memory warning</message>\r\n    <free_memory>456</free_memory>\r\n    <total_memory>1024</total_memory>\r\n  </row>\r\n  <row>\r\n    <timestamp>2022-12-23T12:34:56Z</timestamp>\r\n    <level>warning</level>\r\n    <message>Low memory warning</message>\r\n    <free_memory>456</free_memory>\r\n    <total_memory>1024</total_memory>\r\n  </row>\r\n  <row>\r\n    <timestamp>2022-12-23T12:34:56Z</timestamp>\r\n    <level>warning</level>\r\n    <message>Low memory warning</message>\r\n    <free_memory>456</free_memory>\r\n    <total_memory>1024</total_memory>\r\n  </row>\r\n</root>";
            using (ILogParser aParser = new XMLLogParser(GeneralUtils.GetTestEngineId()))
            {
                int aTotalElements = 0;
                int aStart = 1;
                int aEnd = 2;
                bool aHeaderCalled = false;
                bool aCompCalled = false;
                void AParserLogChunkReady(object theSender, ProgressData theE)
                {
                    if (theE.LineRange.End.Value == theE.LineRange.Start.Value)
                    {
                        return;
                    }

                    aTotalElements += theE.LineRange.End.Value - theE.LineRange.Start.Value;
                    Console.WriteLine(theE.LineRange);
                    Assert.AreEqual(aStart, theE.LineRange.Start.Value);
                    Assert.AreEqual(aEnd, theE.LineRange.End.Value);
                    aStart++;
                    aEnd++;
                }

                void HeaderReady(object theSender, ILogHeader theE)
                {
                    Assert.AreEqual(Constants.Header, theE.ColumnNames[0]);
                    Assert.AreEqual(LogElementType.Bool, theE.ColumnTypes[0]);

                    Assert.AreEqual(Constants.Id, theE.ColumnNames[1]);
                    Assert.AreEqual(LogElementType.Number, theE.ColumnTypes[1]);

                    Assert.AreEqual("timestamp", theE.ColumnNames[2]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[2]);

                    Assert.AreEqual("level", theE.ColumnNames[3]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[3]);

                    Assert.AreEqual("message", theE.ColumnNames[4]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[4]);

                    Assert.AreEqual("free_memory", theE.ColumnNames[5]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[5]);

                    Assert.AreEqual("total_memory", theE.ColumnNames[6]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[6]);
                    aHeaderCalled = true;
                }

                void JobDone(object theSender, EventArgs theE)
                {
                    aCompCalled = true;
                }

                aParser.LogChunkReady += AParserLogChunkReady;
                aParser.HeaderReady += HeaderReady;
                aParser.JobFinished += JobDone;
                aParser.Parse(
                    new LogDefinition() { LogFileType = LogDataType.Json }, aTestData);
                aParser.LogChunkReady -= AParserLogChunkReady;
                aParser.HeaderReady -= HeaderReady;
                Assert.AreEqual(10, aParser.LogGraph.SemanticLogs.Count);
                Assert.AreEqual(aParser.LogGraph.SemanticLogs.Count, aTotalElements);
                int aIndex = 0;
                foreach (List<object> aLog in aParser.LogGraph.SemanticLogs.Values)
                {
                    aIndex++;
                    Assert.AreEqual(true, (aLog[0] as LogBoolContainer).ValueTyped);
                    Assert.AreEqual(aIndex, (aLog[1] as LogIntContainer).ValueTyped);
                    Assert.AreEqual("2022-12-23T12:34:56Z", aLog[2].ToString());
                    Assert.AreEqual("warning", aLog[3].ToString());
                    Assert.AreEqual("Low memory warning", aLog[4].ToString());
                    Assert.AreEqual("456", aLog[5].ToString());
                    Assert.AreEqual("1024", aLog[6].ToString());
                }

                Assert.IsTrue(aHeaderCalled);
                Assert.IsTrue(aCompCalled);
                Assert.IsTrue(aParser.LogStore.GetString(1, out string aRawString));
                Assert.AreEqual("<row>\r\n  <timestamp>2022-12-23T12:34:56Z</timestamp>\r\n  <level>warning</level>\r\n  <message>Low memory warning</message>\r\n  <free_memory>456</free_memory>\r\n  <total_memory>1024</total_memory>\r\n</row>", aRawString);
            }
        }

        [TestMethod]
        public void BasicEventLogTestWithProgressAndDataValidation()
        {
            using (ILogParser aParser = new EventLogParser(GeneralUtils.GetTestEngineId()))
            {
                aParser.Reset();
                aParser.AppendLogChunk(File.ReadAllBytes($"{TestUtils.TestFolder}/EventLogs.evtx"));
                int aTotalElements = 0;
                int aStart = 1;
                int aEnd = 8876;
                bool aHeaderCalled = false;
                bool aCompCalled = false;
                void AParserLogChunkReady(object theSender, ProgressData theE)
                {
                    if (theE.LineRange.End.Value == theE.LineRange.Start.Value)
                    {
                        return;
                    }

                    aTotalElements += theE.LineRange.End.Value - theE.LineRange.Start.Value + 1;
                    Assert.AreEqual(aStart, theE.LineRange.Start.Value);
                    Assert.AreEqual(aEnd, theE.LineRange.End.Value);
                    aStart++;
                    aEnd++;
                }

                void HeaderReady(object theSender, ILogHeader theE)
                {
                    Assert.AreEqual(Constants.Header, theE.ColumnNames[0]);
                    Assert.AreEqual(LogElementType.Bool, theE.ColumnTypes[0]);

                    Assert.AreEqual(Constants.Id, theE.ColumnNames[1]);
                    Assert.AreEqual(LogElementType.Number, theE.ColumnTypes[1]);

                    Assert.AreEqual(EventColumnConsts.Level, theE.ColumnNames[2]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[2]);

                    Assert.AreEqual(EventColumnConsts.DateTime, theE.ColumnNames[3]);
                    Assert.AreEqual(LogElementType.DateTime, theE.ColumnTypes[3]);

                    Assert.AreEqual(EventColumnConsts.Source, theE.ColumnNames[4]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[4]);

                    Assert.AreEqual(EventColumnConsts.EventId, theE.ColumnNames[5]);
                    Assert.AreEqual(LogElementType.Number, theE.ColumnTypes[5]);

                    Assert.AreEqual(EventColumnConsts.TaskCat, theE.ColumnNames[6]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[6]);

                    Assert.AreEqual(EventColumnConsts.Message, theE.ColumnNames[7]);
                    Assert.AreEqual(LogElementType.String, theE.ColumnTypes[7]);
                    aHeaderCalled = true;
                }

                void JobDone(object theSender, EventArgs theE)
                {
                    aCompCalled = true;
                }

                aParser.LogChunkReady += AParserLogChunkReady;
                aParser.HeaderReady += HeaderReady;
                aParser.JobFinished += JobDone;
                aParser.Parse(
                    new LogDefinition() { LogFileType = LogDataType.Json }, string.Empty);
                aParser.LogChunkReady -= AParserLogChunkReady;
                aParser.HeaderReady -= HeaderReady;
                Assert.AreEqual(8876, aParser.LogGraph.SemanticLogs.Count);
                Assert.AreEqual(aParser.LogGraph.SemanticLogs.Count, aTotalElements);
                int aIndex = 0;
                List<object> aLog = aParser.LogGraph.SemanticLogs.Values.Last();
                aIndex++;
                Assert.AreEqual(true, (aLog[0] as LogBoolContainer).ValueTyped);
                Assert.AreEqual(8876, (aLog[1] as LogIntContainer).ValueTyped);
                Assert.AreEqual("Informational", aLog[2].ToString());
                Console.WriteLine(aLog[3].GetType());
                Assert.AreEqual("12-07-2023 20:09:00",(aLog[3] as LogDateTimeContainer).ValueTyped.ToString("dd-MM-yyyy HH:mm:ss"));
                Assert.AreEqual("Microsoft-Windows-Security-SPP", aLog[4].ToString());
                Assert.AreEqual("16384", aLog[5].ToString());
                Assert.AreEqual("None", aLog[6].ToString());
                Assert.IsTrue(aLog[7].ToString().StartsWith("Successfully scheduled Software Protection service for re-start at 2023-07-13T13:24:00Z. Reason: RulesEngine."));
                Assert.IsTrue(aHeaderCalled);
                Assert.IsTrue(aCompCalled);
                Assert.IsTrue(aParser.LogStore.GetString(8876, out string aRawString));
                Assert.AreEqual("<Event xmlns=\"http://schemas.microsoft.com/win/2004/08/events/event\">\r\n  <System>\r\n    <Provider Name=\"Microsoft-Windows-Security-SPP\" Guid=\"{E23B33B0-C8C9-472C-A5F9-F2BDFEA0F156}\" EventSourceName=\"Software Protection Platform Service\" />\r\n    <EventID Qualifiers=\"16384\">16384</EventID>\r\n    <Version>0</Version>\r\n    <Level>4</Level>\r\n    <Task>0</Task>\r\n    <Opcode>0</Opcode>\r\n    <Keywords>0x80000000000000</Keywords>\r\n    <TimeCreated SystemTime=\"2023-07-12T14:39:00.9117990Z\" />\r\n    <EventRecordID>119479</EventRecordID>\r\n    <Correlation />\r\n    <Execution ProcessID=\"0\" ThreadID=\"0\" />\r\n    <Channel>Application</Channel>\r\n    <Computer>DESKTOP-EPFJ56T</Computer>\r\n    <Security />\r\n  </System>\r\n  <EventData>\r\n    <Data>2023-07-13T13:24:00Z</Data>\r\n    <Data>RulesEngine</Data>\r\n  </EventData>\r\n</Event>", aRawString);
            }
        }

        [TestMethod]
        public void BasicSysLogTestWithProgressAndDataValidation()
        {
            string aTestData = "<14>Mar  4 15:53:03 BAR-NG-VF500 BAR-NG-VF500/box_Firewall_Activity:  Info     BAR-NG-VF500 Remove: type=FWD|proto=UDP|srcIF=eth1|srcIP=192.168.70.7|srcPort=35119|srcMAC=08:00:27:da:d7:9c|dstIP=8.8.8.8|dstPort=53|dstService=domain|dstIF=eth0|rule=InternetAccess/<App>:RestrictTim|info=Balanced Session Idle Timeout|srcNAT=192.168.70.7|dstNAT=8.8.8.8|duration=21132|count=1|receivedBytes=130|sentBytes=62|receivedPackets=1|sentPackets=1|user=|protocol=|application=|target=|content=|urlcat\r\n" +
                "<14>Mar  4 15:53:03 BAR-NG-VF500 BAR-NG-VF500/srv_S1_NGFW:  Info     BAR-NG-VF500 State:          REM(Balanced Session Idle Timeout,20) FWD UDP 192.168.70.7:35119 () -> 8.8.8.8:53\r\n" +
                "<14>Mar  4 15:53:03 BAR-NG-VF500 BAR-NG-VF500/box_Firewall_audit:  Info     BAR-NG-VF500 1393928583|Remove:|FWD|eth1|UDP|InternetAccess/<App>:RestrictTim|192.168.70.7|35119|8.8.8.8|53|domain|2001|Balanced Session Idle Timeout|192.168.70.7|35119|8.8.8.8|53|eth0|08:00:27:da:d7:9c|62|1|130|1|21132|590\r\n" +
                "<15>Mar  4 15:53:03 BAR-NG-VF500 BAR-NG-VF500/srv_S1_URL_Cofsd:  Internal BAR-NG-VF500 cofsd: [0000000] 5 DEBUG OF: Generic HTTP Transfer: 6.53%: Download starts in 171 seconds\r\n" +
                "<14>Mar  4 15:53:04 BAR-NG-VF500 BAR-NG-VF500/box_Firewall_Activity:  Info     BAR-NG-VF500 Remove: type=FWD|proto=UDP|srcIF=eth1|srcIP=192.168.70.7|srcPort=38686|srcMAC=08:00:27:da:d7:9c|dstIP=8.8.8.8|dstPort=53|dstService=domain|dstIF=eth0|rule=InternetAccess/<App>:RestrictTim|info=Session Idle Timeout|srcNAT=192.168.70.7|dstNAT=8.8.8.8|duration=60100|count=1|receivedBytes=0|sentBytes=62|receivedPackets=0|sentPackets=1|user=|protocol=|application=|target=|content=|urlcat=\r\n";
            using (ILogParser aParser = new SysLogParser(GeneralUtils.GetTestEngineId()))
            {
                aParser.AppendLogChunk(Encoding.UTF8.GetBytes(aTestData));
                int aTotalElements = 0;
                int aStart = 1;
                int aEnd = 2;
                bool aHeaderCalled = false;
                bool aCompCalled = false;
                void AParserLogChunkReady(object theSender, ProgressData theE)
                {
                    if (theE.LineRange.End.Value == theE.LineRange.Start.Value)
                    {
                        return;
                    }

                    aTotalElements += theE.LineRange.End.Value - theE.LineRange.Start.Value;
                    Console.WriteLine(theE.LineRange);
                    Assert.AreEqual(aStart, theE.LineRange.Start.Value);
                    Assert.AreEqual(aEnd, theE.LineRange.End.Value);
                    aStart++;
                    aEnd++;
                }

                void HeaderReady(object theSender, ILogHeader theE)
                {
                    Assert.AreEqual(Constants.Header, theE.ColumnNames[0]);
                    Assert.AreEqual(LogElementType.Bool, theE.ColumnTypes[0]);

                    Assert.AreEqual(Constants.Id, theE.ColumnNames[1]);
                    Assert.AreEqual(LogElementType.Number, theE.ColumnTypes[1]);

                    Assert.AreEqual("Date", theE.ColumnNames[2]);
                    Assert.AreEqual(LogElementType.Date, theE.ColumnTypes[2]);

                    Assert.AreEqual("Time", theE.ColumnNames[3]);
                    Assert.AreEqual(LogElementType.Time, theE.ColumnTypes[3]);

                    int aI = 4;
                    foreach (var item in new[] { "Facility", "Hostname", "AppName", "Severity", "Message", "Additional Data" })
                    {
                        Assert.AreEqual(item, theE.ColumnNames[aI]);
                        Assert.AreEqual(LogElementType.String, theE.ColumnTypes[4]);
                        aI++;
                    }

                    aHeaderCalled = true;
                }

                void JobDone(object theSender, EventArgs theE)
                {
                    aCompCalled = true;
                }

                aParser.LogChunkReady += AParserLogChunkReady;
                aParser.HeaderReady += HeaderReady;
                aParser.JobFinished += JobDone;
                aParser.Parse(
                    new LogDefinition() { LogFileType = LogDataType.Json }, aTestData);
                aParser.LogChunkReady -= AParserLogChunkReady;
                aParser.HeaderReady -= HeaderReady;
                Assert.AreEqual(5, aParser.LogGraph.SemanticLogs.Count);
                Assert.AreEqual(aParser.LogGraph.SemanticLogs.Count, aTotalElements);
                List<object> aLog = aParser.LogGraph.SemanticLogs.Values.First();
                Assert.AreEqual(true, (aLog[0] as LogBoolContainer).ValueTyped);
                Assert.AreEqual(1, (aLog[1] as LogIntContainer).ValueTyped);
                Assert.AreEqual("04-03-2023", (aLog[2] as LogDateContainer).ValueTyped.ToString("dd-MM-yyyy"));
                Assert.AreEqual("15:53:03", (aLog[3] as LogTimeContainer).ValueTyped.ToString("HH:mm:ss"));
                Assert.AreEqual("UserLevel", aLog[4].ToString());
                Assert.AreEqual("BAR-NG-VF500", aLog[5].ToString());
                Assert.AreEqual("", aLog[6].ToString());
                Assert.AreEqual("Informational", aLog[7].ToString());
                Assert.AreEqual("Info     BAR-NG-VF500 Remove: type=FWD|proto=UDP|srcIF=eth1|srcIP=192.168.70.7|srcPort=35119|srcMAC=08:00:27:da:d7:9c|dstIP=8.8.8.8|dstPort=53|dstService=domain|dstIF=eth0|rule=InternetAccess/<App>:RestrictTim|info=Balanced Session Idle Timeout|srcNAT=192.168.70.7|dstNAT=8.8.8.8|duration=21132|count=1|receivedBytes=130|sentBytes=62|receivedPackets=1|sentPackets=1|user=|protocol=|application=|target=|content=|urlcat\r", aLog[8].ToString());
                Assert.AreEqual("{\"IPv4\":[\"192.168.70.7\",\"8.8.8.8\",\"192.168.70.7\",\"8.8.8.8\"]}", aLog[9].ToString());
                Assert.IsTrue(aHeaderCalled);
                Assert.IsTrue(aCompCalled);
                Assert.IsTrue(aParser.LogStore.GetString(1, out string aRawString));
                Assert.AreEqual("<14>Mar  4 15:53:03 BAR-NG-VF500 BAR-NG-VF500/box_Firewall_Activity:  Info     BAR-NG-VF500 Remove: type=FWD|proto=UDP|srcIF=eth1|srcIP=192.168.70.7|srcPort=35119|srcMAC=08:00:27:da:d7:9c|dstIP=8.8.8.8|dstPort=53|dstService=domain|dstIF=eth0|rule=InternetAccess/<App>:RestrictTim|info=Balanced Session Idle Timeout|srcNAT=192.168.70.7|dstNAT=8.8.8.8|duration=21132|count=1|receivedBytes=130|sentBytes=62|receivedPackets=1|sentPackets=1|user=|protocol=|application=|target=|content=|urlcat\r", aRawString);
            }
        }
    }
}
