﻿using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
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
                files = new List<SpecificFilePath>();
                files.Add(new SpecificFilePath() { ExtractedFile = gameFile.FileName, InternalFilePath = gameFile.FileName });
            }
            else if (!GetUserSelectedFiles(gameFile, out files))
            {
                return true;
            }

            StringBuilder sb = new StringBuilder();
            GameFilePlayAdapter adapter = new GameFilePlayAdapter();
            adapter.HandleGameFile(gameFile, sb, m_config.TempDirectory, 
                new GenericSourcePort(m_utility), files); //this checks File.Exists and might not be same file

            try
            {
                if (!string.IsNullOrEmpty(m_utility.ExtraParameters))
                    sb.Append(" " + m_utility.ExtraParameters);

                Process.Start(m_utility.GetFullExecutablePath(), sb.ToString().Trim());
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
