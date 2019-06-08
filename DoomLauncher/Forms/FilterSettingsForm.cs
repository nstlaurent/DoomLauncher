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
    public partial class FilterSettingsForm : Form
    {
        private FilterForm m_filterForm;

        public FilterSettingsForm(ScreenFilter settings)
        {
            InitializeComponent();

            cmbMode.DropDownStyle = ComboBoxStyle.DropDownList;
            numOpacity.Value = Convert.ToDecimal(settings.Opacity);
            numThickness.Value = Convert.ToDecimal(settings.LineThickness);
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
                Opacity = Convert.ToSingle(numOpacity.Value),
                LineThickness = Convert.ToInt32(numThickness.Value),
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
