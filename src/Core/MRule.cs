using System;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace CmlLib.Core
{
    public static class MRule
    {
        static MRule()
        {
            OSName = getOSName();

            if (Environment.Is64BitOperatingSystem)
                Arch = "64";
            else
                Arch = "32";
        }

        public static readonly string Windows = "windows";
        public static readonly string OSX = "osx";
        public static readonly string Linux = "linux";

        public static string OSName { get; private set; }
        public static string Arch { get; private set; }

        private static string getOSName()
        {
            // RuntimeInformation does not work in .NET Framework
#if NETFRAMEWORK
            var osType = Environment.OSVersion.Platform;

            if (osType == PlatformID.MacOSX)
                return OSX;
            else if (osType == PlatformID.Unix)
                return Linux;
            else
                return Windows;
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return OSX;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Windows;
            else
                return Linux;
#endif
        }

    }
}
