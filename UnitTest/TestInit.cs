using Microsoft.VisualStudio.TestTools.UnitTesting;
using DoomLauncher;
using UnitTest.Tests;
using System.IO;

namespace UnitTest
{
    [TestClass]
    static class TestInit
    {
        [AssemblyInitialize()]
        public static void TestInitialize(TestContext testContext)
        {
            string dataSource = Path.Combine(Directory.GetCurrentDirectory(), "DoomLauncher.sqlite");
            DbDataSourceAdapter adapter = (DbDataSourceAdapter)TestUtil.CreateAdapter();
            DataAccess access = new DataAccess(new SqliteDatabaseAdapter(), DbDataSourceAdapter.CreateConnectionString(dataSource));

            VersionHandler versionHandler = new VersionHandler(access, adapter, new AppConfiguration(adapter));
            versionHandler.HandleVersionUpdate();
        }
    }
}
