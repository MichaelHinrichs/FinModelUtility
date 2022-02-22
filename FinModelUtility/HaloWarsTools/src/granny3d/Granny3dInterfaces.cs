using System;
using System.Collections.Generic;
using System.Numerics;

using fin.math.matrix;


namespace hw.granny3d {
  public interface IGrannyFileInfo {
    string FromFileName { get; }

    IList<IGrannySkeleton> SkeletonHeaderList { get; }
    IList<IGrannyMesh> VertexDataList { get; }
    IList<IGrannyModel> ModelHeaderList { get; }
    IList<IGrannyTrackGroup> TrackGroupHeaderList { get; }
    IList<IGrannyAnimation> AnimationHeaderList { get; }
  }


  public interface IGrannySkeleton {
    string Name { get; }
    IList<IGrannyBone> Bones { get; }
    int LodType { get; }
  }

  public interface IGrannyBone {
    string Name { get; }
    int ParentIndex { get; }
    IGrannyTransform LocalTransform { get; }
    IFinMatrix4x4 InverseWorld4x4 { get; }
    float LodError { get; }
  }


  public interface IGrannyMesh { }

  public interface IGrannyModel { }

  public interface IGrannyTrackGroup { }

  public interface IGrannyAnimation { }


  public interface IGrannyTransform {
    GrannyTransformFlags Flags { get; }
    Vector3 Position { get; }
    Quaternion Orientation { get; }
    Vector3[] ScaleShear { get; }
  }

  [Flags]
  public enum GrannyTransformFlags : int {
    HasPosition = 0x1,
    HasOrientation = 0x2,
    HasScaleShear = 0x4,
  }
}