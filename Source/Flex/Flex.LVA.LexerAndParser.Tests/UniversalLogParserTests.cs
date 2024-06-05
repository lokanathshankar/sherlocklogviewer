using Flex.LVA.Core;
using Flex.LVA.Core.EngineManagement;
using Flex.LVA.Core.Interfaces;
using Flex.LVA.Shared;
using Flex.LVA.Shared.Containers;
using Moq;
using System.Diagnostics;
using System.Text;

namespace Flex.LVA.LexerAndParser.Tests
{
    [TestClass]
    public class UniversalLogParserTests
    {
        private static int myEngineTestId = 10000;

        [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        [TestCategory(CategoryConstants.Progress)]
        public void FullDataCompletionTest()
        {
            ILogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId());
            aParser.Parse(
                SerilizationUtils.DeSerilizeFromJson<LogDefinition>(File.ReadAllText($"{TestUtils.TestFolder}/DefaultSingleLineDefinition.yaml")),
                File.ReadAllText($"{TestUtils.TestFolder}/DefaultSingleLine.log"));
            Assert.AreEqual(48, aParser.LogGraph.SemanticLogs.Count);
        }

        [TestMethod]
        [DataRow]
        [TestCategory(CategoryConstants.Basic)]
        public void TestForSkippingHeaderAndMixedSeparators()
        {
            StringBuilder aBuilder = new StringBuilder();
            aBuilder.AppendLine($"This Line's Parsing Shall Fail");
            aBuilder.AppendLine($"=============================");
            aBuilder.AppendLine($"TestPorc 124 12\t22:04:55,333\tDomain Function1() - Trace Goes here");
            aBuilder.AppendLine($"TestPorc 124 12\t22:04:55,333\tDomain Function1() - Trace Goes here");
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out _);
            aDef.HeaderLineCount = 2;
            aDef.Syntaxes.Add(aSyntax);
            aDef.Syntaxes[0].Elements[2].EndSeparator = "tab";
            aDef.Syntaxes[0].Elements[3].EndSeparator = "tab";
            using (UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
            {
                aParser.Parse(aDef, aBuilder.ToString());
                Assert.AreEqual(2, aParser.LogGraph.SemanticLogs.Count);
                Assert.AreEqual(10, aParser.LogGraph.SemanticLogs[1].Count);
            }
        }

        [TestMethod]
        [DataRow]
        [TestCategory(CategoryConstants.Basic)]
        public void TestForAutoSkipHeader()
        {
            StringBuilder aBuilder = new StringBuilder();
            aBuilder.AppendLine($"This Line's Parsing Shall Fail");
            aBuilder.AppendLine($"=============================");
            aBuilder.AppendLine($"TestPorc 124 12\t22:04:55,333\tDomain... Function1() - Trace Goes here");
            aBuilder.AppendLine($"TestPorc 124 12\t22:04:55,333\tDomain... Function1() - Trace Goes here");
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out _);
            aDef.Syntaxes.Add(aSyntax);
            aDef.Syntaxes[0].Elements[2].EndSeparator = "tab";
            aDef.Syntaxes[0].Elements[3].EndSeparator = "tab";
            using (UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
            {
                aParser.Parse(aDef, aBuilder.ToString());
                Assert.AreEqual(2, aParser.LogGraph.SemanticLogs.Count);
                Assert.AreEqual(10, aParser.LogGraph.SemanticLogs[1].Count);
            }
        }

        [TestMethod]
        [DataRow]
        [TestCategory(CategoryConstants.Basic)]
        public void TestForSkippingHeader()
        {
            StringBuilder aBuilder = new StringBuilder();
            aBuilder.AppendLine($"This Line's Parsing Shall Fail");
            aBuilder.AppendLine($"=============================");
            aBuilder.AppendLine($"TestPorc 124 12 22:04:55,333 Domain Function1() - Trace Goes here");
            aBuilder.AppendLine($"TestPorc 124 12 22:04:55,333 Domain Function1() - Trace Goes here");
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out _);
            aDef.HeaderLineCount = 3;
            aDef.Syntaxes.Add(aSyntax);
            using (UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
            {
                aParser.Parse(aDef, aBuilder.ToString());
                Assert.AreEqual(1, aParser.LogGraph.SemanticLogs.Count);
                Assert.AreEqual(10, aParser.LogGraph.SemanticLogs[1].Count);
            }
        }

