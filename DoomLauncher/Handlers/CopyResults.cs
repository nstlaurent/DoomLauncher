using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher
{
    public class CopyError
    {
        public string FileName;
        public string Error;
    }

    class CopyResults
    {
        public List<string> NewFiles = new List<string>();
        public List<string> ReplacedFiles = new List<string>();
        public List<CopyError> Errors = new List<CopyError>();
    }
}
