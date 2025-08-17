using System;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class PagingControl : UserControl
    {
        public event EventHandler PageIndexChanged;
        public int PageIndex { get; private set; }
        public int Pages { get; private set; }

        public PagingControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Stylizer.StylizeControl(this, DesignMode);
        }

        public void Init(int records, int recordsPerPage, int initialPage)
        {
            Pages = records / recordsPerPage;
            if (records % recordsPerPage != 0)
                Pages++;

            lblPageTotal.Text = Pages.ToString();
            var size = TextRenderer.MeasureText(lblPageTotal.Text, lblPageTotal.Font);

            tblMain.ColumnStyles[3].Width = size.Width;
            tblMain.ColumnStyles[5].Width = size.Width;

            if (Pages > 0)
                SetPageIndex(initialPage, false);
            else
                PageIndex = 0;
        }

        private void SetPageIndex(int index, bool publishEvent)
        {
            if (index < 0)
                index = 0;
            if (index >= Pages)
                index = Pages - 1;

            lblPage.Text = (index + 1).ToString();

            if (PageIndex == index)
                return;

            PageIndex = index;

            if (publishEvent)
                PageIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            SetPageIndex(PageIndex + 1, true);
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            SetPageIndex(PageIndex - 1, true);
        }

        private void BtnFirst_Click(object sender, EventArgs e)
        {
            SetPageIndex(0, true);
        }

        private void BtnLast_Click(object sender, EventArgs e)
        {
            SetPageIndex(Pages - 1, true);
        }
    }
}