        [TestMethod]
        [DataRow]
        [TestCategory(CategoryConstants.Basic)]
        public void TestLogSingleLine()
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out string aLogLine);
            aDef.Syntaxes.Add(aSyntax);
            using (UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
            {
                aParser.Parse(aDef, aLogLine);
                Assert.AreEqual(File.ReadAllText($"{TestUtils.TestFolder}/{nameof(UniversalLogParserTests)}.{nameof(TestLogSingleLine)}.json"), SerilizationUtils.SerilizeToJsonReadable(aParser.LogGraph.SemanticLogs));
            }
        }

        [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        public void TestLogSingleLineTruncationEffectTestForMultiLine()
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out string aLogLine);
            aLogLine += "\n TestPorc  124  12  22:04:55,333        Domain        Function() - Trace Goes here.\n   Trace Continues to line 2\n and line 3\n TestPorc  124  12  22:04:55,333        Domain        Function() - Trace Goes here.";
            aDef.Syntaxes.Add(aSyntax);
            using (UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
            {
                aParser.Parse(aDef, aLogLine);
                Assert.AreEqual(File.ReadAllText($"{TestUtils.TestFolder}/{nameof(UniversalLogParserTests)}.{nameof(TestLogSingleLineTruncationEffectTestForMultiLine)}.json"), SerilizationUtils.SerilizeToJsonReadable(aParser.LogGraph.SemanticLogs));
            }
        }

