using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DoomLauncher.Handlers
{
    public static class AutoCompleteCombo
    {
        public static void SetAutoCompleteCustomSource(ComboBox cmb, IEnumerable<object> datasource, Type dataType, string property)
        {
            AutoCompleteStringCollection collection = new AutoCompleteStringCollection();

            if (dataType != null)
            {
                PropertyInfo pi = dataType.GetProperty(property);
                collection.AddRange(datasource.Select(x => (string)pi.GetValue(x)).ToArray());
            }
            else
            {
                collection.AddRange(datasource.Cast<string>().ToArray());
            }

            cmb.DataSource = datasource;
            cmb.AutoCompleteSource = AutoCompleteSource.CustomSource;
            cmb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmb.AutoCompleteCustomSource = collection;
            cmb.DropDown += Cmb_DropDown;

            if (!datasource.Any())
                cmb.Text = string.Empty;
        }

        private static void Cmb_DropDown(object sender, EventArgs e)
        {
            ((ComboBox)sender).PreviewKeyDown += Cmb_PreviewKeyDown;
        }

        private static void Cmb_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            cmb.PreviewKeyDown -= Cmb_PreviewKeyDown;
            if (cmb.DroppedDown)
                cmb.Focus();
        }
    }
}
