using System;
using System.Collections.Generic;
using System.Linq;
using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.Version
{
    public enum MVersionSortPropertyOption
    {
        Name,
        Version,
        ReleaseDate
    }

    public enum MVersionNullReleaseDateSortOption
    {
        AsLatest,
        AsOldest
    }
    
    public class MVersionSortOption
    {
        public MVersionType[] TypeOrder { get; set; } = 
        {
            MVersionType.Custom, 
            MVersionType.Release, 
            MVersionType.Snapshot, 
            MVersionType.OldBeta, 
            MVersionType.OldAlpha
        };

        public MVersionSortPropertyOption PropertyOrderBy { get; set; }
            = MVersionSortPropertyOption.Version;

        public bool AscendingPropertyOrder { get; set; } = true;

        public MVersionNullReleaseDateSortOption NullReleaseDateSortOption { get; set; } =
            MVersionNullReleaseDateSortOption.AsOldest;
        
        public bool CustomAsRelease { get; set; } = false;
        public bool TypeClassification { get; set; } = true;
    }

    public class MVersionMetadataSorter
    {
        public MVersionMetadataSorter(MVersionSortOption option)
        {
            this.option = option;

            this.typePriority = new Dictionary<MVersionType, int>();
            for (int i = 0; i < option.TypeOrder.Length; i++)
            {
                var type = option.TypeOrder[i];
                typePriority[type] = i;
            }

            var propertyList = new List<MVersionSortPropertyOption>();
            propertyList.Add(option.PropertyOrderBy);
            foreach (MVersionSortPropertyOption item in Enum.GetValues(typeof(MVersionSortPropertyOption)))
            {
                if (option.PropertyOrderBy != item)
                    propertyList.Add(item);
            }

            propertyOptions = propertyList.ToArray();
            
            switch (option.NullReleaseDateSortOption)
            {
                case MVersionNullReleaseDateSortOption.AsLatest:
                    defaultDateTime = DateTime.MaxValue;
                    break;
                case MVersionNullReleaseDateSortOption.AsOldest:
                    defaultDateTime = DateTime.MinValue;
                    break;
            }
        }

        private readonly MVersionSortPropertyOption[] propertyOptions;
        private readonly DateTime defaultDateTime;
        private readonly Dictionary<MVersionType, int> typePriority;
        private readonly MVersionSortOption option;

        public MVersionMetadata[] Sort(IEnumerable<MVersionMetadata> org)
        {
            var filtered = org.Where(x => getTypePriority(x.MType) >= 0)
                .ToArray();
            Array.Sort(filtered, compare);
            return filtered;
        }

        private int getTypePriority(MVersionType type)
        {
            if (option.CustomAsRelease && type == MVersionType.Custom)
                type = MVersionType.Release;
            
            if (typePriority.TryGetValue(type, out int p))
                return p;
            
            return -1;
        }

        private int compareType(MVersionMetadata v1, MVersionMetadata v2)
        {
            var v1TypePrior = getTypePriority(v1.MType);
            var v2TypePrior = getTypePriority(v2.MType);
            return v1TypePrior - v2TypePrior;
        }

        private int compareName(MVersionMetadata v1, MVersionMetadata v2)
        {
            var result = string.CompareOrdinal(v1.Name, v2.Name);
            if (!option.AscendingPropertyOrder)
                result *= -1;
            return result;
        }

        private int compareVersion(MVersionMetadata v1, MVersionMetadata v2)
        {
            bool v1r = System.Version.TryParse(v1.Name, out System.Version v1v);
            bool v2r = System.Version.TryParse(v2.Name, out System.Version v2v);

            if (!v1r && !v2r)
                return 0;
            if (!v1r) // v1 > v2
                return 1;
            if (!v2r) // v1 < v2
                return -1;
            
            var result = v1v.CompareTo(v2v);
            if (!option.AscendingPropertyOrder)
                result *= -1;
            return result;
        }

        private int compareReleaseDate(MVersionMetadata v1, MVersionMetadata v2)
        {
            var v1DateTime = v1.ReleaseTime ?? defaultDateTime;
            var v2DateTime = v2.ReleaseTime ?? defaultDateTime;
            var result = DateTime.Compare(v1DateTime, v2DateTime);
            if (!option.AscendingPropertyOrder)
                result *= -1;
            return result;
        }

        private int compareProperty(MVersionMetadata v1, MVersionMetadata v2, 
            MVersionSortPropertyOption propertyOption)
        {
            switch (propertyOption)
            {
                case MVersionSortPropertyOption.Name:
                    return compareName(v1, v2);
                case MVersionSortPropertyOption.ReleaseDate:
                    return compareReleaseDate(v1, v2);
                case MVersionSortPropertyOption.Version:
                    return compareVersion(v1, v2);
            }

            return 0;
        }

        private int compare(MVersionMetadata v1, MVersionMetadata v2)
        {
            var typeCompareResult = compareType(v1, v2);

            if (option.TypeClassification && typeCompareResult != 0)
                return typeCompareResult;
            
            foreach (var propOption in propertyOptions)
            {
                int result = compareProperty(v1, v2, propOption);
                if (result != 0)
                    return result;
            }

            return typeCompareResult;
        }
    }
}