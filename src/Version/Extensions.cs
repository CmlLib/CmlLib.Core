using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.Version;

public static class Extensions
{
    public static MVersionType GetVersionType(this IVersion version)
    {
        return MVersionTypeConverter.FromString(version.Type);
    }

    public static T? GetInheritedProperty<T>(this IVersion self, Func<IVersion, T> prop)
    {
        IVersion? version = self;
        while (version != null)
        {
            var value = prop.Invoke(version);
            if (value is string valueStr && !string.IsNullOrEmpty(valueStr))
                return value;
            else if (value != null)
                return value;
            version = version.ParentVersion;
        }
        return default;
    }

    public static IEnumerable<T> ConcatInheritedCollection<T>(this IVersion self, Func<IVersion, IEnumerable<T>> prop)
    {
        IVersion? version = self;
        while (version != null)
        {
            var value = prop.Invoke(version);
            if (value != null)
            {
                foreach (var item in value)
                    yield return item;
            }
            version = version.ParentVersion;
        }
    }
}