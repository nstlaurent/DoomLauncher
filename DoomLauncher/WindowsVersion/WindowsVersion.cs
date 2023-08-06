using System;
using System.Runtime.InteropServices;

namespace DoomLauncher.WindowsVersion
{
    public static class WindowsVersionInfo
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OSVERSIONINFOEXW
        {
            public uint dwOSVersionInfoSize;
            public uint dwMajorVersion;
            public uint dwMinorVersion;
            public uint dwBuildNumber;
            public uint dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
            public ushort wServicePackMajor;
            public ushort wServicePackMinor;
            public ushort wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }

        [DllImport("NtosKrnl.exe")]
        private static extern int RtlGetVersion(out OSVERSIONINFOEXW lpVersionInformation);

        public static bool GetOsVersionInfo(out OSVERSIONINFOEXW versionInfo)
        {
            try
            {
                return RtlGetVersion(out versionInfo) == 0;
            }
            catch
            {
                versionInfo = new OSVERSIONINFOEXW();
                return false;
            }
        }

        public static bool IsWindows10OrGreater(in OSVERSIONINFOEXW versionInfo, int build) =>
            versionInfo.dwMajorVersion >= 10 && versionInfo.dwBuildNumber >= build;
    }
}
