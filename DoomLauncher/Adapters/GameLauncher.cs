using DoomLauncher.Adapters.Launch;
using DoomLauncher.Config;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DoomLauncher
{
    public class GameLauncher
    {
        public delegate void GameLaunchExitHandler(GameLaunchInfo info);

        public event GameLaunchExitHandler ProcessExited;

        private readonly List<ILaunchFeature> _features;

        private readonly IDirectoriesConfiguration _directories;

        public GameLauncher(IDirectoriesConfiguration directories, List<ILaunchFeature> features)
        {
            _features = AddGameFileFeatureIfMissing(features);
            _directories = directories;
        }

        public GameLauncher(IDirectoriesConfiguration directories) : this(directories, new List<ILaunchFeature>())
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

        public LaunchResult Launch(IGameFile gameFile, IEnumerable<IGameFile> addFiles, ISourcePortData sourcePort, bool isGameFileIwad)
        {
            if (!Directory.Exists(sourcePort.Directory.GetFullPath()))
            {
                var errorMessage = string.Concat("The source port directory does not exist:", Environment.NewLine, Environment.NewLine,
                    sourcePort.Directory.GetPossiblyRelativePath());
                return LaunchResult.Failure(errorMessage);
            }

            LaunchParameters launchParameters = GetLaunchParameters(gameFile, addFiles, sourcePort, isGameFileIwad);
            if (launchParameters.Failed)
            {
                return LaunchResult.Failure($"Failed to create launch parameters: {launchParameters.ErrorMessage}");
            }
       
            Directory.SetCurrentDirectory(sourcePort.Directory.GetFullPath());

            var gameLaunchInfo = new GameLaunchInfo(this, gameFile, sourcePort, launchParameters.RecordedFileName);
            try
            {
                Process proc = Process.Start(sourcePort.GetFullExecutablePath(), launchParameters.LaunchString);
                proc.EnableRaisingEvents = true;
                proc.Exited += gameLaunchInfo.proc_Exited;
            }
            catch
            {
                return LaunchResult.Failure("Failed to execute the source port process.");
            }

            return LaunchResult.Success(gameLaunchInfo);            
        }

        public LaunchParameters GetLaunchParameters(IGameFile gameFile, IEnumerable<IGameFile> addFiles, ISourcePortData sourcePortData, bool isGameFileIwad)
        {
            var paramList = _features.Select(f => f.CreateParameter(gameFile, addFiles, sourcePortData, isGameFileIwad, _directories));
            var combinedParams = paramList.Aggregate(LaunchParameters.EMPTY, (a, b) => a.Combine(b));
            return combinedParams.WithVariableReplacement("filename", gameFile.FileNameNoPath);
        }

        public class GameLaunchInfo
        {
            public IGameFile GameFile { get; }
            public ISourcePortData SourcePort { get; }
            public string RecordedFileName { get; }

            private readonly GameLauncher _adapter;

            public GameLaunchInfo(GameLauncher adapter, IGameFile gameFile, ISourcePortData sourcePort, string recordedFileName)
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
