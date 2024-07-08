using System;
using DoomLauncher;
using DoomLauncher.Interfaces;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;

namespace UnitTest.Tests
{
    static class TestUtil
    {
        public static IDataSourceAdapter CreateAdapter()
        {
            string dataSource = Path.Combine(Directory.GetCurrentDirectory(), "DoomLauncher.sqlite");
            return new DbDataSourceAdapter(new SqliteDatabaseAdapter(), string.Format(@"Data Source={0}", dataSource));
        }

        public static void CleanDatabase(IDataSourceAdapter adapter)
        {
            var dbAdapter = (DbDataSourceAdapter)adapter;
            var access = new DataAccess(dbAdapter.DbAdapter, dbAdapter.ConnectionString);
            access.ExecuteNonQuery("delete from Files");
            access.ExecuteNonQuery("delete from GameProfiles");
            access.ExecuteNonQuery("delete from GameFiles");
            access.ExecuteNonQuery("delete from IWads");
            access.ExecuteNonQuery("delete from SourcePorts");
            access.ExecuteNonQuery("delete from TagMapping");
            access.ExecuteNonQuery("delete from Tags");
        }

        public static bool AllFieldsEqualIgnore<T>(T obj1, T obj2, params string[] ignore)
        {
            return AllFieldsEqual(obj1, obj2, ignore);
        }

        public static bool AllFieldsEqual<T>(T obj1, T obj2, string[] ignore = null)
        {
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                if (ignore != null && ignore.Contains(prop.Name))
                    continue;

                var value = prop.GetValue(obj1);

                if (value == null)
                {
                    if (prop.GetValue(obj2) != null)
                    {
                        throw new Exception(
                            $"AllFieldsEqual failure: Expected {prop.Name} to be null but was {prop.GetValue(obj2)}\n{GetObjectJson(obj1)}\n{GetObjectJson(obj2)}");
                        return false;
                    }
                }
                else
                {
                    if (!value.Equals(prop.GetValue(obj2)))
                    {
                        throw new Exception(
                            $"AllFieldsEqual failure: Expected {prop.Name} to be {value} but was {prop.GetValue(obj2)}\n{GetObjectJson(obj1)}\n{GetObjectJson(obj2)}");
                        return false;
                    }
                }
            }

            return true;
        }

        private static string GetObjectJson(object obj) =>
            JsonConvert.SerializeObject(obj);

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
