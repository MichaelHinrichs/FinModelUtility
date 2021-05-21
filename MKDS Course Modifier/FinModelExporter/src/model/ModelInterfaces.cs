using System.Collections.Generic;

namespace fin.model {
  public interface IModel {
    IBones Bones { get; }
    IMaterials Materials { get; }
    ISkin Skin { get; }
  }

  public interface IBones {
    IReadOnlyList<IBone> All { get; }
    IBone AddBone(IPosition localPosition);
  }

  public interface IBone {
    IPosition LocalPosition { get; }
    IQuaternion LocalRotation { get; }
    IScale LocalScale { get; }

    IBone SetLocalPosition(IPosition localPosition);
    IBone SetLocalRotation(IQuaternion localRotation);
    IBone SetLocalScale(IScale localScale);
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
    IVertex AddVertex(IPosition position);

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
    float X { get; }
    float Y { get; }
    float Z { get; }
    float W { get; }
  }

  public interface IPosition : IVector4 {}

  public interface INormal : IVector4 {}

  public interface IVector3 {
    float X { get; }
    float Y { get; }
    float Z { get; }
  }

  public interface IScale : IVector3 {
    float X { get; }
    float Y { get; }
    float Z { get; }
  }

  public interface IQuaternion {
    // TODO: These fields.
  }
}