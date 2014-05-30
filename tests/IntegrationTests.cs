using PvcCore;
using PvcPlugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PvcPlugins.GzipTests
{
    public class IntegrationTests
    {
        [Fact]
        public void TestRoundTrip()
        {
            var startText = "start text";

            var inputStream = PvcUtil.StringToStream(startText, "file.txt");
            var inputStreams = new List<PvcStream>();
            inputStreams.Add(inputStream);
            var gzippedStreams = new PvcPlugins.PvcGzip().Execute(inputStreams);
            var gzippedStream = gzippedStreams.Last();
            Assert.Equal(gzippedStreams.Count(), 1);
            Assert.Contains("gzip", gzippedStream.Tags);
            Assert.EndsWith(".gz", gzippedStream.StreamName);
            Assert.True(PvcGzip.IsGzipStream(gzippedStream));

            gzippedStream.ResetStreamPosition();
            var outputStreams = new PvcPlugins.PvcGunzip().Execute(gzippedStreams);
            var outputStream = outputStreams.Last();
            Assert.Equal(outputStreams.Count(), 1);
            Assert.DoesNotContain("gzip", outputStream.Tags);
            Assert.EndsWith(".txt", outputStream.StreamName);
            Assert.True(!PvcGzip.IsGzipStream(outputStream));
            Assert.Equal(outputStream.ToString(), startText);
        }

        [Fact]
        public void TestAddExtension()
        {
            var startText = "start text";
            var inputStream = PvcUtil.StringToStream(startText, "file.txt");
            var inputStreams = new List<PvcStream>();
            inputStreams.Add(inputStream);
            var gzippedStreams = new PvcPlugins.PvcGzip(addExtension: false).Execute(inputStreams);
            var gzippedStream = gzippedStreams.Last();
            Assert.EndsWith(".txt", gzippedStream.StreamName);
        }
    }
}
