namespace CmlLib.Utils
{
    public class SemiVersion
    {
        public bool IsPreRelease { get; private set; }
        public int Major { get; private set; }
        public int Minor { get; private set; }
        public int Patch { get; private set; }
        public string Tag { get; private set; } = "";

        public int ToInt()
        {
            return Major * 100 + Minor * 10 + Patch;
        }

        public override int GetHashCode()
        {
            return ToInt();
        }

        public override bool Equals(object obj)
        {
            if (obj is SemiVersion ver)
                return ToInt() == ver.ToInt();
            if (obj is int num)
                return ToInt() == num;
            if (obj is string verStr)
                return ToInt() == Parse(verStr)?.ToInt();

            return false;
        }

        public override string ToString()
        {
            var ver = $"{Major}.{Minor}.{Patch}";
            if (IsPreRelease)
                ver += "-pre";
            return ver;
        }

        public static SemiVersion? Parse(string version)
        {
            var semiVer = new SemiVersion();

            version = version.Trim();
            if (char.IsLetter(version[0]))
            {
                semiVer.IsPreRelease = true;
                version = version.Substring(1);
            }

            var verSplit = version.Split('.');
            if (verSplit.Length <= 1)
                return null;

            var last = verSplit[verSplit.Length - 1];
            var lastCharArr = last.ToCharArray();
            int versionEndPosition = 0;
            for (; versionEndPosition < last.Length; versionEndPosition++)
            {
                if (!char.IsDigit(lastCharArr[versionEndPosition]))
                {
                    if (versionEndPosition == 0)
                        return null; // invalid version
                    break;
                }
            }

            verSplit[verSplit.Length - 1] = last.Substring(0, versionEndPosition + 1);
            
            if (versionEndPosition > last.Length - 1)
                semiVer.Tag = last.Substring(versionEndPosition + 1);

            if (!int.TryParse(verSplit[0], out int major))
                return null;

            if (!int.TryParse(verSplit[1], out int minor))
                return null;

            int patch = 0;
            if (verSplit.Length > 2 && !int.TryParse(verSplit[2], out patch))
                return null;

            semiVer.Major = major;
            semiVer.Minor = minor;
            semiVer.Patch = patch;
            return semiVer;
        }

        public static int operator <(SemiVersion v1, SemiVersion v2)
        {
            var v1Ver = v1.ToInt();
            var v2Ver = v2.ToInt();

            if (v1Ver == v2Ver)
            {
                if (v1.IsPreRelease && v2.IsPreRelease)
                    return 0;
                if (v1.IsPreRelease)
                    return -1;
                if (v2.IsPreRelease)
                    return 1;
            }

            return v1Ver - v2Ver;
        }

        public static int operator >(SemiVersion v1, SemiVersion v2)
        {
            return (v1 < v2) * -1;
        }
    }
}