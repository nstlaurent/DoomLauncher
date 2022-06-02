using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace DoomLauncherRelease
{
    static class Program
    {
        static string s_argPath;

        static int Main(string[] args)
        {
            if (args.Length > 0)
                s_argPath = args[0];

            string outputDir = "ReleaseBuild";
            string buildDir = CreateBuildDirectory(outputDir);

            if (!Directory.Exists(Path.Combine(GetBaseDir(), outputDir)))
                Directory.CreateDirectory(Path.Combine(GetBaseDir(), outputDir));

            Directory.CreateDirectory(buildDir);
            CopyBuildFiles(buildDir);

            string zipfile = CreateZipFile(outputDir, buildDir, false);
            string zipfiledest = Path.Combine(GetBaseDir(), outputDir, Path.GetFileName(zipfile));
            File.Copy(zipfile, zipfiledest, true);

            zipfile = CreateZipFile(Path.Combine(GetBaseDir(), "Setup", GetBuildOutputDir()), buildDir, true);
            zipfiledest = Path.Combine(GetBaseDir(), outputDir, Path.GetFileName(zipfile));
            File.Copy(zipfile, zipfiledest, true);

            return 0;
        }

        private static string CreateBuildDirectory(string outputDir)
        {
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);

            return Path.Combine(outputDir, "DoomLauncher");
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
            foreach (string dir in GetDirectoriesToCreate())
                Directory.CreateDirectory(Path.Combine(outputdir, dir));

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

        static string[] GetDirectoriesToCreate() =>
            new string[] { "x86", "x64" };

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
                "System.Memory.dll",
                "System.Numerics.Vectors.dll",
                "System.Runtime.CompilerServices.Unsafe.dll",
                "System.Buffers.dll",
                "WadReader.dll",
                "Octokit.dll",
                "Octokit.xml",
                "SevenZipSharp.dll",
                "SharpCompress.dll",
                "x64\\SQLite.Interop.dll",
                "x86\\SQLite.Interop.dll",
                "x64\\7z.dll",
                "x86\\7z.dll",
            };
        }

        static string GetDoomLauncherDir()
        {
            return Path.Combine(GetBaseDir(), "DoomLauncher");
        }

        static string GetBaseDir()
        {
            if (string.IsNullOrEmpty(s_argPath))
                return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\"));
            else
                return s_argPath;
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
