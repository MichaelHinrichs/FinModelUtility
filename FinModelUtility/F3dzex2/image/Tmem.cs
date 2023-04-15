using f3dzex2.displaylist.opcodes;

using fin.model;
using fin.util.hash;

namespace f3dzex2.image {
  /// <summary>
  ///   http://ultra64.ca/files/documentation/online-manuals/man/pro-man/pro13/index13.4.html
  /// </summary>
  public struct TileDescriptor {
    public TileDescriptor() { }

    public TileDescriptorState State { get; set; }
    public N64ColorFormat ColorFormat { get; set; } = N64ColorFormat.RGBA;
    public BitsPerTexel BitsPerTexel { get; set; } = BitsPerTexel._16BPT;

    public override int GetHashCode()
      => FluentHash.Start().With(State).With(ColorFormat).With(BitsPerTexel);

    public override bool Equals(object? obj) {
      if (ReferenceEquals(this, obj)) {
        return true;
      }

      if (obj is TileDescriptor otherTileDescriptor) {
        return this.State == otherTileDescriptor.State &&
               this.ColorFormat == otherTileDescriptor.ColorFormat &&
               this.BitsPerTexel == otherTileDescriptor.BitsPerTexel;
      }

      return false;
    }
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

    public void gsDPLoadBlock(ushort uls,
                              ushort ult,
                              TileDescriptor tileDescriptor,
                              ushort texels,
                              ushort deltaTPerScanline) { }


    public void gsDPSetTile(N64ColorFormat colorFormat,
                            BitsPerTexel bitsPerTexel,
                            uint num64BitValuesPerRow,
                            uint offsetOfTextureInTmem,
                            TileDescriptor tileDescriptor,
                            WrapMode wrapModeS,
                            WrapMode wrapModeT) { }

    public void gsSPTexture(
        ushort scaleS,
        ushort scaleT,
        uint maxExtraMipmapLevels,
        TileDescriptor tileDescriptor,
        TileDescriptorState tileDescriptorState) { }

    public void gsDPSetTextureImage(N64ColorFormat colorFormat,
                                    BitsPerTexel bitsPerTexel,
                                    ushort width,
                                    uint imageSegmentedAddress) { }
  }
}