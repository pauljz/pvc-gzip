pvc.Task("nuget-push", () => {
    pvc.Source("src/Pvc.Gzip.csproj")
       .Pipe(new PvcNuGetPack(
            createSymbolsPackage: true
       ))
       .Pipe(new PvcNuGetPush());
});
