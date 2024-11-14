using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher.Adapters.Launch
{
    public class LaunchResult
    {
        public string ParamString { get; } // Never null

        public string ErrorMessage { get; }

        public string RecordedFileName { get; }

        public bool Failed { get => ErrorMessage != null; }

        public static readonly LaunchResult EMPTY = LaunchResult.Success(""); 

        private LaunchResult(string paramString, string recordedFileName, string errorMessage) 
        { 
            ParamString = paramString;
            ErrorMessage = errorMessage;
            RecordedFileName = recordedFileName;
        }

        public LaunchResult Combine(LaunchResult other)
        {
            if (Failed)
            {
                return this;
            }
            else if (other.Failed)
            {
                return other;
            }
            else 
            {
                return Success($"{ParamString} {other.ParamString}".Trim(), RecordedFileName ?? other.RecordedFileName);
            }
        }

        public static LaunchResult Success(string paramString, string recordedFileName = null)
        {
            if (paramString == null)
            {
                throw new ArgumentNullException(nameof(paramString));
            }
            return new LaunchResult(paramString, recordedFileName, null);
        }

        public static LaunchResult Failure(string errorMessage)
        {
            return new LaunchResult("", null, errorMessage);
        }
    }
}
