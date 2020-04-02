using Newtonsoft.Json.Linq;
using System;

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

        public static string OSName { get; private set; }
        public static string Arch { get; private set; }

        private static string getOSName()
        {
            var osType = Environment.OSVersion.Platform;

            if (osType == PlatformID.MacOSX)
                return "osx";
            else if (osType == PlatformID.Unix)
                return "linux";
            else
                return "windows";
        }

        public bool CheckOSRequire(JArray arr)
        {
            var osName = getOSName();

            foreach (JObject job in arr)
            {
                var action = true; // true : "allow", false : "disallow"
                var containCurrentOS = true; // if 'os' JArray contains current os name

                foreach (var item in job)
                {
                    // action
                    if (item.Key == "action")
                        action = (item.Value.ToString() == "allow" ? true : false);

                    // os (containCurrentOS)
                    else if (item.Key == "os")
                    {
                        foreach (var os in (JObject)item.Value)
                        {
                            if (os.Key == "name" && os.Value.ToString() == osName)
                            {
                                containCurrentOS = true;
                                break;
                            }
                        }
                        containCurrentOS = false;
                    }

                    else if (item.Key == "features") // etc
                        return false;
                }

                if (!action && containCurrentOS)
                    return false;
                else if (action && containCurrentOS)
                    return true;
                else if (action && !containCurrentOS)
                    return false;
            }

            return true;
        }
    }
}
