namespace Flex.LVA.Shared
{
    public class LogDefinition
    {
        public string Name { get; set; }

        public int? HeaderLineCount { get; set; }

        public List<LogSyntax> Syntaxes { get; set; } = new List<LogSyntax>();

        public LogDataType LogFileType {get; set; } = LogDataType.Auto;
        public bool AutoDetected {get; set; } = false;

        /// <summary>
        /// Update TS ConcertEnglishMarkersToCSharp as well
        /// </summary>
        /// <returns></returns>
        public LogDefinition ConvertEnglishMarkersToCSharp()
        {
            foreach (LogSyntax aItem in Syntaxes)
            {
                if(aItem.BeginMarker == null)
                {
                    aItem.BeginMarker = string.Empty;
                }

                if(aItem.EndMarker.Equals("newline", StringComparison.OrdinalIgnoreCase))
                {
                    aItem.EndMarker = "\n";
                }

                if (aItem.BeginMarker.Equals("newline", StringComparison.OrdinalIgnoreCase))
                {
                    aItem.BeginMarker = "\n";
                }
            }

            foreach (LogElement aElement in Syntaxes.SelectMany(theX => theX.Elements))
            {
                if(string.IsNullOrEmpty(aElement.EndSeparator))
                {
                    continue;
                }

                if (aElement.EndSeparator.Trim().Equals("tab", StringComparison.OrdinalIgnoreCase))
                {
                    aElement.EndSeparator = "\t";
                }

                if (aElement.EndSeparator.Trim().Equals("newline", StringComparison.OrdinalIgnoreCase))
                {
                    aElement.EndSeparator = "\n";
                }
            }

            return this;
        }
    }

    public class LogSyntax
    {
        public int Id { get; set; }

        public string BeginMarker { get; set; } = string.Empty;

        public string EndMarker { get; set; } = string.Empty;

        public LogSyntaxType SyntaxType { get; set; } = LogSyntaxType.Parent;
        
        public List<LogElement> Elements { get; set; } = new List<LogElement>();
    }

    public class LogElement
    {
        public string Name { get; set; }

        public string EndSeparator { get; set; }

        public LogElementType Type { get; set; }

        public int? ChildSyntaxId { get; set; }

        public string DateTimeFormat  { get; set; }
    }
}
