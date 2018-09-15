using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DoomLauncher;
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
            if (Directory.Exists(s_testFileDir))
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
            CreateTestFile("test2.zds");
            CreateTestFile("test2.dsg");

            m_detector.StartDetection();

            Assert.AreEqual(0, m_detector.GetNewFiles().Length);
            Assert.AreEqual(0, m_detector.GetModifiedFiles().Length);

            CreateTestFile("test3.zds");

            Assert.AreEqual(1, m_detector.GetNewFiles().Length);
            Assert.IsTrue(ContainsFile(m_detector.GetNewFiles(), "test3.zds"));
        }

        [TestMethod]
        public void TestModified()
        {
            CreateTestFile("test4.zds");
            CreateTestFile("test4.dsg");

            m_detector.StartDetection();

            UpdateTestFile("test4.zds");
            Assert.AreEqual(0, m_detector.GetNewFiles().Length);
            Assert.AreEqual(1, m_detector.GetModifiedFiles().Length);
            Assert.IsTrue(ContainsFile(m_detector.GetModifiedFiles(), "test4.zds"));

            CreateTestFile("test5.zds");
            UpdateTestFile("test4.dsg");

            Assert.AreEqual(1, m_detector.GetNewFiles().Length);
            Assert.AreEqual(2, m_detector.GetModifiedFiles().Length);
            Assert.IsTrue(ContainsFile(m_detector.GetModifiedFiles(), "test4.dsg"));
            Assert.IsTrue(ContainsFile(m_detector.GetNewFiles(), "test5.zds"));
        }
    }
}
