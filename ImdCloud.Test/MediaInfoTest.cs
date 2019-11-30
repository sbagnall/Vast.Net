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
            var mediaInfo = new MediaInfo(testFile);
            
            var actual = mediaInfo.GetMediaInfo();

            Assert.AreEqual(22.805f, actual.DurationSeconds);
            Assert.AreEqual(23.976f, actual.FrameRate);
            Assert.AreEqual(546, actual.FrameCount);
        }

        [Test]
        public void When_the_command_errors()
        {
            var mediaInfo = new MediaInfo("not valid");

            Assert.Catch<Exception>(() => mediaInfo.GetMediaInfo());
        }
    }
}
