using System.Text;

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

    public static StreamReader AsyncStreamReader(string path, Encoding encoding)
    {
        FileStream stream = AsyncReadStream(path);
        return new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: false);
    }

    public static StreamWriter AsyncStreamWriter(string path, Encoding encoding, bool append)
    {
        FileStream stream = AsyncWriteStream(path, append);
        return new StreamWriter(stream, encoding);
    }

    // In .NET Standard 2.0, There is no File.ReadFileTextAsync. so I copied it from .NET Core source code
    public static async Task<string> ReadFileAsync(string path)
    {
        using var reader = AsyncStreamReader(path, Encoding.UTF8);
        var content = await reader.ReadToEndAsync()
            .ConfigureAwait(false); // **MUST be awaited in this scope**
        await reader.BaseStream.FlushAsync().ConfigureAwait(false);
        return content;
    }

    // In .NET Standard 2.0, There is no File.WriteFileTextAsync. so I copied it from .NET Core source code
    public static async Task WriteFileAsync(string path, string content)
    {
        // UTF8 with BOM might not be recognized by minecraft. not tested
        var encoder = new UTF8Encoding(false);
        using var writer = AsyncStreamWriter(path, encoder, false);
        await writer.WriteAsync(content).ConfigureAwait(false); // **MUST be awaited in this scope**
        await writer.FlushAsync().ConfigureAwait(false);
    }

    public static async Task CopyFileAsync(string sourceFile, string destinationFile)
    {
        using var sourceStream = AsyncReadStream(sourceFile);
        using var destinationStream = AsyncWriteStream(destinationFile, false);

        await sourceStream.CopyToAsync(destinationStream).ConfigureAwait(false);

        await destinationStream.FlushAsync().ConfigureAwait(false);
    }
}