        [TestMethod]
        [DataRow]
        [TestCategory(CategoryConstants.Basic)]
        public void RawLogUpdateTest()
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            Mock<IRawLogStore> aMockStore = new Mock<IRawLogStore>();
            GetDefaultDefinition(out aDef, out aSyntax, out string aLogLine);
            aLogLine += "\n TestPorc  124  12  22:04:55,333        Domain        Function() - Trace Goes here.\n   Trace Continues to line 2\n and line 3\n TestPorc  124  12  22:04:55,333        Domain        Function() - Trace Goes here.";
            aDef.Syntaxes.Add(aSyntax);
            using (UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId(), aMockStore.Object))
            {
                aParser.Parse(aDef, aLogLine);
                aMockStore.Verify(theX => theX.StoreRange(1, 0, 65), Times.Once);
                aMockStore.Verify(theX => theX.StoreRange(1, 0, 65), Times.Once);
                aMockStore.Verify(theX => theX.StoreRange(3, 135, 218), Times.Once);
                aMockStore.Verify(theX => theX.StoreRange(4, 219, 247), Times.Once);
                aMockStore.Verify(theX => theX.UpdateRange(3, 247), Times.Once);
                aMockStore.Verify(theX => theX.StoreRange(4, 248, 259), Times.Once);
                aMockStore.Verify(theX => theX.UpdateRange(3, 259), Times.Once);
                aMockStore.Verify(theX => theX.StoreRange(4, 260, 343), Times.Once);
                aMockStore.Verify(theX => theX.StoreRange(2, 66, 134), Times.Once);
            }
        }

        [TestMethod]
        [TestCategory(CategoryConstants.Progress)]
        public void TestLogSingleLineProgressTest()
        {
            StringBuilder aBuilder = new StringBuilder();
            foreach (var aNumber in Enumerable.Range(0, 100))
            {
                aBuilder.AppendLine($"TestPorc 124 12 22:04:55,333 Domain Function{aNumber}() - Trace Goes here {0}.");
            }

            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out _);
            aDef.Syntaxes.Add(aSyntax);
            using (UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
            {
                long aProgressCount = 0;
                bool aHeaderCalled = false;
                bool aCompleteCalled = false;
                bool aChunkCalled = false;
                using (AutoResetEvent aEvent = new AutoResetEvent(false))
                {
                    void LogChunkReady(object _, ProgressData theData)
                    {
                        Console.WriteLine(theData.ProgressPercentage);
                        Interlocked.Increment(ref aProgressCount);
                        aChunkCalled = true;
                    }

                    void OnHeaderReady(object _, ILogHeader theData)
                    {
                        aHeaderCalled = true;
                    }

                    void OnJobFinished(object _, EventArgs theData)
                    {
                        aCompleteCalled = true;
                        aEvent.Set();
                    }

                    aParser.LogChunkReady += LogChunkReady;
                    aParser.HeaderReady += OnHeaderReady;
                    aParser.JobFinished += OnJobFinished;
                    aParser.Parse(aDef, aBuilder.ToString());
                    Assert.IsTrue(aEvent.WaitOne(5000));
                    aParser.HeaderReady -= OnHeaderReady;
                    aParser.LogChunkReady -= LogChunkReady;
                    aParser.JobFinished -= OnJobFinished;
                    Assert.AreEqual(11, aProgressCount);
                    Assert.IsTrue(aHeaderCalled);
                    Assert.IsTrue(aChunkCalled);
                    Assert.IsTrue(aCompleteCalled);
                }
            }
        }

        [TestMethod]
        [DataRow]
        [TestCategory(CategoryConstants.Basic)]
        public void TestLogMultiLineLine()
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            string aLogLine =
@"<----------------------------------
TestPorc
124
12
22:04:55,333
Domain
Function()
Trace Goes here.
---------------------------------->
<----------------------------------
TestPorc
156
12
22:04:57,333
Domain2
Function2()
Trace Goes here2.
---------------------------------->
";
            aDef = new LogDefinition();
            aSyntax = new LogSyntax();
            aSyntax.Id = 1;
            aSyntax.BeginMarker = "<----------------------------------";
            aSyntax.EndMarker = "---------------------------------->";
            aSyntax.SyntaxType = LogSyntaxType.Parent;
            {
                LogElement aElement = new LogElement();
                aElement.Name = "ProcessName";
                aElement.EndSeparator = "\r\n";
                aElement.Type = LogElementType.String;
                aSyntax.Elements.Add(aElement);
            }

            {
                LogElement aElement = new LogElement();
                aElement.Name = "PID";
                aElement.EndSeparator = "\r\n";
                aElement.Type = LogElementType.Number;
                aSyntax.Elements.Add(aElement);
            }

            {
                LogElement aElement = new LogElement();
                aElement.Name = "TID";
                aElement.EndSeparator = "\r\n";
                aElement.Type = LogElementType.Number;
                aSyntax.Elements.Add(aElement);
            }

            {
                LogElement aElement = new LogElement();
                aElement.Name = "Time";
                aElement.EndSeparator = "\r\n";
                aElement.Type = LogElementType.Time;
                aElement.DateTimeFormat = "HH:mm:ss,fff";
                aSyntax.Elements.Add(aElement);
            }

            {
                LogElement aElement = new LogElement();
                aElement.Name = "Domain";
                aElement.EndSeparator = "\r\n";
                aElement.Type = LogElementType.String;
                aSyntax.Elements.Add(aElement);
            }

            {
                LogElement aElement = new LogElement();
                aElement.Name = "Function";
                aElement.EndSeparator = "\r\n";
                aElement.Type = LogElementType.String;
                aSyntax.Elements.Add(aElement);
            }

            {
                LogElement aElement = new LogElement();
                aElement.Name = "Trace";
                aElement.EndSeparator = "\r\n";
                aElement.Type = LogElementType.String;
                aSyntax.Elements.Add(aElement);
            }

            aDef.Syntaxes.Add(aSyntax);
            UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId());
            aParser.Parse(aDef, aLogLine);
            Assert.AreEqual(File.ReadAllText($"{TestUtils.TestFolder}/{nameof(UniversalLogParserTests)}.{nameof(TestLogSingleLine)}.json"), SerilizationUtils.SerilizeToJsonReadable(aParser.LogGraph.SemanticLogs));
        }

        [TestMethod]
        [DataRow]
        [TestCategory(CategoryConstants.Basic)]
        public void TestLogSingleLineDuplicateSpaces()
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out string aLogLine);
            aLogLine = "TestPorc  124  12  22:04:55,333        Domain        Function() - Trace Goes here.\nTestPorc  156   12     22:04:57,333      Domain2 Function2() - Trace Goes here2.";
            aDef.Syntaxes.Add(aSyntax);
            {
                using (UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
                {
                    aParser.Parse(aDef, aLogLine);
                    Assert.AreEqual(File.ReadAllText($"{TestUtils.TestFolder}/{nameof(UniversalLogParserTests)}.{nameof(TestLogSingleLine)}.json"), SerilizationUtils.SerilizeToJsonReadable(aParser.LogGraph.SemanticLogs));
                }
            }
        }

        [TestMethod]
        [DataRow]
        public void TestLogSingleLineDuplicateSpacesWithBeginMarker()
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out string aLogLine);
            aSyntax.BeginMarker = " ";
            aLogLine = " TestPorc  124  12  22:04:55,333        Domain        Function() - Trace Goes here.\n TestPorc  156   12     22:04:57,333      Domain2 Function2() - Trace Goes here2.";
            aDef.Syntaxes.Add(aSyntax);
            {
                using (UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
                {
                    aParser.Parse(aDef, aLogLine);
                    Assert.AreEqual(File.ReadAllText($"{TestUtils.TestFolder}/{nameof(UniversalLogParserTests)}.{nameof(TestLogSingleLine)}.json"), SerilizationUtils.SerilizeToJsonReadable(aParser.LogGraph.SemanticLogs));
                }
            }
        }

        [TestMethod]
        [DataRow(1000)]
        [DataRow(10000)]
        [DataRow(100000)]
        [DataRow(1000000)]
        [TestCategory("BenchMark")]
        public void TestLogSingleLineBenchMark(int theSampleCount)
        {
            StringBuilder aBuilder = new StringBuilder();
            foreach (var aNumber in Enumerable.Range(1, theSampleCount))
            {
                aBuilder.AppendLine($"TestPorc 124 12 22:04:55,333 Domain Function{aNumber}() - Trace Goes here {0}.");
            }

            {
                Stopwatch aWatch = Stopwatch.StartNew();
                LogDefinition aDef;
                LogSyntax aSyntax;
                GetDefaultDefinition(out aDef, out aSyntax, out _);
                aDef.Syntaxes.Add(aSyntax);
                IRawLogStore aStoreMock = new FastRawLogMock();
                using (UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId(), aStoreMock))
                {
                    aParser.Parse(aDef, aBuilder.ToString());
                    aWatch.Stop();
                    float aExpected = theSampleCount * 0.003f;
                    Console.WriteLine($"Total Time In Parsing {aWatch.ElapsedMilliseconds}ms Expected {aExpected}");
                    Assert.AreEqual(theSampleCount, aParser.LogGraph.SemanticLogs.Count);
                    Assert.IsTrue((int)aExpected + 100 > (int)aWatch.ElapsedMilliseconds, $"Expected {aExpected} TheActual {aWatch.ElapsedMilliseconds}");
                }
            }
        }


        [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        [DataRow("|", "Pipe")]
        public void AutoDetectionTestForLogsWtihHeader(string theSep, string theRealName)
        {
            StringBuilder aBuilder = new StringBuilder();
            aBuilder.AppendLine(string.Format("Some random header trace", theSep));
            aBuilder.AppendLine(string.Format("=====================", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            LogDefinition aDef = new LogDefinition();
            aDef.LogFileType = LogDataType.Auto;
            using (UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
            {
                aParser.Parse(aDef, aBuilder.ToString());
                Assert.AreEqual(10, aParser.LogGraph.SemanticLogs.Count);
                Assert.AreEqual(6, aParser.LogGraph.SemanticLogs[1].Count);
                Assert.AreEqual(6, aParser.LogGraph.SemanticLogs[10].Count);
            }
        }

        [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        [DataRow("|", "Pipe")]
        public void AutoDetectionTestForLogs(string theSep, string theRealName)
        {
            StringBuilder aBuilder = new StringBuilder();
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            aBuilder.AppendLine(string.Format("Col1{0} Col2{0} Col{0}", theSep));
            LogDefinition aDef = new LogDefinition();
            aDef.LogFileType = LogDataType.Auto;
            using (UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
            {

                void HeaderReady(object sender, ILogHeader e)
                {
                    Assert.IsTrue(e.LogDefinition.AutoDetected);
                }

                aParser.HeaderReady += HeaderReady;
                aParser.Parse(aDef, aBuilder.ToString());
                aParser.HeaderReady -= HeaderReady;
                Assert.AreEqual(10, aParser.LogGraph.SemanticLogs.Count);
                Assert.AreEqual(6, aParser.LogGraph.SemanticLogs[1].Count);
                Assert.AreEqual(6, aParser.LogGraph.SemanticLogs[10].Count);
            }
        }


        [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        public void FirstParseFailureOrHeaderCase()
        {
            StringBuilder aBuilder = new StringBuilder();
            aBuilder.AppendLine($"This Line's Parsing Shall Fail");
            aBuilder.AppendLine($"=============================");
            aBuilder.AppendLine($"TestPorc 124 12 22:04:55,333 Domain Function1() - Trace Goes here");
            aBuilder.AppendLine($"TestPorc 124 12 22:04:55,333 Domain Function1() - Trace Goes here");
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out _);
            aDef.Syntaxes.Add(aSyntax);
            using (UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
            {
                aParser.Parse(aDef, aBuilder.ToString());
                Assert.AreEqual(2, aParser.LogGraph.SemanticLogs.Count);
                Assert.AreEqual(10, aParser.LogGraph.SemanticLogs[1].Count);
                Assert.AreEqual(10, aParser.LogGraph.SemanticLogs[2].Count);
            }
        }

        [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        [TestCategory(CategoryConstants.Progress)]
        public void VerifyIfAllLogsAreTransferredStartAndEndWithProgressiveLoading()
        {
            using (ILogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
            {
                LogHeader aLogHeader = new LogHeader();
                int aTotalElements = 0;
                void AParser_LogChunkReady(object sender, ProgressData e)
                {
                    aTotalElements += e.LineRange.End.Value - e.LineRange.Start.Value;
                }

                aParser.LogChunkReady += AParser_LogChunkReady;
                aParser.Parse(
                    SerilizationUtils.DeSerilizeFromJson<LogDefinition>(File.ReadAllText($"{TestUtils.TestFolder}/DefaultSingleLineDefinition.yaml")),
                    File.ReadAllText($"{TestUtils.TestFolder}/DefaultSingleLine.log"));
                aParser.LogChunkReady -= AParser_LogChunkReady;

                Assert.AreEqual(48, aTotalElements);
            }
        }


        [TestMethod]
        [TestCategory("WorstCase")]
        public void TestForHugeLogs()
        {
            {
                Stopwatch aWatch = Stopwatch.StartNew();
                var aDef = new LogDefinition();
                var aSyntax = new LogSyntax();
                aSyntax.Id = 1;
                aSyntax.BeginMarker = " ";
                aSyntax.EndMarker = "\n";
                aSyntax.SyntaxType = LogSyntaxType.Parent;
                {
                    {
                        LogElement aElement = new LogElement();
                        aElement.Name = "ProcessName";
                        aElement.EndSeparator = " ";
                        aElement.Type = LogElementType.String;
                        aSyntax.Elements.Add(aElement);
                    }

                    {
                        LogElement aElement = new LogElement();
                        aElement.Name = "PID";
                        aElement.EndSeparator = " ";
                        aElement.Type = LogElementType.Number;
                        aSyntax.Elements.Add(aElement);
                    }

                    {
                        LogElement aElement = new LogElement();
                        aElement.Name = "TID";
                        aElement.EndSeparator = " ";
                        aElement.Type = LogElementType.String;
                        aSyntax.Elements.Add(aElement);
                    }

                    {
                        LogElement aElement = new LogElement();
                        aElement.Name = "SessionId";
                        aElement.EndSeparator = " ";
                        aElement.Type = LogElementType.String;
                        aSyntax.Elements.Add(aElement);
                    }

                    {
                        LogElement aElement = new LogElement();
                        aElement.Name = "SubSessionId";
                        aElement.EndSeparator = " ";
                        aElement.Type = LogElementType.String;
                        aSyntax.Elements.Add(aElement);
                    }

                    {
                        LogElement aElement = new LogElement();
                        aElement.Name = "SessionMode";
                        aElement.EndSeparator = " ";
                        aElement.Type = LogElementType.String;
                        aSyntax.Elements.Add(aElement);
                    }

                    {
                        LogElement aElement = new LogElement();
                        aElement.Name = "Time";
                        aElement.EndSeparator = " ";
                        aElement.Type = LogElementType.DateTime;
                        aElement.DateTimeFormat = "HH:mm:ss,fff";
                        aSyntax.Elements.Add(aElement);
                    }

                    {
                        LogElement aElement = new LogElement();
                        aElement.Name = "Level";
                        aElement.EndSeparator = " ";
                        aElement.Type = LogElementType.String;
                        aSyntax.Elements.Add(aElement);
                    }

                    {
                        LogElement aElement = new LogElement();
                        aElement.Name = "Elapsed";
                        aElement.EndSeparator = " ";
                        aElement.Type = LogElementType.String;
                        aSyntax.Elements.Add(aElement);
                    }

                    {
                        LogElement aElement = new LogElement();
                        aElement.Name = "Domain";
                        aElement.EndSeparator = " ";
                        aElement.Type = LogElementType.String;
                        aSyntax.Elements.Add(aElement);
                    }

                    {
                        LogElement aElement = new LogElement();
                        aElement.Name = "Function";
                        aElement.EndSeparator = " - ";
                        aElement.Type = LogElementType.String;
                        aSyntax.Elements.Add(aElement);
                    }

                    {
                        LogElement aElement = new LogElement();
                        aElement.Name = "Trace";
                        aElement.EndSeparator = null;
                        aElement.Type = LogElementType.String;
                        aSyntax.Elements.Add(aElement);
                    }
                }

                aDef.Syntaxes.Add(aSyntax);
                File.WriteAllText("D:/AxisTemplate.yaml", SerilizationUtils.SerilizeToJsonReadable(aDef));
                using (UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
                {
                    string aFileData = File.ReadAllText(@"D:\AXIS_ERR-20210211.1.LOG");
                    aParser.Parse(aDef, aFileData);
                    aWatch.Stop();
                    Assert.AreEqual(4638898, aParser.LogGraph.SemanticLogs.Count);
                    Console.WriteLine($"Total Time In Parsing {aWatch.ElapsedMilliseconds}ms For {aParser.LogGraph.SemanticLogs.Count}");
                }
            }
        }


        public static void GetDefaultDefinition(out LogDefinition theDef, out LogSyntax theSyntax, out string theLogLine)
        {
            theLogLine = "TestPorc 124 12 22:04:55,333 Domain Function() - Trace Goes here.\nTestPorc 156 12 22:04:57,333 Domain2 Function2() - Trace Goes here2.";
            theDef = new LogDefinition();
            theSyntax = new LogSyntax();
            theSyntax.Id = 1;
            theSyntax.BeginMarker = string.Empty;
            theSyntax.EndMarker = "\n";
            theDef.LogFileType = LogDataType.PlainText;
            theSyntax.SyntaxType = LogSyntaxType.Parent;
            {
                LogElement aElement = new LogElement();
                aElement.Name = "ProcessName";
                aElement.EndSeparator = " ";
                aElement.Type = LogElementType.String;
                theSyntax.Elements.Add(aElement);
            }

            {
                LogElement aElement = new LogElement();
                aElement.Name = "PID";
                aElement.EndSeparator = " ";
                aElement.Type = LogElementType.Number;
                theSyntax.Elements.Add(aElement);
            }

            {
                LogElement aElement = new LogElement();
                aElement.Name = "TID";
                aElement.EndSeparator = " ";
                aElement.Type = LogElementType.Number;
                theSyntax.Elements.Add(aElement);
            }

            {
                LogElement aElement = new LogElement();
                aElement.Name = "Time";
                aElement.EndSeparator = " ";
                aElement.Type = LogElementType.Time;
                aElement.DateTimeFormat = "HH:mm:ss,fff";
                theSyntax.Elements.Add(aElement);
            }

            {
                LogElement aElement = new LogElement();
                aElement.Name = "Domain";
                aElement.EndSeparator = " ";
                aElement.Type = LogElementType.String;
                theSyntax.Elements.Add(aElement);
            }

            {
                LogElement aElement = new LogElement();
                aElement.Name = "Function";
                aElement.EndSeparator = " - ";
                aElement.Type = LogElementType.String;
                theSyntax.Elements.Add(aElement);
            }

            {
                LogElement aElement = new LogElement();
                aElement.Name = "Trace";
                aElement.EndSeparator = null;
                aElement.Type = LogElementType.String;
                theSyntax.Elements.Add(aElement);
            }
        }
    }

    internal class FastRawLogMock : IRawLogStore
    {
        public Stream Writer { get; private set; } = new MemoryStream();

        public void Dispose()
        {
            this.Writer.Dispose();
        }

        public bool GetString(int theRangeId, out string theString)
        {
            theString = string.Empty;
            return true;
        }

        public void LoadStore(in string theFallbackString)
        {
        }

        public void ResetStore()
        {
        }

        public bool StoreRange(int theRangeId, int theStartIndex, int theEndIndex)
        {
            return true;
        }

        public bool UpdateRange(int theRangeId, int theEndIndex)
        {
            return true;
        }
    }
}
