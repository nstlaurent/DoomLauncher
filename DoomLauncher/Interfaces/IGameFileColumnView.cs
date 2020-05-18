using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    public interface IGameFileColumnView : IGameFileView
    {
        event CancelEventHandler CustomRowPaint;

        bool CustomRowColorPaint { get; set; }
        IGameFile CustomRowPaintDataBoundItem { get; }
        Color CustomRowPaintForeColor { get; set; }

        ColumnField[] ColumnFields { get; }
        void SetColumnFields(IEnumerable<ColumnField> columnFields);
        void SetColumnFormat(string colName, string format);
        void SetColumnWidth(string key, int width);
        void SetSortedColumn(string column, SortOrder sort);
        int GetColumnWidth(string key);
        SortOrder GetColumnSort(string key);

        string[] GetColumnKeyOrder();
    }
}
