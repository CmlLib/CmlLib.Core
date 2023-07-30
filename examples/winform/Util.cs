using System.Runtime.InteropServices;

namespace CmlLibWinFormSample
{
    public static class Util
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
