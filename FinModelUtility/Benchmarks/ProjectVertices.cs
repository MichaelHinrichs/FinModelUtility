using BenchmarkDotNet.Attributes;
using fin.math;
using fin.math.matrix;
using System.Numerics;

using SystemMatrix = System.Numerics.Matrix4x4;
using SystemVector4 = System.Numerics.Vector4;

namespace benchmarks {
  public class ProjectVertices {
    public const int n = 100000;


    [Benchmark]
    public void MultiplyWithSystem() {
      var lhs = new SystemMatrix();
      var rhs = new SystemVector4();

      for (var i = 0; i < n; i++) {
        lhs.M11 = 1;
        lhs.M12 = 2;
        lhs.M13 = 3;
        lhs.M14 = 4;
        lhs.M21 = 5;
        lhs.M22 = 6;
        lhs.M23 = 7;
        lhs.M24 = 8;
        lhs.M31 = 9;
        lhs.M32 = 10;
        lhs.M33 = 11;
        lhs.M34 = 12;
        lhs.M41 = 13;
        lhs.M42 = 14;
        lhs.M43 = 15;
        lhs.M44 = 16;

        rhs.X = 2;
        rhs.Y = 3;
        rhs.Z = 4;
        rhs.W = 1;

        var product = SystemVector4.Transform(rhs, lhs);
      }
    }

    [Benchmark]
    public void MultiplyWithFin() {
      var lhs = new FinMatrix4x4();
      var rhs = new FinVector4();
      var result = new FinVector4();

      for (var i = 0; i < n; i++) {
        lhs[0, 0] = 1;
        lhs[0, 1] = 2;
        lhs[0, 2] = 3;
        lhs[0, 3] = 4;
        lhs[1, 0] = 5;
        lhs[1, 1] = 6;
        lhs[1, 2] = 7;
        lhs[1, 3] = 8;
        lhs[2, 0] = 9;
        lhs[2, 1] = 10;
        lhs[2, 2] = 11;
        lhs[2, 3] = 12;
        lhs[3, 0] = 13;
        lhs[3, 1] = 14;
        lhs[3, 2] = 15;
        lhs[3, 3] = 16;

        rhs.Set(2, 3, 4, 1);

        rhs.MultiplyIntoBuffer(lhs, result);
      }
    }
  }
}