using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class UpdateControl : UserControl
    {
        public static readonly string AppUpdateFileName = "AppUpdate.zip";

        private ApplicationUpdateInfo m_info;
        private AppConfiguration m_appConfig;
        private ProgressBarForm m_updateDownloadProgress;
        private WebClient m_updateWebClient;

        public UpdateControl()
        {
            InitializeComponent();

            Stylizer.StylizeControl(this, DesignMode);
        }

        public void Initialize(AppConfiguration appConfig, ApplicationUpdateInfo info)
        {
            m_info = info;
            m_appConfig = appConfig;

            lblVersion.Text = string.Format("{0} - {1}", m_info.Version.ToString(), m_info.ReleaseDate.ToShortDateString());
        }

        private void lnkPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(m_info.ReleasePageUrl);
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            m_updateDownloadProgress = new ProgressBarForm
            {
                Text = "Downloading",
                DisplayText = "Downloading Update...",
                Minimum = 0,
                Maximum = 100,
                StartPosition = FormStartPosition.CenterScreen
            };
            
            StartUpdateDownload(m_info);
            ShowUpdateDownloadProgress();
        }

        private void StartUpdateDownload(ApplicationUpdateInfo info)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ApplicationUpdateInfo>(StartUpdateDownload), new object[] { info });
            }
            else
            {
                m_updateWebClient = new WebClient();
                m_updateWebClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                m_updateWebClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
                m_updateWebClient.DownloadFileAsync(new Uri(info.DownloadUrl), GetUpdateArchive());
            }
        }

        private string GetUpdateArchive()
        {
            return Path.Combine(m_appConfig.TempDirectory.GetFullPath(), AppUpdateFileName);
        }

        private void ShowUpdateDownloadProgress()
        {
            if (InvokeRequired)
                Invoke(new Action(ShowUpdateDownloadProgress));
            else
                m_updateDownloadProgress.ShowDialog(this);
        }

        private void CloseUpdateDownloadProgress()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(CloseUpdateDownloadProgress));
            }
            else
            {
                m_updateWebClient.Dispose();
                m_updateDownloadProgress.Hide();
                m_updateDownloadProgress.Close();

                ApplicationUpdater applicationUpdater = new ApplicationUpdater(GetUpdateArchive(), AppDomain.CurrentDomain.BaseDirectory);
                if (!applicationUpdater.Execute())
                {
                    TextBoxForm form = new TextBoxForm(true, MessageBoxButtons.OK)
                    {
                        Text = "Update Error",
                        HeaderText = "The application was unable to update.Please download the update manually.",
                        DisplayText = applicationUpdater.LastError,
                        StartPosition = FormStartPosition.CenterScreen                        
                    };

                    form.ShowDialog(this);
                }
            }
        }

        private void UpdateDownloadProgress(int value)
        {
            if (InvokeRequired)
                Invoke(new Action<int>(UpdateDownloadProgress));
            else
                m_updateDownloadProgress.Value = value;
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            CloseUpdateDownloadProgress();
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            UpdateDownloadProgress(e.ProgressPercentage);
        }
    }
}
