using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CmlLib.Utils
{
    public static class GameOptionsFile
    {
        public static readonly int MaxOptionFileLength = 1024 * 1024; // 1MB

        public static Dictionary<string, string> ReadFile(string filepath)
        {
            // Default Encoding : OS 
            return ReadFile(filepath, Encoding.Default);
        }

        public static Dictionary<string, string> ReadFile(string filepath, Encoding encoding)
        {
            var fileinfo = new FileInfo(filepath);
            if (fileinfo.Length > MaxOptionFileLength)
                throw new IOException("File is too big");

            var options = new Dictionary<string, string>();

            using (var fs = fileinfo.OpenRead())
            using (var reader = new StreamReader(fs, encoding))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.Contains(":"))
                        options[line] = null;
                    else
                    {
                        var keyvalue = FromKeyValueString(line);
                        options[keyvalue.Key] = keyvalue.Value;
                    }
                }
            }

            return options;
        }

        public static void WriteFile(string filepath, Dictionary<string, string> options)
        {
            var encoding = Encoding.Default;
            WriteFile(filepath, options, encoding);
        }

        public static void WriteFile(string filepath, Dictionary<string, string> options, Encoding encoding)
        {
            using (var fs = File.OpenWrite(filepath))
            using (var writer = new StreamWriter(fs, encoding))
            {
                foreach (KeyValuePair<string, string> keyvalue in options)
                {
                    if (keyvalue.Value == null)
                        writer.WriteLine(keyvalue.Key);
                    else
                    {
                        var line = ToKeyValueString(keyvalue.Key, keyvalue.Value);
                        writer.WriteLine(line);
                    }
                }
            }
        }

        private static string handleEmpty(string value)
        {
            if (value.Contains(" "))
                value = "\"" + value + "\"";
            return value;
        }

        public static string ToKeyValueString(string key, string value, bool useHandleEmpty = true)
        {
            if (useHandleEmpty)
                value = handleEmpty(value);

            return key + ":" + value;
        }

        public static string ToKeyValueString(string key, string[] arrs)
        {
            var sb = new StringBuilder();
            sb.Append('[');
            for (int i = 0; i < arrs.Length; i++)
            {
                sb.Append('\"');
                sb.Append(arrs[i]);
                sb.Append('\"');

                if (i != arrs.Length - 1) // not last element
                    sb.Append(',');
            }
            sb.Append(']');

            return ToKeyValueString(key, sb.ToString(), false);
        }

        public static string ToKeyValueString(string key, bool value)
        {
            var str = value.ToString().ToLowerInvariant();
            return ToKeyValueString(key, str, false);
        }

        public static KeyValuePair<string, string> FromKeyValueString(string keyvalue)
        {
            var spliter = ':';
            var spliterIndex = keyvalue.IndexOf(spliter);

            var key = keyvalue.Substring(0, spliterIndex);
            var value = keyvalue.Substring(spliterIndex + 1);

            return new KeyValuePair<string, string>(key, value);
        }

        public static KeyValuePair<string, string[]> FromKeyValueStringToArray(string keyvalue)
        {
            var kv = FromKeyValueString(keyvalue);
            var value = kv.Value;
            if (!value.StartsWith("[") || !value.EndsWith("]"))
                return new KeyValuePair<string, string[]>(kv.Key, new string[] { value });

            var innerStr = value.Substring(1, value.Length - 2);
            if (!innerStr.Contains(","))
                return new KeyValuePair<string, string[]>(kv.Key, new string[] { innerStr });

            var arrStr = innerStr.Split(',');
            var arr = new List<string>(arrStr.Length);
            foreach (var item in arrStr)
            {
                arr.Add(item.Trim('\"'));
            }

            return new KeyValuePair<string, string[]>(kv.Key, arr.ToArray());
        }
    }
}
