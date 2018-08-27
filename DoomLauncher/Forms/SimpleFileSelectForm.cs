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
    public partial class SimpleFileSelectForm : Form
    {
        public SimpleFileSelectForm()
        {
            InitializeComponent();
        }

        public void Initialize(IEnumerable<string> files)
        {
            foreach (string file in files)
                lstFiles.Items.Add(file);

            if (files.Count() > 0)
                lstFiles.SelectedItem = files.First();
        }

        public string SelectedFile
        {
            get
            {
                if (lstFiles.SelectedItems.Count > 0)
                    return (string)lstFiles.SelectedItems[0];

                return null;
            }
        }
    }
}
