using f3dzex2.displaylist.opcodes;

using fin.model;

namespace f3dzex2.image {
  public class Tmem : ITmem {
    private const int TMEM_SIZE = 4096;
    private readonly byte[] data_ = new byte[TMEM_SIZE];

    private const int TILE_DESCRIPTOR_COUNT = 8;

    private readonly JankTileDescriptor[] tileDescriptors_ =
        new JankTileDescriptor[TILE_DESCRIPTOR_COUNT];

    private IReadOnlyTileDescriptor tile0_;
    private CullingMode cullingMode_;

    public void GsDpLoadBlock(ushort uls,
                              ushort ult,
                              TileDescriptorIndex tileDescriptor,
                              ushort texels,
                              ushort deltaTPerScanline) { }

    public void GsDpSetTile(N64ColorFormat colorFormat,
                            BitsPerTexel bitsPerTexel,
                            uint num64BitValuesPerRow,
                            uint offsetOfTextureInTmem,
                            TileDescriptorIndex tileDescriptor,
                            F3dWrapMode wrapModeS,
                            F3dWrapMode wrapModeT) {
      throw new System.NotImplementedException();
    }

    public void GsDpSetTile(N64ColorFormat colorFormat,
                            BitsPerTexel bitsPerTexel,
                            uint num64BitValuesPerRow,
                            uint offsetOfTextureInTmem,
                            TileDescriptorIndex tileDescriptor,
                            WrapMode wrapModeS,
                            WrapMode wrapModeT) { }

    public void GsDpSetTileSize(ushort uls,
                                ushort ult,
                                TileDescriptorIndex tileDescriptor,
                                ushort width,
                                ushort height) { }

    public void GsSpTexture(
        ushort scaleS,
        ushort scaleT,
        uint maxExtraMipmapLevels,
        TileDescriptorIndex tileDescriptor,
        TileDescriptorState tileDescriptorState) { }

    public void GsDpSetTextureImage(N64ColorFormat colorFormat,
                                    BitsPerTexel bitsPerTexel,
                                    ushort width,
                                    uint imageSegmentedAddress) { }

    public IReadOnlyTileDescriptor Tile0 {
      get => this.tile0_;
      set => this.tile0_ = value;
    }

    public IMaterial GetOrCreateMaterialForTile0() {
      throw new System.NotImplementedException();
    }

    public CullingMode CullingMode {
      set => this.cullingMode_ = value;
    }
  }
}