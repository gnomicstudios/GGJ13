using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Gnomic.Util
{
    public static class DebugTimers
    {
        class Timer
        {
            public string Name;
            public Stopwatch Stopwatch;
        }

        static Dictionary<int, Timer> timers = new Dictionary<int, Timer>();
        static int maxNameLen = 1;

        [Conditional("DEBUGTIMERS")]
        public static void CreateTimer(int timerId, string name)
        {
            Timer t = new Timer();
            t.Stopwatch = new Stopwatch();
            t.Name = name;
            timers[timerId] = t;

            maxNameLen = Math.Max(maxNameLen, name.Length);
        }

        [Conditional("DEBUGTIMERS")]
        public static void Start(int timerId)
        {
            timers[timerId].Stopwatch.Start();
        }

        [Conditional("DEBUGTIMERS")]
        public static void Stop(int timerId)
        {
            timers[timerId].Stopwatch.Stop();
        }

        [Conditional("DEBUGTIMERS")]
        public static void Reset(int timerId)
        {
            timers[timerId].Stopwatch.Reset();
        }

        //[Conditional("DEBUGTIMERS")]
        //public static void AppendTimerData(int timerId, StringBuilder builder)
        //{
        //    Timer t = timers[timerId];
        //    AppendTimerData(t, builder);
        //}

        [Conditional("DEBUGTIMERS")]
        static void AppendTimerData(Timer t, StringBuilder builder, float totalFrameSeconds)
        {
            // Output the name, and extra padding for longest name
            builder.Append(t.Name);
            builder.Append(' ', maxNameLen + 2 - t.Name.Length);

            // Output the milliseconds (to 1 dp)
            long ms = t.Stopwatch.ElapsedMilliseconds;
            long ms1dp = (10 * t.Stopwatch.ElapsedTicks / TimeSpan.TicksPerMillisecond) % 10;

            if (ms < 10)
                builder.Append(' ');

            builder.Append(ms);
            builder.Append('.');
            builder.Append(ms1dp);

            // Now the percentage of total.
            builder.Append(' ');

            if (totalFrameSeconds  > 0.0f)
            {
                int percent = (int)(ms / (totalFrameSeconds * 10.0f));
                if (percent < 100)
                    builder.Append(' ');
                if (percent < 10)
                    builder.Append(' ');

                builder.Append(percent);
                builder.Append('%');
            }

            builder.Append('\n');
        }

        [Conditional("DEBUGTIMERS")]
        public static void AppendTimerData(StringBuilder builder)
        {
            float secondsTotal = (float)timers[0].Stopwatch.ElapsedMilliseconds / 1000.0f;

            foreach (KeyValuePair<int, Timer> kvp in timers)
            {
                AppendTimerData(kvp.Value, builder, secondsTotal);
            }
        }
    }
}
