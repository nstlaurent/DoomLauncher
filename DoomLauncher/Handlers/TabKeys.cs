namespace DoomLauncher
{
    public static class TabKeys
    {
        public static readonly string RecentKey = "Recent";
        public static readonly string LocalKey = "Local";
        public static readonly string IWadsKey = "IWads";
        public static readonly string IdGamesKey = "Id Games";
        public static readonly string UntaggedKey = "Untagged";

        public static string[] KeyNames => new string[] { RecentKey, LocalKey, RecentKey, IdGamesKey, UntaggedKey };
}
}
