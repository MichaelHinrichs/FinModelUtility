using fin.gl;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace uni.debug {
  public class DebugWindow : GameWindow {
    private GlShaderProgram shaderProgram_;

    protected override void OnLoad(EventArgs e) {
      this.shaderProgram_ = GlShaderProgram.FromShaders(@"
# version 110

varying vec4 vertexColor;

void main() {
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex; 
    vertexColor = gl_Color;
}", @"
# version 130 

out vec4 fragColor;

in vec4 vertexColor;

void main() {
    fragColor = vertexColor;
}");
    }

    protected override void OnRenderFrame(FrameEventArgs e) {
      this.shaderProgram_.Use();

      GL.ClearColor(0, 0, 1, 1);
      GlUtil.ClearColorAndDepth();

      GL.Begin(PrimitiveType.Quads);

      GL.Color3((byte) 255, (byte) 0, (byte) 0);

      GL.Vertex2(0, 0);
      GL.Vertex2(1, 0);
      GL.Vertex2(1, 1);
      GL.Vertex2(0, 1);

      GL.End();
      GL.Flush();

      Context.SwapBuffers();
    }
  }
}