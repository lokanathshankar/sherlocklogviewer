namespace Flex.LVA.LexerAndParser.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    using Flex.LVA.Core;
    using Flex.LVA.Core.Interfaces;
    using Flex.LVA.Shared;
    using Moq;

    [TestClass]
    public class QueryCapabilityTests
    {
        private static int myEngineTestId = 15000;

        [TestMethod]
        [DataRow(1000, Constants.Header, true)]
        [DataRow(10000, Constants.Header, true)]
        [DataRow(100000, Constants.Header, true)]
        [DataRow(1000000, Constants.Header, true)]
        [DataRow(1000, "ProcessName", "LongProcessNameLongProcessNameLongProcessNameLongProcessName")]
        [DataRow(10000, "ProcessName", "LongProcessNameLongProcessNameLongProcessNameLongProcessName")]
        [DataRow(100000, "ProcessName", "LongProcessNameLongProcessNameLongProcessNameLongProcessName")]
        [DataRow(1000000, "ProcessName", "LongProcessNameLongProcessNameLongProcessNameLongProcessName")]
        [TestCategory("BenchMark")]
        public void TestEqualsQueryStringBenchMark(int theSampleCount, string theHeader, dynamic theEqualityComparator)
        {
            StringBuilder aBuilder = new StringBuilder();
            foreach (var aNumber in Enumerable.Range(1, theSampleCount))
            {
                aBuilder.AppendLine($"LongProcessNameLongProcessNameLongProcessNameLongProcessName 124 12 22:04:55,333 Domain Function{aNumber}() - Trace Goes here {0}.");
            }

            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out _);
            aDef.Syntaxes.Add(aSyntax);
            UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId());
            aParser.Parse(aDef, aBuilder.ToString());
            Stopwatch aWatch = Stopwatch.StartNew();
            QueryEngine aEngine = new QueryEngine(aParser.LogGraph);
            IReadOnlyCollection<object> aResults = aEngine.Select(theHeader).Same(theEqualityComparator).Execute();
            Assert.AreEqual(aResults.Count, theSampleCount);
            aWatch.Stop();
            float aExpected = theSampleCount * 0.0001f;
            Console.WriteLine($"Total Time In Parsing {aWatch.ElapsedMilliseconds}ms Expected {aExpected}");
            Assert.IsTrue((int)aExpected + 100 > (int)aWatch.ElapsedMilliseconds, $"Expected {aExpected} TheActual {aWatch.ElapsedMilliseconds}");
        }

        [TestMethod]
        [DataRow(Constants.Header, true, true, true)]
        [DataRow(Constants.Header, "true", true, true)]
        [DataRow("TID", 12, 12, 12)]
        [DataRow("TID", "12", 12, 12)]
        [DataRow("Trace", "Trace Goes here 50ms.", "Trace Goes here 50ms.", null, 1)]
        [DataRow("Trace", "Trace 90ms on opcode took here here2.", "Trace 90ms on opcode took here here2.", null, 1)]
        public void TestEqualsQuery(string theHeader, dynamic theEqualityComparator, dynamic theExpected1, dynamic theExpected2, int theExpectedCount = 2)
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out string aLogLine);
            aDef.Syntaxes.Add(aSyntax);
            UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId());
            aParser.Parse(aDef, aLogLine);
            QueryEngine aEngine = new QueryEngine(aParser.LogGraph);
            IReadOnlyCollection<object> aResults = aEngine.Select(theHeader).Same(theEqualityComparator).Execute();
            Assert.AreEqual(theExpectedCount, aResults.Count);
            if (theExpected1 != null)
            {
                Assert.AreEqual(theExpected1, (aResults.ElementAt(0) as dynamic).Value);
            }

            if (theExpected2 != null)
            {
                Assert.AreEqual(theExpected2, (aResults.ElementAt(1) as dynamic).Value);
            }
        }

        [TestMethod]
        [DataRow(Constants.Header, false, true, true)]
        [DataRow(Constants.Header, "false", true, true)]
        [DataRow("TID", 1, 12, 12)]
        [DataRow("TID", "2", 12, 12)]
        [DataRow("Trace", "Trace", "Trace Goes here 50ms.", "Trace 90ms on opcode took here here2.", 2)]
        public void TestNotEqualsQuery(string theHeader, dynamic theEqualityComparator, dynamic theExpected1, dynamic theExpected2, int theExpectedCount = 2)
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out string aLogLine);
            aDef.Syntaxes.Add(aSyntax);
            UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId());
            aParser.Parse(aDef, aLogLine);
            QueryEngine aEngine = new QueryEngine(aParser.LogGraph);
            IReadOnlyCollection<object> aResults = aEngine.Select(theHeader).NotSame(theEqualityComparator).Execute();
            Assert.AreEqual(theExpectedCount, aResults.Count);
            if (theExpected1 != null)
            {
                Assert.AreEqual(theExpected1, (aResults.ElementAt(0) as dynamic).Value);
            }

            if (theExpected2 != null)
            {
                Assert.AreEqual(theExpected2, (aResults.ElementAt(1) as dynamic).Value);
            }
        }

        [TestMethod]
        [DataRow(Constants.Header, "True", true, true)]
        [DataRow("TID", 11, 12, 12)]
        [DataRow("TID", "11", 12, 12)]
        [DataRow("ProcessName", "TestPorb", "TestPorc", "TestPorc")]
        public void TestGreaterQuery(string theHeader, dynamic theEqualityComparator, dynamic theExpected1, dynamic theExpected2, int theExpectedCount = 2)
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out string aLogLine);
            aDef.Syntaxes.Add(aSyntax);
            UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId());
            aParser.Parse(aDef, aLogLine);
            QueryEngine aEngine = new QueryEngine(aParser.LogGraph);
            IReadOnlyCollection<object> aResults = aEngine.Select(theHeader).Greater(theEqualityComparator).Execute();
            Assert.AreEqual(theExpectedCount, aResults.Count);
            if (theExpected1 != null)
            {
                Assert.AreEqual(theExpected1, (aResults.ElementAt(0) as dynamic).Value);
            }

            if (theExpected2 != null)
            {
                Assert.AreEqual(theExpected2, (aResults.ElementAt(1) as dynamic).Value);
            }
        }

        [TestMethod]
        [DataRow(Constants.Header, "True", true, true)]
        [DataRow("TID", 11, 12, 12)]
        [DataRow("TID", 12, 12, 12)]
        [DataRow("TID", "12", 12, 12)]
        [DataRow("TID", "12", 12, 12)]
        [DataRow("ProcessName", "TestPorb", "TestPorc", "TestPorc")]
        [DataRow("ProcessName", "TestPorc", "TestPorc", "TestPorc")]
        public void TestGreaterOrEqualQuery(string theHeader, dynamic theEqualityComparator, dynamic theExpected1, dynamic theExpected2, int theExpectedCount = 2)
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out string aLogLine);
            aDef.Syntaxes.Add(aSyntax);
            UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId());
            aParser.Parse(aDef, aLogLine);
            QueryEngine aEngine = new QueryEngine(aParser.LogGraph);
            IReadOnlyCollection<object> aResults = aEngine.Select(theHeader).GreaterOrEqual(theEqualityComparator).Execute();
            Assert.AreEqual(theExpectedCount, aResults.Count);
            if (theExpected1 != null)
            {
                Assert.AreEqual(theExpected1, (aResults.ElementAt(0) as dynamic).Value);
            }

            if (theExpected2 != null)
            {
                Assert.AreEqual(theExpected2, (aResults.ElementAt(1) as dynamic).Value);
            }
        }

        [TestMethod]
        [DataRow(Constants.Header, "True", true, true)]
        [DataRow("TID", 12, 12, 12)]
        [DataRow("TID", "12", 12, 12)]
        [DataRow("Trace", "Trace Goes", "Trace Goes here 50ms.", null, 1)]
        [DataRow("Trace", "Trace 90", "Trace 90ms on opcode took here here2.", null, 1)]
        public void TestContainsQuery(string theHeader, dynamic theEqualityComparator, dynamic theExpected1, dynamic theExpected2, int theExpectedCount = 2)
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out string aLogLine);
            aDef.Syntaxes.Add(aSyntax);
            UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId());
            LogGraphMutable aParsedLogs;
            aParser.Parse(aDef, aLogLine);
            QueryEngine aEngine = new QueryEngine(aParser.LogGraph);
            IReadOnlyCollection<object> aResults = aEngine.Select(theHeader).Contains(theEqualityComparator).Execute();
            Assert.AreEqual(theExpectedCount, aResults.Count);
            if (theExpected1 != null)
            {
                Assert.AreEqual(theExpected1, (aResults.ElementAt(0) as dynamic).Value);
            }

            if (theExpected2 != null)
            {
                Assert.AreEqual(theExpected2, (aResults.ElementAt(1) as dynamic).Value);
            }
        }

        private static void GetDefaultDefinition(out LogDefinition theDef, out LogSyntax theSyntax, out string theLogLine)
        {
            theLogLine = "TestPorc 124 12 22:04:55,333 Domain Function() - Trace Goes here 50ms.\nTestPorc 156 12 22:04:57,333 Domain2 Function2() - Trace 90ms on opcode took here here2.\n";
            theDef = new LogDefinition();
            theSyntax = new LogSyntax();
            theSyntax.Id = 1;
            theSyntax.BeginMarker = string.Empty;
            theDef.LogFileType = LogDataType.PlainText;
            theSyntax.EndMarker = "\n";
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
                aElement.DateTimeFormat = "HH:mm:ss,fff";
                aElement.EndSeparator = " ";
                aElement.Type = LogElementType.DateTime;
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
}
