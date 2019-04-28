using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class FileDetailsEditForm : Form
    {
        public FileDetailsEditForm()
        {
            InitializeComponent();
        }

        public void Initialize(IDataSourceAdapter adapter)
        {
            Initialize(adapter, null);
        }

        public void Initialize(IDataSourceAdapter adapter, IFileData file)
        {
            DataSourceAdapter = adapter;

            cmbSourcePort.DisplayMember = "Name";
            cmbSourcePort.ValueMember = "SourcePortID";
            cmbSourcePort.DataSource = adapter.GetSourcePorts();

            if (file != null)
            {
                cmbSourcePort.SelectedValue = file.SourcePortID;
                txtDescription.Text = file.Description;
            }
        }

        public ISourcePortData SourcePort
        {
            get { return cmbSourcePort.SelectedItem as ISourcePortData; }
            set { cmbSourcePort.SelectedValue = value.SourcePortID; }
        }

        public string Description
        {
            get { return txtDescription.Text; }
            set { txtDescription.Text = value; }
        }

        public IDataSourceAdapter DataSourceAdapter { get; set; }
    }
}
