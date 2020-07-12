using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CmlLib.Core.Version
{
    public class MVersionCollection : IEnumerable<MVersionMetadata>
    {
        public MVersionCollection(MVersionMetadata[] datas)
        {
            if (datas == null)
                throw new ArgumentNullException(nameof(datas));

            versions = datas;
        }

        public MVersionCollection(MVersionMetadata[] datas, MinecraftPath originalPath)
        {
            if (datas == null)
                throw new ArgumentNullException(nameof(datas));
            if (originalPath == null)
                throw new ArgumentNullException(nameof(originalPath));

            versions = datas;
            VersionPath = originalPath;
        }

        public MinecraftPath VersionPath { get; private set; }
        MVersionMetadata[] versions;

        public MVersionMetadata this[int index]
        {
            get => versions[index];
        }

        public MVersion GetVersion(string name)
        {
            MVersionMetadata versionMetadata = null;

            foreach (var item in versions)
            {
                if (item.Name == name)
                {
                    versionMetadata = item;
                    break;
                }
            }

            if (versionMetadata == null)
                throw new KeyNotFoundException("Cannot find " + name);

            return GetVersion(versionMetadata);
        }

        public MVersion GetVersion(MVersionMetadata versionMetadata)
        {
            if (versionMetadata == null)
                throw new ArgumentNullException(nameof(versionMetadata));

            MVersion startVersion;
            if (VersionPath == null)
                startVersion = MVersionParser.Parse(versionMetadata);
            else
                startVersion = MVersionParser.ParseAndSave(versionMetadata, VersionPath);

            if (startVersion.IsInherited)
            {
                if (startVersion.ParentVersionId == startVersion.Id) // prevent StackOverFlowException
                    throw new InvalidDataException("Invalid version json file : inheritFrom property is equal to id property.");

                var baseVersion = GetVersion(startVersion.ParentVersionId);
                startVersion.InheritFrom(baseVersion);
            }

            return startVersion;
        }

        public IEnumerator<MVersionMetadata> GetEnumerator()
        {
            return ((IEnumerable<MVersionMetadata>)versions).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return versions.GetEnumerator();
        }
    }
}
