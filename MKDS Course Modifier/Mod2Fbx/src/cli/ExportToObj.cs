using fin.util.asserts;

using mod.gcn;

namespace mod.cli {
  public static class ExportToObj {
    public static void ExportToObj(Mod mod) {
      if (mod.m_vertices.Count == 0 &&
          mod.m_vnormals.Count == 0 &&
          mod.m_colltris.m_collinfo.Count == 0) {
        Asserts.Fail("Loaded file has nothing to export!");
      }

      const std::string & filename
          = gTokeniser.isEnd() ? gModFileName + ".obj" : gTokeniser.next();
      std::ofstream os(filename);
      if (!os.is_open()) {
        std::cout << "Error can't open " << filename << std::endl;
        return;
      }

      MODHeader & header = gModFile.m_header;
      os <<
          "# Date " <<
          (u32) header.m_dateTime.m_year <<
          " " <<
          (u32) header.m_dateTime.m_month <<
          " " <<
          (u32) header.m_dateTime.m_day <<
          std::endl;

      if (gModFile.m_vertices.size()) {
        os << "\n# Vertices" << std::endl;
        for ( const Vector3f 
        &vpos : gModFile.m_vertices) {
          os << "v " << vpos << std::endl;
        }
      }

      if (gModFile.m_vnormals.size()) {
        os << "\n# Vertex normals" << std::endl;
        for ( const Vector3f 
        &vnrm : gModFile.m_vnormals) {
          os << "vn " << vnrm << std::endl;
        }
      }

      for (u32 i = 0; i < gModFile.m_texcoords.size(); i++) {
        std::vector<Vector2f> & coords = gModFile.m_texcoords[i];
        if (!coords.size()) {
          continue;
        }

        os << "\n# Texture coordinate " << i << std::endl;
        for ( const Vector2f 
        &vt : coords) {
          os << "vt " << vt.x << " " << vt.y << std::endl;
        }
      }

      if (gModFile.m_colltris.m_collinfo.size()) {
        os << "\no collision_mesh" << std::endl;
        for ( const BaseCollTriInfo 
        &colInfo :
        gModFile.m_colltris.m_collinfo) {
          os <<
              "f " <<
              colInfo.m_indice.x + 1 <<
              " " <<
              colInfo.m_indice.y + 1 <<
              " " <<
              colInfo.m_indice.z + 1 <<
              " " <<
              std::endl;
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

      os.flush();
      os.close();

      std::cout << "Done!" << std::endl;
    }
  }
}