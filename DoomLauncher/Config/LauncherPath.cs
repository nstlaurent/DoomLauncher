using DoomLauncher.Handlers;
using System;
using System.IO;
using System.Text;

namespace DoomLauncher
{
    public class LauncherPath
    {
        public static readonly LauncherPath NoPath = new LauncherPath(string.Empty);

        private static readonly char[] PathSplit = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        private readonly string m_path, m_fullPath;

        public LauncherPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                m_path = string.Empty;
                m_fullPath = string.Empty;
                return;
            }

            m_path = path;
            m_fullPath = m_path;

            if (PathExtensions.IsPartiallyQualified(m_fullPath))
                m_fullPath = Path.Combine(Directory.GetCurrentDirectory(), m_fullPath);
            else
                m_path = GetRelativePath(m_fullPath);
        }

        public static string GetRelativePath(string path)
        {
            string current = GetDataDirectory();
            if (!path.Contains(current))
                return path;
            
            string[] filePath = path.Split(PathSplit, StringSplitOptions.RemoveEmptyEntries);
            string[] currentPath = current.Split(PathSplit, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sb = new StringBuilder(255);
            for (int i = currentPath.Length; i < filePath.Length; i++)
            {
                sb.Append(filePath[i]);
                sb.Append(Path.DirectorySeparatorChar);
            }

            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        public static string GetDataDirectory()
        {
            if (IsInstalled())
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DoomLauncher");

            return Directory.GetCurrentDirectory();
        }

        public static bool IsInstalled() => !File.Exists(Path.Combine(Directory.GetCurrentDirectory(), DbDataSourceAdapter.DatabaseFileName)) && !File.Exists(Path.Combine(Directory.GetCurrentDirectory(), DbDataSourceAdapter.InitDatabaseFileName));

        public string GetFullPath()
        {
            return m_fullPath;
        }

        public string GetPossiblyRelativePath()
        {
            return m_path;
        } 

        public static implicit operator string(LauncherPath p)
        {
            Util.ThrowDebugException("implicit operator should not be assumed");
            return p.GetFullPath();
        }

        public override bool Equals(object obj)
        {
            if (obj is LauncherPath path)
                return GetFullPath() == path.GetFullPath();

            return false;
        }

        public override int GetHashCode()
        {
            return GetFullPath().GetHashCode();
        }
    }
}
