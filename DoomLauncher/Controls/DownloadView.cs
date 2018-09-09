using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class DownloadView : UserControl
    {
        public event EventHandler DownloadCancelled;
        public event EventHandler UserPlay;

        private Dictionary<object, DownloadViewItem> m_downloadLookup = new Dictionary<object, DownloadViewItem>();
        private HashSet<object> m_cancelledDownloads = new HashSet<object>();

        static readonly int s_rowHeight = 64;

        public DownloadView()
        {
            InitializeComponent();

            tblMain.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tblMain.RowStyles.Clear();
            tblMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            UpdateSize();
        }

        public new int Height { get; set; }

        public void AddDownload(object key, string text)
        {
            if (m_cancelledDownloads.Contains(key))
                m_cancelledDownloads.Remove(key);

            if (!m_downloadLookup.ContainsKey(key))
            {
                DownloadViewItem item = CreateDownloadViewItem(text);
                m_downloadLookup.Add(key, item);

                tblMain.RowCount++;
                tblMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                tblMain.RowStyles[tblMain.RowStyles.Count - 2].SizeType = SizeType.Absolute;
                tblMain.RowStyles[tblMain.RowStyles.Count - 2].Height = s_rowHeight;
                tblMain.Controls.Add(item, 0, tblMain.RowStyles.Count - 2);

                UpdateSize();
            }
            else
            {
                DownloadViewItem itemOld = m_downloadLookup[key];
                DownloadViewItem item = CreateDownloadViewItem(text);

                TableLayoutPanelCellPosition pos = tblMain.GetPositionFromControl(itemOld);
                tblMain.Controls.Remove(itemOld);
                tblMain.Controls.Add(item, pos.Column, pos.Row);

                m_downloadLookup.Remove(key);
                m_downloadLookup.Add(key, item);
            }
        }

        private DownloadViewItem CreateDownloadViewItem(string text)
        {
            DownloadViewItem item = new DownloadViewItem
            {
                Dock = DockStyle.Fill,
                Key = text,
                DisplayText = text
            };
            item.Cancelled += item_Cancelled;

            item.ContextMenuStrip = menuOptions;

            return item;
        }

        private void UpdateSize()
        {
            Height = tblMain.RowStyles.Count * s_rowHeight;
        }

        void item_Cancelled(object sender, EventArgs e)
        {
            if (sender is DownloadViewItem item)
            {
                HandleItemCancel(item);
            }
        }

        private void HandleItemCancel(DownloadViewItem item)
        {
            if (item.ProgressValue != 100)
                item.ProgressValue = 0;

            if (DownloadCancelled != null)
            {
                object key = GetKey(item);

                if (key != null)
                {
                    m_cancelledDownloads.Add(key);
                    DownloadCancelled(this, new EventArgs());
                }
            }
        }

        private object GetKey(DownloadViewItem item)
        {
            foreach(KeyValuePair<object, DownloadViewItem> kvp in m_downloadLookup)
            {
                if (kvp.Value == item)
                    return kvp.Key;
            }

            return null;
        }

        private void RemoveDownloadRow(DownloadViewItem item)
        {
            int row = tblMain.GetRow(item);
            if (row > -1)
            {
                tblMain.Controls.Remove(item);
                tblMain.RowStyles.RemoveAt(row);

                for(int i = row + 1; i < tblMain.RowCount; i++) //have to shift the controls up (TableLayoutPanel is dumb)
                {
                    DownloadViewItem itemMove = GetItem(i);

                    if (itemMove != null)
                    {
                        tblMain.Controls.Remove(itemMove);
                        tblMain.Controls.Add(itemMove, 0, i - 1);
                    }
                }
            }
        }

        public void RemoveDownload(object key)
        {
            if (m_downloadLookup.ContainsKey(key))
            {
                DownloadViewItem item = m_downloadLookup[key];
                m_downloadLookup.Remove(key);

                RemoveDownloadRow(item);
                UpdateSize();
            }
        }

        public void UpdateDownload(object key, int percent)
        {
            if (m_downloadLookup.ContainsKey(key))
            {
                DownloadViewItem item = m_downloadLookup[key];
                item.ProgressValue = percent;

                if (percent == 100)
                    item.CancelVisible = false;
            }
        }

        public void UpdateDownload(object key, string text)
        {
            if (m_downloadLookup.ContainsKey(key))
            {
                DownloadViewItem item = m_downloadLookup[key];
                item.DisplayText = text;
            }
        }

        public object[] GetCancelledDownloads()
        {
            return m_cancelledDownloads.ToArray();
        }

        public DownloadViewItem SelectedItem
        {
            get;
            private set;
        }

        private DownloadViewItem GetItem(int row)
        {
            return tblMain.GetControlFromPosition(0, row) as DownloadViewItem;
        }

        private void removeFromHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menuOptions.SourceControl is DownloadViewItem item)
            {
                RemoveDownloadRow(item);
                object key = GetKey(item);

                if (key != null)
                    m_downloadLookup.Remove(key);
            }
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadViewItem item = menuOptions.SourceControl as DownloadViewItem;
            SelectedItem = item;

            if (item != null)
                HandleItemCancel(item);
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadViewItem item = menuOptions.SourceControl as DownloadViewItem;
            SelectedItem = item;

            if (item != null && UserPlay != null && item.ProgressValue == 100)
                UserPlay(this, new EventArgs());
        }
    }
}
