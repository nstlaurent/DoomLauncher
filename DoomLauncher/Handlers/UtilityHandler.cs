using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.IO.Compression;

namespace DoomLauncher
{
    class UtilityHandler
    {
        private IWin32Window m_parent;
        private ISourcePort m_utility;
        private AppConfiguration m_config;

        public UtilityHandler(IWin32Window parent, AppConfiguration config, ISourcePort utility)
        {
            m_parent = parent;
            m_config = config;
            m_utility = utility;
        }

        public bool RuntUtility(IGameFile gameFile)
        {
            SpecificFilesForm form = new SpecificFilesForm();
            form.AutoCheckSupportedExtensions(false);
            form.ShowPkContentsCheckBox(true);
            form.Initialize(m_config.GameFileDirectory, new IGameFile[] { gameFile }, SourcePort.GetSupportedExtensions(m_utility), 
                new string[] { }, m_config.TempDirectory);
            form.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            if (form.ShowDialog(m_parent) == DialogResult.OK)
            {
                var files = form.GetPathedSpecificFiles();

                GameFilePlayAdapter adapter = new GameFilePlayAdapter();
                StringBuilder sb = new StringBuilder();
                adapter.HandleGameFile(gameFile, sb, m_config.GameFileDirectory, m_config.TempDirectory, m_utility, files); //this checks File.Exists and might not be same file

                try
                {
                    //test for single file generic open
                    //var pathFile = files.First();
                    //using (ZipArchive za = ZipFile.OpenRead(pathFile.ExtractedFile))
                    //{
                    //    var entry = za.Entries.Where(x => x.FullName == pathFile.InternalFilePath).FirstOrDefault();
                    //    if (entry != null)
                    //    {
                    //        Process.Start(Util.ExtractTempFile(m_config.TempDirectory.GetFullPath(), entry));
                    //    }
                    //}

                    Process.Start(m_utility.GetFullExecutablePath(), sb.ToString().Trim());
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
    }
}
