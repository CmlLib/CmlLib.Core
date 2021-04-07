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

            versions = new List<MVersionMetadata>(datas);
            MinecraftPath = originalPath;
            LatestReleaseVersion = latestRelease;
            LatestSnapshotVersion = latestSnapshot;
        }

        public MVersionMetadata LatestReleaseVersion { get; private set; }
        public MVersionMetadata LatestSnapshotVersion { get; private set; }
        public MinecraftPath MinecraftPath { get; private set; }
        List<MVersionMetadata> versions;

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

        public void Merge(MVersionCollection from)
        {
            //Console.WriteLine("Start");
            //var stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start();

            foreach (var item in from)
            {
                if (!versions.Contains(item))
                    versions.Add(item);
            }

            if (from.LatestReleaseVersion != null)
                this.LatestReleaseVersion = from.LatestReleaseVersion;

            if (from.LatestSnapshotVersion != null)
                this.LatestSnapshotVersion = from.LatestSnapshotVersion;

            //stopwatch.Stop();
            //Console.WriteLine(stopwatch.Elapsed);
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
