using System;

namespace DoomLauncher
{
    public class ApplicationUpdateInfo
    {
        public ApplicationUpdateInfo(Version version, string downloadUrl, string releasePage, DateTime releaseDate)
        {
            Version = version;
            DownloadUrl = downloadUrl;
            ReleasePageUrl = releasePage;
            ReleaseDate = releaseDate;
        }

        public readonly Version Version;
        public readonly string DownloadUrl;
        public readonly string ReleasePageUrl;
        public readonly DateTime ReleaseDate;
    }
}
