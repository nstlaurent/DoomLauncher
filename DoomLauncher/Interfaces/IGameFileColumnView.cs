using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace DoomLauncher
{
    public interface IGameFileColumnView : IGameFileView, IGameFileSortableView
    {
        event CancelEventHandler CustomRowPaint;

        bool CustomRowColorPaint { get; set; }
        IGameFile CustomRowPaintDataBoundItem { get; }
        Color CustomRowPaintForeColor { get; set; }

        ColumnField[] ColumnFields { get; }
        void SetColumnFields(IEnumerable<ColumnField> columnFields);
        void SetColumnFormat(string colName, string format);
        void SetColumnWidth(string key, int width);
        int GetColumnWidth(string key);

        string[] GetColumnKeyOrder();
    }
}
