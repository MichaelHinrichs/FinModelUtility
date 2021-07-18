namespace UoT {
  // ALL UCODES - BASIC

  // F3DZEX (F3DEX v2.0)
  public enum F3DZEX {
    // GEOMETRY DRAWING
    VTX = 1,
    MODIFYVTX = 2,
    CULLDL = 3,
    BRANCH_Z = 4,
    TRI1 = 5,
    TRI2 = 6,
    QUAD = 7,

    // MATRIX MANIPULATION
    MTX_MODELVIEW = 0,
    MTX_PROJECTION = 4,
    MTX_MUL = 3,
    MTX_LOAD = 2,
    MTX_NOPUSH = 0,
    MTX_PUSH = 1,

    // GENERAL
    RDPHALF_2 = 0xF1,
    SETOTHERMODE_H = 0xE3,
    SETOTHERMODE_L = 0xE2,
    RDPHALF_1 = 0xE1,
    SPNOOP = 0xE0,
    ENDDL = 0xDF,
    DL = 0xDE,
    LOAD_UCODE = 0xDD,
    MOVEMEM = 0xDC,
    MOVEWORD = 0xDB,
    MTX = 0xDA,
    GEOMETRYMODE = 0xD9,
    POPMTX = 0xD8,
    TEXTURE = 0xD7,
    DMA_IO = 0xD6,
    SPECIAL_1 = 0xD5,
    SPECIAL_2 = 0xD4,
    SPECIAL_3 = 0xD3
  }
}