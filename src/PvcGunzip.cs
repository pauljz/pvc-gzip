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
    public class PvcGunzip : PvcPlugin
    {
        public override IEnumerable<PvcStream> Execute(IEnumerable<PvcStream> inputStreams)
        {
            var outputStreams = new List<PvcStream>();
            foreach (var inputStream in inputStreams)
            {
                if (!PvcGzip.IsGzipStream(inputStream))
                {
                    //  It's not a Gzip file. Skip it.
                    outputStreams.Add(inputStream);
                    continue;
                }

                var streamName = inputStream.StreamName.Replace(".gz", "");

                var ms = new MemoryStream();
                using(var gzs = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    gzs.CopyTo(ms);
                }

                var outputStream = new PvcStream(() => ms)
                    .As(streamName, inputStream.OriginalSourcePath);
                outputStream.Tags = inputStream.Tags;
                outputStream.Tags.Remove("gzip");

                outputStreams.Add(outputStream);
            };

            return outputStreams;
        }
    }
}
