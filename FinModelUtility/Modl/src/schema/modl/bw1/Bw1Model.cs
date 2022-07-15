using System.Collections;
using System.Drawing.Text;

using fin.data;
using fin.math;
using fin.util.asserts;

using modl.schema.modl.bw1.node;
using modl.schema.modl.bw1.node.display_list;

using schema;


namespace modl.schema.modl.bw1 {
  public class Bw1Model : IDeserializable {
    public List<NodeBw1> Nodes { get; } = new();
    public ListDictionary<ushort, ushort> CnctParentToChildren { get; } = new();

    public void Read(EndianBinaryReader er) {
      var filenameLength = er.ReadUInt32();
      er.Position += filenameLength;

      er.AssertStringEndian("MODL");

      var size = er.ReadUInt32();
      var expectedEnd = er.Position + size;

      var nodeCount = er.ReadUInt16();
      var additionalDataCount = er.ReadByte();

      var padding = er.ReadByte();

      var someCount = er.ReadUInt32();
      var unknown0 = er.ReadSingles(4);

      var additionalData = er.ReadUInt32s(additionalDataCount);

      this.SkipSection_(er, "XMEM");

      // Reads in nodes (bones)
      {
        this.Nodes.Clear();
        for (var i = 0; i < nodeCount; ++i) {
          var node = new NodeBw1(additionalDataCount);
          node.Read(er);
          this.Nodes.Add(node);
        }
      }

      // Reads in hierarchy, how nodes are "CoNneCTed" or "CoNCaTenated?"?
      {
        er.AssertStringEndian("CNCT");

        var cnctSize = er.ReadUInt32();
        var cnctCount = cnctSize / 4;

        this.CnctParentToChildren.Clear();
        for (var i = 0; i < cnctCount; ++i) {
          var parent = er.ReadUInt16();
          var child = er.ReadUInt16();

          this.CnctParentToChildren.Add(parent, child);
        }
      }

      Asserts.Equal(expectedEnd, er.Position);
    }

    private void SkipSection_(EndianBinaryReader er, string sectionName) {
      er.AssertStringEndian(sectionName);
      var size = er.ReadUInt32();
      var data = er.ReadBytes((int) size);
      ;
    }
  }

  public class NodeBw1 : IDeserializable {
    private int additionalDataCount_;

    public BwTransform Transform { get; } = new();
    public Bw1BoundingBox BoundingBox { get; } = new();

    public float Scale { get; set; }

    public List<Bw1Material> Materials { get; } = new();

    public NodeBw1(int additionalDataCount) {
      this.additionalDataCount_ = additionalDataCount;
    }

