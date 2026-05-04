using DoomLauncher;
using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.IO;
using System.Linq;


namespace UnitTest.Tests
{
    [TestClass]
    public class TestFileHandler
    {
        private IDataSourceAdapter database;

        private readonly IDirectoriesConfiguration config = new DirectoriesConfiguration()
        {
            ScreenshotDirectory = new LauncherPath("Screenshots"),
            ThumbnailDirectory = new LauncherPath("Thumbnails"),
            TitlePicDirectory = new LauncherPath("TitlePics"),
            TileImageDirectory = new LauncherPath("TileImagesTest"),
            DemoDirectory = new LauncherPath("Demos"),
            SaveGameDirectory = new LauncherPath("SaveGames"),
        };

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
            Directory.CreateDirectory("Screenshots");
            Directory.CreateDirectory("Thumbnails");
            Directory.CreateDirectory("TitlePics");
            Directory.CreateDirectory("TileImagesTest");
            Directory.CreateDirectory("Demos");
            Directory.CreateDirectory("SaveGames");
            Directory.CreateDirectory("OriginalDir");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists("Screenshots"))
                Directory.Delete("Screenshots", true);

            if (Directory.Exists("Thumbnails"))
                Directory.Delete("Thumbnails", true);

            if (Directory.Exists("TitlePics"))
                Directory.Delete("TitlePics", true);

            if (Directory.Exists("Demos"))
                Directory.Delete("Demos", true);

            if (Directory.Exists("SaveGames"))
                Directory.Delete("SaveGames", true);

            if (Directory.Exists("TileImagesTest"))
                Directory.Delete("TileImagesTest", true);

            if (Directory.Exists("OriginalDir"))
                Directory.Delete("OriginalDir", true);

            if (File.Exists(@"Resources\happy_DELETE_ME.png"))
                File.Delete(@"Resources\happy_DELETE_ME.png");

