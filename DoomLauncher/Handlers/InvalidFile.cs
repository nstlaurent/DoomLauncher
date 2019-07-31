namespace DoomLauncher
{
    public class InvalidFile
    {
        public InvalidFile(string filename, string reason)
        {
            FileName = filename;
            Reason = reason;
        }

        public string FileName { get; set; }
        public string Reason { get; set; }
    }
}
