using System.Runtime.InteropServices;
using System.Security;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

namespace CmlLib.Core.Internals;

internal static class NativeMethods
{
    #region Chmod

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

    #endregion

    #region IsWindows10OrLater,RtlGetVersion

    // Copyright (c) 2019 pruggitorg
    // https://github.com/pruggitorg/detect-windows-version/blob/master/src/OSVersionExt/Win32API/Win32ApiProvider.cs
    // https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/wdm/nf-wdm-rtlgetversion
    // https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/wdm/ns-wdm-_osversioninfow

    [DllImport("ntdll.dll", EntryPoint = "RtlGetVersion", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern uint RtlGetVersion(ref OSVERSIONINFOEX versionInfo);

    /// <summary>
    /// Contains operating system version information. The information includes major and 
    /// minor version numbers, a build number, a platform identifier, and information about 
    /// product suites and the latest Service Pack installed on the system.
    /// </summary>
    /// <example>
    /// <code>
    /// var osVersionInfo = new OSVERSIONINFOEX { OSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX)) };
    /// </code>
    /// </example>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct OSVERSIONINFOEX
    {
        // The OSVersionInfoSize field must be set to Marshal.SizeOf(typeof(OSVERSIONINFOEX))
        public int OSVersionInfoSize;
        public int MajorVersion;
        public int MinorVersion;
        public int BuildNumber;
        public int PlatformId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string CSDVersion;
    }

    [SecurityCritical]
    public static string GetWindowsVersion(string fallback)
    {
        var osVersionInfo = new OSVERSIONINFOEX { OSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX)) };
        if (RtlGetVersion(ref osVersionInfo) != 0) // NTSTATUS.STATUS_SUCCESS
            return fallback;
        return $"{osVersionInfo.MajorVersion}.{osVersionInfo.MinorVersion}.{osVersionInfo.BuildNumber}";
    }

    #endregion
}