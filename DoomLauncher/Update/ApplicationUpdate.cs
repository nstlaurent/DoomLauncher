using Octokit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DoomLauncher
{
    public class ApplicationUpdate
    {
        private readonly TimeSpan m_timeout;

        public ApplicationUpdate(TimeSpan timeout)
        {
            m_timeout = timeout;
        }

        public async Task<ApplicationUpdateInfo> GetUpdateApplicationInfo(Version currentVersion)
        {
            try
            {
                var github = new GitHubClient(new ProductHeaderValue($"{Util.GitHubUser}.{Util.GitHubRepositoryName}"));
                github.SetRequestTimeout(m_timeout);
                var release = await github.Repository.Release.GetLatest(Util.GitHubUser, Util.GitHubRepositoryName);

                if (release != null)
                {
                    Version remoteVersion = new Version(release.Name);
                    if (remoteVersion > currentVersion)
                    {
                        ReleaseAsset asset;
                        if (LauncherPath.IsInstalled())
                            asset = release.Assets.FirstOrDefault(x => x.Name.EndsWith("_install.zip"));
                        else
                            asset = release.Assets.FirstOrDefault(x => x.Name.EndsWith(".zip") && !x.Name.EndsWith("_install.zip"));

                        if (asset != null)
                            return new ApplicationUpdateInfo(remoteVersion, asset.BrowserDownloadUrl, release.HtmlUrl, asset.CreatedAt.LocalDateTime);
                    }
                }
            }
            catch(RateLimitExceededException)
            {
                return null;
            }

            return null;
        }
    }
}
