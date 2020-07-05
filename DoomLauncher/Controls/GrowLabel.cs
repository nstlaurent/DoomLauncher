using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class GrowLabel : Label
    {
        private bool mGrowing;

        public GrowLabel()
        {
            AutoSize = false;
            IsPath = false;
        }

        public bool IsPath { get; set; }

        private void ResizeLabel()
        {
            if (mGrowing)
                return;

            try
            {
                mGrowing = true;
                Size sz = new Size(Width, int.MaxValue);
                string text = Text;

                if (IsPath)
                    text = Text.Replace(Path.DirectorySeparatorChar, ' ').Replace(Path.AltDirectorySeparatorChar, ' ');

                sz = TextRenderer.MeasureText(text, Font, sz, TextFormatFlags.WordBreak);
                Height = sz.Height;
            }
            finally
            {
                mGrowing = false;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            ResizeLabel();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            ResizeLabel();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ResizeLabel();
        }
    }
}
