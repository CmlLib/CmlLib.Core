using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace CmlLib.Core.Version
{
    public class MVersionCollection : IEnumerable<MVersionMetadata>
    {
        public MVersionCollection(MVersionMetadata[] datas)
            : this(datas, null, null, null)
        {

        }

        public MVersionCollection(MVersionMetadata[] datas, MinecraftPath originalPath)
            : this(datas, originalPath, null, null)
        {

        }

        public MVersionCollection(
            MVersionMetadata[] datas,
            MinecraftPath originalPath,
            MVersionMetadata latestRelease,
            MVersionMetadata latestSnapshot)
        {
            if (datas == null)
                throw new ArgumentNullException(nameof(datas));

            Versions = new OrderedDictionary();
            foreach (var item in datas)
            {
                Versions.Add(item.Name, item);
            }

            MinecraftPath = originalPath;
            LatestReleaseVersion = latestRelease;
            LatestSnapshotVersion = latestSnapshot;
        }

        public MVersionMetadata LatestReleaseVersion { get; private set; }
        public MVersionMetadata LatestSnapshotVersion { get; private set; }
        public MinecraftPath MinecraftPath { get; private set; }
        protected OrderedDictionary Versions;

        public MVersionMetadata this[int index]
        {
            get => (MVersionMetadata)Versions[index];
        }

        [MethodTimer.Time]
        public virtual MVersion GetVersion(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            MVersionMetadata versionMetadata = (MVersionMetadata)Versions[name];
            if (versionMetadata == null)
                throw new KeyNotFoundException("Cannot find " + name);

            return GetVersion(versionMetadata);
        }

        public virtual MVersion GetVersion(MVersionMetadata versionMetadata)
        {
            if (versionMetadata == null)
                throw new ArgumentNullException(nameof(versionMetadata));

            MVersion startVersion;
            if (MinecraftPath == null)
                startVersion = MVersionParser.Parse(versionMetadata);
            else
                startVersion = MVersionParser.ParseAndSave(versionMetadata, MinecraftPath);

            if (startVersion.IsInherited)
            {
                if (startVersion.ParentVersionId == startVersion.Id) // prevent StackOverFlowException
                    throw new InvalidDataException("Invalid version json file : inheritFrom property is equal to id property.");

                var baseVersion = GetVersion(startVersion.ParentVersionId);
                startVersion.InheritFrom(baseVersion);
            }

            return startVersion;
        }

        public virtual void Merge(MVersionCollection from)
        {
            foreach (var item in from)
            {
                if (!Versions.Contains(item.Name))
                    Versions[item.Name] = item;
            }

            if (this.MinecraftPath == null && from.MinecraftPath != null)
                this.MinecraftPath = from.MinecraftPath;

            if (this.LatestReleaseVersion == null && from.LatestReleaseVersion != null)
                this.LatestReleaseVersion = from.LatestReleaseVersion;

            if (this.LatestSnapshotVersion == null && from.LatestSnapshotVersion != null)
                this.LatestSnapshotVersion = from.LatestSnapshotVersion;
        }

        public IEnumerator<MVersionMetadata> GetEnumerator()
        {
            foreach (DictionaryEntry item in Versions)
            {
                var version = (MVersionMetadata)item.Value;
                yield return version;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (DictionaryEntry item in Versions)
            {
                var version = item.Value;
                yield return version;
            }
        }
    }
}
