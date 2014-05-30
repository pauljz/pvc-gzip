pvc-gzip
========

Implements `PvcGzip` and `PvcGunzip` for compressing and decompressing gzip streams.

Adds the `gzip` tag to streams. Tries to be smart and not double-gzip things that are already gzipped.

Usage
-----

```
pvc.Task("gunzip", () => {
        pvc.Source("gzipped.txt.gz")
           .Pipe(new PvcGunzip())
           .Save("uncompressed");
});

pvc.Task("gzip", () => {
        pvc.Source("plaintext.txt")
           .Pipe(new PvcGzip())
           .Save("compressed");
});
```

PvcGzip accepts a couple parameters:

* `addExtension` - (default: `true`) Whether or not to append `.gz` to the stream name. A situation where you might not want this is pre-compressing before uploading to S3.
* `compressionLevel` - (default: `CompressionLevel.Optimal`)