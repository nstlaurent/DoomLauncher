using DoomLauncher.Controls;
using System;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class TagSelectForm : Form
    {
        public TagSelectForm()
        {
            InitializeComponent();
            Stylizer.Stylize(this, DesignMode, StylizerOptions.RemoveTitleBar);
        }

        public TagSelectControl TagSelectControl => tagSelectControl;

        protected override void OnShown(EventArgs e)
        {
            // DataGridViews make no sense sometimes, the checks are being cleared between setting/show so reset the DataSource here
            base.OnShown(e);
            TagSelectControl.SetCheckedTags();
        }
    }
}
