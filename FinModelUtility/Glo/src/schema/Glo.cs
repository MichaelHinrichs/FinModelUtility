using schema;


namespace glo.schema {
  [Schema]
  public sealed partial class Glo : IBiSerializable {
    private readonly string magic_ = "GLO\0";

    public ushort Version { get; set; }

    [ArrayLengthSource(SchemaIntType.UINT16)] public GloObject[] Objects { get; set; }
  }


  [Schema]
  public sealed partial class GloObject : IBiSerializable {
    [ArrayLengthSource(SchemaIntType.UINT16)]
    public GloAnimSeg[] AnimSegs { get; set; }

    [ArrayLengthSource(SchemaIntType.UINT16)] public GloMesh[] Meshes { get; set; }
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

    [ArrayLengthSource(SchemaIntType.UINT16)]
    public GloXyzKey[] MoveKeys { get; set; }

    [ArrayLengthSource(SchemaIntType.UINT16)]
    public GloScaleKey[] ScaleKeys { get; set; }

    [ArrayLengthSource(SchemaIntType.UINT16)]
    public GloQuaternionKey[] RotateKeys { get; set; }

    [ArrayLengthSource(SchemaIntType.UINT16)]
    public GloVertex[] Vertices { get; set; }

    [ArrayLengthSource(SchemaIntType.UINT16)] public GloFace[] Faces { get; set; }

    [ArrayLengthSource(SchemaIntType.UINT16)] public GloSprite[] Sprites { get; set; }

    [Format(SchemaNumberType.UN8)] public float MeshTranslucency { get; set; }
    private readonly byte padding_ = 0;

    public ushort MeshFlags { get; set; }

    public GloMeshPointers Pointers { get; } = new();
  }

  [Schema]
  public sealed partial class GloMeshPointers : IBiSerializable {
    [IfBoolean(SchemaIntType.UINT16)] public GloMesh? Child { get; set; }

    [IfBoolean(SchemaIntType.UINT16)] public GloMesh? Next { get; set; }
  }

  [Schema]
  public sealed partial class GloXyzKey : IBiSerializable {
    public uint Time { get; set; }
    public GloXyz Xyz { get; } = new();
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
  public sealed partial class GloScaleKey : IBiSerializable {
    public uint Time { get; set; }
    public GloScale Scale { get; } = new();
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

    public GloVertexRef[] VertexRefs { get; } = {
        new(),
        new(),
        new(),
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
    public char[] TextureFilename { get; } = new char[16];

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
  public sealed partial class GloScale : IBiSerializable {
    public float Y { get; set; }
    public float X { get; set; }
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