using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoomLauncher.Interfaces;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
using System.Threading;

namespace DoomLauncher
{
    public class RequestScreenshotsEventArgs : EventArgs
    {
        public RequestScreenshotsEventArgs(IGameFile gameFile) { GameFile = gameFile; }
        public IGameFile GameFile { get; private set; }
    }

    public partial class ScreenshotView : BasicFileView
    {
        private Dictionary<PictureBox, IFileData> m_lookup = new Dictionary<PictureBox, IFileData>();
        private double m_aspectWidth = 16, m_aspectHeight = 9;
        private List<PictureBox> m_pictureBoxes = new List<PictureBox>();

        public event EventHandler<RequestScreenshotsEventArgs> RequestScreenshots;

        public ScreenshotView()
        {
            InitializeComponent();
            flpScreenshots.Click += FlpScreenshots_Click;
        }

        private void FlpScreenshots_Click(object sender, EventArgs e)
        {
                        SelectedFile = null;
            foreach (PictureBox pbSet in m_lookup.Keys)
                SetSelectedStyle(pbSet, false);
        }

        private void pbScreen_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {

        }

        public override void SetData(IGameFile gameFile)
        {
            if (m_pictureBoxes.Count == 0)
            {
                InitPictureBoxes();
            }
            else
            {
                foreach (PictureBox pbSet in m_lookup.Keys)
                    SetSelectedStyle(pbSet, false);
            }

            flpScreenshots.SuspendLayout();
            flpScreenshots.Controls.Clear();

            flpScreenshots.ResumeLayout();

            m_lookup.Clear();

            if (gameFile != null && gameFile.GameFileID.HasValue)
            {
                RequestScreenshots?.Invoke(this, new RequestScreenshotsEventArgs(gameFile));
            }
        }

        public override void ClearData()
        {
            flpScreenshots.SuspendLayout();
            flpScreenshots.Controls.Clear();
            flpScreenshots.ResumeLayout();
            m_lookup.Clear();
        }

        private void InitPictureBoxes()
        {
            for (int i = 0; i < 50; i++)
                m_pictureBoxes.Add(CreatePictureBox());
        }

        public void SetScreenshots(List<IFileData> screenshots)
        {
            flpScreenshots.SuspendLayout();
            List<PictureBox>.Enumerator enumerator = m_pictureBoxes.GetEnumerator();

            foreach (IFileData screen in screenshots)
            {
                enumerator.MoveNext();
                if (enumerator.Current == null) break;

                PictureBox pbScreen = enumerator.Current;
                flpScreenshots.Controls.Add(pbScreen);

                FileStream fs = null;

                try
                {
                    if (screen.IsUrl)
                    {
                        pbScreen.CancelAsync();
                        pbScreen.Image = null;
                        pbScreen.LoadAsync(screen.FileName);
                    }
                    else
                    {
                        fs = new FileStream(Path.Combine(DataDirectory.GetFullPath(), screen.FileName), FileMode.Open, FileAccess.Read);
                        pbScreen.Image = Image.FromStream(fs);
                    }

                }
                catch
                {
                    pbScreen.Image = null;
                }
                finally
                {
                    if (fs != null) fs.Close();
                }

                m_lookup.Add(pbScreen, screen);
            }

            flpScreenshots.ResumeLayout();
        }

        private PictureBox CreatePictureBox()
        {
            PictureBox pbScreen = new PictureBox();
            pbScreen.WaitOnLoad = false;
            pbScreen.BackColor = Color.Black;
            pbScreen.Width = 200;
            pbScreen.Height = Convert.ToInt32(pbScreen.Width / (m_aspectWidth / m_aspectHeight));
            pbScreen.SizeMode = PictureBoxSizeMode.StretchImage;
            pbScreen.Margin = new Padding(7);
            pbScreen.Click += pbScreen_Click;
            pbScreen.MouseDown += pbScreen_MouseDown;
            pbScreen.LoadCompleted += pbScreen_LoadCompleted;
            pbScreen.ContextMenuStrip = m_menu;
            return pbScreen;
        }

        void pbScreen_MouseDown(object sender, MouseEventArgs e)
        {
            HandleClick(sender);
        }

        void pbScreen_Click(object sender, EventArgs e)
        {
            HandleClick(sender);
        }

        private void HandleClick(object sender)
        {
            if (sender is PictureBox pb && m_lookup.ContainsKey(pb))
            {
                foreach (PictureBox pbSet in m_lookup.Keys)
                {
                    SetSelectedStyle(pbSet, false);
                }

                SelectedFile = m_lookup[pb];
                SetSelectedStyle(pb, true);
            }
        }

        private void SetSelectedStyle(PictureBox pb, bool selected)
        {
            if (selected)
            {
                pb.BackColor = Color.LightBlue;
                pb.Padding = new Padding(2);
                pb.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            }
            else
            {
                pb.Padding = new Padding(0);
                pb.BorderStyle = System.Windows.Forms.BorderStyle.None;
            }
        }

        protected override IFileData[] Files
        {
            get
            {
                return m_lookup.Values.ToArray();
            }
        }

        protected override List<IFileData> GetSelectedFiles()
        {
            List<IFileData> files = new List<IFileData>();

            if (SelectedFile != null)
                files.Add(SelectedFile);

            return files;
        }

        private IFileData SelectedFile { get; set; }

        public override bool EditAllowed { get { return false; } }
    }
}
