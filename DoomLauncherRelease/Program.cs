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
                string outputDir = "ReleaseBuild";
                string buildDir = CreateBuildDirectory(outputDir);

                if (!Directory.Exists(Path.Combine(GetBaseDir(), outputDir)))
                    Directory.CreateDirectory(Path.Combine(GetBaseDir(), outputDir));

                Directory.CreateDirectory(buildDir);
                CreateFolders(buildDir);
                CopyBuildFiles(buildDir);

                string zipfile = CreateZipFile(outputDir, buildDir);
                string zipfiledest = Path.Combine(GetBaseDir(), outputDir, Path.GetFileName(zipfile));
                File.Copy(zipfile, zipfiledest, true);

                Console.WriteLine(string.Concat("Sucessfully created: ", zipfiledest));

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Concat("Release failure: ", ex.Message, Environment.NewLine, ex.StackTrace));
                return -1;
            }
        }

        private static string CreateBuildDirectory(string outputDir)
        {
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);


            return Path.Combine(outputDir, "DoomLauncher");
        }

        private static void CreateFolders(string outputdir)
        {
            string basePath = Path.Combine(outputdir, "GameFiles_");

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(Path.Combine(basePath, "Demos"));
                Directory.CreateDirectory(Path.Combine(basePath, "SaveGames"));
                Directory.CreateDirectory(Path.Combine(basePath, "Screenshots"));
                Directory.CreateDirectory(Path.Combine(basePath, "Temp"));
            }
        }

        private static string CreateZipFile(string outputDir, string buildDir)
        {
            string zipfile = string.Format("DoomLauncher_{0}.zip", GetVersion(buildDir));
            if (File.Exists(zipfile))
                File.Delete(zipfile);

            ZipFile.CreateFromDirectory(outputDir, zipfile);
            return zipfile;
        }

        private static string GetVersion(string buildDir)
        {
            string prefix = string.Empty;
#if DEBUG
            prefix = "debug_";
#endif
            var v = AssemblyName.GetAssemblyName(Path.Combine(buildDir, "DoomLauncher.exe")).Version;
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
                "WadReader.dll",
                "Octokit.dll",
                "Octokit.xml"
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
