namespace DoomLauncher.Handlers
{
    public class VersionUpdateResults
    {
        public readonly bool RestartRequired;

        public VersionUpdateResults(bool restartRequired)
        {
            RestartRequired = restartRequired;
        }
    }
}
