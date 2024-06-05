using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Flex.LVA.Shared
{
    public class SymanticLogs
    {
        [JsonPropertyName("logs")]
        public List<List<object>> Logs { get; set; }
    }

    public class LogHeader : ILogHeader
    {
        [JsonPropertyName("columnNames")]
        public List<string> ColumnNames { get; } = new List<string>();


        [JsonPropertyName("columnTypes")]
        public List<LogElementType> ColumnTypes { get; } = new List<LogElementType>();

        IReadOnlyList<string> ILogHeader.ColumnNames => new ReadOnlyCollection<string>(ColumnNames);

        IReadOnlyList<LogElementType> ILogHeader.ColumnTypes => new ReadOnlyCollection<LogElementType>(ColumnTypes);

        [JsonPropertyName("logDefinition")]
        public LogDefinition LogDefinition { get; set; }

        private IDictionary<string, int> myIndexes = new Dictionary<string, int>();

        public static LogHeader CreateLogHeader(ILogHeader theLogHeader)
        {
            LogHeader aHeader = new LogHeader();
            for (int aIndex = 0; aIndex < theLogHeader.ColumnNames.Count; aIndex++)
            {
                aHeader.Add(theLogHeader.ColumnNames[aIndex], theLogHeader.ColumnTypes[aIndex]);
            }

            aHeader.LogDefinition = theLogHeader.LogDefinition;
            return aHeader;
        }

        public void Add(string theColumnName, LogElementType theElementType)
        {
            myIndexes.Add(theColumnName, ColumnNames.Count);
            ColumnNames.Add(theColumnName);
            ColumnTypes.Add(theElementType);
        }

        public int Index(string theColumnName)
        {
            return myIndexes[theColumnName];
        }

        public void Clear()
        {
            this.myIndexes.Clear();
            this.ColumnNames.Clear();
            this.ColumnTypes.Clear();
        }
    }

    public interface ILogHeader
    {
        public IReadOnlyList<string> ColumnNames { get; }
        
        public IReadOnlyList<LogElementType> ColumnTypes { get; }

        public LogDefinition LogDefinition { get; set; }
        
        public int Index(string theColumnName);
    }
}
