namespace CmlLib.Core.Internals;

internal static class SevenZipWrapper
{
    public static void DecompressFileLZMA(string inFile, string outFile)
    {
        SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
        FileStream input = new FileStream(inFile, FileMode.Open);
        FileStream output = new FileStream(outFile, FileMode.Create);

        // Read the decoder properties
        byte[] properties = new byte[5];
        var readCount = input.Read(properties, 0, 5);

        if (readCount < 5)
            return;
        
        // Read in the decompress file size.
        byte[] fileLengthBytes = new byte[8];
        readCount = input.Read(fileLengthBytes, 0, 8);

        if (readCount < 8)
            return;
        
        long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

        coder.SetDecoderProperties(properties);
        coder.Code(input, output, input.Length, fileLength, null);
        output.Flush();
        output.Close();
    }
}
