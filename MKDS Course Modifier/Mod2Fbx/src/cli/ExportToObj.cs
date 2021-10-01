using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.util.asserts;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using mod.gcn;
using mod.util;

using Endianness = mod.util.Endianness;

namespace mod.cli {
  public enum Opcode {
    QUADS = 0x80,
    TRIANGLES = 0x90,
    TRIANGLE_STRIP = 0x98,
    TRIANGLE_FAN = 0xa0,
    LINES = 0xa8,
    LINE_STRIP = 0xb0,
    POINTS = 0xb8,
  }

  public static class ExportToObj {
    public static void Export(Mod mod) {
      var hasVertices = mod.vertices.Any();
      var hasNormals = mod.vnormals.Any();
      var hasFaces = mod.colltris.collinfo.Any();

      if (!hasVertices && !hasNormals && !hasFaces) {
        Asserts.Fail("Loaded file has nothing to export!");
      }

      using var os = new StreamWriter(
          File.OpenWrite(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\out\test.obj"));

      var header = mod.header;
      var dateTime = header.dateTime;
      os.WriteLine($"# Date {dateTime.year} {dateTime.month} {dateTime.day}");

      if (hasVertices) {
        os.WriteLine("# Vertices");
        foreach (var vertex in mod.vertices) {
          os.WriteLine($"v {vertex.X} {vertex.Y} {vertex.Z}");
        }
      }

      if (hasNormals) {
        os.WriteLine();
        os.WriteLine("# Vertex normals");
        foreach (var vnrm in mod.vnormals) {
          os.WriteLine($"vn {vnrm.X} {vnrm.Y} {vnrm.Z}");
        }
      }

      for (var i = 0; i < mod.texcoords.Length; ++i) {
        var coords = mod.texcoords[i];
        if (coords.Count == 0) {
          continue;
        }

        os.WriteLine();
        os.WriteLine("# Texture coordinate " + i);
        foreach (var coord in coords) {
          os.WriteLine($"vt {coord.X} {coord.Y}");
        }
      }

      var colInfos = mod.colltris.collinfo;
      if (colInfos.Count != 0) {
        os.WriteLine();
        os.WriteLine("o collision mesh");
        foreach (var colInfo in colInfos) {
          os.WriteLine(
              $"f ${colInfo.indice.X + 1} ${colInfo.indice.Y + 1} ${colInfo.indice.Z + 1}");
        }
      }

      os.WriteLine("# Mesh data");
      for (var i = 0; i < mod.meshes.Count; ++i) {
        var mesh = mod.meshes[i];
        os.WriteLine("o mesh " + i);

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
              var triangles =
                  new Triangles(vertexDescriptor, opcode, faceCount);
              triangles.Read(reader);

              foreach (var face in triangles.Faces) {
                os.WriteLine($"f {face.X} {face.Y} {face.Z}");
              }
            }
          }
        }
      }

      os.Flush();
      os.Close();
    }

    private class Triangles {
      private readonly VertexDescriptor vertexDescriptor_;

      public Opcode Type { get; }
      public int NumFaces { get; }
      public List<Vector3i> Faces { get; } = new();

      public Triangles(
          VertexDescriptor vertexDescriptor,
          Opcode type,
          int numFaces) {
        this.vertexDescriptor_ = vertexDescriptor;
        this.Type = type;
        this.NumFaces = numFaces;
      }

      public void Read(VectorReader reader) {
        var vertexIndices = new List<ushort>();
        for (var i = 0; i < this.NumFaces; i++) {
          var activeAttributes = this.vertexDescriptor_.ActiveAttributes();
          foreach (var (attr, format) in this.vertexDescriptor_) {
            if (attr == Vtx.Position) {
              vertexIndices.Add((ushort) (reader.ReadU16() + 1));
            } else if (format == null) {
              reader.ReadU8();
            } else if (format == VtxFmt.INDEX16) {
              reader.ReadU16();
            } else {
              Asserts.Fail($"Unexpected attribute/format ({attr}/{format})");
            }
          }
        }

        var triangles = new List<ushort>();
        if (this.Type == Opcode.TRIANGLE_STRIP) {
          var n = 2;
          for (var i = 0; i < vertexIndices.Count - 2; ++i) {
            var isEven = n % 2 == 0;

            triangles.Add(vertexIndices[n - 2]);
            if (isEven) {
              triangles.Add(vertexIndices[n]);
              triangles.Add(vertexIndices[n - 1]);
            } else {
              triangles.Add(vertexIndices[n - 1]);
              triangles.Add(vertexIndices[n]);
            }
            n++;
          }
        } else if (this.Type == Opcode.TRIANGLE_FAN) {
          for (var i = 1; i < vertexIndices.Count - 1; ++i) {
            triangles.Add(vertexIndices[i]);
            triangles.Add(vertexIndices[i + 1]);
            triangles.Add(vertexIndices[0]);
          }
        } else {
          Asserts.Fail($"Unhandled opcode {this.Type}");
        }

        this.Faces.Clear();
        for (var i = 0; i < triangles.Count; i += 3) {
          this.Faces.Add(new Vector3i(triangles[i],
                                      triangles[i + 1],
                                      triangles[i + 2]));
        }
      }
    }
  }
}