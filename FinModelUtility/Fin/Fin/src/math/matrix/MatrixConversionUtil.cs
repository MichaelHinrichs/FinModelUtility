using Assimp;

namespace fin.math.matrix {
  using AssimpMatrix = Matrix4x4;
  using SystemMatrix = System.Numerics.Matrix4x4;

  public static class MatrixConversionUtil {
    public static void CopySystemIntoFin(
        SystemMatrix other,
        IFinMatrix4x4 finMatrix) {
      finMatrix[0, 0] = other.M11;
      finMatrix[0, 1] = other.M21;
      finMatrix[0, 2] = other.M31;
      finMatrix[0, 3] = other.M41;

      finMatrix[1, 0] = other.M12;
      finMatrix[1, 1] = other.M22;
      finMatrix[1, 2] = other.M32;
      finMatrix[1, 3] = other.M42;

      finMatrix[2, 0] = other.M13;
      finMatrix[2, 1] = other.M23;
      finMatrix[2, 2] = other.M33;
      finMatrix[2, 3] = other.M43;

      finMatrix[3, 0] = other.M14;
      finMatrix[3, 1] = other.M24;
      finMatrix[3, 2] = other.M34;
      finMatrix[3, 3] = other.M44;
    }

    public static void CopyFinIntoAssimp(
        IReadOnlyFinMatrix4x4 finMatrix,
        ref AssimpMatrix assMatrix) {
      assMatrix.A1 = (float) finMatrix[0, 0];
      assMatrix.B1 = (float) finMatrix[1, 0];
      assMatrix.C1 = (float) finMatrix[2, 0];
      assMatrix.D1 = (float) finMatrix[3, 0];

      assMatrix.A2 = (float) finMatrix[0, 1];
      assMatrix.B2 = (float) finMatrix[1, 1];
      assMatrix.C2 = (float) finMatrix[2, 1];
      assMatrix.D2 = (float) finMatrix[3, 1];

      assMatrix.A3 = (float) finMatrix[0, 2];
      assMatrix.B3 = (float) finMatrix[1, 2];
      assMatrix.C3 = (float) finMatrix[2, 2];
      assMatrix.D3 = (float) finMatrix[3, 2];

      assMatrix.A4 = (float) finMatrix[0, 3];
      assMatrix.B4 = (float) finMatrix[1, 3];
      assMatrix.C4 = (float) finMatrix[2, 3];
      assMatrix.D4 = (float) finMatrix[3, 3];
    }
  }
}