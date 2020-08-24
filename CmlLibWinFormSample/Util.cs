using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CmlLibWinFormSample
{
    public class Util
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        public static long? GetMemoryMb()
        {
            try
            {
                long value = 0;
                if (!GetPhysicallyInstalledSystemMemory(out value))
                    return null;

                return value / 1024;
            }
            catch
            {
                return null;
            }
        }
    }
}
