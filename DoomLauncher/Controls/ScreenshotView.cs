using DoomLauncher.Forms;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class RequestScreenshotsEventArgs : EventArgs
    {
        public RequestScreenshotsEventArgs(IGameFile gameFile) { GameFile = gameFile; }
        public IGameFile GameFile { get; private set; }
    }

    public partial class ScreenshotView : BasicFileView
    {
        private readonly Dictionary<PictureBox, IFileData> m_lookup = new Dictionary<PictureBox, IFileData>();
        private readonly double m_aspectWidth = 16;
        private readonly double m_aspectHeight = 9;
        private readonly List<PictureBox> m_pictureBoxes = new List<PictureBox>();
        private List<IFileData> m_screenshots = new List<IFileData>();
        private int m_pictureWidth;
        private CancellationTokenSource m_ct = new CancellationTokenSource();
        private bool m_imageWorkerComplete = true;

        public event EventHandler<RequestScreenshotsEventArgs> RequestScreenshots;

        public ScreenshotView()
        {
            InitializeComponent();
            flpScreenshots.Click += FlpScreenshots_Click;
        }

        public void SetPictureWidth(int pictureWidth)
        {
            m_pictureWidth = pictureWidth;
            InitPictureBoxes();
        }

        private void FlpScreenshots_Click(object sender, EventArgs e)
        {
            SelectedFile = null;
            foreach (PictureBox pbSet in m_lookup.Keys)
                SetSelectedStyle(pbSet, false);
        }

        public override void SetData(IGameFile gameFile)
        {
            SelectedFile = null;

            foreach (PictureBox pbSet in m_lookup.Keys)
                SetSelectedStyle(pbSet, false);

            flpScreenshots.SuspendLayout();
            flpScreenshots.Controls.Clear();

            flpScreenshots.ResumeLayout();

            m_lookup.Clear();

            if (gameFile != null && gameFile.GameFileID.HasValue)
                RequestScreenshots?.Invoke(this, new RequestScreenshotsEventArgs(gameFile));
        }

        public override void ClearData()
        {
            SelectedFile = null;
            flpScreenshots.SuspendLayout();
            flpScreenshots.Controls.Clear();
            flpScreenshots.ResumeLayout();
            m_lookup.Clear();
        }

        public override bool New()
        {
            if (base.New())
            {
                ThumbnailManager.UpdateThumbnail(GameFile);
                return true;
            }

            return false;
        }

        public override bool Delete()
        {
            if (base.Delete())
            {
                ThumbnailManager.UpdateThumbnail(GameFile);
                return true;
            }
            
            return false;
        }

        public override bool MoveFileOrderUp()
        {
            if (base.MoveFileOrderUp())
            {
                ThumbnailManager.UpdateThumbnail(GameFile);
                return true;
            }

            return false;
        }

        public override bool MoveFileOrderDown()
        {
            if (base.MoveFileOrderDown())
            {
                ThumbnailManager.UpdateThumbnail(GameFile);
                return true;
            }

            return false;
        }

        public override bool SetFileOrderFirst()
        {
            if (base.SetFileOrderFirst())
            {
                ThumbnailManager.UpdateThumbnail(GameFile);
                return true;
            }

            return false;
        }

        private void InitPictureBoxes()
        {
            foreach (var pb in m_pictureBoxes)
            {
                if (flpScreenshots.Controls.Contains(pb))
                    flpScreenshots.Controls.Remove(pb);
                if (pb.Image != null)
                    pb.Image.Dispose();
            }

            m_pictureBoxes.Clear();

            ExpandPictureBoxes(50);
        }

        private void ExpandPictureBoxes(int count)
        {
            for (int i = 0; i < count; i++)
                m_pictureBoxes.Add(CreatePictureBox());

            ToolTip tt = new ToolTip();
            foreach (var pb in m_pictureBoxes)
                tt.SetToolTip(pb, "Double-click to view");
        }

        public void SetScreenshots(List<IFileData> screenshots)
        {
            m_ct.Cancel();
            while (!m_imageWorkerComplete)
                Thread.Sleep(10);
            m_imageWorkerComplete = false;
            m_ct = new CancellationTokenSource();

            flpScreenshots.SuspendLayout();

            foreach (var pb in m_pictureBoxes)
                pb.ImageLocation = string.Empty;

            m_screenshots = screenshots.ToList();
            m_lookup.Clear();

            if (m_screenshots.Count > m_pictureBoxes.Count)
                ExpandPictureBoxes(m_screenshots.Count - m_pictureBoxes.Count);

            List<PictureBox>.Enumerator enumerator = m_pictureBoxes.GetEnumerator();

            foreach (IFileData screen in screenshots)
            {
                enumerator.MoveNext();
                if (enumerator.Current == null)
                    break;

                PictureBox pbScreen = enumerator.Current;
                flpScreenshots.Controls.Add(pbScreen);

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
                        Image image = pbScreen.Image;
                        pbScreen.Image = null;
                        if (image != null)
                            image.Dispose();
                    }
                }
                catch
                {
                    pbScreen.ImageLocation = string.Empty;
                }

                m_lookup.Add(pbScreen, screen);
            }

            var thread = new Thread(new ThreadStart(SetImages));
            thread.Start();
            flpScreenshots.ResumeLayout();
        }

        private void SetImages()
        {
            List<PictureBox>.Enumerator enumerator = m_pictureBoxes.GetEnumerator();
            foreach (var screen in m_screenshots)
            {
                if (m_ct.IsCancellationRequested)
                    break;

                enumerator.MoveNext();
                if (enumerator.Current == null)
                    break;

                PictureBox pbScreen = enumerator.Current;
                try
                {
                    using (var image = Image.FromFile(Path.Combine(DataDirectory.GetFullPath(), screen.FileName)))
                        pbScreen.Image = Util.FixedSize(image, pbScreen.Width, pbScreen.Height, Color.Black);
                }
                catch
                {
                    // Most likely file doesn't exist...
                    // Can also be out of memory exception
                }
            }

            m_imageWorkerComplete = true;
        }

        private PictureBox CreatePictureBox()
        {
            PictureBox pbScreen = new PictureBox
            {
                WaitOnLoad = false,
                BackColor = Color.Black,
                Width = m_pictureWidth
            };

            pbScreen.Height = Convert.ToInt32(pbScreen.Width / (m_aspectWidth / m_aspectHeight));
            pbScreen.SizeMode = PictureBoxSizeMode.Zoom;
            pbScreen.Margin = new Padding(7);
            pbScreen.MouseDown += pbScreen_MouseDown;
            pbScreen.DoubleClick += PbScreen_DoubleClick;
            return pbScreen;
        }

        private void PbScreen_DoubleClick(object sender, EventArgs e)
        {
            HandleDoubleClick(sender);
        }

        void pbScreen_MouseDown(object sender, MouseEventArgs e)
        {
            HandleClick(sender, e);
        }

        private void HandleDoubleClick(object sender)
        {
            if (sender is PictureBox pb && m_lookup.ContainsKey(pb))
            {
                HandleClick(pb, null);
                View();
            }
        }

        public override void View()
        {
            if (m_screenshots.Count > 0)
            {
                ScreenshotViewerForm screenshotForm = new ScreenshotViewerForm();
                screenshotForm.StartPosition = FormStartPosition.CenterParent;
                screenshotForm.SetImages(m_screenshots.Select(x => Path.Combine(DataDirectory.GetFullPath(), x.FileName)).ToArray());
                if (SelectedFile != null)
                    screenshotForm.SetImage(Path.Combine(DataDirectory.GetFullPath(), SelectedFile.FileName));
                screenshotForm.WindowState = FormWindowState.Maximized;
                screenshotForm.ShowDialog(this);
            }
        }

        private void HandleClick(object sender, MouseEventArgs e)
        {
            if (sender is PictureBox pb && m_lookup.ContainsKey(pb))
            {
                if (SelectedFile != m_lookup[pb])
                {
                    foreach (PictureBox pbSet in m_lookup.Keys)
                        SetSelectedStyle(pbSet, false);

                    SelectedFile = m_lookup[pb];
                    SetSelectedStyle(pb, true);
                }

                if (e != null && e.Button == MouseButtons.Right)
                    m_menu.Show(pb.PointToScreen(e.Location));
            }
        }

        private void SetSelectedStyle(PictureBox pb, bool selected)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<PictureBox, bool>(SetSelectedStyle), new object[] { pb, selected });
            }
            else
            {
                if (selected)
                {
                    pb.BackColor = SystemColors.Highlight;
                    pb.Padding = new Padding(2);
                    pb.BorderStyle = BorderStyle.Fixed3D;
                }
                else
                {
                    pb.BackColor = Color.Black;
                    pb.Padding = new Padding(0);
                    pb.BorderStyle = BorderStyle.None;
                }
            }
        }

        protected override IFileData[] Files => m_lookup.Values.ToArray();

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
