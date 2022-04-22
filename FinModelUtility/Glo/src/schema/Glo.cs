using schema;

namespace glo.schema {
  [Schema]
  public sealed partial class Glo : IBiSerializable {
    private readonly string magic_ = "GLO\0";

    public ushort Version { get; set; }

    [ArrayLengthSource(IntType.UINT16)]
    public List<GloObject> Objects { get; set; }

    [ArrayLengthSource(IntType.UINT16)]
    public List<GloMesh> Meshes { get; set; }
  }


  [Schema]
  public sealed partial class GloObject : IBiSerializable {

    [ArrayLengthSource(IntType.UINT16)]
    public List<GloAnimSeg> AnimSegs { get; set; }
  }

  [Schema]
  public sealed partial class GloAnimSeg : IBiSerializable {
    [StringLengthSource(24)]
    public string Name { get; set; }
    public uint StartFrame { get; set; }
    public uint EndFrame { get; set; }
    public uint Flags { get; set; }
    public float Speed { get; set; }
  }


  [Schema]
  public sealed partial class GloMesh : IBiSerializable {
    [StringLengthSource(24)]
    public string Name { get; set; }

    [ArrayLengthSource(IntType.UINT16)]
    public List<GloXyzKey> MoveKeys { get; set; }

    [ArrayLengthSource(IntType.UINT16)]
    public List<GloXyzKey> ScaleKeys { get; set; }

    [ArrayLengthSource(IntType.UINT16)]
    public List<GloQuaternionKey> RotateKeys { get; set; }

    [ArrayLengthSource(IntType.UINT16)]
    public List<GloVertex> Vertices { get; set; }

    [ArrayLengthSource(IntType.UINT16)]
    public List<GloFace> Faces { get; set; }

    [ArrayLengthSource(IntType.UINT16)]
    public List<GloSprite> Sprites { get; set; }

    public ushort MeshTranslucency { get; set; }
    public ushort MeshFlags { get; set; }

    public GloMeshPointers Pointers { get; set; }
  }

  public sealed class GloMeshPointers : IBiSerializable {
    public GloMesh? Child { get; set; }
    public GloMesh? Next { get; set; }

    public void Read(EndianBinaryReader er) {
      var hasChild = er.ReadUInt16() != 0;
      if (hasChild) {
        this.Child = new GloMesh();
        this.Child.Read(er);
      } else {
        this.Child = null;
      }

      var hasNext = er.ReadUInt16() != 0;
      if (hasNext) {
        this.Next = new GloMesh();
        this.Next.Read(er);
      } else {
        this.Next = null;
      }
    }

    public void Write(EndianBinaryWriter ew) {
      throw new NotImplementedException();
    }
  }

  [Schema]
  public sealed partial class GloXyzKey : IBiSerializable {
    public uint Time { get; set; }

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
  }

  [Schema]
  public sealed partial class GloQuaternionKey : IBiSerializable {
    public uint Time { get; set; }

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float W { get; set; }
  }

  [Schema]
  public sealed partial class GloVertex : IBiSerializable {
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
  }

  [Schema]
  public sealed partial class GloFace : IBiSerializable {
    [StringLengthSource(16)]
    public string TextureFilename { get; set; }

    public GloColor Color { get; } = new();
    public ushort Flags { get; set; }

    public GloVertexRef[] VertexRefs { get; } = new GloVertexRef[3] {
      new GloVertexRef(),
      new GloVertexRef(),
      new GloVertexRef(),
    };
  }

  [Schema]
  public sealed partial class GloVertexRef : IBiSerializable {
    public ushort Index { get; set; }
    public float U { get; set; }
    public float V { get; set; }
  }

  [Schema]
  public sealed partial class GloSprite : IBiSerializable {
    [StringLengthSource(16)]
    public string TextureFilename { get; set; }

    public GloColor Color { get; } = new();

    public GloXyz SpritePosition { get; } = new();
    public GloXy SpriteSize { get; } = new();
    public ushort SpriteFlags { get; set; }
  }


  [Schema]
  public sealed partial class GloXy : IBiSerializable {
    public float X { get; set; }
    public float Y { get; set; }
  }

  [Schema]
  public sealed partial class GloXyz : IBiSerializable {
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
  }

  [Schema]
  public sealed partial class GloColor : IBiSerializable {
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }
  }
}