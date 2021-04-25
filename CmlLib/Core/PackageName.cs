using System;
using System.IO;

namespace CmlLib.Core
{
    public class PackageName
    {
        public static PackageName Parse(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var spliter = name.Split(':');
            if (spliter.Length < 3)
                throw new ArgumentException("invalid name");

            PackageName pn = new PackageName();
            pn.names = spliter;

            return pn;
        }

        private PackageName() { }

        private string[] names;

        public string this[int index] => names[index];

        public string Package => names[0];
        public string Name => names[1];
        public string Version => names[2];

        public string GetPath()
        {
            return GetPath("");
        }

        public string GetPath(string nativeId)
        {
            return GetPath(nativeId, "jar");
        }

        public string GetPath(string nativeId, string extension)
        {
            // de.oceanlabs.mcp : mcp_config : 1.16.2-20200812.004259 : mappings
            // de\oceanlabs\mcp \ mcp_config \ 1.16.2-20200812.004259 \ mcp_config-1.16.2-20200812.004259.zip

            // [de.oceanlabs.mcp:mcp_config:1.16.2-20200812.004259@zip]
            // \libraries\de\oceanlabs\mcp\mcp_config\1.16.2-20200812.004259\mcp_config-1.16.2-20200812.004259.zip

            // [net.minecraft:client:1.16.2-20200812.004259:slim]
            // /libraries\net\minecraft\client\1.16.2-20200812.004259\client-1.16.2-20200812.004259-slim.jar

            string filename = string.Join("-", names, 1, names.Length - 1);

            if (!string.IsNullOrEmpty(nativeId))
                filename += "-" + nativeId;
            filename += "." + extension;

            return Path.Combine(GetDirectory(), filename);
        }

        public string GetDirectory()
        {
            string dir = Package.Replace(".", "/");
            return Path.Combine(dir, Name, Version);
        }

        public string GetClassPath()
        {
            return Package + "." + Name;
        }
    }
}
