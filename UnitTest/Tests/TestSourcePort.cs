using Microsoft.VisualStudio.TestTools.UnitTesting;
using DoomLauncher.Interfaces;
using DoomLauncher;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestSourcePort
    { 
        private List<ISourcePortData> CreateTestSourcePorts()
        {
            List<ISourcePortData> sourcePorts = new List<ISourcePortData>();
            for (int i = 0; i < 10; i++)
            {
                sourcePorts.Add(
                    new DoomLauncher.DataSources.SourcePortData()
                    {
                        SourcePortID = i + 1,
                        Name = "test" + i.ToString(),
                        Executable = "test" + i.ToString(),
                        FileOption = "test" + i.ToString(),
                        SettingsFiles = "test" + i.ToString(),
                        SupportedExtensions = "test" + i.ToString(),
                        Directory = new LauncherPath(i.ToString()),
                        LaunchType = i > 6 ? SourcePortLaunchType.Utility : SourcePortLaunchType.SourcePort,
                        ExtraParameters = "test" + i.ToString()
                    }
                );
            }
            return sourcePorts;
        }

        [TestMethod]
        public void TestInsert()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var sourcePorts = CreateTestSourcePorts();
            foreach (var port in sourcePorts)
                adapter.InsertSourcePort(port);

            Assert.AreEqual(sourcePorts.Count(x => x.LaunchType == SourcePortLaunchType.SourcePort), 
                adapter.GetSourcePorts().Count());
            Assert.AreEqual(sourcePorts.Count(x => x.LaunchType == SourcePortLaunchType.Utility), 
                adapter.GetUtilities().Count());
        }

        [TestMethod]
        public void TestGetSourcePorts()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var sourcePorts = adapter.GetSourcePorts();
            var utilites = adapter.GetUtilities();

            Assert.AreEqual(7, sourcePorts.Count());
            Assert.AreEqual(3, utilites.Count());

            List<ISourcePortData> allSourcePorts = new List<ISourcePortData>();
            allSourcePorts.AddRange(sourcePorts);
            allSourcePorts.AddRange(utilites);

            var testSourcePorts = CreateTestSourcePorts();
    
            foreach(var testSourcePort in testSourcePorts)
            {
                var port = allSourcePorts.First(x => x.SourcePortID == testSourcePort.SourcePortID);
                Assert.IsTrue(TestUtil.AllFieldsEqual(testSourcePort, port));
            }
        }

        [TestMethod]
        public void TestGetSourcePort()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();

            var testSourcePorts = CreateTestSourcePorts();

            foreach (var testSourcePort in testSourcePorts)
            {
                var dbPort = adapter.GetSourcePort(testSourcePort.SourcePortID);
                Assert.IsNotNull(dbPort);
                Assert.IsTrue(TestUtil.AllFieldsEqual(testSourcePort, dbPort));
            }
        }

        [TestMethod]
        public void TestUpdate()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();

            List<ISourcePortData> allSourcePorts = new List<ISourcePortData>();
            allSourcePorts.AddRange(adapter.GetSourcePorts());
            allSourcePorts.AddRange(adapter.GetUtilities());

            int somenumber = 100;

            foreach(var sourcePort in allSourcePorts)
            {
                sourcePort.Directory = new LauncherPath(sourcePort.Directory.GetPossiblyRelativePath() + somenumber.ToString());
                sourcePort.Name += somenumber.ToString();
                sourcePort.SettingsFiles += somenumber.ToString();
                sourcePort.SupportedExtensions += somenumber.ToString();

                adapter.UpdateSourcePort(sourcePort);

                var dbPort = adapter.GetSourcePort(sourcePort.SourcePortID);
                Assert.IsNotNull(dbPort);
                Assert.IsTrue(TestUtil.AllFieldsEqual(sourcePort, dbPort));
            }
        }

        [TestMethod]
        public void TestDelete()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();

            List<ISourcePortData> allSourcePorts = new List<ISourcePortData>();
            allSourcePorts.AddRange(adapter.GetSourcePorts());
            allSourcePorts.AddRange(adapter.GetUtilities());

            int count = allSourcePorts.Count;

            foreach (var sourcePort in allSourcePorts)
            {
                adapter.DeleteSourcePort(sourcePort);
                count--;
                Assert.AreEqual(count, adapter.GetSourcePorts().Count() + adapter.GetUtilities().Count());
                Assert.IsNull(adapter.GetSourcePort(sourcePort.SourcePortID));
            }
        }
    }
}
