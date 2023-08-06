using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DoomLauncher.WindowsVersion;

namespace DoomLauncher
{
    public static class ImmersiveDarkMode
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        public static bool UseImmersiveDarkMode(Form form, bool enabled)
        {
            try
            {
                if (!WindowsVersionInfo.GetOsVersionInfo(out var versionInfo))
                    return false;
                
                if (WindowsVersionInfo.IsWindows10OrGreater(versionInfo, 17763))
                {
                    var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                    if (WindowsVersionInfo.IsWindows10OrGreater(versionInfo, 18985))
                        attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;

                    int useImmersiveDarkMode = enabled ? 1 : 0;
                    return DwmSetWindowAttribute(form.Handle, attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;
                }
            }
            catch { }

            return false;
        }
    }
}
