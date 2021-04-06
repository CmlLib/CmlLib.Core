﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CmlLib.Core.Version.FabricMC
{
    public class FabricLoader
    {
        [JsonProperty("separator")]
        public string Separator { get; set; }
        [JsonProperty("build")]
        public string Build { get; set; }
        [JsonProperty("maven")]
        public string Maven { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("stable")]
        public bool Stable { get; set; }
    }
}
