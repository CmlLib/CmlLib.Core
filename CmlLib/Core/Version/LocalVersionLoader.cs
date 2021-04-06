﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Core.Version
{
    public class LocalVersionLoader : IVersionLoader
    {
        public LocalVersionLoader(MinecraftPath path)
        {
            this.MinecraftPath = path;
        }

        MinecraftPath MinecraftPath;

        public MVersionCollection GetVersionMetadatas()
        {
            var list = getFromLocal(MinecraftPath).ToArray();
            return new MVersionCollection(list, MinecraftPath);
        }

        public Task<MVersionCollection> GetVersionMetadatasAsync()
        {
            return Task.Run(GetVersionMetadatas);
        }

        private List<MVersionMetadata> getFromLocal(MinecraftPath path)
        {
            var dirs = new DirectoryInfo(path.Versions).GetDirectories();
            var arr = new List<MVersionMetadata>(dirs.Length);

            for (int i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                var filepath = Path.Combine(dir.FullName, dir.Name + ".json");
                if (File.Exists(filepath))
                {
                    var info = new MVersionMetadata();
                    info.IsLocalVersion = true;
                    info.Name = dir.Name;
                    info.Path = filepath;
                    info.Type = "local";
                    info.MType = MVersionType.Custom;
                    arr.Add(info);
                }
            }

            return arr;
        }
    }
}