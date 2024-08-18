namespace L1.Avalonia.Gif.Decoding;

public class GifFrame
{
    public GifRect Dimensions;
    public TimeSpan FrameDelay;
    public FrameDisposal FrameDisposalMethod;
    public bool HasTransparency, IsInterlaced, IsLocalColorTableUsed;
    public GifColor[]? LocalColorTable;
    public int LZWMinCodeSize, LocalColorTableSize;
    public long LZWStreamPosition;
    public bool ShouldBackup;
    public byte TransparentColorIndex;
}
