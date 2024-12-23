using DoomLauncher.Adapters.Launch;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestLaunchParameters
    {
        [TestMethod]
        public void Combine_AppendsTwoParams()
        {
            var a = LaunchParameters.Param("a");
            var b = LaunchParameters.Param("b");

            var result = a.Combine(b);
            Assert.AreEqual("a b", result.LaunchString);
        }

        [TestMethod]
        public void Combine_EmptyHasNoEffect()
        {
            var a = LaunchParameters.Param("blah");

            Assert.AreEqual("blah", a.Combine(LaunchParameters.EMPTY).LaunchString);
            Assert.AreEqual("blah", LaunchParameters.EMPTY.Combine(a).LaunchString);
        }

        [TestMethod]
        public void Combine_TakesTheFirstRecordedFile()
        {
            var a = LaunchParameters.EMPTY.WithRecordedFileName("recording.file");
            var b = LaunchParameters.EMPTY.WithRecordedFileName("other.file");

            var result = a.Combine(b);

            Assert.AreEqual("recording.file", result.RecordedFileName);
        }

        [TestMethod]
        public void Combine_TakesRecordedFileIfNotAlreadyPresent()
        {
            var a = LaunchParameters.Param("p1");
            var b = LaunchParameters.EMPTY.WithRecordedFileName("therecording.file");

            var result = a.Combine(b);

            Assert.AreEqual("therecording.file", result.RecordedFileName);
        }

        [TestMethod]
        public void Combine_ExclusiveParameterIgnoresOtherParameters()
        {
            var a = LaunchParameters.Param("a");
            var b = LaunchParameters.Param("b");
            var c = LaunchParameters.ExclusiveParam("c");
            var d = LaunchParameters.Param("d");

            var result = a.Combine(b).Combine(c).Combine(d);
            Assert.AreEqual("c", result.LaunchString);
        }

        [TestMethod]
        public void Combine_KeepsTheFailure()
        {
            var failed1 = LaunchParameters.Failure("WRONG!");
            var failed2 = LaunchParameters.Failure("NO!");
            var ok = LaunchParameters.Param("ok");

            var result1 = failed1.Combine(ok);
            Assert.IsTrue(result1.Failed);
            Assert.AreEqual("WRONG!", result1.ErrorMessage);

            var result2 = ok.Combine(failed2);
            Assert.IsTrue(result2.Failed);
            Assert.AreEqual("NO!", result2.ErrorMessage);
        }

        [TestMethod]
        public void Combine_StacksVariableReplacements()
        {
            var param1 = LaunchParameters.EMPTY.WithVariableReplacement("filename", "bongo.wad");
            var param2 = LaunchParameters.EMPTY.WithVariableReplacement("iwad", "freedoom.wad");
            var appliedParam = LaunchParameters.Param("$somethingElse $filename needs $iwad");
            var result1 = appliedParam.Combine(param1).Combine(param2);
            var result2 = param2.Combine(param1).Combine(appliedParam);
            var result3 = param1.Combine(appliedParam).Combine(param2);

            Assert.AreEqual("$somethingElse bongo.wad needs freedoom.wad", result1.LaunchString);
            Assert.AreEqual("$somethingElse bongo.wad needs freedoom.wad", result2.LaunchString);
            Assert.AreEqual("$somethingElse bongo.wad needs freedoom.wad", result3.LaunchString);
        }
    }
}
