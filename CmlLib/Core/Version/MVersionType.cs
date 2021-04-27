namespace CmlLib.Core.Version
{
    public static class MVersionTypeConverter
    {
        public static MVersionType FromString(string str)
        {
            MVersionType e;

            switch (str)
            {
                case "release":
                    e = MVersionType.Release;
                    break;
                case "snapshot":
                    e = MVersionType.Snapshot;
                    break;
                case "old_alpha":
                    e = MVersionType.OldAlpha;
                    break;
                case "old_beta":
                    e = MVersionType.OldBeta;
                    break;
                default:
                    e = MVersionType.Custom;
                    break;
            }

            return e;
        }

        public static string ToString(MVersionType type)
        {
            string c;

            switch (type)
            {
                case MVersionType.OldAlpha:
                    c = "old_alpha";
                    break;
                case MVersionType.OldBeta:
                    c = "old_beta";
                    break;
                case MVersionType.Snapshot:
                    c = "snapshot";
                    break;
                case MVersionType.Release:
                    c = "release";
                    break;
                default:
                    c = "unknown";
                    break;
            }

            return c;
        }

        public static bool CheckOld(string vn)
        {
            return CheckOld(FromString(vn));
        }

        public static bool CheckOld(MVersionType t)
        {
            if (t == MVersionType.OldAlpha || t == MVersionType.OldBeta)
                return true;
            else
                return false;
        }
    }

    public enum MVersionType
    {
        OldAlpha,
        OldBeta,
        Snapshot,
        Release,
        Custom
    }
}
