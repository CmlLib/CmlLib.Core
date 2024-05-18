using CmlLib.Core;
using System.Diagnostics;
using System.Reflection;
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

        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static string? GetLibraryVersion()
        {
            try
            {
                return Assembly.GetAssembly(typeof(MinecraftLauncher))?.GetName().Version?.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
