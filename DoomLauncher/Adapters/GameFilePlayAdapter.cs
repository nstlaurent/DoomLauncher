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
    public class GameFilePlayAdapter
    {
        public event EventHandler ProcessExited;

        private readonly List<ILaunchFeature> _features;

        public GameFilePlayAdapter(List<ILaunchFeature> features)
        {
            var ourFeatures = new List<ILaunchFeature>(features);

            if (!ourFeatures.Exists(f => f is AdditionalFilesLaunchFeature))
            {
                var iWadIndex = ourFeatures.FindIndex(f => f is IWadLaunchFeature);
                var insertIndex = (iWadIndex == -1) ? 0 : iWadIndex + 1;
                ourFeatures.Insert(insertIndex, new AdditionalFilesLaunchFeature(null, null));
            }

            _features = ourFeatures;
        }

        public GameFilePlayAdapter() : this(new List<ILaunchFeature>())
        {
            
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

        public string GetLaunchParameters(LauncherPath gameFileDirectory, LauncherPath tempDirectory, IGameFile gameFile, ISourcePortData sourcePortData, bool isGameFileIwad, out string error)
        {
            LaunchParameters parametersResult = LaunchParameters.EMPTY;
            foreach (var feature in _features)
            {
                var newParameters = feature.CreateParam(sourcePortData, gameFile, isGameFileIwad, gameFileDirectory, tempDirectory);
                parametersResult = parametersResult.Combine(newParameters);
            }

            parametersResult = parametersResult.WithVariableReplacement("filename", gameFile.FileNameNoPath);

            RecordedFileName = parametersResult.RecordedFileName;
            LastError = error = parametersResult.ErrorMessage;

            if (!string.IsNullOrEmpty(LastError))
                return null;

            return parametersResult.ParamString;
        }

        public string LastError { get; private set; } // Output

        public string RecordedFileName { get; private set; } // Output

        public ISourcePortData SourcePort { get; private set; } // Input/Output (but should only be input)
        public IGameFile GameFile { get; private set; } // Input/Output (but should only be input)

        /// TODO reinstate this functionality
        public bool ExtractFiles { get; set; } // Input

        void proc_Exited(object sender, EventArgs e)
        {
            ProcessExited?.Invoke(this, new EventArgs());
        }
    }
}
