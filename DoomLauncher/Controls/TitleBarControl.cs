using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DoomLauncher.Controls
{
    public partial class TitleBarControl : UserControl
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public string Title
        {
            get => lblTitle.Text; 
            set => lblTitle.Text = value;
        }

        public TitleBarControl()
        {
            InitializeComponent();
            flpButtons.MouseDown += FlpButtons_MouseDown;
            flpButtons.DoubleClick += FlpButtons_DoubleClick;
            Stylizer.StylizeControl(this, DesignMode);
        }

        public void SetControlBox(bool set)
        {
            flpButtons.Visible = set;
        }

        private void FlpButtons_DoubleClick(object sender, EventArgs e)
        {
            SetMinMax();
        }

        private void tblMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SetMinMax();
        }

        private void lblTitle_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SetMinMax();
        }

        private void FlpButtons_MouseDown(object sender, MouseEventArgs e)
        {
            HandleMouseDown(e);
        }

        private void tblMain_MouseDown(object sender, MouseEventArgs e)
        {
            HandleMouseDown(e);
        }

        private void HandleMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (e.Clicks != 1)
                return;

            if (ParentForm.WindowState != FormWindowState.Maximized)
                ParentForm.WindowState = FormWindowState.Normal;

            ReleaseCapture();
            SendMessage(ParentForm.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            ParentForm.Close();
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            SetMinMax();
        }

        private void SetMinMax()
        {
            if (ParentForm.WindowState == FormWindowState.Maximized)
                ParentForm.WindowState = FormWindowState.Normal;
            else
                ParentForm.WindowState = FormWindowState.Maximized;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            ParentForm.WindowState = FormWindowState.Minimized;
        }
    }
}
