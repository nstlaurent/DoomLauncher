using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class CToolStripRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            const string ArrowText = "⏵";
            SizeF size = e.Graphics.MeasureString(ArrowText, e.Item.Font);
            Color color = e.Item.Selected ? ColorTheme.Current.HighlightText : ColorTheme.Current.Text;
            e.Graphics.DrawString(ArrowText, e.Item.Font, new SolidBrush(color), new PointF(e.Item.Width - size.Width, 1));
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            Color color = e.Item.Selected ? ColorTheme.Current.HighlightText : ColorTheme.Current.Text;
            e.Graphics.DrawString(e.Item.Text, e.Item.Font, new SolidBrush(color), new PointF(24, 2));
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected)
            {
                Rectangle rectangle = new Rectangle(0, 0, e.Item.Size.Width - 1, e.Item.Size.Height - 1);
                e.Graphics.FillRectangle(new SolidBrush(ColorTheme.Current.Highlight), rectangle);
                return;
            }

            base.OnRenderMenuItemBackground(e);
        }
    }
}
