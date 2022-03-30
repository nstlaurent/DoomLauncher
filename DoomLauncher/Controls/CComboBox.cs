
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class CComboBox : ComboBox
    {
        private const int WM_PAINT = 0xF;

        public CComboBox()
        {
            BorderColor = ColorTheme.Current.ControlLight;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "DimGray")]
        public Color BorderColor { get; set; }

        public void SetEnabled(bool set)
        {
            Enabled = set;
            Visible = set;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg != WM_PAINT)
                return;
            
            using (var g = Graphics.FromHwnd(Handle))
            {
                using (var p = new Pen(BorderColor, 1))
                    g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);

                using (var p = new Pen(BorderColor, 2))
                    g.DrawRectangle(p, 2, 2, Width - SystemInformation.HorizontalScrollBarArrowWidth - 4, Height - 4);
            }            
        }
    }
}
