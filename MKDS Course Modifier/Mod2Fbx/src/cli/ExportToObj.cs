using System.IO;
using System.Linq;

using fin.util.asserts;

using mod.gcn;
using mod.util;

using Endianness = mod.util.Endianness;

namespace mod.cli {
  public enum Vtx {
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
    Tex7Coord
  }

  public enum VtxFmt {
    NOT_PRESENT,
    DIRECT,
    INDEX8,
    INDEX16
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

        foreach (var meshPacket in mesh.packets) {
          foreach (var dlist in meshPacket.displaylists) {
            var reader = new VectorReader(dlist.dlistData, 0, Endianness.Big);

            while (reader.GetRemaining() != 0) {
              var opcode = reader.ReadU8();

              if (opcode != 0x98 && opcode != 0xA0) {
                continue;
              }

              ;

              var faceCount = reader.ReadU16();
              for (var j = 0; j < faceCount; j++) {

              }
            }
          }
        }
      }

      os.Flush();
      os.Close();
    }
  }
}