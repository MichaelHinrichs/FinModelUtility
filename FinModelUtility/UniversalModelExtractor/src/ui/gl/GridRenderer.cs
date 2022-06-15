using Tao.OpenGl;


namespace uni.ui.gl {
  public class GridRenderer {
    public float Spacing { get; } = 32;
    public float Size = 1028;

    public void Render() {
      var size = this.Size;
      var spacing = this.Spacing;

      Gl.glLineWidth(1);

      Gl.glBegin(Gl.GL_LINES);
      Gl.glColor4f(1, 1, 1, 1);

      for (var y = -size / 2; y <= size / 2; y += spacing) {
        Gl.glVertex3f(-size / 2, y, 0);
        Gl.glVertex3f(size / 2, y, 0);
      }

      for (var x = -size / 2; x <= size / 2; x += spacing) {
        Gl.glVertex3f(x, -size / 2, 0);
        Gl.glVertex3f(x, size / 2, 0);
      }

      Gl.glEnd();
    }
  }
}