using Microsoft.Win32;
using System;

namespace DoomLauncher.GameStores.Steam
{
    public static class SteamRegistry
    {
        // As documented at https://help.steampowered.com/en/faqs/view/3C73-90F9-F600-0266
        private const string STEAM_REGISTRY_KEY_32 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam";
        private const string STEAM_REGISTRY_KEY_64 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam";

        public static string GetSteamPath()
        {
            var steamKey = Environment.Is64BitOperatingSystem ? STEAM_REGISTRY_KEY_64 : STEAM_REGISTRY_KEY_32;
            var installPath = Registry.GetValue(steamKey, "InstallPath", null);
            return installPath?.ToString();
        }
    }
}
