namespace Flex.LVA.Core.Interfaces
{
    public interface IEngine : IDisposable
    {
        void PrepareResources(string theLogDefiniton);

        void PrepareLog();
       
        bool GetRawLog(int theLogId, out string theRawLog);

        bool GetRawLogs(IList<int> theLogIds, out string theRawLogs);
        
        public void AppendLogChunk(IEnumerable<byte> theChunk);
    }
}