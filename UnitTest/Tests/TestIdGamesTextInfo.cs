using DoomLauncher.Handlers.Sync;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestIdGamesTextInfo
    {

        [TestMethod]
        public void TestQualityInfoCountsNonNullNonWhitespace()
        {
            var info1 = new IdGamesTextInfo(null, "  ", null, "", null);
            Assert.AreEqual(0, info1.QualityScore);

            var info2 = new IdGamesTextInfo("Eviternity", "", null, null, null);
            Assert.AreEqual(1, info2.QualityScore);

            var info3 = new IdGamesTextInfo("Jenesis", "James \"Jimmy\" Paddock", null, "   ", "    ");
            Assert.AreEqual(2, info3.QualityScore);

            var info4 = new IdGamesTextInfo("Pirate Doom!", "Arch", new DateTime(2014, 6, 18), "   ", "");
            Assert.AreEqual(3, info4.QualityScore);

            var info5 = new IdGamesTextInfo("My House", "Steve Nelson", new DateTime(2023, 3, 2), "Just a normal regular wad just hanging out", null);
            Assert.AreEqual(4, info5.QualityScore);
        }

        [TestMethod]
        public void NullStringsBecomeEmptyStrings()
        {
            var info = new IdGamesTextInfo(null, null, null, null, null);
            Assert.AreEqual("", info.Title);
            Assert.AreEqual("", info.Author);
            Assert.AreEqual("", info.Description);
        }

        [TestMethod]
        public void Combine_PrefersOurFields()
        {
            var info1 = new IdGamesTextInfo("Fancy Lands", "Rufus Wallykins", null, "Does not exist", "Doom 2");
            var info2 = new IdGamesTextInfo("ignoreme", "ignore me", new DateTime(2024, 5, 3), "IGNORE ME", "Doom");

            var result1 = info1.Combine(info2);
            Assert.AreEqual(new IdGamesTextInfo("Fancy Lands", "Rufus Wallykins", new DateTime(2024, 5, 3), "Does not exist", "Doom 2"), result1);

            var result2 = info2.Combine(info1);
            Assert.AreEqual(info2, result2);
        }
    }
}
