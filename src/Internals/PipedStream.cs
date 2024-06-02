namespace CmlLib.Core.Internals;

internal class PipedStream : Stream, IDisposable
{
    private readonly Stream _source;
    private readonly Stream _writer;
    private readonly bool _writeToEndOnClose;

    public PipedStream(Stream source, Stream writer, bool writeToEndOnClose)
    {
        _source = source;
        _writer = writer;
        _writeToEndOnClose = writeToEndOnClose;
    }

    public override bool CanRead => _source.CanRead;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => _source.Length;

    public override long Position 
    {
        get => _source.Position;
        set => throw new InvalidOperationException(); 
    }

    public override void Flush()
    {
        _writer.Flush();   
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var read = _source.Read(buffer, offset, count);
        _writer.Write(buffer, offset, read);
        return read;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new InvalidOperationException();
    }

    public override void SetLength(long value)
    {
        throw new InvalidOperationException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new InvalidOperationException();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_writeToEndOnClose)
            {
                _source.CopyTo(_writer);
            }

            _source.Dispose();
            _writer.Dispose();
        }
        base.Dispose(disposing);
    }
}