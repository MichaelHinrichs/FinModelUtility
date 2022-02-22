using System.Collections.Generic;
using System.IO;

using fin.util.asserts;

using schema;


namespace hw.granny3d {
  /// <summary>
  ///   Based on HaloWarsDocs file template:
  ///   https://github.com/HaloMods/HaloWarsDocs/blob/master/010Editor/Granny.bt
  /// </summary>
  public class GrannyFileInfo : IGrannyFileInfo, IDeserializable {
    public string FromFileName { get; private set; }

    public IList<IGrannySkeleton> SkeletonHeaderList { get; } =
      new List<IGrannySkeleton>();

    public IList<IGrannyMesh> VertexDataList { get; } =
      new List<IGrannyMesh>();

    public IList<IGrannyModel> ModelHeaderList { get; private set; }
    public IList<IGrannyTrackGroup> TrackGroupHeaderList { get; private set; }
    public IList<IGrannyAnimation> AnimationHeaderList { get; private set; }

    public void Read(EndianBinaryReader er) {
      // TODO: Make this offset-agnostic.
      // The reader passed into this method should have an offset of 0 at the
      // start of the granny_file_info object.
      Asserts.Equal(0, er.Position, "Expected to start reading at offset 0.");

      er.ReadUInt64(); // ArtToolInfo
      er.ReadUInt64(); // ExporterInfo

      GrannyUtils.SubreadUInt64Pointer(
          er, ser => { this.FromFileName = ser.ReadStringNT(); });

      var textureCount = er.ReadUInt32();
      GrannyUtils.SubreadUInt64Pointer(er, ser => { });

      var materialCount = er.ReadUInt32();
      GrannyUtils.SubreadUInt64Pointer(er, ser => { });

      var skeletonCount = er.ReadUInt32();
      GrannyUtils.SubreadUInt64Pointer(
          er,
          ser => {
            for (var i = 0; i < skeletonCount; ++i) {
              GrannyUtils.SubreadUInt64Pointer(ser, sser => {
                var skeleton = new GrannySkeleton();
                skeleton.Read(sser);
                this.SkeletonHeaderList.Add(skeleton);
              });
            }
          });

      var vertexDataCount = er.ReadUInt32();
      GrannyUtils.SubreadUInt64Pointer(er, ser => { });

      var modelHeaderCount = er.ReadUInt32();
      GrannyUtils.SubreadUInt64Pointer(er, ser => { });

      var trackGroupHeaderCount = er.ReadUInt32();
      GrannyUtils.SubreadUInt64Pointer(er, ser => { });

      var animationHeaderCount = er.ReadUInt32();
      GrannyUtils.SubreadUInt64Pointer(er, ser => { });
    }
  }
}