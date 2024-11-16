namespace DoomLauncher.Adapters.Launch
{
    public class LaunchResult
    {
        public string ParamString { get; } // Never null

        public string ErrorMessage { get; }

        public string RecordedFileName { get; }

        public bool Exclusive { get; }

        public bool Failed { get => ErrorMessage != null; }

        public static readonly LaunchResult EMPTY = LaunchResult.Param(""); 

        private LaunchResult(string paramString, string recordedFileName, string errorMessage, bool exclusive) 
        { 
            ParamString = paramString ?? "";
            ErrorMessage = errorMessage;
            RecordedFileName = recordedFileName;
            Exclusive = exclusive;
        }

        public LaunchResult Combine(LaunchResult other)
        {
            if (Failed)
            {
                return this;
            }
            else if (other.Failed)
            {
                return other;
            }
            else if (Exclusive)
            {
                return this;
            }
            else
            {
                return ParamWithRecording($"{ParamString} {other.ParamString}".Trim(), RecordedFileName ?? other.RecordedFileName);
            }
        }

        public static LaunchResult ExclusiveParam(string paramString)
        {
            return new LaunchResult(paramString, null, null, true);
        }

        public static LaunchResult Param(string paramString)
        {
            return new LaunchResult(paramString, null, null, false);
        }

        public static LaunchResult ParamWithRecording(string paramString, string recordedFileName)
        {
            return new LaunchResult(paramString, recordedFileName, null, false);
        }

        public static LaunchResult Failure(string errorMessage)
        {
            return new LaunchResult("", null, errorMessage, false);
        }
    }
}
