using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    internal class Stylizer
    {
        private static readonly IThemeColors CurrentTheme = ColorTheme.Current;

        public static void Stylize(Form form, bool designMode, params Control[] ignoreControls)
        {
            if (designMode)
                return;

            foreach (Control control in form.Controls)
            {
                if (ignoreControls.Contains(control))
                    continue;

                StylizeControl(control, designMode);
            }
        }

        public static void StylizeControl(Control control, bool designMode)
        {
            if (designMode)
                return;

            if (control is DataGridView grid)
                StyleGrid(grid);
            else if (control is FormButton formButton)
                StyleFormButton(formButton);
            else if (control is Button button)
                StyleButton(button);
            else if (control is TextBox textBox)
                StyleTextBox(textBox);
            else if (control is LinkLabel linkLabel)
                StyleLinkLabel(linkLabel);
            else if (control is ComboBox comboBox)
                StyleCombo(comboBox);
            else if (control is CheckBox checkBox)
                StyleCheckBox(checkBox);
            else if (control is CheckedListBox checkedListBox)
                StyleCheckdListBox(checkedListBox);
            else if (control is ContextMenuStrip cms)
                StyleContextMenuStrip(cms);
            else if (control is GroupBox groupBox)
                StyleGroupBox(groupBox);
            else
                StyleDefault(control);

            foreach (Control subControl in control.Controls)
                StylizeControl(subControl, designMode);
        }

        private static void StyleTextBox(TextBox textBox)
        {
            textBox.BackColor = CurrentTheme.TextBoxBackground;
            textBox.ForeColor = CurrentTheme.Text;
        }

        private static void TextBox_Paint(object sender, PaintEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Pen p = new Pen(Color.Red);
            Graphics g = e.Graphics;
            int variance = 3;
            g.DrawRectangle(p, new Rectangle(textBox.Location.X - variance, textBox.Location.Y - variance, textBox.Width + variance, textBox.Height + variance));
        }

        private static void StyleGroupBox(GroupBox groupBox)
        {
            groupBox.Paint += GroupBox_Paint;
        }

        private static void GroupBox_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = (GroupBox)sender;
            Graphics g = e.Graphics;
            e.Graphics.Clear(SystemColors.Control);
            e.Graphics.DrawString(box.Text, box.Font, Brushes.Black, 0, 0);

            Brush textBrush = new SolidBrush(CurrentTheme.Text);
            Brush borderBrush = new SolidBrush(CurrentTheme.Border);
            Pen borderPen = new Pen(borderBrush);
            SizeF strSize = g.MeasureString(box.Text, box.Font);
            Rectangle rect = new Rectangle(box.ClientRectangle.X,
                                           box.ClientRectangle.Y + (int)(strSize.Height / 2),
                                           box.ClientRectangle.Width - 1,
                                           box.ClientRectangle.Height - (int)(strSize.Height / 2) - 1);

            
            g.Clear(box.BackColor);
            g.DrawString(box.Text, box.Font, textBrush, box.Padding.Left, 0);

            g.DrawLine(borderPen, rect.Location, new Point(rect.X, rect.Y + rect.Height));
            g.DrawLine(borderPen, new Point(rect.X + rect.Width, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height));
            g.DrawLine(borderPen, new Point(rect.X, rect.Y + rect.Height), new Point(rect.X + rect.Width, rect.Y + rect.Height));
            g.DrawLine(borderPen, new Point(rect.X, rect.Y), new Point(rect.X + box.Padding.Left, rect.Y));
            g.DrawLine(borderPen, new Point(rect.X + box.Padding.Left + (int)(strSize.Width), rect.Y), new Point(rect.X + rect.Width, rect.Y));
        }

        private static void StyleDefault(Control control)
        {
            control.BackColor = CurrentTheme.Window;
            control.ForeColor = CurrentTheme.Text;
        }

        public static void StylizeControl(ToolStripDropDownButton control, bool designMode)
        {
            if (designMode)
                return;

            control.BackColor = CurrentTheme.Window;
            control.ForeColor = CurrentTheme.Text;

            foreach (var item in control.DropDownItems)
            {
                if (!(item is ToolStripMenuItem menuItem))
                    continue;

                menuItem.BackColor = CurrentTheme.Window;
                menuItem.ForeColor = CurrentTheme.Text;
                StyleMenuDropDownItems(menuItem);
            }
        }

        private static void StyleContextMenuStrip(ContextMenuStrip cms)
        {
            cms.BackColor = CurrentTheme.Window;
            cms.ForeColor = CurrentTheme.Text;

            cms.ShowCheckMargin = false;
            cms.ShowImageMargin = false;

            foreach (var item in cms.Items)
            {
                if (!(item is ToolStripMenuItem menuItem))
                    continue;

                menuItem.BackColor = CurrentTheme.Window;
                menuItem.ForeColor = CurrentTheme.Text;
                StyleMenuDropDownItems(menuItem);
            }
        }

        private static void Separator_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private static void StyleMenuDropDownItems(ToolStripMenuItem menuItem)
        {
            foreach (var subItem in menuItem.DropDownItems)
            {
                if (subItem is ToolStripSeparator separator)
                {
                    //separator.Paint += Separator_Paint;
                    separator.BackColor = CurrentTheme.Window;
                    separator.ForeColor = CurrentTheme.Text;
                    continue;
                }

                if (!(subItem is ToolStripMenuItem subMenuItem))
                    continue;

                subMenuItem.BackColor = CurrentTheme.Window;
                subMenuItem.ForeColor = CurrentTheme.Text;
            }
        }

        private static void StyleCheckdListBox(CheckedListBox checkedListBox)
        {
            checkedListBox.BorderStyle = BorderStyle.None;
        }

        private static void StyleLinkLabel(LinkLabel label)
        {
            label.LinkColor = CurrentTheme.LinkText;
        }

        private static void StyleButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = CurrentTheme.ButtonBackground;
        }

        private static void StyleFormButton(FormButton button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = CurrentTheme.Window;
        }

        private static void StyleCheckBox(CheckBox checkBox)
        {
            checkBox.ForeColor = CurrentTheme.Text;
            checkBox.BackColor = CurrentTheme.CheckBoxBackground;
            //checkBox.Appearance = Appearance.Button;
            checkBox.FlatStyle = FlatStyle.Flat;
            checkBox.TextAlign = ContentAlignment.MiddleRight;
            checkBox.FlatAppearance.BorderSize = 0;
            checkBox.FlatAppearance.MouseOverBackColor = CurrentTheme.Window;
            checkBox.FlatAppearance.CheckedBackColor = CurrentTheme.Window;
            checkBox.FlatAppearance.MouseDownBackColor = CurrentTheme.Window;
            checkBox.AutoSize = true;
        }

        private static void StyleCombo(ComboBox comboBox)
        {
            comboBox.BackColor = CurrentTheme.TextBoxBackground;
            comboBox.ForeColor = CurrentTheme.Text;
            comboBox.DrawItem += ComboBox_DrawItem;
            comboBox.FlatStyle = FlatStyle.Flat;
        }

        private static void StyleGrid(DataGridView view)
        {
            view.BackgroundColor = CurrentTheme.WindowDark;

            view.DefaultCellStyle.NullValue = "N/A";
            view.RowHeadersVisible = false;
            view.AutoGenerateColumns = false;
            view.ShowCellToolTips = false;
            view.EnableHeadersVisualStyles = false;

            view.ColumnHeadersDefaultCellStyle.BackColor = CurrentTheme.Window;
            view.ColumnHeadersDefaultCellStyle.ForeColor = CurrentTheme.Text;
            view.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            view.ForeColor = CurrentTheme.Text;
            view.BackgroundColor = CurrentTheme.WindowDark;
            view.RowsDefaultCellStyle.ForeColor = CurrentTheme.Text;
            view.RowsDefaultCellStyle.BackColor = CurrentTheme.GridRow;
            view.AlternatingRowsDefaultCellStyle.ForeColor = CurrentTheme.Text;
            view.AlternatingRowsDefaultCellStyle.BackColor = CurrentTheme.GridRowAlt;
            view.DefaultCellStyle.SelectionBackColor = CurrentTheme.Highlight;

            if (!CurrentTheme.GridRowBorder)
                view.CellPainting += View_CellPainting;
        }

        private static void View_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            e.AdvancedBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
            e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
            e.AdvancedBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
            e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
        }

        private static void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(CurrentTheme.Window), e.Bounds);
        }
    }
}
