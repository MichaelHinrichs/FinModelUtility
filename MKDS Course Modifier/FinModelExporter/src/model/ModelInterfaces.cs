using System.Collections.Generic;

namespace fin.model {
  public interface IModel {
    ISkeleton Skeleton { get; }
    ISkin Skin { get; }
    IMaterials Materials { get; }
    IAnimations Animations { get; }
  }

  public interface ISkeleton {
    IBone Root { get; }
  }

  public interface IBone {
    string Name { get; }

    IBone? Parent { get; }
    IReadOnlyList<IBone> Children { get; }
    IBone AddChild(float x, float y, float z);

    IPosition LocalPosition { get; }
    IQuaternion? LocalRotation { get; }
    IScale? LocalScale { get; }

    IBone SetLocalPosition(float x, float y, float z);
    IBone SetLocalRotationDegrees(float x, float y, float z);
    IBone SetLocalRotationRadians(float x, float y, float z);
    IBone SetLocalScale(float x, float y, float z);
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


  public interface IMaterials {
    IReadOnlyList<IMaterial> All { get; }
    IMaterial AddMaterial();
  }

  public interface IMaterial {
    string Name { get; }

    // TODO: Setting texture layer(s).
    // TODO: Setting logic for combining texture layers.
  }
}