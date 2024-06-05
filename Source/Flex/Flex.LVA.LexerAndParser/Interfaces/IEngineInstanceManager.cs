namespace Flex.LVA.Core.Interfaces
{
    public interface IEngineInstanceManager : IDisposable
    {
        public bool StartLogEngine(out long theRegistrationId, out string theAddress);

        public bool StopLogEngine(long theEngineId);

        public bool GetEngine(long theEngineId, out IEngine theEngine);
    }
}