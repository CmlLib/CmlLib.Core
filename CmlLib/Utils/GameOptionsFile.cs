using System;
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

            var optionDict = new Dictionary<string, string?>();

            using (var fs = fileinfo.OpenRead())
            using (var reader = new StreamReader(fs, encoding))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.Contains(":"))
                        optionDict[line] = null;
                    else
                    {
                        var keyvalue = FromKeyValueString(line);
                        optionDict[keyvalue.Key] = keyvalue.Value;
                    }
                }
            }

            return new GameOptionsFile(optionDict, filepath);
        }
        
        public static KeyValuePair<string, string> FromKeyValueString(string keyvalue)
        {
            var spliter = ':';
            var spliterIndex = keyvalue.IndexOf(spliter);

            var key = keyvalue.Substring(0, spliterIndex);
            var value = keyvalue.Substring(spliterIndex + 1);

            return new KeyValuePair<string, string>(key, value);
        }

        public string? this[string key] => GetRawValue(key);
        public string? FilePath { get; private set; }
        private readonly Dictionary<string, string?> options;

        public GameOptionsFile()
        {
            this.options = new Dictionary<string, string?>();
        }
        
        public GameOptionsFile(Dictionary<string, string?> options)
        {
            this.options = options;
        }
        
        public GameOptionsFile(Dictionary<string, string?> options, string path)
        {
            this.options = options;
            this.FilePath = path;
        }

        public bool ContainsKey(string key)
        {
            return options.ContainsKey(key);
        }

        public string? GetRawValue(string key)
        {
            return options[key];
        }

        public string? GetValueAsString(string key)
        {
            string? value = GetRawValue(key);
            if (value == null)
                return null;
            return value.Trim().Trim('\"');
        }

        public string[]? GetValueAsArray(string key)
        {
            string? value = GetRawValue(key);
            if (value == null)
                return null;
            
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
            string? value = GetRawValue(key);
            if (value == null)
                return 0;
            
            return int.Parse(value);
        }

        public double GetValueAsDouble(string key)
        {
            string? value = GetRawValue(key);
            if (value == null)
                return 0;
            
            return double.Parse(value);
        }

        public bool GetValueAsBool(string key)
        {
            string? value = GetRawValue(key);
            if (value == null)
                return false;
             
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
            var str = obj.ToString();
            if (str == null)
                return;
            SetValue(key, str);
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(FilePath))
                throw new InvalidOperationException("FilePath was null");
            
            Save(FilePath);
        }

        public void Save(string path)
        {
            // IMPORTANT: UTF8 with BOM could not be recognized by minecraft
            Save(path, new UTF8Encoding(false));
        }

        public void Save(Encoding encoding)
        {
            if (string.IsNullOrEmpty(FilePath))
                throw new InvalidOperationException("FilePath was null");
            
            Save(FilePath, encoding);
        }

        public void Save(string path, Encoding encoding)
        {
            using (var fs = File.OpenWrite(path))
            using (var writer = new StreamWriter(fs, encoding))
            {
                foreach (KeyValuePair<string, string?> keyvalue in options)
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
