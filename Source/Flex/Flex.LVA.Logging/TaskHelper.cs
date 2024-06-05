namespace Flex.LVA.Logging
{
    public static class TaskHelper
    {
        public static Task HandleException(this Task theTask, Domain theDomain, string theTaskName)
        {
            theTask.ContinueWith(theT => { 
                using(var aTrace = new Tracer(theDomain, theTaskName))
                {
                    aTrace.Error($"Unhandled Exception In Task {theT.Exception}");
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
            return theTask;
        }

        public static Task SafeStart(this Task theTask, Domain theDomain, string theTaskName)
        {
            theTask.HandleException(theDomain, theTaskName).Start();
            return theTask;
        }

        public static Task<T> HandleException<T>(this Task<T> theTask, Domain theDomain, string theTaskName)
        {
            theTask.ContinueWith(theT => {
                using (var aTrace = new Tracer(theDomain, theTaskName))
                {
                    aTrace.Error($"Unhandled Exception In Task {theT.Exception}");
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
            return theTask;
        }

        public static Task<T> SafeStart<T>(this Task<T> theTask, Domain theDomain, string theTaskName)
        {
            theTask.HandleException(theDomain, theTaskName).Start();
            return theTask;
        }
    }
}
