using DoomLauncher;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
