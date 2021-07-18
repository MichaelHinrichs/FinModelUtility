using Tao.OpenGl;
using Tao.Platform.Windows;

namespace UoT {
  public static class GLExtensions {
    public static bool GLMultiTexture { get; }
    public static bool GLFragProg { get; }
    public static bool GLAnisotropic { get; }
    public static bool GLSL { get; }
    public static bool GLMultiSample { get; }
    public static float AnisotropicSamples { get; }

    public static bool WGLMultiSample { get; }

    static GLExtensions() {
      var extstr = Gl.glGetString(Gl.GL_EXTENSIONS).ToLower();

      GLExtensions.GLMultiTexture = extstr.Contains("gl_arb_multitexture");
      GLExtensions.GLFragProg = extstr.Contains("gl_arb_fragment_program");
      GLExtensions.GLAnisotropic =
          extstr.Contains("gl_ext_texture_filter_anisotropic");
      GLExtensions.GLSL = extstr.Contains("gl_arb_fragment_shader");
      GLExtensions.GLMultiSample = extstr.Contains("gl_arb_multisample");

      if (GLExtensions.GLAnisotropic) {
        Gl.glGetFloatv(Gl.GL_MAX_TEXTURE_MAX_ANISOTROPY_EXT,
                       out var anisotropicSamples);
        GLExtensions.AnisotropicSamples = anisotropicSamples;
      }

      GLExtensions.WGLMultiSample =
          Wgl.wglGetProcAddress("WGL_ARB_Multisample") != null;
    }
  }
}