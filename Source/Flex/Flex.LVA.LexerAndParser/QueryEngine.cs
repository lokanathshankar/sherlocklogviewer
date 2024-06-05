using Flex.LVA.Shared;
using Flex.LVA.Shared.Containers;

namespace Flex.LVA.Core
{
    public class QueryEngine
    {
        private readonly ILogGraph myLogGraph;

        private IEnumerable<object> myCurrentQuery;

        public QueryEngine(ILogGraph theLogGraph)
        {
            myLogGraph = theLogGraph;
        }

        public IReadOnlyCollection<object> Execute()
        {
            return myCurrentQuery.ToList();
        }

        public QueryEngine Greater(object theValue)
        {
            myCurrentQuery = myCurrentQuery.Where(theX => ((LogContainer)theX).Greater(theValue));
            return this;
        }

        public QueryEngine GreaterOrEqual(object theValue)
        {
            myCurrentQuery = myCurrentQuery.Where(theX => ((LogContainer)theX).GreaterOrEqual(theValue));
            return this;
        }

        public QueryEngine Lesser(object theValue)
        {
            myCurrentQuery = myCurrentQuery.Where(theX => ((LogContainer)theX).Lesser(theValue));
            return this;
        }

        public QueryEngine LesserOrEqual(object theValue)
        {
            myCurrentQuery = myCurrentQuery.Where(theX => ((LogContainer)theX).LesserOrEqual(theValue));
            return this;
        }

        public QueryEngine Same(object theValue)
        {
            myCurrentQuery = myCurrentQuery.Where(theX => ((LogContainer)theX).Same(theValue));
            return this;
        }

        public QueryEngine NotSame(object theValue)
        {
            myCurrentQuery = myCurrentQuery.Where(theX => ((LogContainer)theX).NotSame(theValue));
            return this;
        }

        public QueryEngine Contains(object theValue)
        {
            myCurrentQuery = myCurrentQuery.Where(theX => ((LogContainer)theX).Contains(theValue));
            return this;
        }

        public QueryEngine Select(string theColumnName)
        {
            myCurrentQuery = myLogGraph.SemanticLogs.Values.Select(theX => theX[myLogGraph.GraphHeader.Index(theColumnName)]);
            return this;
        }

        internal void Clear()
        {
            myCurrentQuery = null;
        }
    }
}
