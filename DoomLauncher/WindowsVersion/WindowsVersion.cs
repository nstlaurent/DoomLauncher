using System;

namespace DoomLauncher.WindowsVersion
{
    public static class WindowsVersionInfo
    {
        public static bool GetOsVersionInfo(out Version versionInfo)
        {
            try
            {
                versionInfo = Environment.OSVersion.Version;
                return true;
            }
            catch
            {
                versionInfo = new Version();
                return false;
            }
        }

        public static bool IsWindows10OrGreater(Version versionInfo, int build) =>
            versionInfo.Major >= 10 && versionInfo.Build >= build;
    }
}
