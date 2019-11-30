using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

namespace ImdCloud.Test
{
    [TestFixture]
    public class MediaInfoTest
    {
        private static string testFile = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
            @"media\1493668191.mp4");

        [Test]
        public void When_the_command_succeeds()
        {
            var actual = MediaInfo.GetMediaInfo(testFile);

            Assert.AreEqual(22.805f, actual.DurationSeconds);
            Assert.AreEqual(23.976f, actual.FrameRate);
            Assert.AreEqual(546, actual.FrameCount);
        }

        [Test]
        public void When_the_command_errors_it_throws_an_exception()
        {
            Assert.Catch<Exception>(() => MediaInfo.GetMediaInfo("not valid"));
        }

        [Test]
        public void It_runs_the_command_once_and_caches_the_result()
        {
            MediaInfo.GetMediaInfo(testFile);

            Assert.IsTrue(MediaInfo.CachedResults.ContainsKey(testFile));
            Assert.AreEqual(22.805f, MediaInfo.CachedResults[testFile].DurationSeconds);
            Assert.AreEqual(23.976f, MediaInfo.CachedResults[testFile].FrameRate);
            Assert.AreEqual(546, MediaInfo.CachedResults[testFile].FrameCount);
        }
    }
}
