using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

//  Note   At the time of this writing, the .NET Framework 2.0 (code-named "Whidbey") provides
//  a wrapper to simplify using QueryPerformanceCounter and QueryPerformanceFrequency.

namespace AppPlatform.Core.EnterpriseLibrary.Performance
{
    /// <summary>
    /// a managed wrapper class to encapsulate the Microsoft® Win32® functions QueryPerformanceCounter and 
    /// QueryPerformanceFrequency. You can use this class to time the execution of managed code.
    /// </summary>
    /// <example>
    /// 
    /// StopWatch myTimer = new StopWatch();
    /// myTimer.Start();
    /// // do some work to time
    /// double LapTimeInNanoSeconds = myTimer.GetLapTime();
    /// </example>

    public class StopWatch
    {
        [DllImport("KERNEL32")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long start;
        private long lap;
        private long stop;
        private long frequency;
        Decimal multiplier = new Decimal(1.0e9);

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <returns>Constructor</returns>
        public StopWatch()
        {
            if (QueryPerformanceFrequency(out frequency) == false)
            {
                // Frequency not supported
                throw new Win32Exception();
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>void</returns>
        public void Start()
        {
            QueryPerformanceCounter(out start);
            lap = start;
        }

        /// <summary>
        /// </summary>
        /// <returns>void</returns>
        public double GetLapTime()
        {
            QueryPerformanceCounter(out stop);
            double duration =  ((((double)(stop - lap) * (double)multiplier) / (double)(frequency * 1000)));
            lap = stop;
            return duration;
        }
    }
}