    public void Read(EndianBinaryReader er) {
      er.AssertStringEndian("NODE");

      var nodeSize = er.ReadUInt32();
      var nodeStart = er.Position;
      var expectedNodeEnd = nodeStart + nodeSize;

      var headerStart = er.Position;
      var expectedHeaderEnd = headerStart + 0x38;
      {
        // TODO: What are these used for?
        var someMin = er.ReadUInt16();
        var someMax = er.ReadUInt16();

        // TODO: unknown, probably enum values
        var unknowns0 = er.ReadUInt32s(2);

        this.Transform.Read(er);

        // TODO: unknown, also transform??
        // These look very similar to the values defined in the constructor
        var unknowns1 = er.ReadSingles(4);

        ;
      }
      Asserts.Equal(er.Position, expectedHeaderEnd);

      // TODO: additional data
      var additionalData = er.ReadUInt32s(this.additionalDataCount_);
      ;

      this.BoundingBox.Read(er);

      var sectionName = er.ReadStringEndian(4);
      var sectionSize = er.ReadInt32();

      while (sectionName != "MATL") {
        if (sectionName == "VSCL") {
          Asserts.Equal(4, sectionSize);
          this.Scale = er.ReadSingle();
        } else if (sectionName == "RNOD") {
          this.ReadRnod_(er);
        } else {
          throw new NotImplementedException();
        }

        sectionName = er.ReadStringEndian(4);
        sectionSize = er.ReadInt32();
      }

      Asserts.Equal("MATL", sectionName);

      var materialSize = 0x48;
      Asserts.Equal(0, sectionSize % materialSize);

      this.Materials.Clear();
      for (var i = 0; i < sectionSize / materialSize; ++i) {
        this.Materials.Add(er.ReadNew<Bw1Material>());
      }

      var vertexDescriptorValue = (uint) 0;
      while (er.Position < expectedNodeEnd) {
        sectionName = er.ReadStringEndian(4);
        sectionSize = er.ReadInt32();

        var expectedSectionEnd = er.Position + sectionSize;

        switch (sectionName) {
          case "VUV1":
          case "VUV2":
          case "VUV3":
          case "VUV4": {
            // TODO: Need to keep track of section order
            var uvMapIndex = sectionName[3] - '1';
            this.ReadUvMap_(er, uvMapIndex, sectionSize / (2 * 2));
            break;
          }
          case "VPOS": {
            // TODO: Handle this properly
            // Each new VPOS section seems to correspond to a new LOD mesh, but we only need the first one.
            if (Positions.Count > 0) {
              er.Position = expectedNodeEnd;
              goto BreakEarly;
            }

            var vertexPositionSize = 2 * 3;
            Asserts.Equal(0, sectionSize % vertexPositionSize);
            this.ReadPositions_(er, sectionSize / vertexPositionSize);
            break;
          }
          case "VNRM": {
            var normalSize = 3;
            Asserts.Equal(0, sectionSize % normalSize);
            this.ReadNormals_(er, sectionSize / normalSize);
            break;
          }
          case "VNBT": {
            var endianness = er.Endianness;
            er.Endianness = Endianness.BigEndian;

            var nbtSize = 4 * 9;
            Asserts.Equal(0, sectionSize % nbtSize);
            var nbtCount = sectionSize / nbtSize;
            for (var i = 0; i < nbtCount; ++i) {
              this.Normals.Add(new VertexNormal {
                  X = er.ReadSingle(),
                  Y = er.ReadSingle(),
                  Z = er.ReadSingle(),
              });
              er.Position += 24;
            }

            er.Endianness = endianness;
            break;
          }
          case "XBST": {
            this.ReadOpcodes_(er, sectionSize, ref vertexDescriptorValue);
            break;
          }
          case "SCNT": {
            // TODO: Support this
            // This explains why multiple VPOS sections are included.
            Asserts.Equal(4, sectionSize);
            var lodCount = er.ReadUInt32();
            break;
          }
          case "VCOL": {
            er.Position += sectionSize;
            break;
          }
          case "ANIM": {
            er.Position += sectionSize;
            break;
          }
          default: throw new NotImplementedException();
        }

        Asserts.Equal(er.Position, expectedSectionEnd);
      }

      BreakEarly: ;
      Asserts.Equal(er.Position, expectedNodeEnd);
    }


    public Bw1RnodMatrix[] Matrices { get; set; }

    private void ReadRnod_(EndianBinaryReader er) {
      var size = er.ReadUInt32();
      this.Matrices = new Bw1RnodMatrix[size];

      for (var i = 0; i < this.Matrices.Length; ++i) {
        this.Matrices[i] = er.ReadNew<Bw1RnodMatrix>();
      }
    }


    public Uv[][] UvMaps { get; } = new Uv[4][];

    private void ReadUvMap_(EndianBinaryReader er,
                            int uvMapIndex,
                            int uvCount) {
      var endianness = er.Endianness;
      er.Endianness = Endianness.BigEndian;

      var scale = MathF.Pow(2, 11);
      var uvMap = this.UvMaps[uvMapIndex] = new Uv[uvCount];
      for (var i = 0; i < uvCount; ++i) {
        uvMap[i] = new Uv {
            U = er.ReadInt16() / scale,
            V = er.ReadInt16() / scale,
        };
      }

      er.Endianness = endianness;
    }

    public class Uv {
      public float U { get; set; }
      public float V { get; set; }
    }


    public List<VertexPosition> Positions { get; } = new();

    private void ReadPositions_(EndianBinaryReader er, int vertexCount) {
      var endianness = er.Endianness;
      er.Endianness = Endianness.BigEndian;

      for (var i = 0; i < vertexCount; ++i) {
        this.Positions.Add(er.ReadNew<VertexPosition>());
      }

      er.Endianness = endianness;
    }


    public List<VertexNormal> Normals { get; } = new();

    private void ReadNormals_(EndianBinaryReader er, int vertexCount) {
      var endianness = er.Endianness;
      er.Endianness = Endianness.BigEndian;

      for (var i = 0; i < vertexCount; ++i) {
        this.Normals.Add(er.ReadNew<VertexNormal>());
      }

      er.Endianness = endianness;
    }

