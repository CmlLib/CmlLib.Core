namespace CmlLib.Core;

public readonly struct ByteProgress
{
    public ByteProgress(long totalBytes, long progressedBytes)
    {
        TotalBytes = totalBytes;
        ProgressedBytes = progressedBytes;
    }

    public readonly long TotalBytes;
    public readonly long ProgressedBytes;

    public double ToRatio() => (double)ProgressedBytes / TotalBytes;

    public static ByteProgress operator +(ByteProgress a, ByteProgress b)
    {
        return new ByteProgress(a.TotalBytes + b.TotalBytes, a.ProgressedBytes + b.ProgressedBytes);
    }

    public static ByteProgress operator -(ByteProgress a, ByteProgress b)
    {
        return new ByteProgress(a.TotalBytes - b.TotalBytes, a.ProgressedBytes - b.ProgressedBytes);
    }
}