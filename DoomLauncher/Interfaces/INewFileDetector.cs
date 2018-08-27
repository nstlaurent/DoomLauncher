using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Interfaces
{
    public interface INewFileDetector
    {
        void StartDetection();
        string[] GetNewFiles();
        string[] GetModifiedFiles();
    }
}
