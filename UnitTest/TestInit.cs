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
            DbDataSourceAdapter adapter = (DbDataSourceAdapter)TestUtil.CreateAdapter();

            DataCache.Instance.Init(adapter);

            VersionHandler versionHandler = new VersionHandler(adapter.DataAccess, adapter, new AppConfiguration(adapter));
            versionHandler.HandleVersionUpdate();
        }
    }
}
