using Newtonsoft.Json.Linq;
using System;
using System.Runtime.InteropServices;

namespace CmlLib.Core
{
    public class MRule
    {
        static MRule()
        {
            OSName = getOSName();

            if (Environment.Is64BitOperatingSystem)
                Arch = "64";
            else
                Arch = "32";
        }

        public const string Windows = "windows";
        public const string OSX = "osx";
        public const string Linux = "linux";

        public static string OSName { get; private set; }
        public static string Arch { get; private set; }

        private static string getOSName()
        {
#if NETCOREAPP
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return OSX;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Windows;
            else
                return Linux;
#elif NETFRAMEWORK
            var osType = Environment.OSVersion.Platform;

            if (osType == PlatformID.MacOSX)
                return OSX;
            else if (osType == PlatformID.Unix)
                return Linux;
            else
                return Windows;
#endif
        }

        public static bool CheckOSRequire(JArray arr)
        {
            var require = true;

            foreach (JObject job in arr)
            {
                var action = true; // true : "allow", false : "disallow"
                var containCurrentOS = true; // if 'os' JArray contains current os name

                foreach (var item in job)
                {
                    if (item.Key == "action")
                        action = (item.Value.ToString() == "allow" ? true : false);
                    else if (item.Key == "os")
                        containCurrentOS = checkOSContains((JObject)item.Value);
                    else if (item.Key == "features") // etc
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

        static bool checkOSContains(JObject job)
        {
            foreach (var os in job)
            {
                if (os.Key == "name" && os.Value.ToString() == OSName)
                    return true;
            }
            return false;
        }
    }
}
