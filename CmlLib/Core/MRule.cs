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

        public static bool CheckOSRequire(JsonElement arr)
        {
            if (arr.ValueKind != JsonValueKind.Array)
                throw new ArgumentException("input JsonElement was not array");

            var require = true;
            foreach (var token in arr.EnumerateArray())
            {
                if (token.ValueKind != JsonValueKind.Object)
                    continue;

                bool action = true; // true : "allow", false : "disallow"
                bool containCurrentOS = true; // if 'os' JArray contains current os name

                foreach (var item in token.EnumerateObject())
                {
                    if (item.Name == "action")
                        action = (item.Value.GetString() == "allow");
                    else if (item.Name == "os")
                        containCurrentOS = checkOSContains(item.Value);
                    else if (item.Name == "features") // etc
                        return false;
                }

                if (!action && containCurrentOS)
                    require = false;
                else if (action && containCurrentOS)
                    require = true;
                else if (action && !containCurrentOS)
                    require = false;
            }

            return require;
        }

        private static bool checkOSContains(JsonElement? element)
        {
            if (element == null)
                return false;
            
            foreach (var os in element.Value.EnumerateObject())
            {
                if (os.Name == "name" && os.Value.GetString() == OSName)
                    return true;
            }
            return false;
        }
    }
}
