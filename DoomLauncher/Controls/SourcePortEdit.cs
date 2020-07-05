using DoomLauncher.Interfaces;
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
                = txtParameters.Text = string.Empty;

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
        }

        public void UpdateDataSource(ISourcePortData sourcePort)
        {
            sourcePort.Name = txtName.Text;
            sourcePort.Directory = new LauncherPath(m_directory);
            sourcePort.Executable = m_exec;
            sourcePort.SupportedExtensions = txtExtensions.Text;
            sourcePort.FileOption = txtFileOption.Text;
            sourcePort.ExtraParameters = txtParameters.Text;
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
                string file = GetRelativeDirectory(dialog.FileName);
                m_exec = m_directory = string.Empty;
                m_exec = Path.GetFileName(file);
                m_directory = file.Replace(m_exec, string.Empty);

                txtExec.Text = m_exec;
                txtName.Text = Path.GetFileNameWithoutExtension(file);
            }
        }

        private string GetRelativeDirectory(string file)
        {
            string current = LauncherPath.GetDataDirectory();

            if (file.Contains(current))
            {
                string[] filePath = file.Split(Path.DirectorySeparatorChar);
                string[] currentPath = current.Split(Path.DirectorySeparatorChar);

                string[] relativePath = filePath.Except(currentPath).ToArray();

                StringBuilder sb = new StringBuilder();

                foreach(string str in relativePath)
                {
                    sb.Append(str);
                    sb.Append(Path.DirectorySeparatorChar);
                }

                if (sb.Length > 1)
                    sb.Remove(sb.Length - 1, 1);

                return sb.ToString();
            }

            return file;
        }
    }
}
