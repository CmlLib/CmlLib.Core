using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CmlLib.Utils
{
    public class GameOptionsFile : IEnumerable<KeyValuePair<string, string>>
    {
        public static readonly int MaxOptionFileLength = 1024 * 1024; // 1MB

        public static GameOptionsFile ReadFile(string filepath)
        {
            // Default Encoding : OS 
            return ReadFile(filepath, Encoding.Default);
        }

        public static GameOptionsFile ReadFile(string filepath, Encoding encoding)
        {
            var fileinfo = new FileInfo(filepath);
            if (fileinfo.Length > MaxOptionFileLength)
                throw new IOException("File is too big");

            var options = new Dictionary<string, string>();

            using (var fs = fileinfo.OpenRead())
            using (var reader = new StreamReader(fs, encoding))
            {
                string line;
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

            return new GameOptionsFile(options, filepath);
        }
        
        public static KeyValuePair<string, string> FromKeyValueString(string keyvalue)
        {
            var spliter = ':';
            var spliterIndex = keyvalue.IndexOf(spliter);

            var key = keyvalue.Substring(0, spliterIndex);
            var value = keyvalue.Substring(spliterIndex + 1);

            return new KeyValuePair<string, string>(key, value);
        }

        public string this[string key] => GetRawValue(key);
        public string FilePath { get; private set; }
        private readonly Dictionary<string, string> options;

        private GameOptionsFile(Dictionary<string, string> options, string path)
        {
            this.options = options;
            this.FilePath = path;
        }

        public bool ContainsKey(string key)
        {
            return options.ContainsKey(key);
        }

        public string GetRawValue(string key)
        {
            return options[key];
        }

        public string GetValueAsString(string key)
        {
            string value = GetRawValue(key);
            return value.Trim().Trim('\"');
        }

        public string[] GetValueAsArray(string key)
        {
            string value = GetRawValue(key);
            return value
                .Trim()
                .TrimStart('[')
                .TrimEnd(']')
                .Split(',')
                .Select(x => x.Trim().Trim('\"'))
                .ToArray();
        }

        public int GetValueAsInt(string key)
        {
            string value = GetRawValue(key);
            return int.Parse(value);
        }

        public double GetValueAsDouble(string key)
        {
            string value = GetRawValue(key);
            return double.Parse(value);
        }

        public bool GetValueAsBool(string key)
        {
            string value = GetRawValue(key);
            return bool.Parse(value);
        }

        public void SetRawValue(string key, string value)
        {
            options[key] = value;
        }

        public void SetValue(string key, string value)
        {
            SetRawValue(key, handleEmpty(value));
        }

        public void SetValue(string key, int value)
        {
            SetRawValue(key, value.ToString());
        }

        public void SetValue(string key, double value)
        {
            SetRawValue(key, value.ToString(CultureInfo.InvariantCulture));
        }

        public void SetValue(string key, bool value)
        {
            SetRawValue(key, value.ToString().ToLowerInvariant());
        }

        public void SetValue(string key, string[] array)
        {
            var arrayNotation = string.Join(",", array.Select(x => $"\"{x}\""));
            SetRawValue(key, $"[{arrayNotation}]");
        }

        public void SetValue(string key, object obj)
        {
            SetValue(key, obj.ToString());
        }

        public void Save()
        {
            Save(FilePath);
        }

        public void Save(string path)
        {
            Save(path, Encoding.UTF8);
        }

        public void Save(Encoding encoding)
        {
            Save(FilePath, encoding);
        }

        public void Save(string path, Encoding encoding)
        {
            using (var fs = File.OpenWrite(path))
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

        private string ToKeyValueString(string key, string value, bool useHandleEmpty = true)
        {
            if (useHandleEmpty)
                value = handleEmpty(value);

            return key + ":" + value;
        }
        
        private static string handleEmpty(string value)
        {
            if (value.Contains(" "))
                value = "\"" + value + "\"";
            return value;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return options.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
