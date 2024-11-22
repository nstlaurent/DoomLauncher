using DoomLauncher.Adapters.Launch;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestLaunchResult
    {
        [TestMethod]
        public void Combine_AppendsTwoParams()
        {
            var a = LaunchParameters.Param("a");
            var b = LaunchParameters.Param("b");

            var result = a.Combine(b);
            Assert.AreEqual("a b", result.ParamString);
        }

        [TestMethod]
        public void Combine_EmptyHasNoEffect()
        {
            var a = LaunchParameters.Param("blah");

            Assert.AreEqual("blah", a.Combine(LaunchParameters.EMPTY).ParamString);
            Assert.AreEqual("blah", LaunchParameters.EMPTY.Combine(a).ParamString);
        }

        [TestMethod]
        public void Combine_TakesTheFirstRecordedFile()
        {
            var a = LaunchParameters.ParamWithRecording("param1", "recording.file");
            var b = LaunchParameters.ParamWithRecording("param2", "other.file");

            var result = a.Combine(b);

            Assert.AreEqual("recording.file", result.RecordedFileName);
        }

        [TestMethod]
        public void Combine_TakesRecordedFileIfNotAlreadyPresent()
        {
            var a = LaunchParameters.Param("p1");
            var b = LaunchParameters.ParamWithRecording("p2", "therecording.file");

            var result = a.Combine(b);

            Assert.AreEqual("therecording.file", result.RecordedFileName);
        }

        [TestMethod]
        public void Combine_FinalParameterStopsProcessing()
        {
            var a = LaunchParameters.Param("a");
            var b = LaunchParameters.Param("b");
            var c = LaunchParameters.FinalParam("c");
            var d = LaunchParameters.Param("d");

            var result = a.Combine(b).Combine(c).Combine(d);
            Assert.AreEqual("a b c", result.ParamString);
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
    }
}
