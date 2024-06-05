using Flex.LVA.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NodeTypeResolvers;

namespace Flex.LVA.Core.IntellisenseLogs
{
    internal class IntellisenseLogs
    {
        public void RangedDetect(IList<string> theFewLogLines, out LogDefinition theDef, out string theEstimatedSep)
        {
            IDictionary<string, int> aWeights = new Dictionary<string, int>();
            IDictionary<string, LogDefinition> aDefs = new Dictionary<string, LogDefinition>();
            foreach (var item in theFewLogLines)
            {
                if (!this.DetectLogDefinitionUniform(item, out LogDefinition aDef, out string aEstimated))
                {
                    continue;
                }

                aDefs[aEstimated] = aDef;
                if (aWeights.ContainsKey(aEstimated))
                {
                    aWeights[aEstimated]++;
                }
                else
                {
                    aWeights[aEstimated] = 1;
                }
            }

            theEstimatedSep = aWeights.MaxBy(kvp => kvp.Value).Key;
            theDef = aDefs[theEstimatedSep];
        }

        public bool DetectLogDefinitionUniform(string theSingleLogLine, out LogDefinition theDef, out string theEstimatedSep)
        {
            theDef = null;
            theEstimatedSep = null;
            theSingleLogLine = theSingleLogLine.TrimEnd();
            if (string.IsNullOrEmpty(theSingleLogLine))
            {
                return false;
            }

            int aColCount = 0;
            IDictionary<string, string> delimiterMap = new Dictionary<string, string>()
            {
                { " ", "Space" } ,
                { "/", "Slash"},
                { "|", "Pipe"},
                { ",", "Comma"} ,
                { "\t", "Tab"}
            };

            string aSelectedDelimiter = string.Empty;
            IList<(string, int)> aWeights = new List<(string, int)>();
            foreach (string aSep in delimiterMap.Keys)
            {
                aSelectedDelimiter = aSep;
                string[] aSplits = theSingleLogLine.Split(aSep, StringSplitOptions.RemoveEmptyEntries);
                aWeights.Add((aSep, aSplits.Length));
            }

            aWeights = aWeights.OrderByDescending(theX => theX.Item2).ToList();
            aWeights = aWeights.Where(theX => theX.Item2 == aWeights.Max(theX => theX.Item2)).ToList();
            aSelectedDelimiter = RunPriorityCheck(aWeights);
            theDef = new LogDefinition();
            theDef.Name = $"AutoDetect_{delimiterMap[aSelectedDelimiter]}_Delimited";
            theDef.Syntaxes.Add(new LogSyntax());
            if (theSingleLogLine.StartsWith(aSelectedDelimiter))
            {
                theDef.Syntaxes[0].BeginMarker = aSelectedDelimiter;
            }
            else
            {
                theDef.Syntaxes[0].BeginMarker = string.Empty;
            }

            theDef.Syntaxes[0].EndMarker = "\n";
            theDef.Syntaxes[0].SyntaxType = LogSyntaxType.Parent;
            int aColIndex = 1;
            foreach (var _ in theSingleLogLine.Split(aSelectedDelimiter, StringSplitOptions.RemoveEmptyEntries))
            {
                LogElement aElement = new LogElement();
                aElement.Name = $"Column {aColIndex}";
                aElement.EndSeparator = aSelectedDelimiter;
                aElement.Type = LogElementType.String;
                theDef.Syntaxes[0].Elements.Add(aElement);
                aColIndex++;
            }

            if (aColIndex > 1)
            {
                theDef.Syntaxes[0].Elements.Last().EndSeparator = null;
            }

            theEstimatedSep = aSelectedDelimiter;
            return true;
        }

        private static string RunPriorityCheck(IList<(string, int)> aWeights)
        {
            string aDelimiter = null;
            foreach (var aItem in new string[] { ",", "|", "\t", "/", " " })
            {
                try
                {
                    (string, int) aWeight = aWeights.First(theX => theX.Item1 == aItem);
                    aDelimiter = aItem;
                    break;
                }
                catch (InvalidOperationException)
                {
                    continue;
                }
            }

            return aDelimiter;
        }

        public void DetectLogDefinition(string logLine, string traceLine, out LogDefinition theDef)
        {
            int aColCount = 0;
            string[] aSplitLogLine = logLine.Split(traceLine);
            IDictionary<string, string> delimiterMap = new Dictionary<string, string>()
            {
                { " ", "Space" } ,
                { "/", "Slash"},
                { "|", "Pipe"},
                { ",", "Comma"} ,
                { "\t", "Tab"}
            };

            List<string> aDelimiters = new List<string>(delimiterMap.Keys);
            string aSelectedDelimiter = aDelimiters[0];
            foreach (var currentDelimeter in aDelimiters)
            {
                var splitColumn = aSplitLogLine[0].Split(currentDelimeter, StringSplitOptions.RemoveEmptyEntries);
                if (splitColumn.Length > aColCount)
                {
                    aColCount = splitColumn.Length;
                    aSelectedDelimiter = currentDelimeter;
                }
            }

            string[] aSplitNoEmpty = aSplitLogLine[0].Split(aSelectedDelimiter, StringSplitOptions.RemoveEmptyEntries);
            theDef = new LogDefinition();
            theDef.Name = $"AutoDetect_{delimiterMap[aSelectedDelimiter]}";
            theDef.Syntaxes.Add(new LogSyntax());
            if (logLine.StartsWith(aSelectedDelimiter))
            {
                theDef.Syntaxes[0].BeginMarker = aSelectedDelimiter;
            }
            else
            {
                theDef.Syntaxes[0].BeginMarker = string.Empty;
            }

            theDef.Syntaxes[0].EndMarker = "\n";
            theDef.Syntaxes[0].SyntaxType = LogSyntaxType.Parent;
            int aColIndex = 1;
            foreach (var _ in aSplitNoEmpty)
            {
                LogElement aElement = new LogElement();
                aElement.Name = $"Column {aColIndex}";
                aElement.EndSeparator = aSelectedDelimiter;
                aElement.Type = LogElementType.String;
                theDef.Syntaxes[0].Elements.Add(aElement);
            }
        }
    }
}
