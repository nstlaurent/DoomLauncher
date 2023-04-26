using DoomLauncher.DataSources;
using DoomLauncher.Forms;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class RequestScreenshotsEventArgs : EventArgs
    {
        public RequestScreenshotsEventArgs(IGameFile gameFile) { GameFile = gameFile; }
        public IGameFile GameFile { get; private set; }
    }

    class PictureItem
    {
        public PictureItem(PictureBox pb)
        {
            PictureBox = pb;
        }

        public PictureBox PictureBox { get; private set; }
        public IFileData CurrentFile { get; set; }
        public bool Skip { get; set; }
    }

    public partial class ScreenshotView : BasicFileView
    {
        private const double AspectWidth = 16;
        private const double AspectHeight = 9;
        private readonly Dictionary<PictureBox, IFileData> m_lookup = new Dictionary<PictureBox, IFileData>();
        private readonly List<PictureItem> m_pictureBoxes = new List<PictureItem>();
        private List<IFileData> m_screenshots = new List<IFileData>();
        private int m_pictureWidth;
        private CancellationTokenSource m_ct = new CancellationTokenSource();
        private bool m_imageWorkerComplete = true;

        public event EventHandler<RequestScreenshotsEventArgs> RequestScreenshots;

        private static readonly SolidBrush RectangleBrush = new SolidBrush(Color.FromArgb(128, Color.Black));

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

        public override bool ShowCreateNew(IWin32Window parent, IDataSourceAdapter adapter, ISourcePortData sourcePort, string filename, bool isMultiImport,
            out NewFileData newFileData)
        {
            newFileData = null;
            ScreenshotEditForm screenshotEditForm = new ScreenshotEditForm();
            screenshotEditForm.StartPosition = FormStartPosition.CenterParent;
            screenshotEditForm.SetData(adapter.GetGameFile(GameFile.FileName), null);

            if (screenshotEditForm.ShowDialog() == DialogResult.OK)
            {
                newFileData = new NewFileData()
                {
                    UserTitle = screenshotEditForm.Title,
                    UserDescription = screenshotEditForm.Description,
                    Map = screenshotEditForm.Map,
                    SourcePortID = -1,
                };
                return true;
            }

            return false;
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

        public override bool Edit()
        {
            if (SelectedFile == null)
                return false;

            return ScreenshotEditForm.ShowDialogAndUpdate(this, DataSourceAdapter, 
                DataSourceAdapter.GetGameFile(GameFile.FileName), SelectedFile);
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
            lock (m_pictureBoxes)
            {
                foreach (var item in m_pictureBoxes)
                {
                    PictureBox pb = item.PictureBox;
                    if (flpScreenshots.Controls.Contains(pb))
                        flpScreenshots.Controls.Remove(pb);
                    if (pb.Image != null)
                        pb.Image.Dispose();
                }

                m_pictureBoxes.Clear();
                ExpandPictureBoxes(50);
            }
        }

        private void ExpandPictureBoxes(int count)
        {
            for (int i = 0; i < count; i++)
                m_pictureBoxes.Add(new PictureItem(CreatePictureBox()));

            ToolTip tt = new ToolTip();
            foreach (var pb in m_pictureBoxes)
                tt.SetToolTip(pb.PictureBox, "Double-click to view");
        }

        public void SetScreenshots(List<IFileData> screenshots)
        {
            m_ct.Cancel();
            while (!m_imageWorkerComplete)
                Thread.Sleep(10);
            m_imageWorkerComplete = false;
            m_ct = new CancellationTokenSource();

            flpScreenshots.SuspendLayout();

            m_screenshots = screenshots.ToList();
            m_lookup.Clear();

            lock (m_pictureBoxes)
            {
                if (m_screenshots.Count > m_pictureBoxes.Count)
                    ExpandPictureBoxes(m_screenshots.Count - m_pictureBoxes.Count);

                List<PictureItem>.Enumerator enumerator = m_pictureBoxes.GetEnumerator();

                foreach (IFileData screen in screenshots)
                {
                    enumerator.MoveNext();
                    if (enumerator.Current == null)
                        break;

                    PictureBox pbScreen = enumerator.Current.PictureBox;
                    enumerator.Current.Skip = true;
                    flpScreenshots.Controls.Add(pbScreen);

                    m_lookup.Add(pbScreen, screen);

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
                            if (screen.Equals(enumerator.Current.CurrentFile))
                                continue;

                            enumerator.Current.Skip = false;
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
                }
            }

            var thread = new Thread(new ThreadStart(SetImages));
            thread.Start();
            flpScreenshots.ResumeLayout();
        }

        private void SetImages()
        {
            lock (m_pictureBoxes)
            {
                List<PictureItem>.Enumerator enumerator = m_pictureBoxes.GetEnumerator();
                foreach (var screen in m_screenshots)
                {
                    enumerator.MoveNext();
                    if (m_ct.IsCancellationRequested || enumerator.Current == null)
                        break;
                    if (enumerator.Current.Skip)
                        continue;

                    PictureBox pbScreen = enumerator.Current.PictureBox;
                    string file = Path.Combine(DataDirectory.GetFullPath(), screen.FileName);

                    try
                    {
                        using (var image = Image.FromFile(file))
                            pbScreen.Image = image.FixedSize(pbScreen.Width, pbScreen.Height, Color.Black);
                    }
                    catch
                    {
                        // Most likely file doesn't exist...
                        // Can also be out of memory exception
                    }

                    enumerator.Current.CurrentFile = screen;
                }

                enumerator.MoveNext();
                while (enumerator.Current != null)
                {
                    enumerator.Current.CurrentFile = null;
                    enumerator.MoveNext();
                }

                m_imageWorkerComplete = true;
            }
        }

        private PictureBox CreatePictureBox()
        {
            PictureBox pbScreen = new PictureBox
            {
                WaitOnLoad = false,
                BackColor = Color.Black,
                Width = m_pictureWidth
            };

            pbScreen.Height = Convert.ToInt32(pbScreen.Width / (AspectWidth / AspectHeight));
            pbScreen.SizeMode = PictureBoxSizeMode.Zoom;
            pbScreen.Margin = new Padding(7);
            pbScreen.MouseDown += pbScreen_MouseDown;
            pbScreen.DoubleClick += PbScreen_DoubleClick;
            pbScreen.Paint += PbScreen_Paint;
            return pbScreen;
        }

        private void PbScreen_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            if (pb == null || !m_lookup.TryGetValue(pb, out var fileData))
                return;

            string title = FileData.GetTitle(fileData);
            if (string.IsNullOrEmpty(title))
                return;

            DpiScale dpiScale = new DpiScale(e.Graphics);
            int padX = dpiScale.ScaleIntX(3);
            int padY = dpiScale.ScaleIntY(2);
            title = Util.GetClippedEllipsesText(e.Graphics, Font, title, new SizeF(pb.ClientSize.Width, FontHeight));

            SizeF size = e.Graphics.MeasureString(title, Font);
            RectangleF rect = new RectangleF(0, pb.ClientRectangle.Height - size.Height - padY,
                pb.ClientRectangle.Width, size.Height + padY);
            e.Graphics.FillRectangle(RectangleBrush, rect);
            e.Graphics.DrawString(title, Font, Brushes.White, new PointF(padX, pb.ClientRectangle.Height - size.Height - padY));
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
                var imagePaths = m_screenshots.Select(x => Path.Combine(DataDirectory.GetFullPath(), x.FileName)).ToArray();
                screenshotForm.SetImageFileData(DataSourceAdapter, DataSourceAdapter.GetGameFile(GameFile.FileName), imagePaths, m_screenshots);
                screenshotForm.WindowState = FormWindowState.Maximized;
                screenshotForm.Shown += ScreenshotForm_Shown;
                screenshotForm.ShowDialog(this);
                SetData(GameFile);
            }
        }

        private void ScreenshotForm_Shown(object sender, EventArgs e)
        {
            if (!(sender is ScreenshotViewerForm screenshotForm))
                return;

            if (SelectedFile != null)
                screenshotForm.SetImage(Path.Combine(DataDirectory.GetFullPath(), SelectedFile.FileName));
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

        public override bool EditAllowed => true;
    }
}
