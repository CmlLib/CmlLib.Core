using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CmlLib.Core.Version
{
    public class MVersionCollection : IEnumerable<MVersionMetadata>
    {
        public MVersionCollection(MinecraftPath game, MVersionMetadata[] datas)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            if (datas == null)
                throw new ArgumentNullException(nameof(datas));

            mc = game;
            versions = datas;
        }

        MinecraftPath mc;
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

            var startVersion = MVersion.Parse(mc, versionMetadata);

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
