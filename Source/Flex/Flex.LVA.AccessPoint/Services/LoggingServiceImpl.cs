using Flex.LVA.Communication;
using Flex.LVA.Logging;
using Grpc.Core;

namespace Flex.LVA.AccessPoint.Services
{
    public class LoggingServiceImpl : LoggingService.LoggingServiceBase
    {
        public override Task<VoidMessage> Debug(LoggingRequest theRequest, ServerCallContext theContext)
        {
            using (var aTrace = new Tracer(new Domain($"NG.{theRequest.Domain}"), theRequest.Function))
            {
                aTrace.Debug(theRequest.TraceMessage);
            }

            return Task.FromResult(new VoidMessage());
        }

        public override Task<VoidMessage> Info(LoggingRequest theRequest, ServerCallContext theContext)
        {
            using (var aTrace = new Tracer(new Domain($"NG.{theRequest.Domain}"), theRequest.Function))
            {
                aTrace.Info(theRequest.TraceMessage);
            }

            return Task.FromResult(new VoidMessage());
        }
        public override Task<VoidMessage> Error(LoggingRequest theRequest, ServerCallContext theContext)
        {
            using (var aTrace = new Tracer(new Domain($"NG.{theRequest.Domain}"), theRequest.Function))
            {
                aTrace.Error(theRequest.TraceMessage);
            }

            return Task.FromResult(new VoidMessage());
        }

        public override Task<VoidMessage> Verbose(LoggingRequest theRequest, ServerCallContext context)
        {
            using (var aTrace = new Tracer(new Domain($"NG.{theRequest.Domain}"), theRequest.Function))
            {
                aTrace.Verbose(theRequest.TraceMessage);
            }

            return Task.FromResult(new VoidMessage());
        }

        public override Task<VoidMessage> Warn(LoggingRequest theRequest, ServerCallContext context)
        {
            using (var aTrace = new Tracer(new Domain($"NG.{theRequest.Domain}"), theRequest.Function))
            {
                aTrace.Warn(theRequest.TraceMessage);
            }

            return Task.FromResult(new VoidMessage());
        }
    }
}
