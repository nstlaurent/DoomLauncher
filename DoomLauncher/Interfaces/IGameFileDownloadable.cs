using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
