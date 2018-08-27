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
    public partial class TagForm : Form
    {
        private TagControl m_tabCtrl;

        public TagForm()
        {
            InitializeComponent();

            m_tabCtrl = new TagControl();
            m_tabCtrl.Dock = DockStyle.Fill;
            Controls.Add(m_tabCtrl);
        }

        public void Init(IDataSourceAdapter adapter)
        {
            m_tabCtrl.Init(adapter);
        }

        public TagControl TagControl
        {
            get { return m_tabCtrl; }
        }
    }
}
