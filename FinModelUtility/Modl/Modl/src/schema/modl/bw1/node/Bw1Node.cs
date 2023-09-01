using fin.schema;
using fin.schema.data;
using fin.schema.matrix;
using fin.util.asserts;
using fin.util.strings;

using gx;

using modl.schema.modl.common;

using schema.binary;

namespace modl.schema.modl.bw1.node {
  public class Bw1Node : IBwNode, IBinaryDeserializable {
    private int additionalDataCount_;

    public uint WeirdId { get; set; }
    public bool IsHidden => (WeirdId & 0x80) != 0;

    public BwTransform Transform { get; } = new();

    public AutoStringMagicUInt32SizedSection<BwBoundingBox> BoundingBox { get; } =
      new("BBOX".Reverse());

    public float Scale { get; set; }

    public List<IBwMaterial> Materials { get; } = new();

    public Bw1Node(int additionalDataCount) {
      this.additionalDataCount_ = additionalDataCount;
    }

    public static string GetIdentifier(uint weirdId) => $"Node {weirdId}";

    public string GetIdentifier() => Bw1Node.GetIdentifier(this.WeirdId);

    [Unknown]
    public void Read(IEndianBinaryReader er) {
      SectionHeaderUtil.AssertNameAndReadSize(er, "NODE", out var nodeSize);
      var nodeStart = er.Position;
      var expectedNodeEnd = nodeStart + nodeSize;

      er.PushMemberEndianness(Endianness.LittleEndian);

      var headerStart = er.Position;
      var expectedHeaderEnd = headerStart + 0x38;
      {
        // TODO: What are these used for?
        var someMin = er.ReadUInt16();
        var someMax = er.ReadUInt16();

        this.WeirdId = someMin;

        // TODO: unknown, probably enum values
        var unknowns0 = er.ReadUInt32s(2);

        this.Transform.Read(er);

        // TODO: unknown, also transform??
        // These look very similar to the values defined in the constructor
        var unknowns1 = er.ReadSingles(4);
      }
      Asserts.Equal(er.Position, expectedHeaderEnd);

      // TODO: additional data
      var additionalData = er.ReadUInt32s(this.additionalDataCount_);

      this.BoundingBox.Read(er);

      SectionHeaderUtil.ReadNameAndSize(er,
                                        out var sectionName,
                                        out var sectionSize);

      while (sectionName != "MATL") {
        if (sectionName == "VSCL") {
          Asserts.Equal(4, (int) sectionSize);
          this.Scale = er.ReadSingle();
        } else if (sectionName == "RNOD") {
          this.ReadRnod_(er);
        } else {
          throw new NotImplementedException();
        }

        SectionHeaderUtil.ReadNameAndSize(er,
                                          out sectionName,
                                          out sectionSize);
      }

      Asserts.Equal("MATL", sectionName);

      var materialSize = 0x48;
      Asserts.Equal(0, sectionSize % materialSize);

      this.Materials.Clear();
      for (var i = 0; i < sectionSize / materialSize; ++i) {
        this.Materials.Add(er.ReadNew<Bw1Material>());
      }

      er.PopEndianness();

      var vertexDescriptor = new GxVertexDescriptor();
      while (er.Position < expectedNodeEnd) {
        SectionHeaderUtil.ReadNameAndSize(er,
                                          out sectionName,
                                          out sectionSize);

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
            this.ReadPositions_(er, (uint) (sectionSize / vertexPositionSize));
            break;
          }
          case "VNRM": {
            var normalSize = 3;
            Asserts.Equal(0, sectionSize % normalSize);
            this.ReadNormals_(er, (uint) (sectionSize / normalSize));
            break;
          }
          case "VNBT": {
            var nbtSize = 4 * 9;
            Asserts.Equal(0, sectionSize % nbtSize);
            var nbtCount = sectionSize / nbtSize;
            for (var i = 0; i < nbtCount; ++i) {
              this.Normals.Add(new VertexNormal {
                  X = er.ReadSingle(), Y = er.ReadSingle(), Z = er.ReadSingle(),
              });
              er.Position += 24;
            }

            break;
          }
          case "XBST": {
            this.ReadOpcodes_(er, sectionSize, ref vertexDescriptor);
            break;
          }
          case "SCNT": {
            // TODO: Support this
            // This explains why multiple VPOS sections are included.
            Asserts.Equal(4, (int) sectionSize);
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
          case "FACE": {
            // TODO: Support this
            break;
          }
          default: throw new NotImplementedException();
        }

        Asserts.Equal(er.Position, expectedSectionEnd);
      }

      BreakEarly: ;
      Asserts.Equal(er.Position, expectedNodeEnd);
    }


    public Matrix4x4f[] RnodMatrices { get; set; }

    private void ReadRnod_(IEndianBinaryReader er) {
      var size = er.ReadInt32();
      this.RnodMatrices = er.ReadNewArray<Matrix4x4f>(size);
    }


    public VertexUv[][] UvMaps { get; } = new VertexUv[4][];

