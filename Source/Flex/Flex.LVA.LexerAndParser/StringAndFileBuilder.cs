using Flex.LVA.Core.EngineManagement;
using Flex.LVA.Core.Interfaces;
using Flex.LVA.Logging;
using System.Text;

namespace Flex.LVA.Core
{
    public class StringAndFileBuilder
    {
        protected StringBuilder myBuilder = new StringBuilder();

        protected readonly Domain myDomain;
        public IRawLogStore LogStore { get; private set; }

        public StringAndFileBuilder(long theEngineId, IRawLogStore theLogStore, Type theTypeOfCaller)
        {
            this.myDomain = new Domain($"{theTypeOfCaller}.{theEngineId}");
            LogStore = theLogStore;
        }

        public void AppendLogChunk(IEnumerable<byte> theChunk)
        {
            using (var aTracer = new Tracer(myDomain))
            {
                ValueStopwatch aSt = ValueStopwatch.StartNew();
                const int aChunkLength = 4 * 1024 * 1024;
                byte[] aBorrowed = new byte[aChunkLength];
                int aCounter = 0;
                foreach (byte aByte in theChunk)
                {
                    if (aCounter == aChunkLength)
                    {
                        this.LogStore.Writer.Write(aBorrowed, 0, aCounter);
                        aCounter = 0;
                    }

                    myBuilder.Append((char)aByte);
                    aBorrowed[aCounter] = aByte;
                    aCounter++;
                }

                this.LogStore.Writer.Write(aBorrowed, 0, aCounter);
                this.LogStore.Writer.Flush();
                aTracer.Info($"AppendLogChunk Too A Total Of {aSt.RestartGetElapsedMilliSeconds()}");
            }
        }
    }
}