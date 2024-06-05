namespace Flex.LVA.LexerAndParser.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Metadata;
    using System.Text;
    using Flex.LVA.Core;
    using Flex.LVA.Core.EngineManagement;
    using Flex.LVA.Core.Interfaces;
    using Flex.LVA.Shared;
    using Flex.LVA.Shared.Containers;
    using Moq;

    [TestClass]
    public class EngineTests
    {

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        [TestCategory(CategoryConstants.Basic)]
        public void DisposeChecks()
        {
            Mock<IRenderer> aRender = new Mock<IRenderer>();
            using (IEngine aEngine = new Engine(1, aRender.Object))
            {
                aEngine.Dispose();
                aEngine.Dispose();
                aEngine.AppendLogChunk(File.ReadAllBytes($"{TestUtils.TestFolder}/DefaultSingleLine.log"));
                aEngine.PrepareResources(File.ReadAllText($"{TestUtils.TestFolder}/DefaultSingleLineDefinition.yaml"));
                aEngine.PrepareLog();
            }
        }

        [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        public void ConstructionParsersCheck()
        {
            Mock<IRenderer> aRender = new Mock<IRenderer>();
            using (Engine aEngine = new Engine(1, aRender.Object))
            {
                var aParsers = new HashSet<ILogParser>(aEngine.Parsers.Values);
                Assert.AreEqual(6, aParsers.Count);
            }
        }

        [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        [TestCategory(CategoryConstants.Todo)]
        public void GetRawLogTest()
        {

        }


        public delegate void ParseDelegate(LogDefinition theDef, in string theLogString);

        [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        public void LogAppendingTests()
        {
            StringBuilder aBuilder = new StringBuilder();
            foreach (var item in Enumerable.Range(1, new Random().Next(10000)))
            {
                aBuilder.AppendLine($"TestPorc 124 12 22:04:55,333 Domain Function10() - Trace Goes here {item}");
            }

            const int aId = 100;
            Mock<IRenderer> aRender = new Mock<IRenderer>();
            Mock<ILogParser> aParser = new Mock<ILogParser>();
            aRender.Setup(theX => theX.NegotiateHeader(It.IsAny<ILogHeader>())).Returns(true);
            LogHeader aLogHeader = new LogHeader();
            aParser.Setup(theX => theX.LogGraph.GraphHeader).Returns(aLogHeader);
            using (var aEng = new Engine(aParser.Object, aId, aRender.Object))
            {
                aEng.AppendLogChunk(Encoding.UTF8.GetBytes(aBuilder.ToString()));
                aEng.PrepareResources(File.ReadAllText($"{TestUtils.TestFolder}/DefaultSingleLineDefinition.yaml"));
                aEng.PrepareLog();
                aParser.Raise(theX => theX.JobFinished += null, this, EventArgs.Empty);
                aParser.Verify(theX => theX.Parse(It.IsAny<LogDefinition>(), null), Times.Once);
                aParser.Verify(theX => theX.FreeUpMemory(), Times.Exactly(1));
                aParser.Verify(theX => theX.Reset(), Times.Exactly(1));
            }
        }

        [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        public void NegotiateHeaderTest()
        {
            Mock<IRenderer> aRender = new Mock<IRenderer>();
            Mock<ILogParser> aParser = new Mock<ILogParser>();
            aRender.Setup(theX => theX.NegotiateHeader(It.IsAny<ILogHeader>())).Returns(true);
            LogHeader aLogHeader = new LogHeader();
            aParser.Setup(theX => theX.LogGraph.GraphHeader).Returns(aLogHeader);
            using (var aEng = new Engine(aParser.Object, 1, aRender.Object))
            {
                aParser.Raise(theX => theX.HeaderReady += null, this, aLogHeader);
                aRender.Verify(theX => theX.NegotiateHeader(aLogHeader), Times.Once);
            }
        }

        [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        public void NegotiateData()
        {
            IList<int> aFailedLines = null;
            Mock<IRenderer> aRender = new Mock<IRenderer>();
            using (ILogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
            {
                aRender.Setup(theX => theX.NegotiateHeader(It.IsAny<ILogHeader>())).Returns(true);
                LogHeader aLogHeader = new LogHeader();
                aRender.Setup(theX => theX.NegotiateHeader(It.IsAny<ILogHeader>())).Returns(true);
                List<List<object>> aActual = new List<List<object>>();
                using (var aEng = new Engine(aParser, 1, aRender.Object))
                {
                    aEng.PrepareResources(File.ReadAllText($"{TestUtils.TestFolder}/DefaultSingleLineDefinition.yaml"));
                    aRender.Setup(theX => theX.NegotiateData(It.IsAny<List<List<object>>>(), out aFailedLines)).
                        Callback((List<List<object>> theData, out IList<int> _) =>
                        {
                            _ = null;
                            lock (aActual)
                            {
                                aActual.AddRange(theData);
                                for (int aJ = 0; aJ < aActual.Count; aJ++)
                                {
                                    var aRhs = aEng.Parsers[LogDataType.PlainText].LogGraph.SemanticLogs.ElementAt(aJ).Value;
                                    var aLhs = aActual[aJ];
                                    for (int aIndex = 0; aIndex < aLhs.Count; aIndex++)
                                    {
                                        if (!(aRhs[aIndex] as LogContainer).Equals(aLhs[aIndex]))
                                        {
                                            Assert.Fail("Data validation error");
                                        }
                                    }
                                }
                            }
                        }).Returns(true);

                    aRender.Setup(theX => theX.RenderLogs()).Callback(() =>
                    {
                        Assert.AreEqual(48, aEng.Parsers[LogDataType.PlainText].LogGraph.SemanticLogs.Count);
                        Assert.AreEqual(48, aActual.Count);
                    });

                    aParser.Parse(
                        SerilizationUtils.DeSerilizeFromJson<LogDefinition>(File.ReadAllText($"{TestUtils.TestFolder}/DefaultSingleLineDefinition.yaml")),
                        File.ReadAllText($"{TestUtils.TestFolder}/DefaultSingleLine.log"));
                }
            }
        }

        [TestMethod]
        [TestCategory(CategoryConstants.Basic)]
        [TestCategory(CategoryConstants.Progress)]
        public void NegotiateDataProgress()
        {
            IList<int> aFailedLines = null;
            Mock<IRenderer> aRender = new Mock<IRenderer>();
            using (ILogParser aParser = new UniversalLogParser(GeneralUtils.GetTestEngineId()))
            {
                aRender.Setup(theX => theX.NegotiateHeader(It.IsAny<ILogHeader>())).Returns(true);
                LogHeader aLogHeader = new LogHeader();
                aRender.Setup(theX => theX.NegotiateHeader(It.IsAny<ILogHeader>())).Returns(true);
                var aFullLogs = new List<List<object>>();
                using (AutoResetEvent aWaitEvent = new AutoResetEvent(false))
                {
                    using (var aEng = new Engine(aParser, 1, aRender.Object))
                    {
                        aEng.PrepareResources(File.ReadAllText($"{TestUtils.TestFolder}/DefaultSingleLineDefinition.yaml"));
                        aRender.Setup(theX => theX.NegotiateData(It.IsAny<List<List<object>>>(), out aFailedLines)).
                            Callback((List<List<object>> theData, out IList<int> _) =>
                            {
                                _ = null;
                                aFullLogs.AddRange(theData);
                            }).Returns(true);

                        aRender.Setup(theX => theX.RenderLogs()).Returns(() =>
                        {
                            aWaitEvent.Set();
                            return true;
                        });

                        aParser.Parse(
                            SerilizationUtils.DeSerilizeFromJson<LogDefinition>(File.ReadAllText($"{TestUtils.TestFolder}/DefaultSingleLineDefinition.yaml")),
                            File.ReadAllText($"{TestUtils.TestFolder}/DefaultSingleLine.log"));
                        aWaitEvent.WaitOne();
                        Assert.AreEqual(48, aFullLogs.Count);
                    }
                }
            }
        }

        private bool CompareDictionaries<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> theFirst, IReadOnlyDictionary<TKey, TValue> the2Nd)
        {

            IReadOnlyDictionary<TKey, TValue> aMax = the2Nd;
            IReadOnlyDictionary<TKey, TValue> aMin = theFirst;
            if (theFirst.Count > the2Nd.Count)
            {
                aMax = theFirst;
                aMin = the2Nd;
            }

            foreach (var aKvp in aMax)
            {
                if (!aMin.ContainsKey(aKvp.Key))
                {
                    return false;
                }

                if (!aKvp.Value.Equals(aMin[aKvp.Key]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CompareLogHeader(ILogHeader theHeader1, ILogHeader theHeader2)
        {
            return theHeader1.ColumnTypes.SequenceEqual(theHeader2.ColumnTypes) && theHeader1.ColumnNames.SequenceEqual(theHeader2.ColumnNames);
        }
    }
}
