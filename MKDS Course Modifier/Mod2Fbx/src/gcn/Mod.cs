using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using fin.util.asserts;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mod.gcn {
  public class DateTime {
    public ushort year = 2021;
    public byte month = 9;
    public byte day = 18;
  }

  public class ModHeader {
    public readonly DateTime dateTime = new();

    public uint flags = 0;
  }

  public enum ModFlags {
    UseNBT = 0x01
  }

  public class Mod : IGcnSerializable {
    public readonly ModHeader header = new();
    public readonly List<Vector3f> vertices = new();
    public readonly List<Vector3f> vnormals = new();
    public readonly List<NBT> vertexnbt = new();
    public readonly List<ColourU8> vcolours = new();
    public readonly List<Vector2f>[] texcoords = new List<Vector2f>[8];
    public readonly List<Texture> textures = new();
    public readonly List<TextureAttributes> texattrs = new();
    public readonly MaterialContainer materials = new();
    public readonly List<VtxMatrix> vtxMatrix = new();
    public readonly List<Envelope> envelopes = new();
    public readonly List<Mesh> meshes = new();
    public readonly List<Joint> joints = new();
    public readonly List<string> jointNames = new();
    public readonly CollTriInfo colltris = new();
    public readonly CollGrid collgrid = new();
    public readonly List<byte> eofBytes = new();
    public bool hasNormals = false;

    public Mod() {}
    public Mod(EndianBinaryReader reader) => this.Read(reader);

    public void Read(EndianBinaryReader reader) {
      this.hasNormals = false;

      for (var i = 0; i < 8; ++i) {
        this.texcoords[i] = new List<Vector2f>();
      }

      bool stopRead = false;
      while (!stopRead) {
        var position = reader.Position;
        var opcode = reader.ReadUInt32();
        var length = reader.ReadUInt32();

        if ((position & 0x1F) != 0) {
          Asserts.Fail("Error in chunk " +
                       opcode +
                       ", offset " +
                       position +
                       ", chunk start isn't aligned to 0x20, this means an improper read occured.");
          return;
        }

        var ocString = Mod.GetChunkName(opcode);
        /*std::cout <<
            "Reading 0x" <<
            std::hex <<
            opcode <<
            std::dec <<
            ", " <<
            (ocString.has_value() ? ocString.value() : "Unknown chunk") <<
            std::endl;*/

        switch (opcode) {
          case 0:
            reader.Align(0x20);
            this.header.dateTime.year = reader.ReadUInt16();
            this.header.dateTime.month = reader.ReadByte();
            this.header.dateTime.day = reader.ReadByte();
            this.header.flags = reader.ReadUInt32();
            reader.Align(0x20);
            break;
          case 0x10:
            Mod.ReadGenericChunk_(reader, this.vertices);
            break;
          case 0x11:
            this.hasNormals = true;
            Mod.ReadGenericChunk_(reader, this.vnormals);
            break;
          case 0x12:
            Mod.ReadGenericChunk_(reader, this.vertexnbt);
            break;
          case 0x13:
            Mod.ReadGenericChunk_(reader, this.vcolours);
            break;
          case 0x18:
          case 0x19:
          case 0x1A:
          case 0x1B:
          case 0x1C:
          case 0x1D:
          case 0x1E:
          case 0x1F:
            Mod.ReadGenericChunk_(reader, this.texcoords[opcode - 0x18]);
            break;
          case 0x20:
            Mod.ReadGenericChunk_(reader, this.textures);
            break;
          case 0x22:
            Mod.ReadGenericChunk_(reader, this.texattrs);
            break;
          case 0x30:
            var numMaterials = reader.ReadUInt32();
            var numTexEnvironments = reader.ReadUInt32();

            reader.Align(0x20);
            this.materials.texEnvironments.Clear();
            for (var i = 0; i < numTexEnvironments; ++i) {
              var texEnvironment = new TEVInfo();
              texEnvironment.Read(reader);
              this.materials.texEnvironments.Add(texEnvironment);
            }

            this.materials.materials.Clear();
            for (var i = 0; i < numMaterials; ++i) {
              var mat = new Material();
              mat.Read(reader);
              this.materials.materials.Add(mat);
            }
            reader.Align(0x20);
            break;
          case 0x40:
            Mod.ReadGenericChunk_(reader, this.vtxMatrix);
            break;
          case 0x41:
            Mod.ReadGenericChunk_(reader, this.envelopes);
            break;
          case 0x50:
            Mod.ReadGenericChunk_(reader, this.meshes);
            break;
          case 0x60:
            Mod.ReadGenericChunk_(reader, this.joints);
            break;
          case 0x61:
            var numJointNames = reader.ReadUInt32();
            this.jointNames.Clear();
            reader.Align(0x20);
            for (var i = 0; i < numJointNames; ++i) {
              var jointNameLength = reader.ReadUInt32();

              var jointNameBuilder = new StringBuilder((int) jointNameLength);
              for (var c = 0; c < jointNameLength; ++c) {
                jointNameBuilder.Append(reader.ReadChar(Encoding.ASCII));
              }
            }
            reader.Align(0x20);
            break;
          case 0x100:
            this.colltris.Read(reader);
            break;
          case 0x110:
            this.collgrid.Read(reader);
            break;
          case 0xFFFF:
            reader.Position += length;

            while (!reader.Eof) {
              this.eofBytes.Add(reader.ReadByte());
            }

            stopRead = true;
            break;
          default:
            reader.Position += length;
            break;
        }
      }
    }

    private static void ReadGenericChunk_<T>(
        EndianBinaryReader reader,
        List<T> vector) where T : IGcnSerializable, new() {
      var num = reader.ReadUInt32();
      vector.Clear();

      reader.Align(0x20);
      for (var i = 0; i < num; ++i) {
        vector.Add(Mod.ReadGeneric_<T>(reader));
      }
      reader.Align(0x20);
    }

    private static T ReadGeneric_<T>(EndianBinaryReader reader)
        where T : IGcnSerializable, new() {
      var instance = new T();
      instance.Read(reader);
      return instance;
    }

    public void Write(EndianBinaryWriter writer) {
      throw new NotImplementedException();
    }

    public void Reset() {
      this.vertices.Clear();
      this.vnormals.Clear();
      this.vertexnbt.Clear();
      this.vcolours.Clear();
      for (var i = 0; i < 8; i++) {
        this.texcoords[i].Clear();
      }
      this.textures.Clear();
      this.texattrs.Clear();
      this.materials.materials.Clear();
      this.materials.texEnvironments.Clear();
      this.vtxMatrix.Clear();
      this.envelopes.Clear();
      this.meshes.Clear();
      this.joints.Clear();
      this.jointNames.Clear();

      this.colltris.collinfo.Clear();
      this.colltris.roominfo.Clear();

      this.collgrid.Clear();

      this.eofBytes.Clear();
    }

    private static IDictionary<uint, string> CHUNK_NAMES_ =
        new Dictionary<uint, string>();

    static Mod() {
      Mod.CHUNK_NAMES_[0x00] = "Header";
      Mod.CHUNK_NAMES_[0x10] = "Vertices";
      Mod.CHUNK_NAMES_[0x11] = "Vertex Normals";
      Mod.CHUNK_NAMES_[0x12] = "Vertex Normal/Binormal/Tangent Descriptors";
      Mod.CHUNK_NAMES_[0x13] = "Vertex Colours";

      Mod.CHUNK_NAMES_[0x18] = "Texture Coordinate 0";
      Mod.CHUNK_NAMES_[0x19] = "Texture Coordinate 1";
      Mod.CHUNK_NAMES_[0x1A] = "Texture Coordinate 2";
      Mod.CHUNK_NAMES_[0x1B] = "Texture Coordinate 3";
      Mod.CHUNK_NAMES_[0x1C] = "Texture Coordinate 4";
      Mod.CHUNK_NAMES_[0x1D] = "Texture Coordinate 5";
      Mod.CHUNK_NAMES_[0x1E] = "Texture Coordinate 6";
      Mod.CHUNK_NAMES_[0x1F] = "Texture Coordinate 7";

      Mod.CHUNK_NAMES_[0x20] = "Textures";
      Mod.CHUNK_NAMES_[0x22] = "Texture Attributes";
      Mod.CHUNK_NAMES_[0x30] = "Materials";

      Mod.CHUNK_NAMES_[0x40] = "Vertex Matrix";
      Mod.CHUNK_NAMES_[0x41] = "Matrix Envelope";

      Mod.CHUNK_NAMES_[0x50] = "Mesh";
      Mod.CHUNK_NAMES_[0x60] = "Joints";
      Mod.CHUNK_NAMES_[0x61] = "Joint Names";

      Mod.CHUNK_NAMES_[0x100] = "Collision Prism";
      Mod.CHUNK_NAMES_[0x110] = "Collision Grid";
      Mod.CHUNK_NAMES_[0xFFFF] = "End Of File";
    }

    public static string? GetChunkName(uint opcode) {
      if (Mod.CHUNK_NAMES_.TryGetValue(opcode, out var chunkName)) {
        return chunkName;
      }

      return null;
    }
  }
}


