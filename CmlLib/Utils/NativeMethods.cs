using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace CmlLib.Utils
{
    internal static class NativeMethods
    {
        [DllImport("libc", SetLastError = true)]
        private static extern int chmod(string pathname, int mode);

        // user permissions
        private const int S_IRUSR = 0x100;
        private const int S_IWUSR =  0x80;
        private const int S_IXUSR =  0x40;

        // group permission
        private const int S_IRGRP =  0x20;
        private const int S_IWGRP =  0x10;
        private const int S_IXGRP =   0x8;

        // other permissions
        private const int S_IROTH =   0x4;
        private const int S_IWOTH =   0x2;
        private const int S_IXOTH =   0x1;

        public static readonly int Chmod755 = S_IRUSR | S_IXUSR | S_IWUSR
                                            | S_IRGRP | S_IXGRP
                                            | S_IROTH | S_IXOTH;

        public static void Chmod(string path, int mode)
        {
            chmod(path, mode);
        }
    }
}