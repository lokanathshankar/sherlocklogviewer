namespace Flex.LVA.Logging
{
    // Licensed to the .NET Foundation under one or more agreements.
    // The .NET Foundation licenses this file to you under the MIT license.

    using System;
    using System.Diagnostics;

    public struct ValueStopwatch
    {
        private static readonly double myTimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

        private long myStartTimestamp;

        private bool IsActive => this.myStartTimestamp != 0;

        private ValueStopwatch(long theStartTimestamp)
        {
            this.myStartTimestamp = theStartTimestamp;
        }

        public ValueStopwatch() :  this(Stopwatch.GetTimestamp())
        {
        }

        public static ValueStopwatch StartNew() => new ValueStopwatch(Stopwatch.GetTimestamp());

        public int RestartGetElapsedMilliSeconds()
        {
            int aCache = (int)GetElapsedTime().TotalMilliseconds;
            this.myStartTimestamp = Stopwatch.GetTimestamp();
            return aCache;
        }

        public TimeSpan GetElapsedTime()
        {
            // Start timestamp can't be zero in an initialized ValueStopwatch. It would have to be literally the first thing executed when the machine boots to be 0.
            // So it being 0 is a clear indication of default(ValueStopwatch)
            if (!IsActive)
            {
                throw new InvalidOperationException("An uninitialized, or 'default', ValueStopwatch cannot be used to get elapsed time.");
            }

            var aEnd = Stopwatch.GetTimestamp();
            var aTimestampDelta = aEnd - this.myStartTimestamp;
            var aTicks = (long)(myTimestampToTicks * aTimestampDelta);
            return new TimeSpan(aTicks);
        }
    }
}
