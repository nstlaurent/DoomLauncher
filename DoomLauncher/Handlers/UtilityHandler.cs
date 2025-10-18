using DoomLauncher.Adapters.Launch;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using static DoomLauncher.SpecificFilesForm;

namespace DoomLauncher
{
    class UtilityHandler
    {
        private readonly IWin32Window m_parent;
        private readonly ISourcePortData m_utility;
        private readonly AppConfiguration m_config;

        public UtilityHandler(IWin32Window parent, AppConfiguration config, ISourcePortData utility)
        {
            m_parent = parent;
            m_config = config;
            m_utility = utility;
        }

        public bool RunUtility(IGameFile gameFile)
        {
            List<SpecificFilePath> files;
            if (gameFile.IsUnmanaged())
            {
                files = new List<SpecificFilePath>
                {
                    new SpecificFilePath() { ExtractedFile = gameFile.FileName, InternalFilePath = gameFile.FileName }
                };
            }
            else if (!GetUserSelectedFiles(gameFile, out files))
            {
                return true;
            }

            var features = new List<ILaunchFeature> 
            { 
                new UtilityFilesLaunchFeature(files),
                new SourcePortExtraParametersLaunchFeature()
            };

            GameLauncher launcher = new GameLauncher(m_config, features);
            var launchParameters = launcher.GetLaunchParameters(gameFile, Array.Empty<IGameFile>(), m_utility, false);

            if (launchParameters.Failed)
                return false;

            try
            {
                Process.Start(m_utility.GetFullExecutablePath(), launchParameters.LaunchString);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool GetUserSelectedFiles(IGameFile gameFile, out List<SpecificFilePath> files)
        {
            SpecificFilesForm form = new SpecificFilesForm();
            form.AutoCheckSupportedExtensions(false);
            form.ShowPkContentsCheckBox(true);
            form.Initialize(m_config.GameFileDirectory, new IGameFile[] { gameFile }, SourcePortData.GetSupportedExtensions(m_utility),
                new string[] { }, m_config.TempDirectory);
            form.StartPosition = FormStartPosition.CenterParent;

            if (form.ShowDialog(m_parent) != DialogResult.OK)
            {
                files = new List<SpecificFilePath>();
                return false;
            }

            files = form.GetPathedSpecificFiles();
            return true;
        }
    }
}
