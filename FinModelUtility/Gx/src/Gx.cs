namespace gx {
  /// <summary>
  ///   Shamelessly copied from https://github.com/magcius/noclip.website/blob/1fa39ab5d9d014095c77a8509bf6ba87d3296200/src/gx/gx_enum.ts
  /// </summary>

  public enum GxOpcode : byte {
    NOP = 0x0,

    DRAW_QUADS = 0x80,
    // Early code for GX_DRAW_QUADS? Seen in Luigi's Mansion.
    DRAW_QUADS_2 = 0x88,
    DRAW_TRIANGLES = 0x90,
    DRAW_TRIANGLE_STRIP = 0x98,
    DRAW_TRIANGLE_FAN = 0xA0,
    DRAW_LINES = 0xA8,
    DRAW_LINE_STRIP = 0xB0,
    DRAW_POINTS = 0xB8,

    LOAD_INDX_A = 0x20,
    LOAD_INDX_B = 0x28,
    LOAD_INDX_C = 0x30,
    LOAD_INDX_D = 0x38,

    LOAD_BP_REG = 0x61,
    LOAD_CP_REG = 0x08,
    LOAD_XF_REG = 0x10,
  }

  public enum GxAttributeType {
    NOT_PRESENT = 0,
    DIRECT = 1,
    INDEX_8 = 2,
    INDEX_16 = 3,
  }

  public enum GxAttribute {
    PNMTXIDX = 0,
    TEX0MTXIDX = 1,
    TEX1MTXIDX = 2,
    TEX2MTXIDX = 3,
    TEX3MTXIDX = 4,
    TEX4MTXIDX = 5,
    TEX5MTXIDX = 6,
    TEX6MTXIDX = 7,
    TEX7MTXIDX = 8,
    POS = 9,
    NRM = 10,
    CLR0 = 11,
    CLR1 = 12,
    TEX0 = 13,
    TEX1 = 14,
    TEX2 = 15,
    TEX3 = 16,
    TEX4 = 17,
    TEX5 = 18,
    TEX6 = 19,
    TEX7 = 20,
    MAX = TEX7,
    // NOTE: NBT is a fake API entry-point for GX! It's listed here in case some tools
    // use it as serialization, but you should always switch to NRM in your high-level code.
    _NBT = 25,
    NULL = 0xFF,
  }

  public enum GxComponentCount {
    // Position
    POS_XY = 0,
    POS_XYZ = 1,
    // Normal
    NRM_XYZ = 0,
    NRM_NBT = 1,
    NRM_NBT3 = 2,
    // Color
    CLR_RGB = 0,
    CLR_RGBA = 1,
    // TexCoord
    TEX_S = 0,
    TEX_ST = 1,
  }

  public enum GxComponentType {
    U8 = 0,
    S8 = 1,
    U16 = 2,
    S16 = 3,
    F32 = 4,

    RGB565 = 0,
    RGB8 = 1,
    RGBX8 = 2,
    RGBA4 = 3,
    RGBA6 = 4,
    RGBA8 = 5,
  }
}