    public List<BwMesh> Meshes { get; } = new();

    private void ReadOpcodes_(EndianBinaryReader er,
                              int sectionSize,
                              ref uint vertexDescriptorValue) {
      var endianness = er.Endianness;
      er.Endianness = Endianness.BigEndian;

      var start = er.Position;
      var expectedEnd = start + sectionSize;

      var materialIndex = er.ReadUInt32();

      var flags = er.ReadUInt32();
      var gxDataSize = er.ReadUInt32();

      Asserts.Equal(expectedEnd, er.Position + gxDataSize);

      var triangleStrips = new List<BwTriangleStrip>();
      var mesh = new BwMesh {
          Flags = flags,
          MaterialIndex = materialIndex,
          TriangleStrips = triangleStrips
      };
      this.Meshes.Add(mesh);

      while (er.Position < expectedEnd) {
        var opcode = er.ReadByte();
        var opcodeEnum = (BwOpcode) opcode;

        if (opcodeEnum == BwOpcode.LOAD_CP_REG) {
          var command = er.ReadByte();
          var value = er.ReadUInt32();

          if (command == 0x50) {
            vertexDescriptorValue &= ~ ((uint) 0x1FFFF);
            vertexDescriptorValue |= value;
          } else if (command == 0x60) {
            value <<= 17;
            vertexDescriptorValue &= 0x1FFFF;
            vertexDescriptorValue |= value;
          } else {
            throw new NotImplementedException();
          }
        } else if (opcodeEnum == BwOpcode.LOAD_XF_REG) {
          var lengthMinusOne = er.ReadUInt16();
          var length = lengthMinusOne + 1;

          // http://hitmen.c02.at/files/yagcd/yagcd/chap5.html#sec5.11.4
          var firstXfRegisterAddress = er.ReadUInt16();

          var values = er.ReadUInt32s(length);
          // TODO: Complete
        } else if (opcodeEnum == BwOpcode.TRIANGLE_STRIP) {
          var vertexAttributeIndicesList = new List<BwVertexAttributeIndices>();

          var vertexDescriptor = new VertexDescriptor();
          vertexDescriptor.FromValue(vertexDescriptorValue);

          var triangleStrip = new BwTriangleStrip {
              VertexAttributeIndicesList = vertexAttributeIndicesList,
          };
          triangleStrips.Add(triangleStrip);

          var vertexCount = er.ReadUInt16();
          for (var i = 0; i < vertexCount; ++i) {
            var vertexAttributeIndices = new BwVertexAttributeIndices {
                Fraction = 1d * i / vertexCount
            };
            vertexAttributeIndicesList.Add(vertexAttributeIndices);

            foreach (var (vertexAttribute, vertexFormat) in
                     vertexDescriptor) {
              var value = vertexFormat switch {
                  null                  => er.ReadByte(),
                  VertexFormat.INDEX_8  => er.ReadByte(),
                  VertexFormat.INDEX_16 => er.ReadUInt16(),
                  _                     => throw new NotImplementedException(),
              };

              switch (vertexAttribute) {
                case VertexAttribute.PosMatIdx: {
                  Asserts.Equal(0, value % 3);
                  value /= 3;
                  vertexAttributeIndices.NodeIndex = value;
                  break;
                }
                case VertexAttribute.Position: {
                  vertexAttributeIndices.PositionIndex = value;
                  break;
                }
                case VertexAttribute.Normal: {
                  vertexAttributeIndices.NormalIndex = value;
                  break;
                }
                case VertexAttribute.Tex0Coord:
                case VertexAttribute.Tex1Coord:
                case VertexAttribute.Tex2Coord:
                case VertexAttribute.Tex3Coord:
                case VertexAttribute.Tex4Coord:
                case VertexAttribute.Tex5Coord:
                case VertexAttribute.Tex6Coord:
                case VertexAttribute.Tex7Coord: {
                  var index = vertexAttribute - VertexAttribute.Tex0Coord;
                  vertexAttributeIndices.TexCoordIndices[index] = value;
                  break;
                }
                case VertexAttribute.Color0:
                case VertexAttribute.Color1: {
                  break;
                }
                default: {
                  throw new NotImplementedException();
                }
              }
            }
          }
        } else if (opcodeEnum == BwOpcode.NOP) { } else {
          throw new NotImplementedException();
        }
      }

      er.Endianness = endianness;
      Asserts.Equal(expectedEnd, er.Position);
    }

