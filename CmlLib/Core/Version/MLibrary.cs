using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace CmlLib.Core.Version
{
    public class MLibrary
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Url { get;  set; }
        public string Hash { get; set; }
        public bool IsRequire { get; set; }
        public bool IsNative { get; set; }
    }
}
