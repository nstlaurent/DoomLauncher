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
    public partial class TagEditForm : Form
    {
        private TagEdit m_tagEdit;

        public TagEditForm()
        {
            InitializeComponent();

            m_tagEdit = new TagEdit();
            m_tagEdit.Dock = DockStyle.Fill;
            tblMain.Controls.Add(m_tagEdit, 0, 0);
        }

        public TagEdit TagEditControl
        {
            get { return m_tagEdit; }
        }
    }
}
