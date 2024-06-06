namespace L1.Avalonia.Gif.Decoding;

public class GifHeader
{
    private GifColor[] _globarColorTable;
    public int BackgroundColorIndex;
    public GifRect Dimensions;
    public ulong GlobalColorTableCacheID;
    public int GlobalColorTableSize;
    public GifColor[] GlobarColorTable;
    public bool HasGlobalColorTable;
    public long HeaderSize;
    public GifRepeatBehavior IterationCount;
    internal int Iterations = -1;
}
