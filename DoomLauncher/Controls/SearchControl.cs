using PresentationControls;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class SearchControl : UserControl
    {
        public event EventHandler SearchTextChanged;
        public event PreviewKeyDownEventHandler SearchTextKeyPreviewDown;

        private Controls.CheckBoxList m_checkBoxList = new Controls.CheckBoxList();

        public SearchControl()
        {
            InitializeComponent();
            Stylizer.StylizeControl(m_checkBoxList, DesignMode);
            txtSearch.TextChanged += txtSearch_TextChanged;
            txtSearch.PreviewKeyDown += TxtSearch_PreviewKeyDown;
        }

        private void TxtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            SearchTextKeyPreviewDown?.Invoke(this, e);
        }

        void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchTextChanged?.Invoke(this, e);
        }

        public void SetSearchFilter(string item, bool check)
        {
            m_checkBoxList.SetChecked(item, check);
        }

        public bool GetSearchFilter(string item)
        {
            return m_checkBoxList.IsChecked(item);
        }

        public string[] GetSearchFilters()
        {
            return m_checkBoxList.Items.ToArray();
        }

        public string[] GetSelectedSearchFilters()
        {
            return m_checkBoxList.GetCheckedItems();
        }

        public void SetSearchFilters(IEnumerable<string> items)
        {
            m_checkBoxList.SetItems(items);
        }

        public string SearchText
        {
            get => txtSearch.Text;
            set => txtSearch.Text = value;
        }

        private void btnFilters_Click(object sender, EventArgs e)
        {
            
            Popup popup = new Popup(m_checkBoxList)
            {
                Width = btnFilters.Location.X + btnFilters.Width,
                Height = m_checkBoxList.Height
            };
            popup.Show(txtSearch);
        }
    }
}
