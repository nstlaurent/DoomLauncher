using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class FlowLayoutPanelDB : FlowLayoutPanel
    {
        public bool CaptureArrowKeys { get; set; } = false; 

        public FlowLayoutPanelDB()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (CaptureArrowKeys)
            {
                switch (keyData)
                {
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Left:
                    case Keys.Right:
                        return true;
                    case Keys.Shift | Keys.Up:
                    case Keys.Shift | Keys.Down:
                    case Keys.Shift | Keys.Left:
                    case Keys.Shift | Keys.Right:
                        return true;
                }
            }

            return base.IsInputKey(keyData);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_CLIPCHILDREN
                return cp;
            }
        }
    }
}
