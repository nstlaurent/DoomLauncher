using DoomLauncher.Adapters.Launch;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DoomLauncher
{
    public class GameFilePlayAdapter
    {
        public event EventHandler ProcessExited;

        private readonly List<ILaunchFeature> _features;

        public GameFilePlayAdapter(List<ILaunchFeature> features)
        {
            _features = AddGameFileFeatureIfMissing(new List<ILaunchFeature>(features));
        }

        public GameFilePlayAdapter() : this(new List<ILaunchFeature>())
        {
            
        }

        private static List<ILaunchFeature> AddGameFileFeatureIfMissing(List<ILaunchFeature> proposedFeatures)
        {
            if (!proposedFeatures.Exists(f => f is GameFilesLaunchFeature))
            {
                var iWadIndex = proposedFeatures.FindIndex(f => f is IWadLaunchFeature);
                var insertIndex = (iWadIndex == -1) ? 0 : iWadIndex + 1;
                proposedFeatures.Insert(insertIndex, new GameFilesLaunchFeature(null, null));
            }
            return proposedFeatures;
        }

        public LaunchResult Launch(LauncherPath gameFileDirectory, LauncherPath tempDirectory,
            IGameFile gameFile, ISourcePortData sourcePort, bool isGameFileIwad)
        {
            if (!Directory.Exists(sourcePort.Directory.GetFullPath()))
            {
                var errorMessage = string.Concat("The source port directory does not exist:", Environment.NewLine, Environment.NewLine,
                    sourcePort.Directory.GetPossiblyRelativePath());
                return LaunchResult.Failure(errorMessage);
            }

            GameFile = gameFile;
            SourcePort = sourcePort;

            LaunchParameters launchParameters = GetLaunchParameters(gameFileDirectory, tempDirectory, gameFile, sourcePort, isGameFileIwad);
            if (launchParameters.Failed)
            {
                return LaunchResult.Failure($"Failed to create launch parameters: {launchParameters.ErrorMessage}");
            }
       
            Directory.SetCurrentDirectory(sourcePort.Directory.GetFullPath());

            try
            {
                Process proc = Process.Start(sourcePort.GetFullExecutablePath(), launchParameters.ParamString);
                proc.EnableRaisingEvents = true;
                proc.Exited += proc_Exited;
            }
            catch
            {
                return LaunchResult.Failure("Failed to execute the source port process.");
            }

            return LaunchResult.Success();            
        }

        public LaunchParameters GetLaunchParameters(LauncherPath gameFileDirectory, LauncherPath tempDirectory, IGameFile gameFile, ISourcePortData sourcePortData, bool isGameFileIwad)
        {
            LaunchParameters parametersResult = LaunchParameters.EMPTY;
            foreach (var feature in _features)
            {
                var newParameters = feature.CreateParam(sourcePortData, gameFile, isGameFileIwad, gameFileDirectory, tempDirectory);
                parametersResult = parametersResult.Combine(newParameters);
            }

            parametersResult = parametersResult.WithVariableReplacement("filename", gameFile.FileNameNoPath);

            RecordedFileName = parametersResult.RecordedFileName;

            return parametersResult;
        }

        // Depended on by Exit event handler
        public string RecordedFileName { get; private set; } // Output 

        // Depended on by Exit event handler
        public ISourcePortData SourcePort { get; private set; } // Output

        // Depended on by statistics event handler, which is using the GameFilePlayAdapter
        // attached to the PlaySession
        // Depended on by Exit event handler
        public IGameFile GameFile { get; private set; } // Output

        /// TODO reinstate this functionality
        public bool ExtractFiles { get; set; } // Input

        void proc_Exited(object sender, EventArgs e)
        {
            ProcessExited?.Invoke(this, new EventArgs());
        }
    }
}
