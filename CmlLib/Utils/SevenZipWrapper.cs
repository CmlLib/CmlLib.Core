using System;
using System.IO;
using SevenZip.Compression.LZMA;

namespace CmlLib.Utils;

internal static class SevenZipWrapper
{
    public static void DecompressFileLZMA(string inFile, string outFile)
    {
        var coder = new Decoder();
        var input = new FileStream(inFile, FileMode.Open);
        var output = new FileStream(outFile, FileMode.Create);

        // Read the decoder properties
        var properties = new byte[5];
        var readCount = input.Read(properties, 0, 5);

        if (readCount < 5)
            return;

        // Read in the decompress file size.
        var fileLengthBytes = new byte[8];
        readCount = input.Read(fileLengthBytes, 0, 8);

        if (readCount < 8)
            return;

        var fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

        coder.SetDecoderProperties(properties);
        coder.Code(input, output, input.Length, fileLength, null);
        output.Flush();
        output.Close();
    }
}
