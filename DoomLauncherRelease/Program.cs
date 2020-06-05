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

                string zipfile = CreateZipFile(outputDir, buildDir, false);
                string zipfiledest = Path.Combine(GetBaseDir(), outputDir, Path.GetFileName(zipfile));
                File.Copy(zipfile, zipfiledest, true);

                zipfile = CreateZipFile(Path.Combine(GetBaseDir(), "Setup", GetBuildOutputDir()), buildDir, true);
                zipfiledest = Path.Combine(GetBaseDir(), outputDir, Path.GetFileName(zipfile));
                File.Copy(zipfile, zipfiledest, true);

                return 0;
            }
            catch
            {
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
                Directory.CreateDirectory(Path.Combine(basePath, "Thumbnails"));
            }
        }

        private static string CreateZipFile(string outputDir, string buildDir, bool installer)
        {
            string zipfile = $"DoomLauncher_{GetVersion(buildDir)}{GetInstallPost(installer)}.zip";
            if (File.Exists(zipfile))
                File.Delete(zipfile);

            ZipFile.CreateFromDirectory(outputDir, zipfile);
            return zipfile;
        }

        private static string GetInstallPost(bool installer)
        {
            if (installer)
                return "_install";

            return string.Empty;
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
            string buildDir = Path.Combine(GetDoomLauncherDir(), GetBuildBinOutputDir());

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

        static string GetBuildBinOutputDir()
        {
#if DEBUG
            return Path.Combine("bin", GetBuildOutputDir());
#else
            return Path.Combine("bin", GetBuildOutputDir());
#endif
        }

        static string GetBuildOutputDir()
        {
#if DEBUG
            return "Debug";
#else
            return "Release";
#endif
        }
    }
}
