using System.Numerics;

using fin.color;
using fin.util.color;

using gx;

using schema.binary;

namespace dat.schema {
  [Flags]
  public enum PObjFlags : ushort {
    OBJTYPE_SKIN = 0 << 12,
    OBJTYPE_SHAPEANIM = 1 << 12,
    OBJTYPE_ENVELOPE = 1 << 13,
    OBJTYPE_MASK = 0x3000,

    CULLFRONT = 1 << 14,
    CULLBACK = 1 << 15,
  }

  /// <summary>
  ///   Polygon object.
  /// </summary>
  public partial class PObj : IBinaryDeserializable {
    private readonly Dat dat_;

    public PObj(Dat dat) {
      this.dat_ = dat;
    }

    [BinarySchema]
    public partial class PObjHeader : IBinaryDeserializable {
      public uint StringOffset { get; set; }
      public uint NextPObjOffset { get; set; }
      public uint VertexDescriptorListOffset { get; set; }
      public PObjFlags Flags { get; set; }
      public ushort DisplayListSize { get; set; }
      public uint DisplayOffset { get; set; }
      public uint WeightListOffset { get; set; }
    }

    public PObjHeader Header { get; } = new();
    public PObj? NextPObj { get; private set; }

    public List<VertexDescriptor> VertexDescriptors { get; } = new();
    public List<DatPrimitive> Primitives { get; } = new();

