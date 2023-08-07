using Microsoft.Win32;
using System;

namespace DoomLauncher.WindowsVersion
{
    public enum WindowsTheme
    {
        Dark,
        Light,
        Unknown
    }

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

        public static WindowsTheme GetTheme()
        {
            try
            {
                int res = (int)Registry.GetValue(
                    "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                    "AppsUseLightTheme", -1);
                switch (res)
                {
                    case 0: return WindowsTheme.Dark;
                    case 1: return WindowsTheme.Light;
                    default: return WindowsTheme.Unknown;
                }
            }
            catch
            {
                return WindowsTheme.Unknown;
            }
        }
    }
}
