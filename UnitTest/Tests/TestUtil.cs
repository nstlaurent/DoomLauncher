using DoomLauncher;
using DoomLauncher.Interfaces;
using System.IO;
using System.IO.Compression;

namespace UnitTest.Tests
{
    static class TestUtil
    {
        public static IDataSourceAdapter CreateAdapter()
        {
            string dataSource = Path.Combine(Directory.GetCurrentDirectory(), "DoomLauncher.sqlite");
            return new DbDataSourceAdapter(new SqliteDatabaseAdapter(), string.Format(@"Data Source={0}", dataSource));
        }

        public static bool AllFieldsEqual<T>(T obj1, T obj2)
        {
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(obj1);

                if (value == null)
                {
                    if (prop.GetValue(obj2) != null)
                        return false;
                }
                else
                {
                    if (!value.Equals(prop.GetValue(obj2)))
                        return false;
                }
            }

            return true;
        }

        public static void CopyResourceFile(string filename)
        {
            DeleteResourceFile(filename);
            File.Copy(Path.Combine("Resources", filename), filename);
        }

        public static void DeleteResourceFile(string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);
        }

        public static void ExtractResourceToDirectory(string filename, string directory)
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);

            string file = Path.Combine("Resources", filename);
            using (ZipArchive za = ZipFile.OpenRead(file))
                za.ExtractToDirectory(directory);
        }
    }
}
