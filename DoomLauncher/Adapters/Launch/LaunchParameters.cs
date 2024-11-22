using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using IWshRuntimeLibrary;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Windows.Media.Media3D;

namespace DoomLauncher.Adapters.Launch
{
    public class LaunchParameters
    {
        public string ParamString { get; } // Never null

        public string ErrorMessage { get; }

        public string RecordedFileName { get; }

        public bool Failed { get => ErrorMessage != null; }

        // Generic, ordered dictionaries do not exist (yet) in .NET. Tuple list is fine for us, we don't need random access.
        //private readonly List<(LaunchParameterType, List<string>)> _parameters;

        private readonly bool _isFinal;

        public static readonly LaunchParameters EMPTY = new LaunchParameters("", null, null, false);

        private LaunchParameters(string paramString, string recordedFileName, string errorMessage, bool isFinal)
        {
            ParamString = paramString ?? "";
            ErrorMessage = errorMessage;
            RecordedFileName = recordedFileName;
            _isFinal = isFinal;
        }

        public LaunchParameters Combine(LaunchParameters other)
        {
            if (Failed)
            {
                return this;
            }
            else if (other.Failed)
            {
                return other;
            }
            else if (_isFinal)
            {
                return this;
            }
            else
            {
                /*
                var newParameters = new List<(LaunchParameterType, List<string>)>();
                foreach ((var ourParamType, var ourParamList) in _parameters)
                {
                    foreach ((var theirParamType, var theirParamList) in other._parameters)
                    {
                        if (ourParamType == theirParamType)
                        {
                            var newParamList = new List<string>(ourParamList);
                            newParamList.AddRange(theirParamList);
                            newParameters.Add((ourParamType, newParamList));
                        }
                    }
                }*/

                return new LaunchParameters($" {ParamString.Trim()} {other.ParamString.Trim()}", RecordedFileName ?? other.RecordedFileName, null, other._isFinal);
            }
        }

        public static LaunchParameters FinalParam(string paramString)
        {
            return new LaunchParameters(paramString, null, null, true);
        }

        public static LaunchParameters Param(string paramString)
        {
            return new LaunchParameters(paramString, null, null, false);
        }

        public static LaunchParameters ParamWithRecording(string paramString, string recordedFileName)
        {
            return new LaunchParameters(paramString, recordedFileName, null, false);
        }

        public static LaunchParameters Failure(string errorMessage)
        {
            return new LaunchParameters("", null, errorMessage, false);
        }
    }
}
