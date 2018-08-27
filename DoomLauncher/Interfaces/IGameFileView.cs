using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public interface IGameFileView
    {
        event AddingNewEventHandler ToolTipTextNeeded;
        event EventHandler ContentDoubleClicked;
        event EventHandler SelectionChange;

        void SetDisplayText(string text);
        void SetContextMenuStrip(ContextMenuStrip menu);
        void SetColumnFields(IEnumerable<Tuple<string, string>> columnFields);
        void SetColumnFormat(string colName, string format);
        object DataSource { get; set; }
        object SelectedItem { get; set; }
        object[] SelectedItems { get; }
        object ItemForRow(int rowIndex);
    }
}
