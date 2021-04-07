﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CmlLib.Core.Version.FabricMC
{
    public class FabricVersionLoader : IVersionLoader
    {
        public string ApiServer = "https://meta.fabricmc.net";
        public string LoaderVersion = null;

        protected string GetVersionName(string version, string loaderVersion)
        {
            return $"fabric-loader-{LoaderVersion}-{version}";
        }

        public MVersionCollection GetVersionMetadatas()
        {
            if (string.IsNullOrEmpty(LoaderVersion))
            {
                var loaders = GetFabricLoaders().GetAwaiter().GetResult();
                LoaderVersion = loaders[0].Version;
            }

            var url = "https://meta.fabricmc.net/v2/versions/game/intermediary";
            var res = "";
            using (var wc = new WebClient())
            {
                res = wc.DownloadString(url);
            }

            var versions = parseVersions(res, LoaderVersion);
            return new MVersionCollection(versions.ToArray());
        }

        public async Task<MVersionCollection> GetVersionMetadatasAsync()
        {
            if (string.IsNullOrEmpty(LoaderVersion))
            {
                var loaders = await GetFabricLoaders();
                LoaderVersion = loaders[0].Version;
            }

            var url = "https://meta.fabricmc.net/v2/versions/game/intermediary";
            var res = "";
            using (var wc = new WebClient())
            {
                res = await wc.DownloadStringTaskAsync(url);
            }

            var versions = parseVersions(res, LoaderVersion);
            return new MVersionCollection(versions.ToArray());
        }

        private List<MVersionMetadata> parseVersions(string res, string loader)
        {
            var jarr = JArray.Parse(res);
            var versionList = new List<MVersionMetadata>(jarr.Count);

            foreach (var item in jarr)
            {
                var versionName = item["version"]?.ToString();
                var jsonUrl = $"{ApiServer}/v2/versions/loader/{versionName}/{loader}/profile/json";

                var versionMetadata = new MVersionMetadata()
                {
                    IsLocalVersion = false,
                    MType = MVersionType.Custom,
                    Name = GetVersionName(versionName, loader),
                    Path = jsonUrl,
                    Type = "fabric"
                };
                versionList.Add(versionMetadata);
            }

            return versionList;
        }

        public async Task<FabricLoader[]> GetFabricLoaders()
        {
            var res = "";
            using (var wc = new WebClient())
            {
                res = await wc.DownloadStringTaskAsync(ApiServer + "/v2/versions/loader");
            }

            var jarr = JArray.Parse(res);
            var loaderList = new List<FabricLoader>(jarr.Count);
            foreach (var item in jarr)
            {
                loaderList.Add(item.ToObject<FabricLoader>());
            }

            return loaderList.ToArray();
        }
    }
}