/*static inline void writeGenericChunk(util::fstream_writer
&writer, auto & vector,
u32 chunkIdentifier) {
  std::cout <<
      "Writing 0x" <<
      std::hex <<
      chunkIdentifier <<
      std::dec <<
      ", " <<
      MOD::getChunkName(chunkIdentifier).value() <<
      std::endl;

  u32 subchunkPos = startChunk(writer, chunkIdentifier);
  writer.writeU32(vector.size());

  writer.align(0x20);
  for (auto & contents : vector)
  {
    contents.write(writer);
  }
  finishChunk(writer, subchunkPos);
}
}*/

// NOTE: the control flow and layout of this function is a replica of a
// decompiled version of the DMD->MOD process, found in plugTexConv
/*void MOD::write(util::fstream_writer & writer) {
  // Write header
  u32 headerPos = startChunk(writer, 0);
  writer.align(0x20);
  writer.writeU16(m_header.m_dateTime.m_year);
  writer.writeU8(m_header.m_dateTime.m_month);
  writer.writeU8(m_header.m_dateTime.m_day);
  writer.writeU32(m_header.m_flags);
  finishChunk(writer, headerPos);

  if (m_vertices.size()) {
    writeGenericChunk(writer, m_vertices, 0x10);
  }

  if (m_vcolours.size()) {
    writeGenericChunk(writer, m_vcolours, 0x13);
  }

  if (m_vnormals.size()) {
    writeGenericChunk(writer, m_vnormals, 0x11);
  }

  if (m_header.m_flags & static_cast<u32>(MODFlags::UseNBT) &&
      m_vertexnbt.size()) {
    writeGenericChunk(writer, m_vnormals, 0x12);
  }

  for (std::size_t i = 0; i < m_texcoords.size(); i++) {
    if (m_texcoords[i].size()) {
      writeGenericChunk(writer, m_texcoords[i], i + 0x18);
    }
  }

  if (m_textures.size()) {
    writeGenericChunk(writer, m_textures, 0x20);
  }

  if (m_texattrs.size()) {
    writeGenericChunk(writer, m_texattrs, 0x22);
  }

  if (m_materials.m_materials.size()) {
    std::cout <<
        "Writing 0x30, " <<
        MOD::getChunkName(0x30).value() <<
        std::endl;

    const u32 start = startChunk(writer, 0x30);
    writer.writeU32(m_materials.m_materials.size());
    writer.writeU32(m_materials.m_texEnvironments.size());
    writer.align(0x20);

    for (mat::TEVInfo & tevInfo : m_materials.m_texEnvironments)
    {
      tevInfo.write(writer);
    }

    for (mat::Material & material : m_materials.m_materials)
    {
      material.write(writer);
    }
    finishChunk(writer, start);
  }

  if (m_envelopes.size()) {
    writeGenericChunk(writer, m_envelopes, 0x41);
  }

  if (m_vtxMatrix.size()) {
    writeGenericChunk(writer, m_vtxMatrix, 0x40);
  }

  if (m_meshes.size()) {
    writeGenericChunk(writer, m_meshes, 0x50);
  }

  if (m_joints.size()) {
    writeGenericChunk(writer, m_joints, 0x60);

    if (m_jointNames.size()) {
      std::cout <<
          "Writing 0x61, " <<
          MOD::getChunkName(0x61).value() <<
          std::endl;

      const u32 start = startChunk(writer, 0x61);
      writer.writeU32(m_jointNames.size());
      writer.align(0x20);
      for (std:: string & name :
      m_jointNames) {
        writer.writeU32(name.size());
        for (std::size_t i = 0; i < name.size(); i++) {
          writer.writeU8(name[i]);
        }
      }
      finishChunk(writer, start);
    }
  }

  if (m_colltris.m_collinfo.size()) {
    std::cout <<
        "Writing 0x100, " <<
        MOD::getChunkName(0x100).value() <<
        std::endl;
    m_colltris.write(writer);

    const u32 start = startChunk(writer, 0x110);
    writer.align(0x20);
    m_collgrid.m_boundsMin.write(writer);
    m_collgrid.m_boundsMax.write(writer);
    writer.writeF32(m_collgrid.m_unknown1);
    writer.writeU32(m_collgrid.m_gridX);
    writer.writeU32(m_collgrid.m_gridY);
    writer.writeU32(m_collgrid.m_groups.size());
    for (CollGroup & group : m_collgrid.m_groups)
    {
      group.write(writer);
    }
    for (s32 & i : m_collgrid.m_unknown2)
    {
      writer.writeS32(i);
    }
    writer.align(0x20);
    finishChunk(writer, start);
  }

  // Finalise writing with 0xFFFF chunk and append any INI file
  finishChunk(writer, startChunk(writer, 0xFFFF));
  if (m_eofBytes.size()) {
    std::cout <<
        "Writing 0xffff, " <<
        MOD::getChunkName(0xffff).value() <<
        std::endl;

    writer.write(reinterpret_cast<char*>(m_eofBytes.data()),
                 m_eofBytes.size());
  }
}*/