using System;

using fin.model;
using OpenTK.Graphics.OpenGL;


namespace fin.gl {
  public static partial class GlUtil {
    private static (DepthMode, DepthCompareType) depthModeAndCompareType =
        (DepthMode.USE_DEPTH_BUFFER, DepthCompareType.LEqual);
    
    public static void ResetDepth()
      => SetDepth(DepthMode.USE_DEPTH_BUFFER, DepthCompareType.LEqual);

    public static bool SetDepth(
        DepthMode depthMode,
        DepthCompareType depthCompareType) {
      if (GlUtil.depthModeAndCompareType == (depthMode, depthCompareType)) {
        return false;
      }

      GlUtil.depthModeAndCompareType = (depthMode, depthCompareType);

      switch (depthMode) {
        case DepthMode.USE_DEPTH_BUFFER: {
          GL.DepthFunc(ConvertFinDepthCompareTypeToGl_(depthCompareType));
          GL.Enable(EnableCap.DepthTest);
          GL.DepthMask(true);
          break;
        }
        case DepthMode.IGNORE_DEPTH_BUFFER: {
          GL.Disable(EnableCap.DepthTest);
          GL.DepthMask(false);
          break;
        }
        case DepthMode.SKIP_WRITE_TO_DEPTH_BUFFER: {
          GL.DepthFunc(ConvertFinDepthCompareTypeToGl_(depthCompareType));
          GL.Enable(EnableCap.DepthTest);
          GL.DepthMask(false);
          break;
        }
        default:
          throw new ArgumentOutOfRangeException(nameof(depthMode), depthMode,
                                                null);
      }

      return true;
    }

    private static DepthFunction ConvertFinDepthCompareTypeToGl_(
        DepthCompareType finDepthCompareType)
      => finDepthCompareType switch {
          DepthCompareType.LEqual  => DepthFunction.Lequal,
          DepthCompareType.Less    => DepthFunction.Less,
          DepthCompareType.Equal   => DepthFunction.Equal,
          DepthCompareType.Greater => DepthFunction.Greater,
          DepthCompareType.NEqual  => DepthFunction.Notequal,
          DepthCompareType.GEqual  => DepthFunction.Gequal,
          DepthCompareType.Always  => DepthFunction.Always,
          DepthCompareType.Never   => DepthFunction.Never,
          _ => throw new ArgumentOutOfRangeException(
              nameof(finDepthCompareType), finDepthCompareType, null)
      };


  }
}