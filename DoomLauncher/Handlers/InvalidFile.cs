namespace DoomLauncher
{
    public class InvalidFile
    {
        public InvalidFile(string filename, string reason)
        {
            FileName = filename;
            Reason = reason;
        }

        public string FileName { get; }
        public string Reason { get; }
    }
}
