using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher.Forms
{
    public enum ScreenFilterType
    {
        Ellipse,
        Scanline
    }

    public class ScreenFilter
    {
        public ScreenFilterType Type { get; set; }
        public int BlockSize { get; set; }     
        public float SpacingX { get; set; }
        public float SpacingY { get; set; }
        public bool Stagger { get; set; }
        public int ScanlineSpacing { get; set; }
        public bool VerticalScanlines { get; set; }
        public bool HorizontalScanlines { get; set; }
        public bool Enabled { get; set; }
    }

    public partial class FilterSettingsForm : Form
    {
        private FilterForm m_filterForm;

        public FilterSettingsForm(ScreenFilter settings)
        {
            InitializeComponent();
            cmbMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMode.SelectedIndex = (int)settings.Type;
            numBlockSize.Value = settings.BlockSize;
            numSpacingX.Value = Convert.ToDecimal(settings.SpacingX);
            numSpacingY.Value = Convert.ToDecimal(settings.SpacingY);
            chkStagger.Checked = settings.Stagger;
            numScanlineSize.Value = Convert.ToDecimal(settings.ScanlineSpacing);
            chkVertical.Checked = settings.VerticalScanlines;
            chkHorizontal.Checked = settings.HorizontalScanlines;
        }

        public ScreenFilter GetFilterSettings()
        {
            return new ScreenFilter()
            {
                Type = (ScreenFilterType)cmbMode.SelectedIndex,
                BlockSize = Convert.ToInt32(numBlockSize.Value),
                SpacingX = Convert.ToSingle(numSpacingX.Value),
                SpacingY = Convert.ToSingle(numSpacingY.Value),
                Stagger = chkStagger.Checked,
                ScanlineSpacing = Convert.ToInt32(numScanlineSize.Value),
                VerticalScanlines = chkVertical.Checked,
                HorizontalScanlines = chkHorizontal.Checked
            };
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (m_filterForm == null)
            {
                m_filterForm = new FilterForm(Screen.FromControl(this), GetFilterSettings());
                m_filterForm.Show(this);
                Task.Delay(3000).ContinueWith(t => CloseFilterFormTask());
            }
        }

        private void CloseFilterFormTask()
        {
            Invoke(new Action(CloseFilterForm));
        }

        private void CloseFilterForm()
        {
            m_filterForm.Close();
            m_filterForm = null;
        }

        private void cmbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            grpEllipse.Enabled = cmbMode.SelectedIndex != 1;
            grpScanline.Enabled = cmbMode.SelectedIndex == 1;
        }
    }
}
