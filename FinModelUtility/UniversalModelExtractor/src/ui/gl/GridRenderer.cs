using Tao.OpenGl;


namespace uni.ui.gl {
  public class GridRenderer {
    public float Spacing { get; } = 32;
    public float Size = 1024;

    public void Render() {
      var size = this.Size;
      var spacing = this.Spacing;

      Gl.glLineWidth(1);

      Gl.glBegin(Gl.GL_LINES);

      for (var y = 0f; y <= size / 2; y += spacing) {
        if (y == 0) {
          Gl.glColor4f(1, 0, 0, 1);
        } else {
          Gl.glColor4f(1, 1, 1, 1);

          Gl.glVertex3f(-size / 2, -y, 0);
          Gl.glVertex3f(size / 2, -y, 0);
        }

        Gl.glVertex3f(-size / 2, y, 0);
        Gl.glVertex3f(size / 2, y, 0);
      }

      for (var x = 0f; x <= size / 2; x += spacing) {
        if (x == 0) {
          Gl.glColor4f(0, 1, 0, 1);
        } else {
          Gl.glColor4f(1, 1, 1, 1);

          Gl.glVertex3f(-x, -size / 2, 0);
          Gl.glVertex3f(-x, size / 2, 0);
        }

        Gl.glVertex3f(x, -size / 2, 0);
        Gl.glVertex3f(x, size / 2, 0);
      }

      Gl.glEnd();
    }
  }
}