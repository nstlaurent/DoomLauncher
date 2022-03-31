using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace DoomLauncher
{
    public partial class CComboBox : UserControl
    {
        private readonly ElementHost m_ctrlHost;
        private readonly System.Windows.Controls.ComboBox m_combo;
        private readonly System.Windows.Forms.ComboBox m_formCombo;
        private IEnumerable<object> m_datasource = Array.Empty<object>();

        public System.Windows.Forms.ComboBox Combo => m_formCombo;

        public IEnumerable<object> DataSource { get => m_datasource; set => SetDataSource(value); }

        public CComboBox()
        {
            InitializeComponent();
            m_ctrlHost = new ElementHost();
            m_ctrlHost.Dock = DockStyle.Fill;

            m_formCombo = new ComboBox();
            m_formCombo.Visible = false;
            m_formCombo.DataSourceChanged += formCombo_DataSourceChanged;

            m_combo = new System.Windows.Controls.ComboBox();
            m_combo.IsEditable = true;
            m_combo.TextInput += combo_TextInput;
            m_combo.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                               new System.Windows.Controls.TextChangedEventHandler(ComboBox_TextChanged));
            //m_combo.ItemContainerStyle = new System.Windows.Style(typeof(System.Windows.Controls.ComboBox));
            ////m_combo.ItemContainerStyle = new System.Windows.Controls.ItemContainer
            //m_combo.ItemContainerStyle.Setters.Add(new System.Windows.Setter()
            //{
            //    Property = System.Windows.Controls.ItemsControl.BackgroundProperty,
            //    Value = GetBrush(ColorTheme.Current.Window)
            //});


            //= GetBrush(ColorTheme.Current.Window);
            //m_combo.Foreground = GetBrush(ColorTheme.Current.Text);
            m_ctrlHost.Child = m_combo;

            this.Controls.Add(m_ctrlHost);
        }

        private void SetDataSource(IEnumerable<object> data)
        {
            m_datasource = data;
            SetComboItems(data);
        }

        private void SetComboItems(IEnumerable<object> data)
        {
            m_combo.Items.Clear();
            foreach (object item in data)
                m_combo.Items.Add(item);

            if (data.Any())
                m_combo.SelectedIndex = 0;
        }

        private void ComboBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

            //m_combo.Items.Clear();
            //if (m_datasource is IEnumerable<string> strings)
            //{
            //    foreach (string str in strings)
            //        if (str.StartsWith(, StringComparison.OrdinalIgnoreCase))
            //            m_combo.Items.Add(str);
            //}
        }

        private void combo_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            
        }

        private static System.Windows.Media.SolidColorBrush GetBrush(System.Drawing.Color color)
        {
            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        public void DataSourceChanged()
        {
            m_combo.Items.Clear();
            IEnumerable<object> data = m_formCombo.DataSource as IEnumerable<object>;
            foreach (object item in data)
                m_combo.Items.Add(item);

            if (data.Any())
                m_combo.SelectedIndex = 0;
        }

        private void formCombo_DataSourceChanged(object sender, EventArgs e)
        {
            
        }
    }
}
