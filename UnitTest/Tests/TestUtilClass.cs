using DoomLauncher;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace UnitTest.Tests
{
    [TestClass]
    public class TestUtilClass
    {

        [TestMethod]
        public void ApproxEquals_RespectsDefaultTolerance()
        {
            Assert.IsTrue(5.0.ApproxEquals(5.01));
            Assert.IsTrue(5.0.ApproxEquals(4.999));
            Assert.IsFalse(5.0.ApproxEquals(9.666));
            Assert.IsFalse(5.0.ApproxEquals(-5.0));
        }

        [TestMethod]
        public void ApproxEquals_RespectsSelectedTolerance()
        {
            Assert.IsTrue(5.0.ApproxEquals(5.19, 0.2));
            Assert.IsTrue(5.0.ApproxEquals(4.85, 0.2));
            Assert.IsFalse(5.0.ApproxEquals(5.21, 0.2));
            Assert.IsFalse(5.0.ApproxEquals(-5.1, 0.2));
        }

        [TestMethod]
        public void ApproxEquals_RespectsDefaultToleranceF()
        {
            Assert.IsTrue(5.0f.ApproxEquals(5.009f));
            Assert.IsTrue(5.0f.ApproxEquals(4.999f));
            Assert.IsFalse(5.0f.ApproxEquals(9.666f));
            Assert.IsFalse(5.0f.ApproxEquals(-5.0f));
        }

        [TestMethod]
        public void ApproxEquals_RespectsSelectedToleranceF()
        {
            Assert.IsTrue(5.0f.ApproxEquals(5.19f, 0.2f));
            Assert.IsTrue(5.0f.ApproxEquals(4.85f, 0.2f));
            Assert.IsFalse(5.0f.ApproxEquals(5.21f, 0.2f));
            Assert.IsFalse(5.0f.ApproxEquals(-5.1f, 0.2f));
        }

        [TestMethod]
        public void ApproxEquals_ZeroToleranceIsEquals()
        {
            Assert.IsTrue(5.0.ApproxEquals(5.0, 0.0));
            Assert.IsFalse(5.0.ApproxEquals(5.001, 0.0));
            Assert.IsFalse(5.0.ApproxEquals(4.999, 0.0));
        }
    }
}
