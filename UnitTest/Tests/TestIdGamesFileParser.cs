using DoomLauncher.Handlers.Sync;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestIdGamesFileParser
    {
        private static string[] s_formats = new string[] { "dd/M/yy", "dd/MM/yyyy", "dd MMMM yyyy" };

        [TestMethod]
        public void TestEmpty()
        {
            IdGamesTextFileParser parser = new IdGamesTextFileParser(s_formats);
            var info = parser.Parse(string.Empty);

            Assert.AreEqual(string.Empty, info.Title);
            Assert.AreEqual(string.Empty, info.Author);
            Assert.AreEqual(null, info.ReleaseDate);
            Assert.AreEqual(string.Empty, info.Description);
        }

        [TestMethod]
        public void TestBadDates()
        {
            IdGamesTextFileParser parser = new IdGamesTextFileParser(s_formats);

            var info = parser.Parse("Release date: this is garbage");
            Assert.AreEqual(null, info.ReleaseDate);

            parser.Parse("Release date: ");
            Assert.AreEqual(null, info.ReleaseDate);

            parser.Parse("Release date:");
            Assert.AreEqual(null, info.ReleaseDate);

            parser.Parse("Release date: 1234");
            Assert.AreEqual(null, info.ReleaseDate);
        }

        [TestMethod]
        public void TestDates()
        {
            IdGamesTextFileParser parser = new IdGamesTextFileParser(s_formats);

            string[] dates = new string[]
            {
                "Release date : 5/April/16",
                "Release date:4/05/2016",
                " Release date: 4.05.2016",
                " Release date: 4 05 2016",
                "Date finished: 5 April 2016",
                "release Date: April 5th 2016",
                "Release date: April 5th, 2016",
                "Release date: April 5th 16",
                "Release date: junk 5 April 2016 junk",
            };

            DateTime assert = DateTime.Parse("4/5/2016", CultureInfo.InvariantCulture);

            foreach (string date in dates)
            {
                var info = parser.Parse(date);
                Assert.AreEqual(info.ReleaseDate, assert);
            }
        }

        [TestMethod]
        public void TestStrings()
        {
            string test = @"===========================================================================
                        Advanced engine needed  : PrBoom+ or GZdoom.
                        Primary purpose         : Deathmatch
                        ===========================================================================
                        TitLe                   : Onslaught DM 3 (v.1.1)
                        Filename                : onsl3.wad
                        Release DATE            : 01/17/07
                        AUTHORS                 : Hobomaster22, Ak-01
                        dEsCrIpTioN             : 21 head to head deathmatch maps with faced paced action.  This is a 1on1 specific mapset. For FFA more than 4 players is not recommended.";

            IdGamesTextFileParser parser = new IdGamesTextFileParser(s_formats);
            var info = parser.Parse(test);

            Assert.AreEqual("Onslaught DM 3 (v.1.1)", info.Title);
            Assert.AreEqual(DateTime.Parse("01/17/07", CultureInfo.InvariantCulture), info.ReleaseDate);
            Assert.AreEqual("Hobomaster22, Ak-01", info.Author); //Could be Author: or Authors:
            Assert.AreEqual("21 head to head deathmatch maps with faced paced action.  This is a 1on1 specific mapset. For FFA more than 4 players is not recommended.", info.Description);

            test = test.Replace("AUTHORS", "AUTHOR");
            parser.Parse(test);
            Assert.AreEqual("Onslaught DM 3 (v.1.1)", info.Title);
            Assert.AreEqual(DateTime.Parse("01/17/07", CultureInfo.InvariantCulture), info.ReleaseDate);
            Assert.AreEqual("Hobomaster22, Ak-01", info.Author); //Could be Author: or Authors:
            Assert.AreEqual("21 head to head deathmatch maps with faced paced action.  This is a 1on1 specific mapset. For FFA more than 4 players is not recommended.", info.Description);

            test = @"===========================================================================
                    Primary purpose         : Deathmatch
                    ===========================================================================
                    TitLe                   : 
                    Filename                : 
                    Release DATE            : 
                    AUTHORS                 : 
                    dEsCrIpTioN             : ";
            info = parser.Parse(test);
            Assert.AreEqual(string.Empty, info.Title);
            Assert.AreEqual(null, info.ReleaseDate);
            Assert.AreEqual(string.Empty, info.Author);
            Assert.AreEqual(string.Empty, info.Description);
        }
    }
}
