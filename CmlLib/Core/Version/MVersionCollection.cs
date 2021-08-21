using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;

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
            MinecraftPath? originalPath,
            MVersionMetadata? latestRelease,
            MVersionMetadata? latestSnapshot)
        {
            if (datas == null)
                throw new ArgumentNullException(nameof(datas));

            Versions = new OrderedDictionary();
            foreach (MVersionMetadata? item in datas)
            {
                Versions.Add(item.Name, item);
            }

            MinecraftPath = originalPath;
            LatestReleaseVersion = latestRelease;
            LatestSnapshotVersion = latestSnapshot;
        }

        public MVersionMetadata? LatestReleaseVersion { get; private set; }
        public MVersionMetadata? LatestSnapshotVersion { get; private set; }
        public MinecraftPath? MinecraftPath { get; private set; }
        protected OrderedDictionary Versions;
        
        public MVersionMetadata this[int index] => (MVersionMetadata)Versions[index]!;

        public MVersionMetadata GetVersionMetadata(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            
            // Versions[name] will return null if index does not exists
            // Casting from null to MVersionMetadata does not throw NullReferenceException
            MVersionMetadata? versionMetadata = (MVersionMetadata?)Versions[name];
            if (versionMetadata == null)
                throw new KeyNotFoundException("Cannot find " + name);

            return versionMetadata;
        }
        
        
        public virtual MVersion GetVersion(string name)
        {
            return internalGetVersion(name, false).GetAwaiter().GetResult();
        }

        public virtual Task<MVersion> GetVersionAsync(string name)
        {
            return internalGetVersion(name, true);
        }

        public virtual MVersion GetVersion(MVersionMetadata versionMetadata)
        {
            return internalGetVersion(versionMetadata, false).GetAwaiter().GetResult();
        }

        public virtual Task<MVersion> GetVersionAsync(MVersionMetadata versionMetadata)
        {
            return internalGetVersion(versionMetadata, true);
        }

        private async Task<MVersion> internalGetVersion(MVersionMetadata versionMetadata, bool async)
        {
            if (versionMetadata == null)
                throw new ArgumentNullException(nameof(versionMetadata));

            MVersion startVersion;
            if (async)
            {
                if (MinecraftPath == null)
                    startVersion = versionMetadata.GetVersion();
                else
                    startVersion = versionMetadata.GetVersion(MinecraftPath);
            }
            else
            {
                if (MinecraftPath == null)
                    startVersion = await versionMetadata.GetVersionAsync()
                        .ConfigureAwait(false);
                else
                    startVersion = await versionMetadata.GetVersionAsync(MinecraftPath)
                        .ConfigureAwait(false);
            }

            if (startVersion.IsInherited && !string.IsNullOrEmpty(startVersion.ParentVersionId))
            {
                if (startVersion.ParentVersionId == startVersion.Id) // prevent StackOverFlowException
                    throw new InvalidDataException(
                        "Invalid version json file : inheritFrom property is equal to id property.");

                var baseVersion = await internalGetVersion(startVersion.ParentVersionId, async)
                    .ConfigureAwait(false);
                startVersion.InheritFrom(baseVersion);
            }

            return startVersion;
        }
        
        private Task<MVersion> internalGetVersion(string name, bool async)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var versionMetadata = GetVersionMetadata(name);
            if (async)
                return GetVersionAsync(versionMetadata);
            else
                return Task.FromResult(GetVersion(versionMetadata));
        }
        
        public bool Contains(string? versionName)
            => !string.IsNullOrEmpty(versionName) && Versions.Contains(versionName);

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
                var version = (MVersionMetadata)item.Value!;
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
