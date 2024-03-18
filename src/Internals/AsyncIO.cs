namespace CmlLib.Core.Internals;

internal static class AsyncIO
{
    // from dotnet core source code, default buffer size of file stream is 4096
    private const int DefaultBufferSize = 4096;

    // from .NET Framework reference source code
    // If we use the path-taking constructors we will not have FileOptions.Asynchronous set and
    // we will have asynchronous file access faked by the thread pool. We want the real thing.
    // https://source.dot.net/#System.Private.CoreLib/File.cs,c7e38336f651ba69
    public static FileStream AsyncReadStream(string path)
    {
        FileStream stream = new FileStream(
            path, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize,
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        return stream;
    }

    // https://source.dot.net/#System.Private.CoreLib/File.cs,b5563532b5be50f6
    public static FileStream AsyncWriteStream(string path, bool append)
    {
        FileStream stream = new FileStream(
            path, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read, DefaultBufferSize,
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        return stream;
    }

    public static async Task WriteFileAsync(string path, Stream stream)
    {
        using var writer = AsyncWriteStream(path, false);
        await stream.CopyToAsync(writer);
    }
}