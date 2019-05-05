using System;
using System.Diagnostics;

namespace TypingApp.Utilities
{
    public static class Watch
    {
        public static Stopwatch watch = new Stopwatch();
        public static void SetupWatch()
        {
            watch.Reset();
            watch.Start();
            while (watch.ElapsedMilliseconds < 1200)
            {
                int seed = Environment.TickCount;
                long result = 0;
                for (int i = 0; i < 100000000; ++i)
                {
                    result ^= i ^ seed; // Some useless bit operations
                }
            }
            watch.Stop();
        }
    }
}
