using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CmlLib.Utils
{
    public class JarFile
    {
        public JarFile(string path)
        {
            this.Path = path;
        }

        public string Path { get; private set; }

        public Dictionary<string, string> GetManifest()
        {
            string manifest = null;

            using (var fs = File.OpenRead(Path))
            using (var s = new ZipInputStream(fs))
            {
                ZipEntry e;
                while ((e = s.GetNextEntry()) != null)
                {
                    if (e.Name == "META-INF/MANIFEST.MF")
                    {
                        manifest = readStreamString(s);
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(manifest))
                return null;

            var dict = new Dictionary<string, string>();
            foreach (var item in manifest.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(item))
                    continue;

                var split = item.Split(':');

                var key = split[0].Trim();

                if (split.Length == 1)
                    dict.Add(key, null);
                else if (split.Length == 2)
                    dict.Add(key, split[1].Trim());
                else
                {
                    var value = string.Join(":", split, 1, split.Length - 1).Trim();
                    dict.Add(key, value);
                }
            }

            return dict;
        }

        private static string readStreamString(Stream s)
        {
            var str = new StringBuilder();
            var buffer = new byte[1024];
            while (true)
            {
                int size = s.Read(buffer, 0, buffer.Length);
                if (size == 0)
                    break;

                str.Append(Encoding.UTF8.GetString(buffer, 0, size));
            }

            return str.ToString();
        }
    }
}
