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
    public partial class MetaDataForm : Form
    {
        public MetaDataForm()
        {
            InitializeComponent();
            gameFileEdit1.SetShowCheckBoxes(true);
        }

        public GameFileEdit GameFileEdit
        {
            get { return gameFileEdit1; }
        }
    }
}
