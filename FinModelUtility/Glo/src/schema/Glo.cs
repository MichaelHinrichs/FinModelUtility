using fin.schema.color;
using fin.schema.vector;

using schema;
using schema.attributes.align;


namespace glo.schema {
  [BinarySchema]
  [Endianness(Endianness.LittleEndian)]
  public sealed partial class Glo : IBiSerializable {
    private readonly string magic_ = "GLO\0";

    public ushort Version { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT16)] public GloObject[] Objects { get; set; }
  }


  [BinarySchema]
  public sealed partial class GloObject : IBiSerializable {
    [ArrayLengthSource(SchemaIntegerType.UINT16)]
    public GloAnimSeg[] AnimSegs { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT16)] public GloMesh[] Meshes { get; set; }
  }

  [BinarySchema]
  public sealed partial class GloAnimSeg : IBiSerializable {
    [StringLengthSource(24)]
    public string Name { get; set; }
    public uint StartFrame { get; set; }
    public uint EndFrame { get; set; }
    public uint Flags { get; set; }
    public float Speed { get; set; }
  }


  [BinarySchema]
  public sealed partial class GloMesh : IBiSerializable {
    [StringLengthSource(24)]
    public string Name { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT16)]
    public GloXyzKey[] MoveKeys { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT16)]
    public GloScaleKey[] ScaleKeys { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT16)]
    public GloQuaternionKey[] RotateKeys { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT16)]
    public GloVertex[] Vertices { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT16)] public GloFace[] Faces { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT16)] public GloSprite[] Sprites { get; set; }

    [NumberFormat(SchemaNumberType.UN8)] public float MeshTranslucency { get; set; }
    private readonly byte padding_ = 0;

    public ushort MeshFlags { get; set; }

    public GloMeshPointers Pointers { get; } = new();
  }

  [BinarySchema]
  public sealed partial class GloMeshPointers : IBiSerializable {
    [IfBoolean(SchemaIntegerType.UINT16)] public GloMesh? Child { get; set; }

    [IfBoolean(SchemaIntegerType.UINT16)] public GloMesh? Next { get; set; }
  }

  [BinarySchema]
  public sealed partial class GloXyzKey : IBiSerializable {
    public uint Time { get; set; }
    public Vector3f Xyz { get; } = new();
  }

  [BinarySchema]
  public sealed partial class GloQuaternionKey : IBiSerializable {
    public uint Time { get; set; }

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float W { get; set; }
  }

  [BinarySchema]
  public sealed partial class GloScaleKey : IBiSerializable {
    public uint Time { get; set; }
    public Vector3f Scale { get; } = new();
  }

  [BinarySchema]
  public sealed partial class GloVertex : IBiSerializable {
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
  }

  [BinarySchema]
  public sealed partial class GloFace : IBiSerializable {
    [StringLengthSource(16)]
    public string TextureFilename { get; set; }

    public Rgba32 Color { get; } = new();
    public ushort Flags { get; set; }

    public GloVertexRef[] VertexRefs { get; } = {
        new(),
        new(),
        new(),
    };
  }

  [BinarySchema]
  public sealed partial class GloVertexRef : IBiSerializable {
    public ushort Index { get; set; }
    public float U { get; set; }
    public float V { get; set; }
  }

  [BinarySchema]
  public sealed partial class GloSprite : IBiSerializable {
    public char[] TextureFilename { get; } = new char[16];

    public Rgba32 Color { get; } = new();

    public Vector3f SpritePosition { get; } = new();
    public Vector2f SpriteSize { get; } = new();
    public ushort SpriteFlags { get; set; }
  }
}