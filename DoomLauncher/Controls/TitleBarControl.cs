using System;
using System.Drawing;
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

        public FormWindowState WindowState { get; private set; }
        private bool m_maximized;
        private bool m_dragging;
        private bool m_settingState;
        private Size? m_size;
        private Point? m_location;

        public string Title
        {
            get => lblTitle.Text; 
            set => lblTitle.Text = value;
        }

        public bool CanClose { get; set; } = true;
        public bool ControlBox
        {
            get { return flpButtons.Visible; }
            set { flpButtons.Visible = value; }
        }

        public TitleBarControl()
        {
            InitializeComponent();
            flpButtons.MouseDown += FlpButtons_MouseDown;
            flpButtons.DoubleClick += FlpButtons_DoubleClick;
            Stylizer.StylizeControl(this, DesignMode);
            Load += TitleBarControl_Load;
        }

        public void HandleWindowStateChange(FormWindowState state)
        {
            if (state == FormWindowState.Maximized)
                SetMaximized();
        }

        public void SetNormal() => SetNormal(true);

        private void TitleBarControl_Load(object sender, EventArgs e)
        {
            if (ParentForm == null)
                return;

            ParentForm.LocationChanged += TitleBarControl_LocationChanged;
            WindowState = ParentForm.WindowState;

            if (WindowState == FormWindowState.Normal)
            {
                m_size = ParentForm.ClientSize;
                m_location = ParentForm.Location;
            }
        }

        private void TitleBarControl_LocationChanged(object sender, EventArgs e)
        {
            if (!m_settingState && m_dragging && WindowState == FormWindowState.Maximized)
            {
                m_dragging = false;
                SetNormal(false);
            }

            if (WindowState == FormWindowState.Normal)
                m_location = ParentForm.Location;
            else if (WindowState == FormWindowState.Minimized && ParentForm.WindowState != FormWindowState.Minimized)
                WindowState = m_maximized ? FormWindowState.Maximized : FormWindowState.Normal;
        }

        private void SetNormal(bool centerY)
        {
            m_maximized = false;
            WindowState = FormWindowState.Normal;
            ParentForm.WindowState = FormWindowState.Normal;
            var bounds = Screen.GetWorkingArea(ParentForm);

            if (m_size == null || ParentForm is MainForm)
            {
                ParentForm.Location = new Point(bounds.X + (int)(bounds.Width * 0.12),
                    centerY ? bounds.Y + (int)(bounds.Height * 0.12) : 0);
                ParentForm.Size = new Size((int)(bounds.Width * 0.75), (int)(bounds.Height * 0.75));
                return;
            }

            if (m_location.HasValue)
                ParentForm.Location = m_location.Value;
            ParentForm.Size = m_size.Value;
        }

        private void SetMaximized()
        {
            m_maximized = true;
            WindowState = FormWindowState.Maximized;
            ParentForm.WindowState = FormWindowState.Normal;
            var bounds = Screen.GetWorkingArea(ParentForm);
            ParentForm.Location = bounds.Location;
            ParentForm.Width = bounds.Width;
            ParentForm.Height = bounds.Height;
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

            m_dragging = true;

            ReleaseCapture();
            SendMessage(ParentForm.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (CanClose)
                ParentForm.Close();
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            SetMinMax();
        }

        private void SetMinMax()
        {
            m_settingState = true;
            if (m_maximized)
                SetNormal(true);
            else
                SetMaximized();
            m_settingState = false;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            ParentForm.WindowState = FormWindowState.Minimized;
        }
    }
}
