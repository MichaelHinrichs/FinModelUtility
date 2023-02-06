using fin.schema.color;
using fin.schema.vector;

using schema.binary;
using schema.binary.attributes.endianness;


namespace glo.schema {
  [BinarySchema]
  [Endianness(Endianness.LittleEndian)]
  public sealed partial class Glo : IBinaryConvertible {
    private readonly string magic_ = "GLO\0";

    public ushort Version { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT16)] public GloObject[] Objects { get; set; }
  }


  [BinarySchema]
  public sealed partial class GloObject : IBinaryConvertible {
    [ArrayLengthSource(SchemaIntegerType.UINT16)]
    public GloAnimSeg[] AnimSegs { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT16)] public GloMesh[] Meshes { get; set; }
  }

  [BinarySchema]
  public sealed partial class GloAnimSeg : IBinaryConvertible {
    [StringLengthSource(24)]
    public string Name { get; set; }
    public uint StartFrame { get; set; }
    public uint EndFrame { get; set; }
    public uint Flags { get; set; }
    public float Speed { get; set; }
  }


  [Flags]
  public enum GloMeshFlags : ushort {
    GOURAUD_SHADED = 1 << 0,
    PRELIT = 1 << 3,
    FACE_COLOR = 1 << 10,
  }

  [BinarySchema]
  public sealed partial class GloMesh : IBinaryConvertible {
    [StringLengthSource(24)]
    public string Name { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT16)]
    public GloXyzKey[] MoveKeys { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT16)]
    public GloScaleKey[] ScaleKeys { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT16)]
    public GloQuaternionKey[] RotateKeys { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT16)]
    public Vector3f[] Vertices { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT16)] public GloFace[] Faces { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT16)] public GloSprite[] Sprites { get; set; }

    [NumberFormat(SchemaNumberType.UN8)] public float MeshTranslucency { get; set; }
    private readonly byte padding_ = 0;

    public GloMeshFlags MeshFlags { get; set; }

    public GloMeshPointers Pointers { get; } = new();
  }

  [BinarySchema]
  public sealed partial class GloMeshPointers : IBinaryConvertible {
    [IfBoolean(SchemaIntegerType.UINT16)] public GloMesh? Child { get; set; }

    [IfBoolean(SchemaIntegerType.UINT16)] public GloMesh? Next { get; set; }
  }

  [BinarySchema]
  public sealed partial class GloXyzKey : IBinaryConvertible {
    public uint Time { get; set; }
    public Vector3f Xyz { get; } = new();
  }

  [BinarySchema]
  public sealed partial class GloQuaternionKey : IBinaryConvertible {
    public uint Time { get; set; }

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float W { get; set; }
  }

  [BinarySchema]
  public sealed partial class GloScaleKey : IBinaryConvertible {
    public uint Time { get; set; }
    public Vector3f Scale { get; } = new();
  }

  [BinarySchema]
  public sealed partial class GloFace : IBinaryConvertible {
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
  public sealed partial class GloVertexRef : IBinaryConvertible {
    public ushort Index { get; set; }
    public float U { get; set; }
    public float V { get; set; }
  }

  [BinarySchema]
  public sealed partial class GloSprite : IBinaryConvertible {
    [StringLengthSource(16)]
    public string TextureFilename { get; set; }

    public Rgba32 Color { get; } = new();

    public Vector3f SpritePosition { get; } = new();
    public Vector2f SpriteSize { get; } = new();
    public ushort SpriteFlags { get; set; }
  }
}