using DoomLauncher.Adapters.Launch;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DoomLauncher
{
    [Flags]
    public enum GameFilePlayAdapterOptions
    {
        None = 0,
        ExtraParamsOnly = 1
    }

    public class GameFilePlayAdapter
    {
        public event EventHandler ProcessExited;

        private readonly GameFilePlayAdapterOptions m_options;

        public GameFilePlayAdapter(GameFilePlayAdapterOptions options = GameFilePlayAdapterOptions.None)
        {
            m_options = options;
            AdditionalFiles = Array.Empty<IGameFile>();
            ExtractFiles = true;
        }

        public bool Launch(LauncherPath gameFileDirectory, LauncherPath tempDirectory,
            IGameFile gameFile, ISourcePortData sourcePort, bool isGameFileIwad)
        {
            LastError = string.Empty;
            if (!Directory.Exists(sourcePort.Directory.GetFullPath()))
            {
                LastError = string.Concat("The source port directory does not exist:", Environment.NewLine, Environment.NewLine, 
                    sourcePort.Directory.GetPossiblyRelativePath());
                return false;
            }

            GameFile = gameFile;
            SourcePort = sourcePort;

            string launchParameters = GetLaunchParameters(gameFileDirectory, tempDirectory, gameFile, sourcePort, isGameFileIwad, out var error);
            if (launchParameters == null)
            {
                if (string.IsNullOrEmpty(LastError))
                    LastError = $"Failed to create launch parameters: {error}";
                return false;
            }
       
            Directory.SetCurrentDirectory(sourcePort.Directory.GetFullPath());

            try
            {
                Process proc = Process.Start(sourcePort.GetFullExecutablePath(), launchParameters);
                proc.EnableRaisingEvents = true;
                proc.Exited += proc_Exited;
            }
            catch
            {
                LastError = "Failed to execute the source port process.";
                return false;
            }

            return true;            
        }

        public string GetLaunchParameters(LauncherPath gameFileDirectory, LauncherPath tempDirectory,
            IGameFile gameFile, ISourcePortData sourcePortData, bool isGameFileIwad, out string error)
        {
            error = string.Empty;

            ISourcePortFlavor sourcePortFlavor = sourcePortData.GetFlavor();
            StringBuilder sb = new StringBuilder();


            var launchParameters = new List<LaunchFeature>();
            
            if (IWad != null)
                launchParameters.Add(new IWadLaunchFeature(IWad, gameFileDirectory, tempDirectory));

            var additionalFiles = AdditionalFiles != null ? new List<IGameFile>(AdditionalFiles) : new List<IGameFile>();
            var specificFiles = SpecificFiles != null ? new List<string>(SpecificFiles) : new List<string>();
            launchParameters.Add(new AdditionalFilesLaunchFeature(
                additionalFiles,
                specificFiles, 
                gameFileDirectory, tempDirectory, isGameFileIwad));

            launchParameters.Add(new MapSkillLaunchFeature(Map, Skill));

            if (Record)
                launchParameters.Add(new RecordLaunchFeature(tempDirectory));

            if (PlayDemo)
                launchParameters.Add(new PlayDemoLaunchFeature(PlayDemoFile));

            launchParameters.Add(new ExtraParametersLaunchFeature(ExtraParameters, m_options.HasFlag(GameFilePlayAdapterOptions.ExtraParamsOnly)));

            launchParameters.Add(new SourcePortExtraParametersLaunchFeature());

            if (SaveStatistics)
                launchParameters.Add(new StatisticsReaderLaunchFeature());

            if (!string.IsNullOrEmpty(LoadSaveFile))
                launchParameters.Add(new LoadSaveLaunchFeature(LoadSaveFile));

            var replaceFilenameVariable = LaunchParameters.WithVariableReplacement("filename", gameFile.FileNameNoPath);

            var paramResult = launchParameters.Aggregate(replaceFilenameVariable, 
                (result, param) => result.Combine(param.CreateParam(sourcePortData, gameFile)));

            RecordedFileName = paramResult.RecordedFileName;
            LastError = paramResult.ErrorMessage;
            if (!string.IsNullOrEmpty(LastError))
                return null;

            sb.Append(paramResult.ParamString);

            return sb.ToString();
        }

        //This function is currently only used for loading files by utility (which also uses ISourcePort).
        //This uses Util.ExtractTempFile to avoid extracting files with the same name where the user can have the previous file locked.
        //E.g. opening MAP01 from a pk3, and then opening another MAP01 from a different pk3
        public bool HandleGameFile(
            IGameFile gameFile, 
            StringBuilder sb, // Output
            LauncherPath tempDirectory,
            ISourcePortFlavor sourcePortFlavor, 
            List<SpecificFilesForm.SpecificFilePath> pathFiles)
        {

            var utilityLaunchFeature = new UtilityFilesLaunchFeature(pathFiles, tempDirectory);
            var result = utilityLaunchFeature.CreateParam(sourcePortFlavor, gameFile);

            if (result.Failed)
            {
                LastError = result.ErrorMessage;
                return false;
            }
            else
            {
                sb.Append(result.ParamString);
                return true;
            }
        }

        public string LastError { get; private set; } // Output

        public string RecordedFileName { get; private set; } // Output

        public IGameFile IWad { get; set; } // Input
        public string Map { get; set; } // Input
        public string Skill { get; set; } // Input
        public bool Record { get; set; } // Input
        public bool PlayDemo { get; set; } // Input
        public IGameFile[] AdditionalFiles { get; set; } // Input
        public string ExtraParameters { get; set; } // Input
        public string[] SpecificFiles { get; set; } // Input
        public bool SaveStatistics { get; set; } // Input
        public string LoadSaveFile { get; set; } // Input

        public ISourcePortData SourcePort { get; private set; } // Input/Output (but should only be input)
        public IGameFile GameFile { get; private set; } // Input/Output (but should only be input)
        
        public string PlayDemoFile { get; set; } // Input

        public bool ExtractFiles { get; set; } // Input
        public bool IgnoreExtractError { get; set; } // Input

        void proc_Exited(object sender, EventArgs e)
        {
            ProcessExited?.Invoke(this, new EventArgs());
        }
    }
}
