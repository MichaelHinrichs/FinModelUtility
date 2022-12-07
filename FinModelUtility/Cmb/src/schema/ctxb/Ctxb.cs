﻿using cmb.schema.cmb;
using fin.util.strings;
using schema;
using schema.attributes.child_of;
using schema.attributes.endianness;
using schema.attributes.memory;
using schema.attributes.size;
using System.IO;


namespace cmb.schema.ctxb {
  [BinarySchema]
  [Endianness(Endianness.LittleEndian)]
  public partial class Ctxb : IBiSerializable {
    public CtxbHeader Header { get; } = new();
    public CtxbTexChunk Chunk { get; } = new();
  }

  [BinarySchema]
  public partial class CtxbHeader : IChildOf<Ctxb>, IBiSerializable {
    public Ctxb Parent { get; set; }

    private readonly string magic_ = "ctxb";

    [SizeOfStreamInBytes]
    public int CtxbSize { get; private set; }

    private readonly uint texCount_ = 1;
    private readonly uint padding_ = 0;

    [PointerTo($"{nameof(Parent)}.{nameof(Ctxb.Chunk)}")]
    public int ChunkOffset { get; private set; }

    [PointerTo($"{nameof(Parent)}.{nameof(Ctxb.Chunk)}.{nameof(CtxbTexChunk.Entry)}.{nameof(CtxbTexEntry.Data)}")]
    public int DataOffset { get; private set; }
  }

  [BinarySchema]
  public partial class CtxbTexChunk : IBiSerializable {
    private readonly string magic_ = "tex" + AsciiUtil.GetChar(0x20);

    public int ChunkSize { get; private set; }

    private readonly uint texCount_ = 1;

    public CtxbTexEntry Entry { get; } = new();
  }

  [BinarySchema]
  public partial class CtxbTexEntry : IBiSerializable {
    public uint DataLength { get; private set; }
    public ushort mimapCount { get; private set; }

    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool isEtc1 { get; private set; }

    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool isCubemap { get; private set; }

    public ushort width { get; private set; }
    public ushort height { get; private set; }
    public GlTextureFormat imageFormat { get; private set; }

    [StringLengthSource(16)]
    public string name { get; private set; }

    private uint padding_;

    [ArrayLengthSource(nameof(DataLength))]
    public byte[] Data { get; private set; }
  }
}