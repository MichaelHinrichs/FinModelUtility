using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.model;
using fin.model.impl;
using fin.util.asserts;

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

      // TODO: This is the wrong way to set up vertices, this array is just locations. We will need to piece them together with face data below
      var finVertices = new IVertex[mod.vertices.Count];
      for (var i = 0; i < mod.vertices.Count; ++i) {
        var vertex = mod.vertices[i];
        var finVertex = model.Skin.AddVertex(vertex.X, vertex.Y, vertex.Z);
        finVertices[i] = finVertex;
      }

      /**
        if (hasNormals) {
          var vnrm = mod.vnormals[i];
          finVertex.SetLocalNormal(vnrm.X, vnrm.Y, vnrm.Z);
        }
       */

      /*for (var i = 0; i < mod.texcoords.Length; ++i) {
        var coords = mod.texcoords[i];
        if (coords.Count == 0) {
          continue;
        }

        os.WriteLine();
        os.WriteLine("# Texture coordinate " + i);
        foreach (var coord in coords) {
          os.WriteLine($"vt {coord.X} {coord.Y}");
        }
      }*/

      /*var colInfos = mod.colltris.collinfo;
      if (colInfos.Count != 0) {
        os.WriteLine();
        os.WriteLine("o collision mesh");
        foreach (var colInfo in colInfos) {
          os.WriteLine(
              $"f ${colInfo.indice.X + 1} ${colInfo.indice.Y + 1} ${colInfo.indice.Z + 1}");
        }
      }*/

      foreach (var mesh in mod.meshes) {
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
              var vertexIndices = new List<ushort>();
              for (var f = 0; f < faceCount; f++) {
                foreach (var (attr, format) in vertexDescriptor) {
                  if (attr == Vtx.Position) {
                    vertexIndices.Add(reader.ReadU16());
                  } else if (format == null) {
                    reader.ReadU8();
                  } else if (format == VtxFmt.INDEX16) {
                    reader.ReadU16();
                  } else {
                    Asserts.Fail(
                        $"Unexpected attribute/format ({attr}/{format})");
                  }
                }
              }

              var vertices =
                  vertexIndices.Select(vertexIndex => finVertices[vertexIndex])
                               .ToArray();
              if (opcode == Opcode.TRIANGLE_FAN) {
                model.Skin.AddTriangleFan(vertices);
              } else if (opcode == Opcode.TRIANGLE_STRIP) {
                model.Skin.AddTriangleStrip(vertices);
              }
            }
          }
        }
      }

      return model;
    }
  }
}