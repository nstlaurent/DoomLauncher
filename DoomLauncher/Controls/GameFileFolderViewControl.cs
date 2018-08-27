using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;

namespace DoomLauncher
{
    public partial class GameFileFolderViewControl : GameFileViewControl
    {
        public GameFileFolderViewControl() : base()
        {
            base.DataChanged += GameFileFolderViewControl_DataChanged;
            base.ColumnsChanged += GameFileFolderViewControl_ColumnsChanged;
        }

        void GameFileFolderViewControl_ColumnsChanged(object sender, EventArgs e)
        {
            DataGridViewImageColumn col = new DataGridViewImageColumn();
            Image image = DoomLauncher.Properties.Resources.bon2a;
            col.Image = image;
            col.HeaderText = "";
            col.Name = "img";
            col.Width = 24;

            base.GridView.Columns.Insert(0, col);
        }

        void GameFileFolderViewControl_DataChanged(object sender, EventArgs e)
        {
            
        }
        //public event AddingNewEventHandler ToolTipTextNeeded;
        //public event EventHandler ContentDoubleClicked;
        //public event EventHandler SelectionChange;

        //public GameFileFolderViewControl()
        //{
        //    InitializeComponent();

        //    ctrlView.DataChanged += GameFileFolderViewControl_DataChanged;
        //    ctrlView.ToolTipTextNeeded += ctrlView_ToolTipTextNeeded;
        //    ctrlView.ContentDoubleClicked += ctrlView_ContentDoubleClicked;
        //    ctrlView.SelectionChange += ctrlView_SelectionChange;
        //}

        //void ctrlView_SelectionChange(object sender, EventArgs e)
        //{
        //    if (SelectionChange != null)
        //    {
        //        SelectionChange(this, e);
        //    }
        //}

        //void ctrlView_ContentDoubleClicked(object sender, EventArgs e)
        //{
        //    if (ContentDoubleClicked != null)
        //    {
        //        ContentDoubleClicked(this, e);
        //    }
        //}

        //void ctrlView_ToolTipTextNeeded(object sender, AddingNewEventArgs e)
        //{
        //    if (ToolTipTextNeeded != null)
        //    {
        //        ToolTipTextNeeded(this, e);
        //    }
        //}

        //void GameFileFolderViewControl_DataChanged(object sender, EventArgs e)
        //{
        
        //}

        //public void SetDisplayText(string text) { ctrlView.SetDisplayText(text); }
        //public void SetContextMenuStrip(ContextMenuStrip menu) { ctrlView.SetContextMenuStrip(menu); }
        //public void SetColumnFields(IEnumerable<Tuple<string, string>> columnFields)
        //{
        //    //Tuple<string,string> item = new Tuple<string,string>("FileName", "Name");
        //    //List<Tuple<string, string>> fields = columnFields.ToList();
        //    //fields.Insert(0, item);
        //    ctrlView.SetColumnFields(columnFields);
        //}
        //public void SetColumnFormat(string colName, string format) { ctrlView.SetColumnFormat(colName, format); }
        //public object DataSource
        //{
        //    get { return ctrlView.DataSource; }
        //    set { ctrlView.DataSource = value; }
        //}
        //public object SelectedItem
        //{
        //    get { return ctrlView.SelectedItem; }
        //    set { ctrlView.SelectedItem = value; }
        //}
        //public object[] SelectedItems
        //{
        //    get { return ctrlView.SelectedItems; }
        //}
        //public object ItemForRow(int rowIndex) { return ctrlView.ItemForRow(rowIndex); }
    }
}
