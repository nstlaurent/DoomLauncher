using System.Collections.Generic;
using System.Windows.Forms;

namespace DoomLauncher.Handlers
{
    public class ToolTipGroup
    {

        private readonly List<(ToolTip, Control)> m_toolTips = new List<(ToolTip, Control)>();

        public void SetToolTip(Control control, string caption)
        {
            var tooltip = new ToolTip();
            tooltip.SetToolTip(control,  " " + caption);
            tooltip.BackColor = ColorTheme.Current.TextBoxBackground;
            tooltip.ForeColor = ColorTheme.Current.Text;
            tooltip.OwnerDraw = true;
            tooltip.Draw += ToolTip_Draw;
            tooltip.Popup += ToolTip_Popup;
            m_toolTips.Add((tooltip, control));
        }

        private void ToolTip_Popup(object sender, PopupEventArgs e)
        {
            foreach (var item in m_toolTips)
            {
                if (ReferenceEquals(sender, item.Item1))
                    continue;

                item.Item1.Hide(item.Item2);
            }
        }

        private static void ToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();
            e.DrawBorder();
            e.DrawText(TextFormatFlags.WordBreak);
        }
    }
}
