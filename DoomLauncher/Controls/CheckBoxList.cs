using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher.Controls
{
    public class CheckListBoxEventArgs
    {
        public readonly bool Checked;
        public readonly string Text;

        public CheckListBoxEventArgs(bool check, string text)
        {
            Checked = check;
            Text = text;
        }
    }

    public partial class CheckBoxList : UserControl
    {
        public event EventHandler<CheckListBoxEventArgs> ItemCheck;

        public List<string> Items { get; private set; } = new List<string>();
        private List<CCheckBox> m_checkBoxes = new List<CCheckBox>();

        public CheckBoxList()
        {
            InitializeComponent();
        }

        public void SetItems(IEnumerable<string> items)
        {
            tblMain.RowStyles.Clear();
            Items = items.ToList();

            DpiScale dpiScale = new DpiScale(CreateGraphics());
            int rowHeight = dpiScale.ScaleIntY(20);
            int padding = dpiScale.ScaleIntX(4);
            int row = 0;
            foreach (var item in items)
            {
                var checkBox = new CCheckBox()
                {
                    Dock = DockStyle.Fill,
                    Text = item,
                    Margin = new Padding(padding, padding, 0, 0)
                    
                };
                checkBox.CheckedChanged += CheckBox_CheckedChanged;
                m_checkBoxes.Add(checkBox);

                tblMain.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight));
                tblMain.Controls.Add(checkBox, 0, row);
                row++;
            }
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CCheckBox)sender;
            ItemCheck?.Invoke(this, new CheckListBoxEventArgs(checkBox.Checked, checkBox.Text));
        }

        public string[] GetCheckedItems()
        {
            List<string> checkedItems = new List<string>();
            foreach (var checkBox in m_checkBoxes)
            {
                if (checkBox.Checked)
                    checkedItems.Add(checkBox.Text);
            }
            return checkedItems.ToArray();
        }

        public bool IsChecked(string text)
        {
            foreach (var checkBox in m_checkBoxes)
            {
                if (checkBox.Text == text)
                    return checkBox.Checked;
            }
            return false;
        }

        public void SetChecked(string text, bool check)
        {
            foreach (var checkBox in m_checkBoxes)
            {
                if (checkBox.Text == text)
                {
                    checkBox.Checked = check;
                    break;
                }
            }
        }
    }
}
