using UoT.util;

namespace UoT {
  public enum RDP {
    // GENERAL
    G_SETCIMG = 0xFF,
    G_SETZIMG = 0xFE,
    G_SETTIMG = 0xFD,
    G_SETCOMBINE = 0xFC,
    G_SETENVCOLOR = 0xFB,
    G_SETBLENDCOLOR = 0xF9,
    G_SETFOGCOLOR = 0xF8,
    G_SETPRIMCOLOR = 0xFA,
    G_SETFILLCOLOR = 0xF7,
    G_FILLRECT = 0xF6,
    G_SETTILE = 0xF5,
    G_LOADTILE = 0xF4,
    G_LOADBLOCK = 0xF3,
    G_SETTILESIZE = 0xF2,
    G_LOADTLUT = 0xF0,
    G_RDPSETOTHERMODE = 0xEF,
    G_SETPRIMDEPTH = 0xEE,
    G_SETSCISSOR = 0xED,
    G_SETCONVERT = 0xEC,
    G_SETKEYR = 0xEB,
    G_SETKEYGB = 0xEA,
    G_RDPFULLSYNC = 0xE9,
    G_RDPTILESYNC = 0xE8,
    G_RDPPIPESYNC = 0xE7,
    G_RDPLOADSYNC = 0xE6,
    G_TEXRECTFLIP = 0xE5,
    G_TEXRECT = 0xE4,

    // TEXTURE PARAMS
    G_TX_WRAP = 0,
    G_TX_MIRROR = 1,
    G_TX_CLAMP = 2,

    // GEOMETRY MODE
    G_ZBUFFER = 0x1,
    G_SHADE = 0x4,
    G_CULL_FRONT = 0x200,
    G_CULL_BACK = 0x400,
    G_CULL_BOTH = 0x600,
    G_FOG = 0x10000,
    G_LIGHTING = 0x20000,
    G_TEXTURE_GEN = 0x40000,
    G_TEXTURE_GEN_LINEAR = 0x80000,
    G_LOD = 0x100000,
    G_SHADING_SMOOTH = 0x200000,
    G_CLIPPING = 0x800000,

    // SETOTHERMODE_H
    G_MDSFT_BLENDMASK = 0,
    G_MDSFT_ALPHADITHER = 4,
    G_MDSFT_RGBDITHER = 6,
    G_MDSFT_COMBKEY = 8,
    G_MDSFT_TEXTCONV = 9,
    G_MDSFT_TEXTFILT = 12,
    G_MDSFT_TEXTLUT = 14,
    G_MDSFT_TEXTLOD = 16,
    G_MDSFT_TEXTDETAIL = 17,
    G_MDSFT_TEXTPERSP = 19,
    G_MDSFT_CYCLETYPE = 20,
    G_MDSFT_COLORDITHER = 22,
    G_MDSFT_PIPELINE = 23,

    // COLOR COMBINER
    G_CCMUX_COMBINED = 0,
    G_CCMUX_TEXEL0 = 1,
    G_CCMUX_TEXEL1 = 2,
    G_CCMUX_PRIMITIVE = 3,
    G_CCMUX_SHADE = 4,
    G_CCMUX_ENVIRONMENT = 5,
    G_CCMUX_CENTER = 6,
    G_CCMUX_SCALE = 6,
    G_CCMUX_COMBINED_ALPHA = 7,
    G_CCMUX_TEXEL0_ALPHA = 8,
    G_CCMUX_TEXEL1_ALPHA = 9,
    G_CCMUX_PRIMITIVE_ALPHA = 10,
    G_CCMUX_SHADE_ALPHA = 11,
    G_CCMUX_ENV_ALPHA = 12,
    G_CCMUX_LOD_FRACTION = 13,
    G_CCMUX_PRIM_LOD_FRAC = 14,
    G_CCMUX_NOISE = 7,
    G_CCMUX_K4 = 7,
    G_CCMUX_K5 = 15,
    G_CCMUX_1 = 6,
    G_CCMUX_0 = 31,

    // ALPHA COMBINER
    G_ACMUX_COMBINED = 0,
    G_ACMUX_TEXEL0 = 1,
    G_ACMUX_TEXEL1 = 2,
    G_ACMUX_PRIMITIVE = 3,
    G_ACMUX_SHADE = 4,
    G_ACMUX_ENVIRONMENT = 5,
    G_ACMUX_LOD_FRACTION = 0,
    G_ACMUX_PRIM_LOD_FRAC = 6,
    G_ACMUX_1 = 6,
    G_ACMUX_0 = 7
  }

  public enum RdpCycleMode {
    G_CYC_1CYCLE = 0,
    G_CYC_2CYCLE = 1,
  }

  public static class RDP_Defs {
    public static double[] FIXED2FLOATRECIP = {
        0.5d, 0.25d, 0.125d, 0.0625d, 0.03125d, 0.015625d, 0.0078125d,
        0.00390625d, 0.001953125d, 0.0009765625d, 0.00048828125d,
        0.000244140625d, 0.000122070313d, 0.0000610351563d, 0.0000305175781d,
        0.0000152587891d
    };

