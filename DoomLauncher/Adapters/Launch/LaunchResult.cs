
namespace DoomLauncher.Adapters.Launch
{
    public class LaunchResult
    {
        public string ErrorMessage { get; }

        public bool Failed
        {
            get => !string.IsNullOrEmpty(ErrorMessage);
        }

        private LaunchResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public static LaunchResult Success() =>
            new LaunchResult(null);

        public static LaunchResult Failure(string errorMessage) =>
            new LaunchResult(errorMessage);
    }
}
