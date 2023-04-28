using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DoomLauncher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Directory.SetCurrentDirectory(AssemblyDirectory);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(GetLaunchArgs(args)));
            }
            catch (Exception ex)
            {
                Util.DisplayUnexpectedException(null, ex);
            }
        }

        static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private static readonly string[] ArgKeys = new string[]
        { 
            nameof(LaunchArgs.LaunchGameFileID),
        };

        private static readonly string[] FlagKeys = new string[]
        {
            nameof(LaunchArgs.AutoClose)
        };

        static LaunchArgs GetLaunchArgs(string[] args)
        {
            LaunchArgs launchArgs = new LaunchArgs();

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (!arg.StartsWith("-"))
                {
                    launchArgs.LaunchFileName = args[i];
                    break;
                }

                arg = arg.Substring(1);

                var flag = FlagKeys.FirstOrDefault(x => x.Equals(arg, StringComparison.OrdinalIgnoreCase));
                if (flag != null)
                {
                    SetLaunchFlag(launchArgs, flag);
                    continue;
                }

                var argKey = ArgKeys.FirstOrDefault(x => x.Equals(arg, StringComparison.OrdinalIgnoreCase));
                if (!ArgKeys.Any(x => x.Equals(arg, StringComparison.OrdinalIgnoreCase)))
                    continue;

                var pi = typeof(LaunchArgs).GetProperty(argKey);
                if (pi == null)
                    continue;

                i++;
                if (i >= args.Length)
                    break;

                if (!int.TryParse(args[i], out int value))
                    continue;

                pi.SetValue(launchArgs, value);
            }

            return launchArgs;
        }

        private static void SetLaunchFlag(LaunchArgs launchArgs, string flag)
        {
            var pi = typeof(LaunchArgs).GetProperty(flag);
            if (pi == null)
                return;

            pi.SetValue(launchArgs, true);
        }
    }
}
