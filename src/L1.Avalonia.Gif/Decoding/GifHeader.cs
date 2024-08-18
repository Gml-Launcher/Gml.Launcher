namespace L1.Avalonia.Gif.Decoding;

public class GifHeader
{
    public int BackgroundColorIndex;
    public GifRect Dimensions;
    public ulong GlobalColorTableCacheID;
    public int GlobalColorTableSize;
    public GifColor[]? GlobarColorTable; // Default to empty array
    public bool HasGlobalColorTable;
    public long HeaderSize;
    public GifRepeatBehavior? IterationCount;
    internal int Iterations = -1;
}
