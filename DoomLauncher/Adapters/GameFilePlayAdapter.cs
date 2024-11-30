using DoomLauncher.Adapters.Launch;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DoomLauncher
{


    public class GameFilePlayAdapter
    {
        public delegate void GameLaunchExitHandler(GameLaunchInfo info);

        public event GameLaunchExitHandler ProcessExited;

        private readonly List<ILaunchFeature> _features;

        public GameFilePlayAdapter(List<ILaunchFeature> features)
        {
            _features = AddGameFileFeatureIfMissing(features);
        }

        public GameFilePlayAdapter() : this(new List<ILaunchFeature>())
        {
            
        }

        private static List<ILaunchFeature> AddGameFileFeatureIfMissing(List<ILaunchFeature> proposedFeatures)
        {
            var updatedFeatures = new List<ILaunchFeature>(proposedFeatures);
            if (!updatedFeatures.Exists(f => f is GameFilesLaunchFeature))
            {
                var iWadIndex = updatedFeatures.FindIndex(f => f is IWadLaunchFeature);
                var insertIndex = (iWadIndex == -1) ? 0 : iWadIndex + 1;
                updatedFeatures.Insert(insertIndex, new GameFilesLaunchFeature(null, null));
            }
            return updatedFeatures;
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

            LaunchParameters launchParameters = GetLaunchParameters(gameFileDirectory, tempDirectory, gameFile, sourcePort, isGameFileIwad);
            if (launchParameters.Failed)
            {
                return LaunchResult.Failure($"Failed to create launch parameters: {launchParameters.ErrorMessage}");
            }
       
            Directory.SetCurrentDirectory(sourcePort.Directory.GetFullPath());

            var gameLaunchInfo = new GameLaunchInfo(this, gameFile, sourcePort, launchParameters.RecordedFileName);
            try
            {
                Process proc = Process.Start(sourcePort.GetFullExecutablePath(), launchParameters.ParamString);
                proc.EnableRaisingEvents = true;
                proc.Exited += gameLaunchInfo.proc_Exited;
            }
            catch
            {
                return LaunchResult.Failure("Failed to execute the source port process.");
            }

            return LaunchResult.Success(gameLaunchInfo);            
        }

        public LaunchParameters GetLaunchParameters(LauncherPath gameFileDirectory, LauncherPath tempDirectory, IGameFile gameFile, ISourcePortData sourcePortData, bool isGameFileIwad)
        {
            var paramList = _features.Select(f => f.CreateParam(sourcePortData, gameFile, isGameFileIwad, gameFileDirectory, tempDirectory));
            var combinedParams = paramList.Aggregate(LaunchParameters.EMPTY, (a, b) => a.Combine(b));
            return combinedParams.WithVariableReplacement("filename", gameFile.FileNameNoPath);
        }

        /// TODO reinstate this functionality
        public bool ExtractFiles { get; set; } // Input

        public class GameLaunchInfo
        {
            public IGameFile GameFile { get; }
            public ISourcePortData SourcePort { get; }
            public string RecordedFileName { get; }

            private readonly GameFilePlayAdapter _adapter;

            public GameLaunchInfo(GameFilePlayAdapter adapter, IGameFile gameFile, ISourcePortData sourcePort, string recordedFileName)
            {
                _adapter = adapter;
                GameFile = gameFile;
                SourcePort = sourcePort;
                RecordedFileName = recordedFileName;
            }

            public void proc_Exited(object sender, EventArgs e)
            {
                _adapter.ProcessExited.Invoke(this);
            }
        }
    }
}
