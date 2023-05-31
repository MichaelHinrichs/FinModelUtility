using System;
using System.Drawing;

using OpenTK.Graphics.OpenGL;


namespace fin.gl {
  public static partial class GlUtil {
    public static bool IsInitialized { get; private set; }

    public static void Init() {
      if (IsInitialized) {
        return;
      }

      // (Set up DLL here, if ever needed again.)

      IsInitialized = true;
    }

    private static readonly object GL_LOCK_ = new();


    public static void RunLockedGl(Action handler) {
      lock (GL_LOCK_) {
        handler();
      }
    }


    public static void ResetGl(Color backgroundColor) {
      GL.ShadeModel(ShadingModel.Smooth);
      GL.Enable(EnableCap.PointSmooth);
      GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);

      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

      GL.ClearDepth(5.0F);

      GlUtil.ResetDepth();

      GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

      GL.Enable(EnableCap.Normalize);

      GlUtil.ResetFlipFaces();
      GlUtil.ResetCulling();
      GlUtil.ResetBlending();

      GL.ClearColor(backgroundColor.R / 255f, 
                    backgroundColor.G / 255f,
                    backgroundColor.B / 255f, 1);
    }
  }
}