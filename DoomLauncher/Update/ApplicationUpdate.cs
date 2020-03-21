using Octokit;
using System;
using System.Threading.Tasks;
using System.Linq;

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
            var github = new GitHubClient(new ProductHeaderValue($"{Util.GitHubUser}.{Util.GitHubRepositoryName}"));
            github.SetRequestTimeout(m_timeout);
            var release = await github.Repository.Release.GetLatest(Util.GitHubUser, Util.GitHubRepositoryName);

            if (release != null)
            {
                Version remoteVersion = new Version(release.Name);
                if (remoteVersion > currentVersion)
                {
                    var asset = release.Assets.FirstOrDefault(x => x.Name.EndsWith(".zip"));
                    if (asset != null)
                        return new ApplicationUpdateInfo(remoteVersion, asset.BrowserDownloadUrl, release.HtmlUrl, asset.CreatedAt.LocalDateTime);
                }
            }

            return null;
        }
    }
}
