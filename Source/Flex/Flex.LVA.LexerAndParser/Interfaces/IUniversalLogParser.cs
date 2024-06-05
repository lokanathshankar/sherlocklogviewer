using Flex.LVA.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.LVA.Core.Interfaces
{
    public interface ILogParser : IProgressUpdateProvider , IDisposable
    {
        ILogGraph LogGraph { get; }

        void Parse(LogDefinition theLogDefinition, in string theStringToParse);

        void Reset();

        void FreeUpMemory();


        void AppendLogChunk(IEnumerable<byte> theChunk);

        IRawLogStore LogStore { get; }

    }
}