    private void ReadUvMap_(IEndianBinaryReader er,
                            int uvMapIndex,
                            uint uvCount) {
      var scale = MathF.Pow(2, 11);
      var uvMap = this.UvMaps[uvMapIndex] = new VertexUv[uvCount];
      for (var i = 0; i < uvCount; ++i) {
        uvMap[i] = new VertexUv {
            U = er.ReadInt16() / scale, V = er.ReadInt16() / scale,
        };
      }
    }


    public List<VertexPosition> Positions { get; } = new();

    private void ReadPositions_(IEndianBinaryReader er, uint vertexCount)
      => this.Positions.AddRange(
          er.ReadNewArray<VertexPosition>((int) vertexCount));


    public List<VertexNormal> Normals { get; } = new();

    private void ReadNormals_(IEndianBinaryReader er, uint vertexCount)
      => this.Normals.AddRange(
          er.ReadNewArray<VertexNormal>((int) vertexCount));

    public List<BwMesh> Meshes { get; } = new();

    private void ReadOpcodes_(IEndianBinaryReader er,
                              uint sectionSize,
                              ref GxVertexDescriptor vertexDescriptor) {
      var start = er.Position;
      var expectedEnd = start + sectionSize;

      var materialIndex = er.ReadUInt32();

      var posMatIdxMap = er.ReadNew<Bw1PosMatIdxMap>();

      var gxDataSize = er.ReadUInt32();
      Asserts.Equal(expectedEnd, er.Position + gxDataSize);

      var triangleStrips = new List<BwTriangleStrip>();
      var mesh = new BwMesh {
          MaterialIndex = materialIndex, TriangleStrips = triangleStrips
      };
      this.Meshes.Add(mesh);

      while (er.Position < expectedEnd) {
        var opcode = er.ReadByte();
        var opcodeEnum = (GxOpcode) opcode;

        if (opcodeEnum == GxOpcode.LOAD_CP_REG) {
          var command = er.ReadByte();
          var value = er.ReadUInt32();

          if (command == 0x50) {
            vertexDescriptor =
                new GxVertexDescriptor(
                    (vertexDescriptor.Value & ~((uint) 0x1FFFF)) |
                    value);
          } else if (command == 0x60) {
            vertexDescriptor =
                new GxVertexDescriptor(
                    (vertexDescriptor.Value & 0x1FFFF) |
                    (value << 17));
          } else {
            throw new NotImplementedException();
          }
        } else if (opcodeEnum == GxOpcode.LOAD_XF_REG) {
          var lengthMinusOne = er.ReadUInt16();
          var length = lengthMinusOne + 1;

          // http://hitmen.c02.at/files/yagcd/yagcd/chap5.html#sec5.11.4
          var firstXfRegisterAddress = er.ReadUInt16();

          var values = er.ReadUInt32s(length);
          // TODO: Implement
        } else if (opcodeEnum == GxOpcode.DRAW_TRIANGLE_STRIP) {
          var vertexCount = er.ReadUInt16();
          var vertexAttributeIndicesList =
              new List<BwVertexAttributeIndices>(vertexCount);

          var triangleStrip = new BwTriangleStrip {
              VertexAttributeIndicesList = vertexAttributeIndicesList,
          };
          triangleStrips.Add(triangleStrip);

          for (var i = 0; i < vertexCount; ++i) {
            var vertexAttributeIndices = new BwVertexAttributeIndices {
                TexCoordIndices = new ushort?[8],
            };

            foreach (var (vertexAttribute, vertexFormat) in
                     vertexDescriptor) {
              var value = vertexFormat switch {
                  null => er.ReadByte(),
                  GxAttributeType.INDEX_8 => er.ReadByte(),
                  GxAttributeType.INDEX_16 => er.ReadUInt16(),
                  _ => throw new NotImplementedException(),
              };

              switch (vertexAttribute) {
                case GxVertexAttribute.PosMatIdx: {
                  Asserts.Equal(0, value % 3);
                  value /= 3;
                  vertexAttributeIndices.NodeIndex = posMatIdxMap[value];
                  break;
                }
                case GxVertexAttribute.Position: {
                  vertexAttributeIndices.PositionIndex = value;
                  break;
                }
                case GxVertexAttribute.Normal: {
                  vertexAttributeIndices.NormalIndex = value;
                  break;
                }
                case GxVertexAttribute.Tex0Coord:
                case GxVertexAttribute.Tex1Coord:
                case GxVertexAttribute.Tex2Coord:
                case GxVertexAttribute.Tex3Coord:
                case GxVertexAttribute.Tex4Coord:
                case GxVertexAttribute.Tex5Coord:
                case GxVertexAttribute.Tex6Coord:
                case GxVertexAttribute.Tex7Coord: {
                  var index = vertexAttribute - GxVertexAttribute.Tex0Coord;
                  vertexAttributeIndices.TexCoordIndices[index] = value;
                  break;
                }
                case GxVertexAttribute.Color0:
                case GxVertexAttribute.Color1: {
                  break;
                }
                default: {
                  throw new NotImplementedException();
                }
              }
            }

            vertexAttributeIndicesList.Add(vertexAttributeIndices);
          }
        } else if (opcodeEnum == GxOpcode.NOP) { } else {
          throw new NotImplementedException();
        }
      }

      Asserts.Equal(expectedEnd, er.Position);
    }
  }
}