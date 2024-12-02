
using System.Collections.Generic;
using System.Text;

namespace DoomLauncher.Adapters.Launch
{
    public class LaunchParameters
    {
        public string LaunchString { get => _launchString; } 

        private readonly string _launchString; // Never null

        public string ErrorMessage { get; } // Null unless failure

        public string RecordedFileName { get; } // Null unless supplied

        public bool Failed { get => ErrorMessage != null; }

        private readonly IDictionary<string, string> _variableReplacements; // Never null

        private readonly bool _isExclusive; // false unless supplied

        public static readonly LaunchParameters EMPTY = new LaunchParameters("", null, null, false, null);

        private LaunchParameters(string launchString, string recordedFileName, string errorMessage, bool isExclusive, 
            IDictionary<string, string> variableReplacements)
        {
            ErrorMessage = errorMessage;
            RecordedFileName = recordedFileName;
            _variableReplacements = variableReplacements ?? new Dictionary<string, string>();
            _isExclusive = isExclusive;
            _launchString = ReplaceVariables(launchString ?? "");
        }

        public LaunchParameters Combine(LaunchParameters other)
        {
            if (Failed || _isExclusive)
            {
                return this;
            }
            else if (other.Failed || other._isExclusive)
            {
                return other;
            }
            else
            {
                return new LaunchParameters(
                    $"{LaunchString.Trim()} {other.LaunchString.Trim()}".Trim(), 
                    RecordedFileName ?? other.RecordedFileName, 
                    null, 
                    other._isExclusive,
                    CombineDictionaries(_variableReplacements, other._variableReplacements));
            }
        }

        public LaunchParameters WithRecordedFileName(string recordedFileName)
        {
            return Combine(new LaunchParameters("", recordedFileName, null, false, null));
        }

        public LaunchParameters WithVariableReplacement(string variable, string value)
        {
            var dict = new Dictionary<string, string>
            {
                { variable, value }
            };
            return Combine(new LaunchParameters("", null, null, false, dict));
        }

        private IDictionary<A, A> CombineDictionaries<A>(IDictionary<A, A> ourDict, IDictionary<A, A> otherDict)
        {
            var combinedDictionary = new Dictionary<A, A>(ourDict);
            foreach (var key in otherDict.Keys)
            {
                if (!combinedDictionary.ContainsKey(key))
                    combinedDictionary[key] = otherDict[key];
            }
            return combinedDictionary;
        }

        private string ReplaceVariables(string paramString)
        {
            var sb = new StringBuilder(paramString);
            foreach (var key in _variableReplacements.Keys)
            {
                sb.Replace($"${key}", _variableReplacements[key]);
            }
            return sb.ToString();
        }

        public static LaunchParameters ExclusiveParam(string paramString)
        {
            return new LaunchParameters(paramString, null, null, true, null);
        }

        public static LaunchParameters Param(string paramString)
        {
            return new LaunchParameters(paramString, null, null, false, null);
        }

        public static LaunchParameters Failure(string errorMessage)
        {
            return new LaunchParameters("", null, errorMessage, false, null);
        }
    }
}
