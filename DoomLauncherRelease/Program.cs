using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace DoomLauncherRelease
{
    static class Program
    {
        static int Main(string[] args)
        {
            try
            {
                string outputdir = "ReleaseBuild";

                if (Directory.Exists(outputdir))
                    Directory.Delete(outputdir, true);

                if (!Directory.Exists(Path.Combine(GetBaseDir(), outputdir)))
                    Directory.CreateDirectory(Path.Combine(GetBaseDir(), outputdir));

                Directory.CreateDirectory(outputdir);
                CreateFolders(outputdir);
                CopyBuildFiles(outputdir);

                string zipfile = CreateZipFile(outputdir);
                string zipfiledest = Path.Combine(GetBaseDir(), outputdir, Path.GetFileName(zipfile));
                File.Copy(zipfile, zipfiledest, true);

                Console.WriteLine(string.Concat("Sucessfully created: ", zipfiledest));

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Concat("Relase failure: ", ex.Message, Environment.NewLine, ex.StackTrace));
                return -1;
            }
        }

        private static void CreateFolders(string outputdir)
        {
            string basePath = Path.Combine(outputdir, "GameFiles_");

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(Path.Combine(basePath, "Demos"));
                Directory.CreateDirectory(Path.Combine(basePath, "GameWads"));
                Directory.CreateDirectory(Path.Combine(basePath, "SaveGames"));
                Directory.CreateDirectory(Path.Combine(basePath, "Screenshots"));
                Directory.CreateDirectory(Path.Combine(basePath, "Temp"));
            }
        }

        private static string CreateZipFile(string outputdir)
        {
            string zipfile = string.Format("DoomLauncher_{0}.zip", GetVersion(outputdir));
            if (File.Exists(zipfile))
                File.Delete(zipfile);

            ZipFile.CreateFromDirectory(outputdir, zipfile);
            return zipfile;
        }

        private static string GetVersion(string outputdir)
        {
            string prefix = string.Empty;
#if DEBUG
            prefix = "debug_";
#endif
            var v = AssemblyName.GetAssemblyName(Path.Combine(outputdir, "DoomLauncher.exe")).Version;
            return prefix + v.ToString();
        }

        private static void CopyBuildFiles(string outputdir)
        {
            var files = GetFilesToCopy();
            string buildDir = Path.Combine(GetDoomLauncherDir(), GetBuildOutputDir());

            foreach (var file in files)
                File.Copy(Path.Combine(buildDir, file), Path.Combine(outputdir, file));

            File.Copy(Path.Combine(GetDoomLauncherDir(), "DoomLauncher.sqlite"), Path.Combine(outputdir, "DoomLauncher_.sqlite"));

            string tileimages = "TileImages";
            string source = Path.Combine(buildDir, tileimages);
            string dest = Path.Combine(outputdir, tileimages);
            Directory.CreateDirectory(Path.Combine(outputdir, tileimages));

            var imagefiles = Directory.GetFiles(source);
            Array.ForEach(imagefiles, x => File.Copy(x, Path.Combine(dest, Path.GetFileName(x))));
        }

        static string[] GetFilesToCopy()
        {
            return new string[]
            {
                "CheckBoxComboBox.dll",
                "DoomLauncher.exe",
                "DoomLauncher.exe.config",
                "DoomLauncher.ico",
                "DoomLauncher.VisualElementsManifest.xml",
                "Equin.ApplicationFramework.BindingListView.dll",
                "Help.pdf",
                "Newtonsoft.Json.dll",
                "Newtonsoft.Json.xml",
                "System.Data.SQLite.dll",
                "WadReader.dll"
            };
        }

        static string GetDoomLauncherDir()
        {
            return Path.Combine(GetBaseDir(), "DoomLauncher");
        }

        static string GetBaseDir()
        {
            return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\"));
        }

        static string GetBuildOutputDir()
        {
#if DEBUG
            return Path.Combine("bin", "Debug");
#else
            return Path.Combine("bin", "Release");
#endif
        }
    }
}
