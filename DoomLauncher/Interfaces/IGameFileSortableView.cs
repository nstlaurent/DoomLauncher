using System.Windows.Forms;

namespace DoomLauncher
{
    public interface IGameFileSortableView
    {
        string GetSortedColumnKey();
        SortOrder GetColumnSort(string key);

        void SetSortedColumn(string key, SortOrder sort);
    }
}
