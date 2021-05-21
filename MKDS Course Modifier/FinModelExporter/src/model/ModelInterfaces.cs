using System.Collections.Generic;

namespace fin.model {
  public interface IModel {
    IBones Bones { get; }
    IMaterials Materials { get; }
    ISkin Skin { get; }
  }

  public interface IBones {
    IReadOnlyList<IBone> All { get; }
    IBone AddBone(float x, float y, float z);
  }

  public interface IBone {
    IPosition LocalPosition { get; }
    IQuaternion LocalRotation { get; }
    IScale LocalScale { get; }

    IBone SetLocalPosition(float x, float y, float z);
    IBone SetLocalRotationDegrees(float x, float y, float z);
    IBone SetLocalRotationRadians(float x, float y, float z);
    IBone SetLocalScale(float x, float y, float z);
  }

  public interface IMaterials {
    IReadOnlyList<IMaterial> All { get; }
    IMaterial AddMaterial();
  }

  public interface IMaterial {
    // TODO: Setting texture layer(s).
    // TODO: Setting logic for combining texture layers.
  }

  public interface ISkin {
    IReadOnlyList<IVertex> Vertices { get; }
    IVertex AddVertex(float x, float y, float z);

    IReadOnlyList<IPrimitive> Primitives { get; }
    IPrimitive AddTriangle(IVertex v1, IVertex v2, IVertex v3);
    IPrimitive AddQuad(IVertex v1, IVertex v2, IVertex v3, IVertex v4);
  }

  public interface IVertex {
    IPosition GlobalPosition { get; }
    INormal GlobalNormal { get; }

    IVertex SetWeights((IBone, float) weights);
    IVertex SetGlobalPosition(float x, float y, float z);

    IVertex SetGlobalNormal(float x, float y, float z);
    // TODO: Setting colors.
    // TODO: Setting multiple texture UVs.
  }

  public enum PrimitiveType {
    TRIANGLE,
    QUAD,
    // TODO: Other types.
  }

  public interface IPrimitive {
    IReadOnlyList<IVertex> Vertices { get; }

    IPrimitive SetMaterial(IMaterial material);
  }

  public interface IVector4 {
    float X { get; set; }
    float Y { get; set; }
    float Z { get; set; }
    float W { get; }
  }

  public interface IPosition : IVector4 {}

  public interface INormal : IVector4 {}

  public interface IVector3 {
    float X { get; set; }
    float Y { get; set; }
    float Z { get; set; }
  }

  public interface IScale : IVector3 {}

  public interface IQuaternion {
    float XDegrees { get; }
    float YDegrees { get; }
    float ZDegrees { get; }
    IQuaternion SetDegrees(float x, float y, float z);

    float XRadians { get; }
    float YRadians { get; }
    float ZRadians { get; }
    IQuaternion SetRadians(float x, float y, float z);
  }
}