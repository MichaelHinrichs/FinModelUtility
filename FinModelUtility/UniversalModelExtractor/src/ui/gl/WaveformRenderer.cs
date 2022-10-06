using fin.audio;
using OpenTK.Graphics.OpenGL;


namespace uni.ui.gl {
  public class WaveformRenderer {
    public IActiveSound<short>? ActiveSound { get; set; }

    public int Width { get; set; }
    public int MiddleY { get; set; }
    public int Amplitude { get; set; }

    public void Render() {
      if (ActiveSound == null) {
        return;
      }

      var samplesPerPoint = 50;
      var pointsPerX = 4;
      var pointCount = Width / pointsPerX;
      var points = new float[pointCount];
      for (var i = 0; i < pointCount; ++i) {
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

      var waveformAmplitude = 32;
      var waveformMiddleY = MiddleY - waveformAmplitude;

      GL.Color3(1f, 0, 0);
      GL.LineWidth(1);

      GL.Begin(PrimitiveType.LineStrip);
      for (var i = 0; i < pointCount; ++i) {
        var x = i * pointsPerX;
        var y = waveformMiddleY + waveformAmplitude * points[i];
        GL.Vertex2(x, y);
      }
      GL.End();
    }
  }
}