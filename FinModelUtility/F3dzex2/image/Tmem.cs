namespace f3dzex2.image {
  /// <summary>
  ///   http://ultra64.ca/files/documentation/online-manuals/man/pro-man/pro13/index13.4.html
  /// </summary>
  public struct TileDescriptor {
    public TileDescriptor() {}

    public N64ColorFormat ColorFormat { get; set; } = N64ColorFormat.RGBA;
    public BitsPerTexel BitsPerTexel { get; set; } = BitsPerTexel._16BPT;
  }

  /// <summary>
  ///   The N64 handles texture loading in a somewhat complicated way. Textures
  ///   are not referenced directly from memory, but rather through "tiles",
  ///   which are windows into a shared pool of memory.
  ///
  ///   https://fgfc.ddns.net/PerfectGold/Textures.htm
  ///   http://ultra64.ca/files/documentation/online-manuals/man/pro-man/pro13/index13.5.html
  ///   http://ultra64.ca/files/documentation/online-manuals/man/pro-man/pro13/13-09.html#:~:text=Internally%2C%20the%20RDP%20treats%20loading,the%20tile%20to%20be%20rendered.
  /// </summary>
  public class Tmem {
    private const int TMEM_SIZE = 4096;
    private readonly byte[] data_ = new byte[TMEM_SIZE];

    private const int TILE_DESCRIPTOR_COUNT = 8;

    private readonly TileDescriptor[] tileDescriptors_ =
        new TileDescriptor[TILE_DESCRIPTOR_COUNT];

    // Loads tile from TMEM into tile descriptor.
  }
}
