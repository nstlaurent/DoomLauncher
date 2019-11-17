using System;
using System.Threading.Tasks;
using DoomLauncher;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestUpdateInfo
    {
        [TestMethod]
        public async Task TestGetVersion()
        {
            ApplicationUpdate applicationUpdate = new ApplicationUpdate(TimeSpan.FromSeconds(30));
            Version version = new Version("1.0.0.0");
            ApplicationUpdateInfo info = await applicationUpdate.GetUpdateApplicationInfo(version);

            Assert.IsNotNull(info);
            Assert.IsTrue(info.Version > version);
            Assert.IsTrue(info.DownloadUrl.EndsWith(".zip"));

            info = await applicationUpdate.GetUpdateApplicationInfo(info.Version);

            Assert.IsNull(info);
        }
    }
}
