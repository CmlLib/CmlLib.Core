namespace CmlLib.Core.VersionMetadata;

// Sort IVersionMetadata
// TODO: measure performance and optimizing
// TODO: refactoring as IComparer
public class VersionMetadataSorter
{
    public VersionMetadataSorter(MVersionSortOption option)
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
    }

    private readonly MVersionSortPropertyOption[] propertyOptions;
    private readonly Dictionary<MVersionType, int> typePriority;
    private readonly MVersionSortOption option;

    public IVersionMetadata[] Sort(IEnumerable<IVersionMetadata> org)
    {
        var filtered = org.Where(x => getTypePriority(x.GetVersionType()) >= 0)
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

    private int compareType(IVersionMetadata v1, IVersionMetadata v2)
    {
        var v1TypePrior = getTypePriority(v1.GetVersionType());
        var v2TypePrior = getTypePriority(v2.GetVersionType());
        return v1TypePrior - v2TypePrior;
    }

    private int compareName(IVersionMetadata v1, IVersionMetadata v2)
    {
        var result = string.CompareOrdinal(v1.Name, v2.Name);
        if (!option.AscendingPropertyOrder)
            result *= -1;
        return result;
    }

    private int compareVersion(IVersionMetadata v1, IVersionMetadata v2)
    {
        bool v1r = System.Version.TryParse(v1.Name, out System.Version? v1v);
        bool v2r = System.Version.TryParse(v2.Name, out System.Version? v2v);

        if (!v1r && !v2r)
            return 0;
        if (!v1r || v1v == null) // v1 > v2
            return 1;
        if (!v2r || v2v == null) // v1 < v2
            return -1;
        
        var result = v1v.CompareTo(v2v);
        if (!option.AscendingPropertyOrder)
            result *= -1;
        return result;
    }

    private int compareReleaseDate(IVersionMetadata v1, IVersionMetadata v2)
    {
        var result = DateTimeOffset.Compare(v1.ReleaseTime, v2.ReleaseTime);
        if (!option.AscendingPropertyOrder)
            result *= -1;
        return result;
    }

    private int compareProperty(IVersionMetadata v1, IVersionMetadata v2, 
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

    private int compare(IVersionMetadata v1, IVersionMetadata v2)
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