using DoomLauncher.Interfaces;
using DoomLauncher.Stylize;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class SourcePortEdit : UserControl
    {
        private string m_directory, m_exec;

        public SourcePortEdit()
        {
            InitializeComponent();

            txtFileOption.Text = "-file"; //default for source port, currently user will not see this but it will be populated in the database

            Stylizer.StylizeControl(this, DesignMode);
        }

        public void SetSupportedExtensions(string text)
        {
            txtExtensions.Text = text;
        }

        public void ShowOptions(bool set)
        {
            var controls = new Control[] { lblFileOption, txtFileOption };
            foreach (var ctrl in controls)
                ctrl.Visible = set;

            if (set)
                txtFileOption.Text = string.Empty;
        }

        public void SetDataSource(ISourcePortData sourcePort)
        {
            m_directory = sourcePort.Directory.GetPossiblyRelativePath();
            m_exec = sourcePort.Executable;

            txtName.Text = txtExec.Text = txtExtensions.Text = txtFileOption.Text
                = txtParameters.Text = txtAltSave.Text = string.Empty;

            if (!string.IsNullOrEmpty(sourcePort.Name)) 
                txtName.Text = sourcePort.Name;
            if (sourcePort.Directory != null && sourcePort.Executable != null) 
                txtExec.Text = sourcePort.Executable;
            if (!string.IsNullOrEmpty(sourcePort.SupportedExtensions)) 
                txtExtensions.Text = sourcePort.SupportedExtensions;
            if (!string.IsNullOrEmpty(sourcePort.FileOption))
                txtFileOption.Text = sourcePort.FileOption;
            if (!string.IsNullOrEmpty(sourcePort.ExtraParameters))
                txtParameters.Text = sourcePort.ExtraParameters;
            if (sourcePort.AltSaveDirectory != null)
                txtAltSave.Text = sourcePort.AltSaveDirectory.GetPossiblyRelativePath();
            chkArchive.Checked = sourcePort.Archived;
        }

        public void UpdateDataSource(ISourcePortData sourcePort)
        {
            sourcePort.Name = txtName.Text;
            sourcePort.Directory = new LauncherPath(m_directory);
            sourcePort.Executable = m_exec;
            sourcePort.SupportedExtensions = txtExtensions.Text;
            sourcePort.FileOption = txtFileOption.Text;
            sourcePort.ExtraParameters = txtParameters.Text;
            sourcePort.AltSaveDirectory = new LauncherPath(txtAltSave.Text);
            sourcePort.Archived = chkArchive.Checked;
        }

        public string SourcePortName { get { return txtName.Text;  } }
        public string SourcePortExec { get { return txtExec.Text; } }
        public LauncherPath GetSourcePortDirectory() => new LauncherPath(m_directory);

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Executable (*.exe)|*.exe|All Files (*.*)|*.*";

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string file = LauncherPath.GetRelativePath(dialog.FileName);
                
                string exec = Path.GetFileName(file);
                string directory = file.Replace(exec, string.Empty);

                if (string.IsNullOrEmpty(directory))
                {
                    StyledMessageBox.Show(this, "The source port can't be in the same directory as Doom Launcher.", "Invalid Path", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                m_exec = exec;
                m_directory = directory;

                txtExec.Text = m_exec;
                if (string.IsNullOrEmpty(txtName.Text))
                    txtName.Text = Path.GetFileNameWithoutExtension(file);
            }
        }

        private void btnAltSaveBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog(this) == DialogResult.OK)
                txtAltSave.Text = LauncherPath.GetRelativePath(dialog.SelectedPath);
        }
    }
}
