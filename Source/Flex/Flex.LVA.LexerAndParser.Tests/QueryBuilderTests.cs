namespace Flex.LVA.LexerAndParser.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Flex.LVA.Core;
    using Flex.LVA.Core.Interfaces;
    using Flex.LVA.Shared;
    using Moq;

    [TestClass]
    public class QueryBuilderTests
    {
        private static int myEngineTestId = 5000;

        [TestMethod]
        [DataRow(Constants.Header, "True", true, true)]
        [DataRow("TID", 12, 12, 12)]
        [DataRow("TID", "12", 12, 12)]
        [DataRow("Trace", "Trace Goes", "Trace Goes here 50ms.", null, 1)]
        [DataRow("Trace", "Trace 90", "Trace 90ms on opcode took here here2.", null, 1)]
        public void TestEqualQueryBuilder(string theHeader, dynamic theEqualityComparator, dynamic theExpected1, dynamic theExpected2, int theExpectedCount = 2)
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out string aLogLine);
            aDef.Syntaxes.Add(aSyntax);
            UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId());
            aParser.Parse(aDef, aLogLine);
            QueryEngine aEngine = new QueryEngine(aParser.LogGraph);
            QueryBuilder aBuilder = new QueryBuilder(aEngine);
            aBuilder.Build($"select '{theHeader}' contains '{theEqualityComparator}'");
            IReadOnlyCollection<object> aResults = aBuilder.Execute();
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
        [DataRow(Constants.Header, "false", true, true)]
        [DataRow("TID", 1, 12, 12)]
        [DataRow("TID", "1", 12, 12)]
        [DataRow("Trace", "Trace", "Trace Goes here 50ms.", "Trace 90ms on opcode took here here2.", 2)]
        public void TestNotEqualQueryBuilder(string theHeader, dynamic theEqualityComparator, dynamic theExpected1, dynamic theExpected2, int theExpectedCount = 2)
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out string aLogLine);
            aDef.Syntaxes.Add(aSyntax);
            using (UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
            {
                aParser.Parse(aDef, aLogLine);
                QueryEngine aEngine = new QueryEngine(aParser.LogGraph);
                QueryBuilder aBuilder = new QueryBuilder(aEngine);
                aBuilder.Build($"select '{theHeader}' notequal '{theEqualityComparator}'");
                IReadOnlyCollection<object> aResults = aBuilder.Execute();
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
        }

        [TestMethod]
        [DataRow(Constants.Header, true, true, true)]
        [DataRow(Constants.Header, "true", true, true)]
        [DataRow("TID", 12, 12, 12)]
        [DataRow("TID", "12", 12, 12)]
        [DataRow("Trace", "Trace Goes here 50ms.", "Trace Goes here 50ms.", null, 1)]
        [DataRow("Trace", "Trace 90ms on opcode took here here2.", "Trace 90ms on opcode took here here2.", null, 1)]
        public void TestContainsQueryBuilder(string theHeader, dynamic theEqualityComparator, dynamic theExpected1, dynamic theExpected2, int theExpectedCount = 2)
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out string aLogLine);
            aDef.Syntaxes.Add(aSyntax);
            UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId());
            aParser.Parse(aDef, aLogLine);
            QueryEngine aEngine = new QueryEngine(aParser.LogGraph);
            QueryBuilder aBuilder = new QueryBuilder(aEngine);
            aBuilder.Build($"select '{theHeader}' equals '{theEqualityComparator}'");
            IReadOnlyCollection<object> aResults = aBuilder.Execute();
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
        public void TestGreaterQueryBuilder(string theHeader, dynamic theEqualityComparator, dynamic theExpected1, dynamic theExpected2, int theExpectedCount = 2)
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out string aLogLine);
            aDef.Syntaxes.Add(aSyntax);
            UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId());
            aParser.Parse(aDef, aLogLine);
            QueryEngine aEngine = new QueryEngine(aParser.LogGraph);
            QueryBuilder aBuilder = new QueryBuilder(aEngine);
            aBuilder.Build($"select '{theHeader}' greater '{theEqualityComparator}'");
            IReadOnlyCollection<object> aResults = aBuilder.Execute();
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
        public void TestGreaterOrEqualQueryBuilder(string theHeader, dynamic theEqualityComparator, dynamic theExpected1, dynamic theExpected2, int theExpectedCount = 2)
        {
            LogDefinition aDef;
            LogSyntax aSyntax;
            GetDefaultDefinition(out aDef, out aSyntax, out string aLogLine);
            aDef.Syntaxes.Add(aSyntax);
            UniversalLogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId());
            aParser.Parse(aDef, aLogLine);
            QueryEngine aEngine = new QueryEngine(aParser.LogGraph);
            QueryBuilder aBuilder = new QueryBuilder(aEngine);
            aBuilder.Build($"select '{theHeader}' greaterorequal '{theEqualityComparator}'");
            IReadOnlyCollection<object> aResults = aBuilder.Execute();
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
            theDef.LogFileType = LogDataType.PlainText;
            theSyntax = new LogSyntax();
            theSyntax.Id = 1;
            theSyntax.BeginMarker = string.Empty;
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
                aElement.EndSeparator = " ";
                aElement.DateTimeFormat = "HH:mm:ss,fff";
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
