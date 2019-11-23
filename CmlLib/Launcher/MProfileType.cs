using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Launcher
{
    public static class MProfileTypeConverter
    {
        public static MProfileType FromString(string str)
        {
            MProfileType e;

            switch (str)
            {
                case "release":
                    e = MProfileType.Release;
                    break;
                case "snapshot":
                    e = MProfileType.Snapshot;
                    break;
                case "old_alpha":
                    e = MProfileType.OldAlpha;
                    break;
                case "old_beta":
                    e = MProfileType.OldBeta;
                    break;
                default:
                    e = MProfileType.Custom;
                    break;
            }

            return e;
        }

        public static string ToString(MProfileType type)
        {
            var c = "";

            switch (type)
            {
                case MProfileType.OldAlpha:
                    c = "old_alpha";
                    break;
                case MProfileType.OldBeta:
                    c = "old_beta";
                    break;
                case MProfileType.Snapshot:
                    c = "snapshot";
                    break;
                case MProfileType.Release:
                    c = "release";
                    break;
                case MProfileType.Custom:
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

        public static bool CheckOld(MProfileType t)
        {
            if (t == MProfileType.OldAlpha || t == MProfileType.OldBeta)
                return true;
            else
                return false;
        }
    }

    public enum MProfileType
    {
        OldAlpha,
        OldBeta,
        Snapshot,
        Release,
        Custom
    }
}
