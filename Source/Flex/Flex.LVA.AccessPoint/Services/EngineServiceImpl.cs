using Flex.LVA.Communication;
using Flex.LVA.Core.Interfaces;
using Flex.LVA.Logging;
using Grpc.Core;
using System.Diagnostics;

namespace Flex.LVA.AccessPoint.Services
{
    public class EngineServiceImpl : EngineService.EngineServiceBase
    {
        private static readonly Domain myDomain = new Domain(typeof(EngineServiceImpl));

        private IEngineInstanceManager myEngineInstanceManager;

        public EngineServiceImpl(IEngineInstanceManager theEngineInstanceManager)
        {
            myEngineInstanceManager = theEngineInstanceManager;
        }

        public override Task<StringMessage> GetRawLogs(GetRawLogsRequest theRequest, ServerCallContext context)
        {
            return new Task<StringMessage>(() =>
            {
                using (var aTrace = new Tracer(myDomain))
                {
                    if (!myEngineInstanceManager.GetEngine(theRequest.RegistrationData.Id, out IEngine aEngineToUse))
                    {
                        aTrace.Error($"Unable to get the engine for id {theRequest.RegistrationData.Id}, aborting.");
                        return new StringMessage();
                    }

                    if (!aEngineToUse.GetRawLogs(theRequest.LogIds.ToList(), out string aStringToSend))
                    {
                        aTrace.Error($"Unable to get the Raw Logs {theRequest.RegistrationData.Id}, aborting.");
                        return new StringMessage();
                    }

                    return new StringMessage() { Value = aStringToSend };
                }
            }).SafeStart(myDomain, nameof(GetRawLogs));
        }

        public override Task<VoidMessage> PrepareResources(PrepareResourceRequest theRequest, ServerCallContext context)
        {
            return new Task<VoidMessage>(() =>
            {
                using (var aTrace = new Tracer(myDomain))
                {
                    if (!myEngineInstanceManager.GetEngine(theRequest.RegistrationData.Id, out IEngine aEngineToUse))
                    {
                        aTrace.Error($"Unable to get the engine for id {theRequest.RegistrationData.Id}, aborting.");
                        return new VoidMessage();
                    }

                    aEngineToUse.PrepareResources(theRequest.LogDefinition);
                    return new VoidMessage();
                }
            }).SafeStart(myDomain, nameof(GetRawLogs));
        }
        public override Task<VoidMessage> AppendLogChunk(LogChunkRequest theRequest, ServerCallContext context)
        {
            return new Task<VoidMessage>(() =>
            {
                using (var aTrace = new Tracer(myDomain))
                {
                    if (!myEngineInstanceManager.GetEngine(theRequest.RegistrationData.Id, out IEngine aEngineToUse))
                    {
                        aTrace.Error($"Unable to get the engine for id {theRequest.RegistrationData.Id}, aborting.");
                        return new VoidMessage();
                    }

                    aEngineToUse.AppendLogChunk(theRequest.LogChunk);
                    return new VoidMessage();
                }
            }).SafeStart(myDomain, nameof(AppendLogChunk));
        }

        public override Task<VoidMessage> PrepareLogs(RegistrationData theRequest, ServerCallContext theContext)
        {
            return new Task<VoidMessage>(() =>
            {
                using (var aTrace = new Tracer(myDomain))
                {
                    if (!myEngineInstanceManager.GetEngine(theRequest.Id, out IEngine aEngineToUse))
                    {
                        aTrace.Error($"Unable to get the engine for id {theRequest.Id}, aborting.");
                        return new VoidMessage();
                    }

                    aEngineToUse.PrepareLog();
                    return new VoidMessage();
                }
            }).SafeStart(myDomain, nameof(PrepareLogs));
        }

        public override Task<StringMessage> GetRawLog(GetRawLogRequest theRawLogRequest, ServerCallContext theContext)
        {
            return new Task<StringMessage>(() =>
            {
                using (var aTrace = new Tracer(myDomain))
                {
                    StringMessage aReturn = new StringMessage();
                    if (!myEngineInstanceManager.GetEngine(theRawLogRequest.RegistrationData.Id, out IEngine aEngineToUse))
                    {
                        aTrace.Error($"Unable to get the engine for id {theRawLogRequest.RegistrationData.Id}, aborting.");
                        return aReturn;
                    }

                    if (!aEngineToUse.GetRawLog(theRawLogRequest.LogId, out string aRawLog))
                    {
                        aTrace.Error($"Unable to get get raw log {theRawLogRequest.RegistrationData.Id} and Log Id {theRawLogRequest.LogId}, aborting.");
                    }

                    aReturn.Value = aRawLog;
                    return aReturn;
                }
            }).SafeStart(myDomain, nameof(PrepareLogs));
        }
    }
}
