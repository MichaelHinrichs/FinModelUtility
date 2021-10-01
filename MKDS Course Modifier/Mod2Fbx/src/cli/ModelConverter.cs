using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.model;
using fin.model.impl;
using fin.util.asserts;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using mod.gcn;
using mod.util;

using Endianness = mod.util.Endianness;

namespace mod.cli {
  public static class ModelConverter {
    public enum Opcode {
      QUADS = 0x80,
      TRIANGLES = 0x90,
      TRIANGLE_STRIP = 0x98,
      TRIANGLE_FAN = 0xa0,
      LINES = 0xa8,
      LINE_STRIP = 0xb0,
      POINTS = 0xb8,
    }

    public static IModel Convert(Mod mod) {
      var model = new ModelImpl();

      var hasVertices = mod.vertices.Any();
      var hasNormals = mod.vnormals.Any();
      var hasFaces = mod.colltris.collinfo.Any();

      if (!hasVertices && !hasNormals && !hasFaces) {
        Asserts.Fail("Loaded file has nothing to export!");
      }


      /*var colInfos = mod.colltris.collinfo;
      if (colInfos.Count != 0) {
        os.WriteLine();
        os.WriteLine("o collision mesh");
        foreach (var colInfo in colInfos) {
          os.WriteLine(
              $"f ${colInfo.indice.X + 1} ${colInfo.indice.Y + 1} ${colInfo.indice.Z + 1}");
        }
      }*/


      // Writes bones
      var jointCount = mod.joints.Count;
      var jointChildren = new List<int>[jointCount];
      for (var i = 0; i < jointCount; ++i) {
        jointChildren[i] = new();
      }
      for (var i = 0; i < jointCount; ++i) {
        var joint = mod.joints[i];
        var parentIndex = (int) joint.parentIdx;
        if (parentIndex != -1) {
          jointChildren[parentIndex].Add(i);
        }
      }
      // TODO: Is 0 as the first a safe assumption?
      var jointQueue = new Queue<(int, IBone?)>();
      jointQueue.Enqueue((0, null));
      while (jointQueue.Count > 0) {
        var (jointIndex, parent) = jointQueue.Dequeue();

        var joint = mod.joints[jointIndex];

        var bone =
            (parent ?? model.Skeleton.Root).AddChild(
                joint.position.X,
                joint.position.Y,
                joint.position.Z);
        bone.SetLocalRotationRadians(joint.rotation.X,
                                     joint.rotation.Y,
                                     joint.rotation.Z);

        if (mod.jointNames.Count > 0) {
          var jointName = mod.jointNames[jointIndex];
          bone.Name = jointName;
        }

        foreach (var childIndex in jointChildren[jointIndex]) {
          jointQueue.Enqueue((childIndex, bone));
        }
      }

      // Writes skin
      foreach (var mesh in mod.meshes) {
        ModelConverter.AddMesh_(mod, mesh, model);
      }

      return model;
    }

    private static void AddMesh_(Mod mod, Mesh mesh, IModel model) {
      var vertexDescriptor = new VertexDescriptor();
      vertexDescriptor.FromPikmin1(mesh.vtxDescriptor, mod.hasNormals);

      foreach (var meshPacket in mesh.packets) {
        foreach (var dlist in meshPacket.displaylists) {
          var reader = new VectorReader(dlist.dlistData, 0, Endianness.Big);

          while (reader.GetRemaining() != 0) {
            var opcodeByte = reader.ReadU8();
            var opcode = (Opcode) opcodeByte;

            if (opcode != Opcode.TRIANGLE_STRIP &&
                opcode != Opcode.TRIANGLE_FAN) {
              continue;
            }

            var faceCount = reader.ReadU16();
            var positionIndices = new List<ushort>();
            var normalIndices = new List<ushort>();

            var texCoordIndices = new List<ushort>[8];
            for (var t = 0; t < 8; ++t) {
              texCoordIndices[t] = new List<ushort>();
            }

            for (var f = 0; f < faceCount; f++) {
              foreach (var (attr, format) in vertexDescriptor) {
                if (format == null) {
                  reader.ReadU8();
                  continue;
                }

                if (attr == Vtx.Position) {
                  positionIndices.Add(ModelConverter.Read_(reader, format));
                } else if (attr == Vtx.Normal) {
                  normalIndices.Add(ModelConverter.Read_(reader, format));
                } else if (attr is >= Vtx.Tex0Coord and <= Vtx.Tex7Coord) {
                  texCoordIndices[attr - Vtx.Tex0Coord]
                      .Add(ModelConverter.Read_(reader, format));
                } else if (format == VtxFmt.INDEX16) {
                  reader.ReadU16();
                } else {
                  Asserts.Fail(
                      $"Unexpected attribute/format ({attr}/{format})");
                }
              }
            }

            var finVertexList = new List<IVertex>();
            for (var v = 0; v < positionIndices.Count; ++v) {
              var position = mod.vertices[positionIndices[v]];
              var finVertex =
                  model.Skin.AddVertex(position.X, position.Y, position.Z);

              if (normalIndices.Count > 0) {
                var normal = mod.vnormals[normalIndices[v]];
                finVertex.SetLocalNormal(normal.X, normal.Y, normal.Z);
              }

              for (var t = 0; t < 8; ++t) {
                if (texCoordIndices[t].Count > 0) {
                  var texCoord = mod.texcoords[t][texCoordIndices[t][v]];
                  finVertex.SetUv(t, texCoord.X, texCoord.Y);
                }
                texCoordIndices[t] = new List<ushort>();
              }

              finVertexList.Add(finVertex);
            }

            var finVertices = finVertexList.ToArray();
            if (opcode == Opcode.TRIANGLE_FAN) {
              model.Skin.AddTriangleFan(finVertices);
            } else if (opcode == Opcode.TRIANGLE_STRIP) {
              model.Skin.AddTriangleStrip(finVertices);
            }
          }
        }
      }
    }

    private static ushort Read_(VectorReader reader, VtxFmt? format) {
      if (format == VtxFmt.INDEX16) {
        return reader.ReadU16();
      } else if (format == VtxFmt.INDEX8) {
        return reader.ReadU8();
      }

      Asserts.Fail($"Unsupported format: {format}");
      return 0;
    }
  }
}