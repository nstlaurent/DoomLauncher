using DoomLauncher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestFileDetector
    {
        private NewFileDetector m_detector;
        private static string s_testFileDir = "testfiles";

        [TestInitialize]
        public void Init()
        {
            m_detector = new NewFileDetector(new string[] { ".zds", ".dsg" }, s_testFileDir);

            if (Directory.Exists(s_testFileDir))
                Directory.Delete(s_testFileDir, true);

            Directory.CreateDirectory(s_testFileDir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(s_testFileDir, true);
        }

        private static void CreateTestFile(string filename)
        {
            File.WriteAllText(Path.Combine(s_testFileDir, filename), string.Empty);
        }

        private static void UpdateTestFile(string filename)
        {
            File.WriteAllText(Path.Combine(s_testFileDir, filename), "test");
        }

        private static void DeleteTestFile(string filename)
        {
            File.Delete(Path.Combine(s_testFileDir, filename));
        }

        private static bool ContainsFile(string[] filenames, string filename)
        {
            return filenames.Any(x => Path.GetFileName(x) == filename);
        }

        [TestMethod]
        public void TestEmpty()
        {
            m_detector.StartDetection();

            Assert.AreEqual(0, m_detector.GetNewFiles().Length);
            Assert.AreEqual(0, m_detector.GetModifiedFiles().Length);
        }

        [TestMethod]
        public void TestNew()
        {
            m_detector.StartDetection();

            CreateTestFile("test1.zds");

            Assert.AreEqual(1, m_detector.GetNewFiles().Length);
            Assert.IsTrue(ContainsFile(m_detector.GetNewFiles(), "test1.zds"));

            CreateTestFile("test1.dsg");

            Assert.AreEqual(2, m_detector.GetNewFiles().Length);
            Assert.IsTrue(ContainsFile(m_detector.GetNewFiles(), "test1.dsg"));
        }

        [TestMethod]
        public void TestNewWithExisting()
        {
            CreateTestFile("test1.zds");
            CreateTestFile("test1.dsg");

            m_detector.StartDetection();

            Assert.AreEqual(0, m_detector.GetNewFiles().Length);
            Assert.AreEqual(0, m_detector.GetModifiedFiles().Length);

            CreateTestFile("test2.zds");

            Assert.AreEqual(1, m_detector.GetNewFiles().Length);
            Assert.IsTrue(ContainsFile(m_detector.GetNewFiles(), "test2.zds"));
        }

        [TestMethod]
        public void TestModified()
        {
            CreateTestFile("test1.zds");
            CreateTestFile("test1.dsg");

            m_detector.StartDetection();

            UpdateTestFile("test1.zds");
            Assert.AreEqual(0, m_detector.GetNewFiles().Length);
            Assert.AreEqual(1, m_detector.GetModifiedFiles().Length);
            Assert.IsTrue(ContainsFile(m_detector.GetModifiedFiles(), "test1.zds"));

            CreateTestFile("test2.zds");
            UpdateTestFile("test1.dsg");

            Assert.AreEqual(1, m_detector.GetNewFiles().Length);
            Assert.AreEqual(2, m_detector.GetModifiedFiles().Length);
            Assert.IsTrue(ContainsFile(m_detector.GetModifiedFiles(), "test1.dsg"));
            Assert.IsTrue(ContainsFile(m_detector.GetNewFiles(), "test2.zds"));
        }

        [TestMethod]
        public void TestDelete()
        {
            CreateTestFile("test1.zds");
            CreateTestFile("test1.dsg");

            m_detector.StartDetection();
            Assert.AreEqual(0, m_detector.GetNewFiles().Length);

            DeleteTestFile("test1.zds");
            Assert.AreEqual(1, m_detector.GetDeletedFiles().Length);
            Assert.IsTrue(ContainsFile(m_detector.GetDeletedFiles(), "test1.zds"));

            DeleteTestFile("test1.dsg");
            Assert.AreEqual(2, m_detector.GetDeletedFiles().Length);
            Assert.IsTrue(ContainsFile(m_detector.GetDeletedFiles(), "test1.dsg"));
        }
    }
}
