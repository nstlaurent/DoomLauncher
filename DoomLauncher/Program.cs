using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DoomLauncher
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Directory.SetCurrentDirectory(AssemblyDirectory);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var splash = new SplashScreen();
                splash.StartPosition = FormStartPosition.CenterScreen;
                splash.Show();
                splash.Invalidate();

                if (!ProgramInit.Init())
                    return;

                MainForm form = new MainForm(GetLaunchArgs(args), splash);
                form.Load += Form_Load;
                Application.Run(form);
            }
            catch (Exception ex)
            {
                Util.DisplayUnexpectedException(null, ex);
            }
        }

        private static async void Form_Load(object sender, EventArgs e)
        {
            MainForm form = sender as MainForm;
            await form.Init();
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
