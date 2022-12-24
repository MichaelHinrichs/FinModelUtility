using BenchmarkDotNet.Attributes;
using fin.math;
using MathNet.Numerics.LinearAlgebra.Single;
using System.Numerics;

using SystemMatrix = System.Numerics.Matrix4x4;

namespace benchmarks {
  public class MultMatrices {
    public const int n = 100000;


    [Benchmark]
    public void MultiplyWithSystem() {
      var lhs = new SystemMatrix();
      var rhs = new SystemMatrix();

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

        rhs.M11 = 4;
        rhs.M12 = 5;
        rhs.M13 = 6;
        rhs.M14 = 2;
        rhs.M21 = 5;
        rhs.M22 = 8;
        rhs.M23 = 9;
        rhs.M24 = 3;
        rhs.M31 = 1;
        rhs.M32 = 3;
        rhs.M33 = 4;
        rhs.M34 = 6;
        rhs.M41 = 7;
        rhs.M42 = 8;
        rhs.M43 = 9;
        rhs.M44 = 5;

        var product = Matrix4x4.Multiply(rhs, lhs);
      }
    }

    [Benchmark]
    public void MultiplyWithFin() {
      var lhs = new FinMatrix4x4();
      var rhs = new FinMatrix4x4();
      var result = new FinMatrix4x4();

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

        rhs[0, 0] = 4;
        rhs[0, 1] = 5;
        rhs[0, 2] = 6;
        rhs[0, 3] = 2;
        rhs[1, 0] = 5;
        rhs[1, 1] = 8;
        rhs[1, 2] = 9;
        rhs[1, 3] = 3;
        rhs[2, 0] = 1;
        rhs[2, 1] = 3;
        rhs[2, 2] = 4;
        rhs[2, 3] = 6;
        rhs[3, 0] = 7;
        rhs[3, 1] = 8;
        rhs[3, 2] = 9;
        rhs[3, 3] = 5;

        rhs.MultiplyIntoBuffer(lhs, result);
      }
    }
  }
}