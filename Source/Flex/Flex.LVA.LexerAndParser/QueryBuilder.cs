using Flex.LVA.Shared;

namespace Flex.LVA.Core
{
    public class QueryBuilder
    {
        private const char myQuote = '\'';
        private readonly IDictionary<string, Func<object, QueryEngine>> myTokenActions = new Dictionary<string, Func<object, QueryEngine>>();
        private readonly QueryEngine myQueryEngine;

        public QueryBuilder(QueryEngine theEngine)
        {
            myQueryEngine = theEngine;
            myTokenActions.Add("equals", myQueryEngine.Same);
            myTokenActions.Add("contains", myQueryEngine.Contains);
            myTokenActions.Add("greater", myQueryEngine.Greater);
            myTokenActions.Add("greaterorequal", myQueryEngine.GreaterOrEqual);
            myTokenActions.Add("lesser", myQueryEngine.Lesser);
            myTokenActions.Add("lesserorequal", myQueryEngine.LesserOrEqual);
            myTokenActions.Add("notequal", myQueryEngine.NotSame);
        }

        public void Build(string theQueryString)
        {
            myQueryEngine.Clear();
            // aBuilder.Build($"select '{theHeader}' equals '{theEqualityComparator}'");
            int aStartIndex = theQueryString.IndexOf("Select", StringComparison.OrdinalIgnoreCase);
            int aColumnNameStartIndex = theQueryString.IndexOf(myQuote, aStartIndex) + 1;
            int aColumnNameEndIndex = theQueryString.IndexOf(myQuote, aColumnNameStartIndex + 1);
            string aColumnName = theQueryString.Substring(aColumnNameStartIndex, aColumnNameEndIndex - aColumnNameStartIndex);
            int aPointerToNext = aColumnNameEndIndex + 1;
            myQueryEngine.Select(aColumnName);
            while (true)
            {
                string aToken = GetNextToken(theQueryString, aPointerToNext, out aPointerToNext);
                if(aToken == null)
                {
                    return;
                }

                if(!myTokenActions.TryGetValue(aToken, out var aActionToRun))
                {
                    throw new InvalidDataException($"Query Token {aToken} Unknown.");
                }

                string aInput = GetNextValue(theQueryString, aPointerToNext, out aPointerToNext);
                aActionToRun.Invoke(aInput);
            }
        
        }

        private string GetNextValue(string theQueryString, int theIndexToStartSearchFrom, out int theCurrentPosition)
        {
            theCurrentPosition = -1;
            if (theIndexToStartSearchFrom == -1)
            {
                return null;
            }

            int aStartIndex = theQueryString.IndexOf(myQuote, theIndexToStartSearchFrom);
            if(aStartIndex == -1)
            {
                return null;
            }

            aStartIndex = aStartIndex + 1;
            int aEndIndex = theQueryString.IndexOf(myQuote, aStartIndex);
            if (aEndIndex == -1)
            {
                return null;
            }

            return theQueryString.Substring(aStartIndex, aEndIndex - aStartIndex);
        }

        private string GetNextToken(string theQueryString, int theIndexToStartSearchFrom, out int theCurrentPosition)
        {
            theCurrentPosition = -1;
            if (theIndexToStartSearchFrom == -1)
            {
                return null;
            }

            int aStartIndex = theQueryString.IndexOf(' ', theIndexToStartSearchFrom);
            while (theQueryString[aStartIndex] == Constants.Space)
            {
                aStartIndex++;
            }

            int aEndIndex = theQueryString.IndexOf(' ', aStartIndex);
            if(aEndIndex == -1)
            {
                return null;
            }

            theCurrentPosition = aEndIndex;
            return theQueryString.Substring(aStartIndex, aEndIndex - aStartIndex).ToLowerInvariant();
        }

        public IReadOnlyCollection<object> Execute()
        {
            return myQueryEngine.Execute();
        }
    }
}
