namespace DoomLauncher.Interfaces
{
    public interface INewFileDetector
    {
        void StartDetection();
        string[] GetNewFiles();
        string[] GetModifiedFiles();
        string[] GetDeletedFiles();
    }
}
