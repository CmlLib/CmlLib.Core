using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CmlLib.Core.Version
{
    public class MProfileMetadataCollection : IEnumerable<MProfileMetadata>
    {
        public MProfileMetadataCollection(Minecraft game, MProfileMetadata[] datas)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(mc));

            if (datas == null)
                throw new ArgumentNullException(nameof(datas));

            mc = game;
            profiles = datas;
        }

        Minecraft mc;
        MProfileMetadata[] profiles;

        public MProfileMetadata this[int index]
        {
            get => profiles[index];
        }

        public MProfile GetProfile(string name)
        {
            MProfileMetadata profileMetadata = null;

            foreach (var item in profiles)
            {
                if (item.Name == name)
                {
                    profileMetadata = item;
                    break;
                }
            }

            if (profileMetadata == null)
                throw new KeyNotFoundException("Cannot find " + name);

            return GetProfile(profileMetadata);
        }

        public MProfile GetProfile(MProfileMetadata profileMetadata)
        {
            if (profileMetadata == null)
                throw new ArgumentNullException(nameof(profileMetadata));

            var startProfile = MProfile.Parse(mc, profileMetadata);

            if (startProfile.IsInherited)
            {
                if (startProfile.ParentProfileId == startProfile.Id) // prevent StackOverFlowException
                    throw new InvalidDataException("Invalid Profile : inheritFrom property is equal to id property.");

                var baseProfile = GetProfile(startProfile.ParentProfileId);
                startProfile.InheritFrom(baseProfile);
            }

            return startProfile;
        }

        public IEnumerator<MProfileMetadata> GetEnumerator()
        {
            return ((IEnumerable<MProfileMetadata>)profiles).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return profiles.GetEnumerator();
        }
    }
}
