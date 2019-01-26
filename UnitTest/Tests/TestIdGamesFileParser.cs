using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DoomLauncher.TextFileParsers;

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
            parser.Parse(string.Empty);

            Assert.AreEqual(string.Empty, parser.Title);
            Assert.AreEqual(string.Empty, parser.Author);
            Assert.AreEqual(null, parser.ReleaseDate);
            Assert.AreEqual(string.Empty, parser.Description);
        }

        [TestMethod]
        public void TestBadDates()
        {
            IdGamesTextFileParser parser = new IdGamesTextFileParser(s_formats);

            parser.Parse("Release date: this is garbage");
            Assert.AreEqual(null, parser.ReleaseDate);

            parser.Parse("Release date: ");
            Assert.AreEqual(null, parser.ReleaseDate);

            parser.Parse("Release date:");
            Assert.AreEqual(null, parser.ReleaseDate);

            parser.Parse("Release date: 1234");
            Assert.AreEqual(null, parser.ReleaseDate);
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

            DateTime assert = DateTime.Parse("4/5/2016");

            foreach (string date in dates)
            {
                parser.Parse(date);
                Assert.AreEqual(parser.ReleaseDate, assert);
            }
        }

        [TestMethod]
        public void TestStrings()
        {
            string test = @"===========================================================================
                        Primary purpose         : Deathmatch
                        ===========================================================================
                        TitLe                   : Onslaught DM 3 (v.1.1)
                        Filename                : onsl3.wad
                        Release DATE            : 01/17/07
                        AUTHORS                 : Hobomaster22, Ak-01
                        dEsCrIpTioN             : 21 head to head deathmatch maps with faced paced action.  This is a 1on1 specific mapset. For FFA more than 4 players is not recommended.";

            IdGamesTextFileParser parser = new IdGamesTextFileParser(s_formats);
            parser.Parse(test);

            Assert.AreEqual("Onslaught DM 3 (v.1.1)", parser.Title);
            Assert.AreEqual(DateTime.Parse("01/17/07"), parser.ReleaseDate);
            Assert.AreEqual("Hobomaster22, Ak-01", parser.Author); //Could be Author: or Authors:
            Assert.AreEqual("21 head to head deathmatch maps with faced paced action.  This is a 1on1 specific mapset. For FFA more than 4 players is not recommended.", parser.Description);

            test = test.Replace("AUTHORS", "AUTHOR");
            parser.Parse(test);
            Assert.AreEqual("Onslaught DM 3 (v.1.1)", parser.Title);
            Assert.AreEqual(DateTime.Parse("01/17/07"), parser.ReleaseDate);
            Assert.AreEqual("Hobomaster22, Ak-01", parser.Author); //Could be Author: or Authors:
            Assert.AreEqual("21 head to head deathmatch maps with faced paced action.  This is a 1on1 specific mapset. For FFA more than 4 players is not recommended.", parser.Description);

            test = @"===========================================================================
                    Primary purpose         : Deathmatch
                    ===========================================================================
                    TitLe                   : 
                    Filename                : 
                    Release DATE            : 
                    AUTHORS                 : 
                    dEsCrIpTioN             : ";
            parser.Parse(test);
            Assert.AreEqual(string.Empty, parser.Title);
            Assert.AreEqual(null, parser.ReleaseDate);
            Assert.AreEqual(string.Empty, parser.Author);
            Assert.AreEqual(string.Empty, parser.Description);
        }
    }
}