    public static string[] ColorAStr = {
        "cCOMBINED", "cTEXEL0", "cTEXEL1", "cPRIM", "cSHADE", "cENV", "1.0",
        "cNOISE", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0"
    };

    public static string[] ColorBStr = {
        "cCOMBINED", "cTEXEL0", "cTEXEL1", "cPRIM", "cSHADE", "cENV",
        "cKEYCENTER", "CONVK4", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0",
        "0.0"
    };

    public static string[] ColorCStr = {
        "cCOMBINED", "cTEXEL0", "cTEXEL1", "cPRIM", "cSHADE", "cENV",
        "cKEYSCALE", "aCOMBINED", "aTEXEL0", "aTEXEL1", "aPRIM", "aSHADE",
        "aENV", "LODFRAC", "PRIMLODFRAC", "CONVK5", "0.0", "0.0", "0.0", "0.0",
        "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0",
        "0.0", "0.0"
    };


    public static string[] ColorDStr = {
        "cCOMBINED", "cTEXEL0", "cTEXEL1", "cPRIM", "cSHADE", "cENV", "1.0",
        "0.0"
    };

    public static string[] AlphaAStr = {
        "aCOMBINED", "aTEXEL0", "aTEXEL1", "aPRIM", "aSHADE", "aENV", "1.0",
        "0.0"
    };

    public static string[] AlphaBStr = {
        "aCOMBINED", "aTEXEL0", "aTEXEL1", "aPRIM", "aSHADE", "aENV", "1.0",
        "0.0"
    };

    public static string[] AlphaCStr = {
        "LODFRAC", "aTEXEL0", "aTEXEL1", "aPRIM", "aSHADE", "aENV",
        "PRIMLODFRAC", "0.0"
    };

    public static string[] AlphaDStr = {
        "aCOMBINED", "aTEXEL0", "aTEXEL1", "aPRIM", "aSHADE", "aENV", "1.0",
        "0.0"
    };

    public static uint[] G_COMBINERMUX0 = {
        0x11FFFFU, 0x127E03U, 0x127E03U, 0x127E03U, 0x121603U, 0x267E04U,
        0x41FFFFU, 0x127E0CU, 0x267E04U, 0x262A04U, 0x121803U, 0x121803U,
        0x41FFFFU, 0x11FFFFU, 0x41C7FFU, 0x41FFFFU, 0x127E60U, 0x272C04U,
        0x20AC04U, 0x26A004U, 0x277E04U, 0x20FE04U, 0x272E04U, 0x272C04U,
        0x20A203U, 0x11FE04U, 0x20AC03U, 0x272C03U, 0x271204U, 0x11FE04U,
        0x272C80U, 0x11FE04U, 0x119C04U, 0x119604U, 0x262A04U, 0x262A04U,
        0x262A04U, 0x127E03U, 0x267E04U, 0x11FE04U, 0x119C04U, 0x271204U,
        0x272C80U, 0x127E03U, 0x267E03U
    };


    public static uint[] G_COMBINERMUX1 = {
        0xFFFFFC38, 0xFFFFFDF8, 0xFFFFF3F8,
        0xFFFFF7F8, 0xFF5BFFF8, 0x1F0CFDFF,
        0xFFFFFC38, 0xFFFFFDF8, 0x1FFCFDF8,
        0x1F0C93FF, 0xFF5BFFF8, 0xFF0FFFFF,
        0xFFFFF638, 0xFFFFF238, 0xFFFFFE38,
        0xFFFFF838, 0xFFFFF3F8, 0x1F0C93FF,
        0xFF0F93FF, 0x1FFC93F8, 0x1F0CF7FF,
        0xFF0FF7FF, 0x1F0C93FF, 0x1F1093FF,
        0xFF13FFFF, 0xFFFFF7F8, 0xFF0F93FF,
        0x1F0C93FF, 0xFF0FF3FF, 0xFFFFFFF8,
        0x1F0CFFFF, 0xFFFFF3F8, 0x350CF37F,
        0xFF5BFFF8, 0x1F5893F8, 0x1F1093FF,
        0xFFFFF9F8, 0xFFFFF9F8, 0x1F10FDFF,
        0xFF0FF3FF, 0xFFFFFFF8, 0x1F0CFFFF,
        0x350CF37F, 0xFFFFF9F8, 0x1FFCFDF8
    };

    public static IDisplayListInstruction? FindLinkedCommand(
        N64DisplayList DL,
        byte Command,
        int StartIndex) {
      var commands = Asserts.Assert(DL.Commands);
      for (var i = 0; i < commands.Length; ++i) {
        var command = commands[i];
        if (command.CMDParams[0] == Command) {
          return command;
        }
        
        if (i > StartIndex && command.CMDParams[0] == (int)F3DZEX.VTX) {
          return null;
        }
      }
      return null;
    }
  }
}