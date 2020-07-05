using DoomLauncher;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestFile
    {
        private static List<IFileData> CreateTestFiles()
        {
            List<IFileData> ret = new List<IFileData>();
            int id = 1;
            for (int j = 1; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    ret.Add(new FileData
                    {
                        FileID = id,
                        FileName = id.ToString(),
                        FileTypeID = (FileType)j,
                        FileOrder = id,
                        Description = id.ToString(),
                        DateCreated = DateTime.Parse("1/1/2017"),
                        GameFileID = id % 3,
                        SourcePortID = id,
                        OriginalFileName = id.ToString(),
                        OriginalFilePath = id.ToString()
                    });
                    id++;
                }
            }

            return ret;
        }

        [TestMethod]
        public void TestInsertFile()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var testFiles = CreateTestFiles();

            foreach (var file in testFiles)
                adapter.InsertFile(file);
        }

        [TestMethod]
        public void TestGetFile()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var testFiles = CreateTestFiles();

            var gameFileIDs = testFiles.Select(x => x.GameFileID).Distinct();
            foreach(var id in gameFileIDs)
            {
                var dbFiles = adapter.GetFiles(new DoomLauncher.DataSources.GameFile() { GameFileID = id });
                Assert.IsTrue(testFiles.Count(x => x.GameFileID == id) == dbFiles.Count());
                
                foreach(var dbFile in dbFiles)
                    Assert.IsTrue(TestUtil.AllFieldsEqual(dbFile, testFiles.First(x => x.FileID == dbFile.FileID)));

                for(int i = 1; i < 4; i++)
                {
                    dbFiles = adapter.GetFiles(new DoomLauncher.DataSources.GameFile() { GameFileID = id }, (FileType)i);

                    foreach (var dbFile in dbFiles)
                        Assert.IsNotNull(testFiles.FirstOrDefault(x => x.FileID == dbFile.FileID && x.FileTypeID == (FileType)i));
                }
            }
        }

        [TestMethod]
        public void TestUpdateFile()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var files = adapter.GetFiles(new DoomLauncher.DataSources.GameFile() { GameFileID = 1 });

            foreach(var file in files)
            {
                //we only update these three fields
                file.SourcePortID += 100;
                file.Description += "100";
                file.FileOrder += 100;

                adapter.UpdateFile(file);
            }

            var newFiles = adapter.GetFiles(new GameFile() { GameFileID = 1 });

            foreach(var file in files)
            {
                var newFile = newFiles.FirstOrDefault(x => x.FileID == file.FileID);
                Assert.IsNotNull(newFile);
                Assert.IsTrue(TestUtil.AllFieldsEqual(file, newFile));
            }
        }

        [TestMethod]
        public void TestDeleteFile()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var gameFileIDs = CreateTestFiles().Select(x => x.GameFileID).Distinct();

            int id = gameFileIDs.First();

            var dbFiles = adapter.GetFiles(new GameFile() { GameFileID = id });
            int count = dbFiles.Count();
            foreach (var dbFile in dbFiles)
            {
                adapter.DeleteFile(dbFile);
                count--;
                Assert.AreEqual(count, adapter.GetFiles(new GameFile() { GameFileID = id }).Count());
            }


            id = gameFileIDs.Skip(1).First();
            adapter.DeleteFile(new GameFile() { GameFileID = id });
            Assert.AreEqual(0, adapter.GetFiles(new GameFile() { GameFileID = id }).Count());
        }
    }
}
