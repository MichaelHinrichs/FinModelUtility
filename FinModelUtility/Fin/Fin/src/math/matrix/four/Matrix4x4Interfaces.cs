using System.Numerics;

using fin.model;

namespace fin.math.matrix.four {
  public interface IFinMatrix4x4
      : IFinMatrix<IFinMatrix4x4, IReadOnlyFinMatrix4x4, Matrix4x4,
            Vector3, Quaternion, Vector3>,
        IReadOnlyFinMatrix4x4 { }

  public interface IReadOnlyFinMatrix4x4
      : IReadOnlyFinMatrix<IFinMatrix4x4, IReadOnlyFinMatrix4x4, Matrix4x4,
          Vector3, Quaternion, Vector3> {
    void CopyTranslationInto(out Position dst);
    void CopyScaleInto(out Scale dst);
  }
}