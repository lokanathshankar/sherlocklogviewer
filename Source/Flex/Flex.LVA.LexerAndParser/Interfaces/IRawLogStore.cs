using System.Runtime.CompilerServices;

namespace Flex.LVA.Core.Interfaces
{
    public interface IRawLogStore : IDisposable
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool UpdateRange(int theRangeId, int theEndIndex);
        
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool StoreRange(int theRangeId, int theStartIndex, int theEndIndex);
        
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool GetString(int theRangeId, out string theString);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LoadStore(in string theFallbackString);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ResetStore();

        internal Stream Writer { get; }

    }
}