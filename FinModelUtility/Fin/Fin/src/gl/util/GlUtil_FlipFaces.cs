using OpenTK.Graphics.OpenGL;


namespace fin.gl {
  public static partial class GlUtil {
    private static bool flipFaces_;

    public static void ResetFlipFaces() => SetFlipFaces(false);

    public static void SetFlipFaces(bool flipFaces) {
      if (GlUtil.flipFaces_ == flipFaces) {
        return;
      }

      GlUtil.flipFaces_ = flipFaces;
      GL.FrontFace(flipFaces ? FrontFaceDirection.Cw : FrontFaceDirection.Ccw);
    }
  }
}