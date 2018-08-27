using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.DataSources
{
    class IdGamesGameFile : GameFile, IGameFileDownloadable, IDisposable
    {
        public event DownloadProgressChangedEventHandler DownloadProgressChanged;
        public event AsyncCompletedEventHandler DownloadCompleted;

        private WebClient m_webClient;

        public override string Title
        {
            get { return title; }
            set { title = value; }
        }

        public override string Author
        {
            get { return author; }
            set { author = value; }
        }

        public override DateTime? ReleaseDate
        {
            get { return date; }
            set { if (value.HasValue) date = value.Value; }
        }

        public override string Description
        {
            get { return description; }
            set { description = value; }
        }

        public override double? Rating
        {
            get { return rating; }
            set { if (value.HasValue) rating = value.Value; }
        }

        public override int FileSizeBytes
        {
            get { return size; }
            set { size = value; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_webClient != null)
                m_webClient.Dispose();
        }

        public int id { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string description { get; set; }
        public DateTime? date { get; set; }
        public string filename { get; set; }
        public string dir { get; set; }
        public double rating { get; set; }
        public int size { get; set; }

        public void Download(IGameFileDataSourceAdapter adapter, string dlFilename)
        {
            IdGamesDataAdapater dataAdapter = adapter as IdGamesDataAdapater;

            if (dataAdapter != null)
            {
                m_webClient = new WebClient();
                m_webClient.DownloadProgressChanged += client_DownloadProgressChanged;
                m_webClient.DownloadFileCompleted += client_DownloadFileCompleted;
                m_webClient.DownloadFileAsync(new Uri(dataAdapter.MirrorUrl + dir + filename), dlFilename, id);
            }
        }

        public void Cancel()
        {
            if (m_webClient != null)
                m_webClient.CancelAsync();
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            m_webClient.Dispose();
            m_webClient = null;

            DownloadCompleted?.Invoke(this, e);
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChanged?.Invoke(this, e);
        }

        public override bool Equals(object obj)
        {
            IdGamesGameFile gameFile = obj as IdGamesGameFile;
            if (gameFile != null)
                return id == gameFile.id;

            return false;
        }

        public override int GetHashCode()
        {
            return id;
        }
    }
}
