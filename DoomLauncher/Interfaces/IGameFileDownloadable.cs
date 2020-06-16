using System.ComponentModel;
using System.Net;

namespace DoomLauncher.Interfaces
{
    interface IGameFileDownloadable
    {
        event DownloadProgressChangedEventHandler DownloadProgressChanged;
        event AsyncCompletedEventHandler DownloadCompleted;

        void Download(IGameFileDataSourceAdapter adapter, string dlFilename);
        void Cancel();
        string FileName { get; set; }
        int FileSizeBytes { get; set; }
    }
}