            var dataAccess = ((DbDataSourceAdapter)database).DataAccess;
            dataAccess.ExecuteNonQuery("delete from GameFiles");
            dataAccess.ExecuteNonQuery("delete from Files");
        }

        [TestMethod]
        public void InsertAndSave_NullGameFileFails()
        {
            var fileHandler = new FileHandler(database, config);
            var image = Image.FromFile(@"Resources\happy.png");
            var fileData = fileHandler.InsertAndSave(null, FileType.Thumbnail, image, "png");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertAndSave_NullGameFileIdFails()
        {
            var fileHandler = new FileHandler(database, config);

            var gameFile = new GameFile()
            {
                GameFileID = null,
                FileName = "InsertAndSave_NullGameFileIdFails.zip",
            };

            var image = Image.FromFile(@"Resources\happy.png");
            var fileData = fileHandler.InsertAndSave(gameFile, FileType.Screenshot, image, "png");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertAndSave_CreatesFile()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndSave_CreatesFile.zip");
            var image = Image.FromFile(@"Resources\happy.png");

            var fileData = fileHandler.InsertAndSave(gameFile, FileType.Screenshot, image, "png");

            Assert.IsTrue(File.Exists(config.ScreenshotDirectory.GetFullPath(fileData.FileName)));
        }

        [TestMethod]
        public void InsertAndSave_CreatesDatabaseEntry()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndSave_CreatesDatabaseEntry.zip");
            var image = Image.FromFile(@"Resources\happy.png");

            var fileData = fileHandler.InsertAndSave(gameFile, FileType.Screenshot, image, "png");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();

            Assert.IsNotNull(fileData);
            Assert.IsNotNull(fileDataFromDB);
            Assert.AreEqual(fileData.FileName, fileDataFromDB.FileName);
        }

        [TestMethod]
        public void InsertAndSave_AddsFullFileNameToFileData()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndSave_AddsFullFileNameToFileData.zip");
            var image = Image.FromFile(@"Resources\happy.png");

            var fileData = fileHandler.InsertAndSave(gameFile, FileType.Screenshot, image, "png");
            var expectedFullName = config.ScreenshotDirectory.GetFullPath(fileData.FileName);

            Assert.AreEqual(expectedFullName, fileData.FullFileName);
        }

        [TestMethod]
        public void InsertAndSave_AppliesEdits()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndSave_AppliesEdits.zip");
            var image = Image.FromFile(@"Resources\happy.png");

            var fileData = fileHandler.InsertAndSave(gameFile, FileType.Screenshot, image, "png", x =>
            {
                x.Description = "Hello";
                x.SourcePortID = 444;
            });
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();


            Assert.AreEqual("Hello", fileData.Description);
            Assert.AreEqual(444, fileData.SourcePortID);
            Assert.AreEqual("Hello", fileDataFromDB.Description);
            Assert.AreEqual(444, fileDataFromDB.SourcePortID);
        }

        [TestMethod]
        public void InsertAndSave_HasNullOriginalFile()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndSave_AppliesEdits.zip");
            var image = Image.FromFile(@"Resources\happy.png");

            var fileData = fileHandler.InsertAndSave(gameFile, FileType.Screenshot, image, "png");

            Assert.IsNull(fileData.OriginalFileName);
        }

        [TestMethod]
        public void DeleteFile_DeletesFileOnDisk()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("DeleteFile_DeletesFileOnDisk.zip");
            var image = Image.FromFile(@"Resources\happy.png");

            var fileData = fileHandler.InsertAndSave(gameFile, FileType.Screenshot, image, "png");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();

            // We definitely inserted it
            Assert.IsTrue(File.Exists(config.ScreenshotDirectory.GetFullPath(fileData.FileName)));

            fileHandler.DeleteFile(fileDataFromDB);

            // We definitely deleted it
            Assert.IsFalse(File.Exists(config.ScreenshotDirectory.GetFullPath(fileData.FileName)));
        }

        [TestMethod]
        public void DeleteFile_DeletesDatabaseEntry()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("DeleteFile_DeletesDatabaseEntry.zip");
            var image = Image.FromFile(@"Resources\happy.png");

            var fileData = fileHandler.InsertAndSave(gameFile, FileType.Screenshot, image, "png");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();

            // We definitely inserted it in DB and on disk
            Assert.IsNotNull(fileDataFromDB);
            Assert.IsTrue(File.Exists($@"Screenshots\{fileDataFromDB.FileName}"));

            fileHandler.DeleteFile(fileDataFromDB);
            var deletedFileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();

            // We definitely deleted it in DB and on disk
            Assert.IsNull(deletedFileDataFromDB);
            Assert.IsFalse(File.Exists($@"Screenshots\{fileDataFromDB.FileName}"));
        }

        [TestMethod]
        public void DeleteFile_DoesntDeleteFixedContentOnDisk()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("DeleteFile_DoesntDeleteFixedContent.zip");
            var image = Image.FromFile(@"Resources\happy.png");

            var fileData = fileHandler.InsertAndSave(gameFile, FileType.TileImage, image, "png");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.TileImage).FirstOrDefault();

            // This test doesnt make sense unless TileImages are fixed content
            Assert.IsTrue(FileType.TileImage.IsFixedContent());

            // We definitely inserted it
            Assert.IsNotNull(fileDataFromDB);
            Assert.IsTrue(File.Exists($@"TileImagesTest\{fileDataFromDB.FileName}"));

            fileHandler.DeleteFile(fileDataFromDB);
            var deletedFileDataFromDB = database.GetFiles(gameFile, FileType.TileImage).FirstOrDefault();

            // We deleted the DB record...
            Assert.IsNull(deletedFileDataFromDB);

            //... but not the file on disk.
            Assert.IsTrue(File.Exists($@"TileImagesTest\{fileDataFromDB.FileName}"));
        }

        [TestMethod]
        public void DeleteFile_DeletesDerivedFilesToo()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("DeleteFile_DeletesDerivedFilesToo.zip");
            var image = Image.FromFile(@"Resources\happy.png");

            var fileData = fileHandler.InsertAndSave(gameFile, FileType.Screenshot, image, "png");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();

            var derivedFileData = fileHandler.InsertAndCopy(gameFile, FileType.Thumbnail, @"Resources\happy.png", file => 
                {
                    file.DerivedFromFileID = fileDataFromDB.FileID;
                });
            var derivedFileDataFromDB = database.GetFiles(gameFile, FileType.Thumbnail).FirstOrDefault();

            // We definitely inserted both in DB and on disk
            Assert.IsNotNull(fileDataFromDB);
            Assert.IsTrue(File.Exists($@"Screenshots\{fileDataFromDB.FileName}"));
            Assert.IsNotNull(derivedFileData);
            Assert.IsTrue(File.Exists($@"Thumbnails\{derivedFileDataFromDB.FileName}"));

            // Delete only the first one
            fileHandler.DeleteFile(fileDataFromDB);
            var deletedFileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();
            var derivedDeletedFileDataFromDB = database.GetFiles(gameFile, FileType.Thumbnail).FirstOrDefault();

            // We definitely deleted both in DB and on disk
            Assert.IsNull(deletedFileDataFromDB);
            Assert.IsFalse(File.Exists($@"Screenshots\{fileDataFromDB.FileName}"));
            Assert.IsNull(derivedDeletedFileDataFromDB);
            Assert.IsFalse(File.Exists($@"Thumbnails\{derivedFileDataFromDB.FileName}"));
        }

        [TestMethod]
        public void InsertAndCopy_InsertsDatabaseEntry()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndCopy_InsertsDatabaseEntry.zip");

            var fileData = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\happy.png");
            var filesInDB = database.GetFiles(gameFile).ToList();

            Assert.AreEqual(1, filesInDB.Count());
            Assert.AreEqual(fileData.FileName, filesInDB[0].FileName);
        }

        [TestMethod]
        public void InsertAndCopy_InsertsFileOnDisk()
        {
            var fileHandler = new FileHandler(database, config);

            // Nothing up my sleeve
            var existingTitlePics = Directory.EnumerateFiles(config.TitlePicDirectory.GetFullPath());
            Assert.IsFalse(existingTitlePics.Any());

            IGameFile gameFile = CreateSavedGameFile("InsertAndCopy_InsertsFileOnDisk.zip");

            // Insert a file as a TitlePic
            var fileData = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\happy.png");

            // Tada!
            existingTitlePics = Directory.EnumerateFiles(config.TitlePicDirectory.GetFullPath());
            Assert.AreEqual(1, existingTitlePics.Count());
            Assert.IsTrue(File.Exists(config.TitlePicDirectory.GetFullPath(fileData.FileName)));

            // Source image is still around
            Assert.IsTrue(File.Exists(@"Resources\happy.png"));
        }

        [TestMethod]
        public void InsertAndCopy_NullGameFileFails()
        {
            var fileHandler = new FileHandler(database, config);
            var fileData = fileHandler.InsertAndCopy(null, FileType.TitlePic, @"Resources\happy.png");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertAndCopy_NullGameFileIdFails()
        {
            var fileHandler = new FileHandler(database, config);

            var gameFile = new GameFile()
            {
                GameFileID = null,
                FileName = "InsertAndCopy_NullGameFileIdFails.zip",
            };

            var fileData = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\happy.png");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertAndCopy_AppliesEdits()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndCopy_AppliesEdits.zip");

            var fileData = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\happy.png", x =>
            {
                x.Description = "Hi";
                x.SourcePortID = 433;
            });
            var fileDataFromDB = database.GetFiles(gameFile, FileType.TitlePic).FirstOrDefault();


            Assert.AreEqual("Hi", fileData.Description);
            Assert.AreEqual(433, fileData.SourcePortID);
            Assert.AreEqual("Hi", fileDataFromDB.Description);
            Assert.AreEqual(433, fileDataFromDB.SourcePortID);
        }

        [TestMethod]
        public void InsertAndCopy_DoesntCopyFileThatIsntThere()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndCopy_DoesntCopyFileThatIsntThere.zip");

            var fileData = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\made-up-file.png");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.TitlePic).FirstOrDefault();

            Assert.IsNull(fileData);
            Assert.IsNull(fileDataFromDB);
            Assert.IsFalse(File.Exists(fileHandler.GetFullFileName(FileType.TitlePic, @"Resources\made-up-file.png")));
        }

        [TestMethod]
        public void InsertAndCopy_RemembersOriginalFile()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndCopy_RemembersOriginalFile.zip");

            var fileData = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\happy.png");

            Assert.IsNotNull(fileData);
            Assert.AreEqual("happy.png", fileData.OriginalFileName);
        }

        [TestMethod]
        public void InsertAndRefer_InsertsDatabaseEntry()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndRefer_InsertsDatabaseEntry.zip");

            var theRightLocation = config.TileImageDirectory.GetFullPath("happy.png");
            File.Copy(@"Resources\happy.png", theRightLocation);
            Assert.IsTrue(File.Exists(theRightLocation));

            var fileData = fileHandler.InsertAndRefer(gameFile, FileType.TileImage, theRightLocation);

            Assert.IsNotNull(fileData);
        }

        [TestMethod]
        public void InsertAndRefer_FailsIfNotFixedContent()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndRefer_FailsIfNotFixedContent.zip");

            var theRightLocation = config.TitlePicDirectory.GetFullPath("happy.png");
            File.Copy(@"Resources\happy.png", theRightLocation);
            Assert.IsTrue(File.Exists(theRightLocation));

            var fileData = fileHandler.InsertAndRefer(gameFile, FileType.TitlePic, theRightLocation);

            Assert.IsFalse(FileType.TitlePic.IsFixedContent());
            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertAndRefer_WontWorkWithAFileOutsideTheRightDirectory()
        {
            var fileHandler = new FileHandler(database, config);

            IGameFile gameFile = CreateSavedGameFile("InsertAndRefer_WontWorkWithAFileOutsideTheRightDirectory.zip");

            // The file exists
            Assert.IsTrue(File.Exists(@"Resources\happy.png"));

            var fileData = fileHandler.InsertAndRefer(gameFile, FileType.TileImage, @"Resources\happy.png");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertAndRefer_WontWorkWithAFileThatDoesntExist()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndRefer_InsertsDatabaseEntry.zip");

            Assert.IsFalse(File.Exists("made_up_file.txt"));

            var fileData = fileHandler.InsertAndRefer(gameFile, FileType.TitlePic, "made_up_file.txt");
            var fileDataInDB = database.GetFiles(gameFile, FileType.TitlePic).FirstOrDefault();

            Assert.IsNull(fileDataInDB);
        }

        [TestMethod]
        public void InsertAndRefer_NullGameFileFails()
        {
            var fileHandler = new FileHandler(database, config);

            var theRightLocation = config.TitlePicDirectory.GetFullPath("happy.png");
            File.Copy(@"Resources\happy.png", theRightLocation);
            Assert.IsTrue(File.Exists(theRightLocation));

            var fileData = fileHandler.InsertAndRefer(null, FileType.TitlePic, theRightLocation);

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertAndRefer_NullGameFileIdFails()
        {
            var fileHandler = new FileHandler(database, config);

            var gameFile = new GameFile()
            {
                GameFileID = null,
                FileName = "InsertAndRefer_NullGameFileIdFails.zip",
            };

            var theRightLocation = config.TitlePicDirectory.GetFullPath("happy.png");
            File.Copy(@"Resources\happy.png", theRightLocation);
            Assert.IsTrue(File.Exists(theRightLocation));

            var fileData = fileHandler.InsertAndRefer(gameFile, FileType.TitlePic, theRightLocation);

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertAndRefer_AppliesEdits()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndRefer_AppliesEdits.zip");

            var theRightLocation = config.TileImageDirectory.GetFullPath("happy.png");
            File.Copy(@"Resources\happy.png", theRightLocation);
            Assert.IsTrue(File.Exists(theRightLocation));

            var fileData = fileHandler.InsertAndRefer(gameFile, FileType.TileImage, theRightLocation, x =>
            {
                x.Description = "Hi";
                x.SourcePortID = 433;
            });
            var fileDataFromDB = database.GetFiles(gameFile, FileType.TileImage).FirstOrDefault();


            Assert.AreEqual("Hi", fileData.Description);
            Assert.AreEqual(433, fileData.SourcePortID);
            Assert.AreEqual("Hi", fileDataFromDB.Description);
            Assert.AreEqual(433, fileDataFromDB.SourcePortID);
        }

        [TestMethod]
        public void InsertAndRefer_RemembersOriginalFile()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndRefer_AppliesEdits.zip");

            var theRightLocation = config.TileImageDirectory.GetFullPath("happy.png");
            File.Copy(@"Resources\happy.png", theRightLocation);
            Assert.IsTrue(File.Exists(theRightLocation));

            var fileData = fileHandler.InsertAndRefer(gameFile, FileType.TileImage, theRightLocation);

            Assert.IsNotNull(fileData);
            Assert.AreEqual("happy.png", fileData.OriginalFileName);
        }

        [TestMethod]
        public void InsertAndMove_InsertsDatabaseEntry()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndMove_InsertsDatabaseEntry.zip");

            // We don't want to lose our normal copy!
            File.Copy(@"Resources\happy.png", @"Resources\happy_DELETE_ME.png");
            Assert.IsTrue(File.Exists(@"Resources\happy_DELETE_ME.png"));

            var fileData = fileHandler.InsertAndMove(gameFile, FileType.TitlePic, @"Resources\happy_DELETE_ME.png");
            var filesInDB = database.GetFiles(gameFile).ToList();

            Assert.AreEqual(1, filesInDB.Count());
            Assert.AreEqual(fileData.FileName, filesInDB[0].FileName);
        }

        [TestMethod]
        public void InsertAndMove_InsertsFileOnDiskAndDeletesOldFile()
        {
            var fileHandler = new FileHandler(database, config);

            // Nothing up my sleeve
            var existingTitlePics = Directory.EnumerateFiles(config.TitlePicDirectory.GetFullPath());
            Assert.IsFalse(existingTitlePics.Any());

            // We don't want to lose our normal copy!
            File.Copy(@"Resources\happy.png", @"Resources\happy_DELETE_ME.png");
            Assert.IsTrue(File.Exists(@"Resources\happy_DELETE_ME.png"));

            IGameFile gameFile = CreateSavedGameFile("InsertAndMove_InsertsFileOnDiskAndDeletesOldFile.zip");

            // Insert a file as a TitlePic
            var fileData = fileHandler.InsertAndMove(gameFile, FileType.TitlePic, @"Resources\happy_DELETE_ME.png");

            // Tada!
            existingTitlePics = Directory.EnumerateFiles(config.TitlePicDirectory.GetFullPath());
            Assert.AreEqual(1, existingTitlePics.Count());
            Assert.IsTrue(File.Exists(config.TitlePicDirectory.GetFullPath(fileData.FileName)));

            // Source image is gone now
            Assert.IsFalse(File.Exists(@"Resources\happy_DELETE_ME.png"));
        }

        [TestMethod]
        public void InsertAndMove_NullGameFileFails()
        {
            var fileHandler = new FileHandler(database, config);
            File.Copy(@"Resources\happy.png", @"Resources\happy_DELETE_ME.png");
            var fileData = fileHandler.InsertAndMove(null, FileType.TitlePic, @"Resources\happy_DELETE_ME.png");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertAndMove_NullGameFileIdFails()
        {
            var fileHandler = new FileHandler(database, config);

            var gameFile = new GameFile()
            {
                GameFileID = null,
                FileName = "InsertAndMove_NullGameFileIdFails.zip",
            };

            File.Copy(@"Resources\happy.png", @"Resources\happy_DELETE_ME.png");
            var fileData = fileHandler.InsertAndMove(gameFile, FileType.TitlePic, @"Resources\happy_DELETE_ME.png");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertAndMove_AppliesEdits()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndMove_AppliesEdits.zip");
            File.Copy(@"Resources\happy.png", @"Resources\happy_DELETE_ME.png");

            var fileData = fileHandler.InsertAndMove(gameFile, FileType.TitlePic, @"Resources\happy_DELETE_ME.png", x =>
            {
                x.Description = "good";
                x.SourcePortID = 665;
            });
            var fileDataFromDB = database.GetFiles(gameFile, FileType.TitlePic).FirstOrDefault();


            Assert.AreEqual("good", fileData.Description);
            Assert.AreEqual(665, fileData.SourcePortID);
            Assert.AreEqual("good", fileDataFromDB.Description);
            Assert.AreEqual(665, fileDataFromDB.SourcePortID);
        }

        [TestMethod]
        public void InsertAndMove_DoesntMoveFileThatIsntThere()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndMove_DoesntMoveFileThatIsntThere.zip");

            var fileData = fileHandler.InsertAndMove(gameFile, FileType.Demo, @"Resources\made-up-file.demo");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Demo).FirstOrDefault();

            Assert.IsNull(fileData);
            Assert.IsNull(fileDataFromDB);
            Assert.IsFalse(File.Exists(fileHandler.GetFullFileName(FileType.Demo, @"Resources\made-up-file.demo")));
        }

        [TestMethod]
        public void InsertAndMove_RemembersOriginalFile()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndMove_RemembersOriginalFile.zip");
            File.Copy(@"Resources\happy.png", @"Resources\happy_DELETE_ME.png");

            var fileData = fileHandler.InsertAndMove(gameFile, FileType.TitlePic, @"Resources\happy_DELETE_ME.png");

            Assert.IsNotNull(fileData);
            Assert.AreEqual("happy_DELETE_ME.png", fileData.OriginalFileName);
        }

        [TestMethod]
        public void UpdateFromOriginal_DoesNothingIfOriginalFileDoesntExist()
        {
            var fileHandler = new FileHandler(database, config);

            // Set up original file
            File.Copy(@"Resources\happy.png", @"OriginalDir\happy.png");
            Assert.IsTrue(File.Exists(@"OriginalDir\happy.png"));

            // Insert a copy
            IGameFile gameFile = CreateSavedGameFile("UpdateFromOriginal_DoesNothingIfOriginalFileDoesntExist.zip");
            var file = fileHandler.InsertAndCopy(gameFile, FileType.Demo, @"OriginalDir\happy.png");
            var localWriteTime = new FileInfo(file.FullFileName).LastWriteTime;

            // Touch the original file so we know if it was copied or not
            var originalFileInfo = TestUtil.TouchFile(@"OriginalDir\happy.png");

            // Delete original file
            originalFileInfo.Delete();

            fileHandler.UpdateFromOriginal("OriginalDir", file, f =>
            {
                f.Description = "shouldn't happen";
            });

            // Local file should the same write date as before
            var newLocalTime = new FileInfo(file.FullFileName).LastWriteTime;
            Assert.AreEqual(localWriteTime, newLocalTime);
            Assert.IsNull(file.Description);
        }

        [TestMethod]
        public void UpdateFromOriginal_DoesNothingIfOriginalFileIsOlderThanLocal()
        {
            var fileHandler = new FileHandler(database, config);

            // Set up original file
            File.Copy(@"Resources\happy.png", @"OriginalDir\happy.png");
            Assert.IsTrue(File.Exists(@"OriginalDir\happy.png"));

            // Insert a copy
            IGameFile gameFile = CreateSavedGameFile("UpdateFromOriginal_DoesNothingIfOriginalFileIsOlderThanLocal.zip");
            var file = fileHandler.InsertAndCopy(gameFile, FileType.Demo, @"OriginalDir\happy.png");
            var localWriteTime = new FileInfo(file.FullFileName).LastWriteTime;

            // Touch the original file so we know if it was copied or not
            var originalFileInfo = TestUtil.TouchFile(@"OriginalDir\happy.png");

            // Make the local file look like it's more up to date than the original
            file.DateCreated = file.DateCreated.AddDays(3);
            fileHandler.UpdateFromOriginal("OriginalDir", file);

            // Local file should the same write date as before
            var newLocalTime = new FileInfo(file.FullFileName).LastWriteTime;
            Assert.AreEqual(localWriteTime, newLocalTime);
        }

        [TestMethod]
        public void UpdateFromOriginal_CopiesOriginalFileToLocal()
        {
            var fileHandler = new FileHandler(database, config);

            // Set up original file
            File.Copy(@"Resources\happy.png", @"OriginalDir\happy.png");
            Assert.IsTrue(File.Exists(@"OriginalDir\happy.png"));

            // Insert a copy
            IGameFile gameFile = CreateSavedGameFile("UpdateFromOriginal_CopiesOriginalFileToLocal.zip");
            var file = fileHandler.InsertAndCopy(gameFile, FileType.Demo, @"OriginalDir\happy.png");
            var localWriteTime = new FileInfo(file.FullFileName).LastWriteTime;

            // Touch the original file so we know if it was copied or not
            var originalFileInfo = TestUtil.TouchFile(@"OriginalDir\happy.png");

            fileHandler.UpdateFromOriginal("OriginalDir", file);

            // Local file should have a newer write date now
            var newLocalTime = new FileInfo(file.FullFileName).LastWriteTime;
            Assert.IsTrue(newLocalTime > localWriteTime);
        }

        [TestMethod]
        public void UpdateFromOriginal_UpdatesDatabase()
        {
            var fileHandler = new FileHandler(database, config);

            File.Copy(@"Resources\happy.png", @"OriginalDir\happy.png");
            Assert.IsTrue(File.Exists(@"OriginalDir\happy.png"));

            IGameFile gameFile = CreateSavedGameFile("UpdateFromOriginal_UpdatesDatabase.zip");
            var file = fileHandler.InsertAndCopy(gameFile, FileType.Demo, @"OriginalDir\happy.png");
            var localWriteTime = new FileInfo(file.FullFileName).LastWriteTime;

            // Touch the original file so we know if it was copied or not
            var originalFileInfo = TestUtil.TouchFile(@"OriginalDir\happy.png");

            fileHandler.UpdateFromOriginal("OriginalDir", file, f =>
            {
                f.Description = "updated detail";
            });

            var fileFromDB = fileHandler.GetFiles(gameFile, FileType.Demo).First();
            Assert.AreEqual("updated detail", fileFromDB.Description);
        }

        [TestMethod]
        public void UpdateFromOriginal_SetsDateCreatedToOriginalFileLastWrite()
        {
            var fileHandler = new FileHandler(database, config);

            File.Copy(@"Resources\happy.png", @"OriginalDir\happy.png");
            Assert.IsTrue(File.Exists(@"OriginalDir\happy.png"));

            IGameFile gameFile = CreateSavedGameFile("UpdateFromOriginal_SetsDateCreatedToOriginalFileLastWrite.zip");
            var file = fileHandler.InsertAndCopy(gameFile, FileType.Demo, @"OriginalDir\happy.png");
            var localWriteTime = new FileInfo(file.FullFileName).LastWriteTime;

            // Touch the original file so we know if it was copied or not
            var originalFileInfo = TestUtil.TouchFile(@"OriginalDir\happy.png");

            fileHandler.UpdateFromOriginal("OriginalDir", file);

            Assert.AreEqual(originalFileInfo.LastWriteTime, file.DateCreated);
        }

        class UrlFileData : FileData
        {
            public override bool IsUrl { get { return true; } }
        }

        [TestMethod]
        public void DeleteFile_DoesNothingIfFileIsUrl()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("DeleteFile_DoesNothingIfFileIsUrl.zip");

            var file = fileHandler.InsertAndCopy(gameFile, FileType.Thumbnail, @"Resources\happy.png");

            var urlFileData = new UrlFileData()
            {
                FileID = file.FileID,
                GameFileID = file.GameFileID,
                FileName = file.FileName,
                FileTypeID = file.FileTypeID,
                SourcePortID = file.SourcePortID
            };

            fileHandler.DeleteFile(urlFileData);

            var thumbnails = database.GetFiles(gameFile, FileType.Thumbnail);
            Assert.AreEqual(1, thumbnails.Count());
        }

        [TestMethod]
        public void DeleteFiles_DeletesAttachedFilesOfTheGivenType()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("DeleteFiles_DeletesAttachedFilesOfTheGivenType.zip");

            // 1 thumbnail, 2 screenshots
            var wrong = fileHandler.InsertAndCopy(gameFile, FileType.Thumbnail, @"Resources\happy.png");
            var right1 = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");
            var right2 = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");

            // Yep they're here
            var thumbnails = database.GetFiles(gameFile, FileType.Thumbnail);
            var screenshots = database.GetFiles(gameFile, FileType.Screenshot);

            Assert.AreEqual(1, thumbnails.Count());
            Assert.AreEqual(2, screenshots.Count());

            // Delete screenshots
            fileHandler.DeleteFiles(gameFile, FileType.Screenshot);

            // Thumbnail is still here, screenshots are gone now
            thumbnails = database.GetFiles(gameFile, FileType.Thumbnail);
            screenshots = database.GetFiles(gameFile, FileType.Screenshot);

            Assert.AreEqual(1, thumbnails.Count());
            Assert.AreEqual(0, screenshots.Count());
        }

        [TestMethod]
        public void DeleteFiles_DeletesAllAttachedFilesIfNoFileTypeGiven()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("DeleteFiles_DeletesAllAttachedFilesIfNoFileTypeGiven.zip");

            var file1 = fileHandler.InsertAndCopy(gameFile, FileType.Thumbnail, @"Resources\happy.png");
            var file2 = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\happy.png");
            var file3 = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");
            var file4 = fileHandler.InsertAndCopy(gameFile, FileType.Demo, @"Resources\happy.png");
            var file5 = fileHandler.InsertAndCopy(gameFile, FileType.SaveGame, @"Resources\happy.png");

            // Yep they're here
            var allFiles = database.GetFiles(gameFile);
            Assert.AreEqual(5, allFiles.Count());

            // Delete all
            fileHandler.DeleteFiles(gameFile);

            // Thumbnail is still here, screenshots are gone now
            allFiles = database.GetFiles(gameFile);

            Assert.AreEqual(0, allFiles.Count());
        }

        [TestMethod]
        public void GetFiles_ReturnsFilesFromTheDatabase()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("GetFiles_ReturnsFilesFromTheDatabase.zip");

            // 1 thumbnail, 2 screenshots
            var wrong = fileHandler.InsertAndCopy(gameFile, FileType.Thumbnail, @"Resources\happy.png");
            var right1 = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");
            var right2 = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");

            var fileDataList = fileHandler.GetFiles(gameFile, FileType.Screenshot);
            Assert.AreEqual(2, fileDataList.Count());
        }

        [TestMethod]
        public void GetFiles_ReturnsFilesWithMultiplesTypesFromTheDatabaseInOrder()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("GetFiles_ReturnsFilesFromTheDatabase.zip");

            var wrong = fileHandler.InsertAndCopy(gameFile, FileType.Thumbnail, @"Resources\happy.png");
            var screenshot = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");
            var demo = fileHandler.InsertAndCopy(gameFile, FileType.Demo, @"Resources\happy.png");
            var titlePic = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\happy.png");

            var fileDataList = fileHandler.GetFiles(gameFile, FileType.Demo, FileType.TitlePic, FileType.Screenshot);
            Assert.AreEqual(3, fileDataList.Count());
            Assert.AreEqual(demo.FileID, fileDataList[0].FileID);
            Assert.AreEqual(titlePic.FileID, fileDataList[1].FileID);
            Assert.AreEqual(screenshot.FileID, fileDataList[2].FileID);
        }

        private IGameFile CreateSavedGameFile(string fileName)
        {
            IGameFile gameFile = new GameFile() { FileName = fileName };
            database.InsertGameFile(gameFile);
            return gameFile;
        }
    }
}
