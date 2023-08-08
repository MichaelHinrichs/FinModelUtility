using fin.ui.audio;
using fin.ui.graphics.gl;

using OpenTK.Graphics.OpenGL;


namespace uni.ui.gl {
  public class WaveformRenderer {
    public IActiveSound<short>? ActiveSound { get; set; }

    public int Width { get; set; }
    public float MiddleY { get; set; }
    public float Amplitude { get; set; }

    public void Render() {
      if (ActiveSound == null) {
        return;
      }

      GlTransform.PassMatricesIntoGl();

      var samplesPerPoint = 25;
      var xPerPoint = 1;
      var pointCount = Width / xPerPoint;
      var points = new float[pointCount + 1];
      for (var i = 0; i <= pointCount; ++i) {
        float totalSample = 0;
        for (var s = 0; s < samplesPerPoint; ++s) {
          var sampleOffset =
              this.ActiveSound.SampleOffset + i * samplesPerPoint + s;
          sampleOffset %= this.ActiveSound.SampleCount;

          var sample =
              this.ActiveSound.Stream.GetPcm(AudioChannelType.MONO,
                                             sampleOffset);
          totalSample += sample;
        }
        var meanSample = totalSample / samplesPerPoint;

        float shortMin = short.MinValue;
        float shortMax = short.MaxValue;

        var normalizedShortSample =
            (meanSample - shortMin) / (shortMax - shortMin);

        var floatMin = -1f;
        var floatMax = 1f;

        var floatSample =
            floatMin + normalizedShortSample * (floatMax - floatMin);

        points[i] = floatSample;
      }

      GL.Color3(1f, 0, 0);
      GL.LineWidth(1);

      GL.Begin(PrimitiveType.LineStrip);
      for (var i = 0; i <= pointCount; ++i) {
        var x = i * xPerPoint;
        var y = this.MiddleY + this.Amplitude * points[i];
        GL.Vertex2(x, y);
      }
      GL.End();
    }
  }
}