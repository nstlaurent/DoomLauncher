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
        }

        public void Init(int records, int recordsPerPage)
        {
            Pages = records / recordsPerPage;
            if (records % recordsPerPage != 0)
                Pages++;

            lblPageTotal.Text = Pages.ToString();
            var size = TextRenderer.MeasureText(lblPageTotal.Text, lblPageTotal.Font);

            tblMain.ColumnStyles[3].Width = size.Width;
            tblMain.ColumnStyles[5].Width = size.Width;

            PageIndex = 0;
            if (Pages > 0)
                SetPageIndex(0);
        }

        public bool SetPageIndex(int index)
        {
            if (index < 0)
                index = 0;
            if (index >= Pages)
                index = Pages - 1;

            lblPage.Text = (index + 1).ToString();

            if (PageIndex == index)
                return false;

            PageIndex = index;

            PageIndexChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            SetPageIndex(PageIndex + 1);
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            SetPageIndex(PageIndex - 1);
        }

        private void BtnFirst_Click(object sender, EventArgs e)
        {
            SetPageIndex(0);
        }

        private void BtnLast_Click(object sender, EventArgs e)
        {
            SetPageIndex(Pages - 1);
        }
    }
}
