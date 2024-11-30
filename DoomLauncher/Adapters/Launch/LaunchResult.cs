
using static DoomLauncher.GameLauncher;

namespace DoomLauncher.Adapters.Launch
{
    public class LaunchResult
    {
        public GameLaunchInfo GameLaunchInfo { get; }

        public string ErrorMessage { get; }

        public bool Failed
        {
            get => !string.IsNullOrEmpty(ErrorMessage);
        }

        private LaunchResult(GameLaunchInfo gameLaunchInfo, string errorMessage)
        {
            GameLaunchInfo = gameLaunchInfo;
            ErrorMessage = errorMessage;
        }

        public static LaunchResult Success(GameLaunchInfo gameLaunchInfo) =>
            new LaunchResult(gameLaunchInfo, null);

        public static LaunchResult Failure(string errorMessage) =>
            new LaunchResult(null, errorMessage);
    }
}
