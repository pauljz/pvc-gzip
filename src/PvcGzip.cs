using PvcCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvcPlugins
{
    public class PvcGzip : PvcPlugin
    {
        private bool addExtension;
        private CompressionLevel compressionLevel;

        private static byte[] gzipSignature = new byte[3] { 0x1f, 0x8b, 0x08 };

        public PvcGzip(
            bool addExtension = true,
            CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            this.addExtension = addExtension;
            this.compressionLevel = compressionLevel;
        }

        public static bool IsGzipStream(PvcStream stream)
        {
            var bytes = new byte[3];
            stream.Read(bytes, 0, 3);
            stream.ResetStreamPosition();
            return bytes.SequenceEqual(gzipSignature);
        }

        public override IEnumerable<PvcStream> Execute(IEnumerable<PvcStream> inputStreams)
        {
            var outputStreams = new List<PvcStream>();

            foreach (var inputStream in inputStreams)
            {
                if (PvcGzip.IsGzipStream(inputStream))
                {
                    // It's already a Gzip file, skip it.
                    outputStreams.Add(inputStream);
                    continue;
                }

                var streamName = inputStream.StreamName;
                if (addExtension)
                    streamName += ".gz";
                
                var ms = new MemoryStream();
                using (var gzs = new GZipStream(ms, this.compressionLevel, true))
                {
                    inputStream.CopyTo(gzs);
                }

                var outputStream = new PvcStream(() => ms)
                    .As(streamName, inputStream.OriginalSourcePath);
                outputStream.Tags = inputStream.Tags;
                outputStream.Tags.Add("gzip");

                outputStreams.Add(outputStream);
            };

            return outputStreams;
        }
    }
}
