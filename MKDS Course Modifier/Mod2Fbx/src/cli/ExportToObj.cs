using System.IO;
using System.Linq;

using fin.util.asserts;

using mod.gcn;

namespace mod.cli {
  public static class ExportToObj {
    public static void ExportToObj(Mod mod) {
      var hasVertices = mod.m_vertices.Any();
      var hasNormals = mod.m_vnormals.Any();
      var hasFaces = mod.m_colltris.m_collinfo.Count.Any();

      if (!hasVertices && !hasNormals && !hasFaces) {
        Asserts.Fail("Loaded file has nothing to export!");
      }

      using var os = new StreamWriter(
          File.OpenWrite(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\out\test.obj"));

      var header = mod.m_header;
      var dateTime = header.dateTime;
      os.WriteLine($"# Date {dateTime.year} {dateTime.month} {dateTime.day}");

      if (hasVertices) {
        os.WriteLine("# Vertices");
        foreach (var vertex in mod.m_vertices) {
          os.WriteLine($"v {vertex}");
        }
      }

      if (hasNormals) {
        os.WriteLine();
        os.WriteLine("# Vertex normals");
        foreach (var vnrm in mod.m_vnormals) {
          os.WriteLine($"vn {vnrm}");
        }
      }

      for (var i = 0; i < mod.m_texcoords.Length; ++i) {
        var coords = mod.m_texcoords[i];
        if (coords.Count == 0) {
          continue;
        }

        os.WriteLine();
        os.WriteLine("# Texture coordinate " + i);
        foreach (var coord in coords) {
          os.WriteLine($"vt {coord}");
        }
      }

      var colInfos = mod.m_colltris.m_collinfo;
      if (colInfos.Count != 0) {
        os.WriteLine();
        os.WriteLine("o collision mesh");
        foreach (var colInfo in colInfos) {
          os.WriteLine($"f ${colInfo.m_indice.x + 1} ${colInfo.m_indice.y + 1} ${colInfo.m_indice.z + 1}");
        }
      }

/*os << "# Mesh data" << std::endl;
for (std::size_t i = 0; i < gModFile.m_meshes.size(); i++) {
    const Mesh& mesh = gModFile.m_meshes[i];
    os << "o mesh" << i << std::endl;
    for (const MeshPacket& packet : mesh.m_packets) {
        for (const DisplayList& dlist : packet.m_displaylists) {
            util::vector_reader reader(dlist.m_dlistData, 0,
util::vector_reader::Endianness::Big);

            while (reader.getRemaining()) {
                u8 opcode = reader.readU8();

                if (opcode != 0x98 && opcode != 0xA0) {
                    continue;
                }

                enum Vtx {
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
                };

                enum VtxFmt { NOT_PRESENT, DIRECT, INDEX8, INDEX16 };

                u16 faceCount = reader.readU16();
                for (u32 j = 0; j < faceCount; j++) { }
            }
        }
    }
}*/

      os.Flush();
      os.Close();
    }
  }
}