    public enum BwOpcode : byte {
      NOP = 0x0,
      LOAD_CP_REG = 0x8,
      LOAD_XF_REG = 0x10,
      TRIANGLE_STRIP = 0x98,
    }


    public class
        VertexDescriptor : IEnumerable<(VertexAttribute, VertexFormat?)> {
      public bool HasPosMat { get; set; }
      public bool[] HasTexMats { get; } = new bool[8];
      public VertexFormat PositionFormat { get; set; }
      public VertexFormat NormalFormat { get; set; }
      public VertexFormat[] ColorFormats { get; } = new VertexFormat[2];
      public bool[] HasTexCoord { get; } = new bool[8];

      public void FromValue(uint value) {
        var posMatFlag = value & 1;
        this.HasPosMat = posMatFlag == 1;
        value >>= 1;

        for (var i = 0; i < 8; ++i) {
          this.HasTexMats[i] = (value & 1) == 1;
          value >>= 1;
        }

        var positionFormatFlag = value & 3;
        this.PositionFormat = (VertexFormat) positionFormatFlag;
        value >>= 2;

        var normalFormatFlag = value & 3;
        this.NormalFormat = (VertexFormat) normalFormatFlag;
        value >>= 2;

        for (var i = 0; i < 2; ++i) {
          this.ColorFormats[i] = (VertexFormat) (value & 3);
          value >>= 2;
        }

        for (var i = 0; i < 8; ++i) {
          this.HasTexCoord[i] = (value & 1) == 1;
          value >>= 1;
        }

        if (value != 0) {
          throw new NotImplementedException();
        }
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public IEnumerator<(VertexAttribute, VertexFormat?)> GetEnumerator() {
        if (this.HasPosMat) {
          yield return (VertexAttribute.PosMatIdx, null);
        }

        for (var i = 0; i < this.HasTexMats.Length; ++i) {
          if (this.HasTexMats[i]) {
            yield return (VertexAttribute.Tex0MatIdx + i, null);
          }
        }

        if (this.PositionFormat != VertexFormat.NOT_PRESENT) {
          yield return (VertexAttribute.Position, this.PositionFormat);
        }
        if (this.NormalFormat != VertexFormat.NOT_PRESENT) {
          yield return (VertexAttribute.Normal, this.NormalFormat);
        }
        for (var i = 0; i < this.ColorFormats.Length; ++i) {
          var colorFormat = this.ColorFormats[i];
          if (colorFormat != VertexFormat.NOT_PRESENT) {
            yield return (VertexAttribute.Color0 + i, colorFormat);
          }
        }

        for (var i = 0; i < this.HasTexCoord.Length; ++i) {
          if (this.HasTexCoord[i]) {
            yield return (VertexAttribute.Tex0Coord + i, null);
          }
        }
      }
    }

    public enum VertexAttribute {
      PosMatIdx,

      Tex0MatIdx,
      Tex1MatIdx,
      Tex2MatIdx,
      Tex3MatIdx,
      Tex4MatIdx,
      Tex5MatIdx,
      Tex6MatIdx,
      Tex7MatIdx,

      Position,
      Normal,

      Color0,
      Color1,

      Tex0Coord,
      Tex1Coord,
      Tex2Coord,
      Tex3Coord,
      Tex4Coord,
      Tex5Coord,
      Tex6Coord,
      Tex7Coord,
    }

    public enum VertexFormat : byte {
      NOT_PRESENT = 0,
      DIRECT = 1,
      INDEX_8 = 2,
      INDEX_16 = 3
    }

    public class BwMesh {
      public uint Flags { get; set; }
      public uint MaterialIndex { get; set; }
      public List<BwTriangleStrip> TriangleStrips { get; set; }
    }

    public class BwTriangleStrip {
      public List<BwVertexAttributeIndices> VertexAttributeIndicesList {
        get;
        set;
      }
    }

    public class BwVertexAttributeIndices {
      public double Fraction { get; set; }
      public ushort PositionIndex { get; set; }
      public ushort? NormalIndex { get; set; }
      public ushort? NodeIndex { get; set; }
      public ushort?[] TexCoordIndices { get; } = new ushort?[8];
    }
  }
}