using System;

namespace CmlLib
{
    public class _Test
    {
        public static string tstr = "462,31";

        [MethodTimer.Time]
        public static void testTimer()
        {
            System.Diagnostics.Trace.WriteLine("Trace");
            Console.WriteLine("Hello");
        }
    }
}