    public void Read(IBinaryReader br) {
      this.Header.Read(br);

      // TODO: Read weights
      // https://github.com/jam1garner/Smash-Forge/blob/c0075bca364366bbea2d3803f5aeae45a4168640/Smash%20Forge/Filetypes/Melee/DAT.cs#L1515C21-L1515C38

      var pObjType = this.Header.Flags & PObjFlags.OBJTYPE_SKIN;
      switch (pObjType) {
        case PObjFlags.OBJTYPE_SKIN: {
          br.Position = this.Header.VertexDescriptorListOffset;

          // Reads vertex descriptors
          while (true) {
            var vertexDescriptor = new VertexDescriptor();
            vertexDescriptor.Read(br);

            if (vertexDescriptor.Attribute == GxAttribute.NULL) {
              break;
            }

            this.VertexDescriptors.Add(vertexDescriptor);
          }

          var startingOffset = br.Position = this.Header.DisplayOffset;

          // Reads display list
          while (br.Position - startingOffset < this.Header.DisplayListSize * 32) {
            var opcode = (GxOpcode) br.ReadByte();
            if (opcode == GxOpcode.NOP) {
              break;
            }

            switch (opcode) {
              case GxOpcode.LOAD_CP_REG: {
                var command = br.ReadByte();
                var value = br.ReadUInt32();

                // TODO: Is this actually needed???
                if (command == 0x50) {
                  this.dat_.VertexDescriptorValue &= ~((uint) 0x1FFFF);
                  this.dat_.VertexDescriptorValue |= value;
                } else if (command == 0x60) {
                  value <<= 17;
                  this.dat_.VertexDescriptorValue &= 0x1FFFF;
                  this.dat_.VertexDescriptorValue |= value;
                } else {
                  throw new NotImplementedException();
                }

                break;
              }
              case GxOpcode.LOAD_XF_REG: {
                var lengthMinusOne = br.ReadUInt16();
                var length = lengthMinusOne + 1;

                // http://hitmen.c02.at/files/yagcd/yagcd/chap5.html#sec5.11.4
                var firstXfRegisterAddress = br.ReadUInt16();

                var values = br.ReadUInt32s(length);
                // TODO: Implement
                break;
              }
              case GxOpcode.DRAW_TRIANGLES:
              case GxOpcode.DRAW_QUADS:
              case GxOpcode.DRAW_TRIANGLE_STRIP: {
                var vertexCount = br.ReadUInt16();
                var vertices = new DatVertex[vertexCount];

                for (var i = 0; i < vertexCount; ++i) {
                  var boneId = -1;
                  Vector3? position = null;
                  Vector3? normal = null;
                  Vector2? uv0 = null;
                  Vector2? uv1 = null;
                  IColor? color = null;

                  foreach (var vertexDescriptor in this.VertexDescriptors) {
                    var vertexAttribute = vertexDescriptor.Attribute;
                    var vertexFormat = vertexDescriptor.AttributeType;

                    if (vertexAttribute == GxAttribute.CLR0 &&
                        vertexFormat == GxAttributeType.DIRECT) {
                      switch (vertexDescriptor.ColorComponentType) {
                        case ColorComponentType.RGB565: {
                          color = ColorUtil.ParseRgb565(br.ReadUInt16());
                          break;
                        }
                        case ColorComponentType.RGB888: {
                          color = FinColor.FromRgbBytes(
                              br.ReadByte(),
                              br.ReadByte(),
                              br.ReadByte());
                          break;
                        }
                        case ColorComponentType.RGBX8888: {
                          color = FinColor.FromRgbBytes(
                              br.ReadByte(),
                              br.ReadByte(),
                              br.ReadByte());
                          br.ReadByte();
                          break;
                        }
                        case ColorComponentType.RGBA4444: {
                          ColorUtil.SplitRgba4444(
                              br.ReadUInt16(),
                              out var r,
                              out var g,
                              out var b,
                              out var a);
                          color = FinColor.FromRgbaBytes(r, g, b, a);
                          break;
                        }
                        case ColorComponentType.RGBA6: {
                          var c = br.ReadUInt24();
                          var r = ((((c >> 18) & 0x3F) << 2) |
                                   (((c >> 18) & 0x3F) >> 4)) / (float) 0xFF;
                          var g = ((((c >> 12) & 0x3F) << 2) |
                                   (((c >> 12) & 0x3F) >> 4)) / (float) 0xFF;
                          var b = ((((c >> 6) & 0x3F) << 2) |
                                   (((c >> 6) & 0x3F) >> 4)) / (float) 0xFF;
                          var a = ((((c) & 0x3F) << 2) | (((c) & 0x3F) >> 4)) /
                                  (float) 0xFF;
                          color = FinColor.FromRgbaFloats(r, g, b, a);
                          break;
                        }
                        case ColorComponentType.RGBA8888: {
                          color = FinColor.FromRgbaBytes(
                              br.ReadByte(),
                              br.ReadByte(),
                              br.ReadByte(),
                              br.ReadByte());
                          break;
                        }
                      }

                      continue;
                    }

                    var value = vertexFormat switch {
                        GxAttributeType.DIRECT => br.ReadByte(),
                        GxAttributeType.INDEX_8 => br.ReadByte(),
                        GxAttributeType.INDEX_16 => br.ReadUInt16(),
                        _ => throw new NotImplementedException(),
                    };

                    var offset = vertexDescriptor.ArrayOffset +
                                 vertexDescriptor.Stride * value;

                    switch (vertexAttribute) {
                      case GxAttribute.PNMTXIDX: {
                        boneId = value;
                        break;
                      }
                      case GxAttribute.POS: {
                        position = br.SubreadAt(
                            offset,
                            sbr => sbr.ReadVector3(vertexDescriptor));
                        break;
                      }
                      case GxAttribute.NRM: {
                        normal = br.SubreadAt(
                            offset,
                            sbr => sbr.ReadVector3(vertexDescriptor));
                        break;
                      }
                      case GxAttribute.TEX0: {
                        uv0 = br.SubreadAt(
                            offset,
                            sbr => sbr.ReadVector2(vertexDescriptor));
                        break;
                      }
                      case GxAttribute.TEX1: {
                        uv1 = br.SubreadAt(
                            offset,
                            sbr => sbr.ReadVector2(vertexDescriptor));
                        break;
                      }
                      default: throw new NotImplementedException();
                    }
                  }

                  if (position != null) {
                    vertices[i] = new DatVertex {
                        BoneId = boneId,
                        Position = position.Value,
                        Normal = normal,
                        Uv0 = uv0,
                        Uv1 = uv1,
                        Color = color,
                    };
                  }
                }

                this.Primitives.Add(new DatPrimitive {
                    Type = opcode,
                    Vertices = vertices
                });
                break;
              }
              default: {
                break;
              }
            }
          }

          break;
        }
        case PObjFlags.OBJTYPE_ENVELOPE: {
          break;
        }
        case PObjFlags.OBJTYPE_SHAPEANIM: {
          break;
        }
        default: throw new NotImplementedException();
      }

      if (this.Header.NextPObjOffset != 0) {
        br.Position = this.Header.NextPObjOffset;

        this.NextPObj = new PObj(this.dat_);
        this.NextPObj.Read(br);
      }
    }
  }
}