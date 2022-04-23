using schema;


namespace glo.schema {
  [Schema]
  public sealed partial class Glo : IDeserializable {
    private readonly string magic_ = "GLO\0";

    public ushort Version { get; set; }

    [ArrayLengthSource(IntType.UINT16)] public GloObject[] Objects { get; set; }
  }


  [Schema]
  public sealed partial class GloObject : IDeserializable {
    [ArrayLengthSource(IntType.UINT16)]
    public GloAnimSeg[] AnimSegs { get; set; }

    [ArrayLengthSource(IntType.UINT16)] public GloMesh[] Meshes { get; set; }
  }

  [Schema]
  public sealed partial class GloAnimSeg : IDeserializable {
    public char[] Name { get; } = new char[24];
    public uint StartFrame { get; set; }
    public uint EndFrame { get; set; }
    public uint Flags { get; set; }
    public float Speed { get; set; }
  }


  [Schema]
  public sealed partial class GloMesh : IDeserializable {
    public char[] Name { get; } = new char[24];

    [ArrayLengthSource(IntType.UINT16)]
    public GloXyzKey[] MoveKeys { get; set; }

    [ArrayLengthSource(IntType.UINT16)]
    public GloXyzKey[] ScaleKeys { get; set; }

    [ArrayLengthSource(IntType.UINT16)]
    public GloQuaternionKey[] RotateKeys { get; set; }

    [ArrayLengthSource(IntType.UINT16)]
    public GloVertex[] Vertices { get; set; }

    [ArrayLengthSource(IntType.UINT16)] public GloFace[] Faces { get; set; }

    [ArrayLengthSource(IntType.UINT16)] public GloSprite[] Sprites { get; set; }

    public ushort MeshTranslucency { get; set; }
    public ushort MeshFlags { get; set; }

    public GloMeshPointers Pointers { get; } = new();
  }

  public sealed class GloMeshPointers : IDeserializable {
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
  }

  [Schema]
  public sealed partial class GloXyzKey : IDeserializable {
    public uint Time { get; set; }

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
  }

  [Schema]
  public sealed partial class GloQuaternionKey : IDeserializable {
    public uint Time { get; set; }

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float W { get; set; }
  }

  [Schema]
  public sealed partial class GloVertex : IDeserializable {
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
  }

  [Schema]
  public sealed partial class GloFace : IDeserializable {
    public char[] TextureFilename { get; } = new char[16];

    public GloColor Color { get; } = new();
    public ushort Flags { get; set; }

    public GloVertexRef[] VertexRefs { get; } = new GloVertexRef[3] {
        new GloVertexRef(),
        new GloVertexRef(),
        new GloVertexRef(),
    };
  }

  [Schema]
  public sealed partial class GloVertexRef : IDeserializable {
    public ushort Index { get; set; }
    public float U { get; set; }
    public float V { get; set; }
  }

  [Schema]
  public sealed partial class GloSprite : IDeserializable {
    public char[] TextureFilename { get; } = new char[16];

    public GloColor Color { get; } = new();

    public GloXyz SpritePosition { get; } = new();
    public GloXy SpriteSize { get; } = new();
    public ushort SpriteFlags { get; set; }
  }


  [Schema]
  public sealed partial class GloXy : IDeserializable {
    public float X { get; set; }
    public float Y { get; set; }
  }

  [Schema]
  public sealed partial class GloXyz : IDeserializable {
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
  }

  [Schema]
  public sealed partial class GloColor : IDeserializable {
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }
  }
}