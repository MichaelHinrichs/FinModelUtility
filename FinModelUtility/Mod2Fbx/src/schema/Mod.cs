﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using fin.schema.color;
using fin.schema.vector;
using fin.util.asserts;

using mod.schema.collision;

using schema;

namespace mod.schema {
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

  public class Mod : IBiSerializable {
    public readonly ModHeader header = new();
    public readonly List<Vector3f> vertices = new();
    public readonly List<Vector3f> vnormals = new();
    public readonly List<NBT> vertexnbt = new();
    public readonly List<Rgba32> vcolours = new();
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

        var chunkId = (ChunkId) reader.ReadUInt32();
        var chunkName = Chunk.GetName(chunkId);

        var length = reader.ReadUInt32();

        if ((position & 0x1F) != 0) {
          Asserts.Fail("Error in chunk " +
                       chunkId +
                       ", offset " +
                       position +
                       ", chunk start isn't aligned to 0x20, this means an improper read occured.");
          return;
        }

        /*std::cout <<
            "Reading 0x" <<
            std::hex <<
            opcode <<
            std::dec <<
            ", " <<
            (ocString.has_value() ? ocString.value() : "Unknown chunk") <<
            std::endl;*/

        var beforePosition = reader.Position;

        switch (chunkId) {
          case ChunkId.HEADER:
            reader.Align(0x20);
            this.header.dateTime.year = reader.ReadUInt16();
            this.header.dateTime.month = reader.ReadByte();
            this.header.dateTime.day = reader.ReadByte();
            this.header.flags = reader.ReadUInt32();
            reader.Align(0x20);
            break;
          case ChunkId.VERTICES:
            Mod.ReadGenericChunk_(reader, this.vertices);
            break;
          case ChunkId.VERTEX_NORMALS:
            this.hasNormals = true;
            Mod.ReadGenericChunk_(reader, this.vnormals);
            break;
          case ChunkId.VERTEX_NBTS:
            Mod.ReadGenericChunk_(reader, this.vertexnbt);
            break;
          case ChunkId.VERTEX_COLOURS:
            Mod.ReadGenericChunk_(reader, this.vcolours);
            break;
          case ChunkId.TEX_COORD_0:
          case ChunkId.TEX_COORD_1:
          case ChunkId.TEX_COORD_2:
          case ChunkId.TEX_COORD_3:
          case ChunkId.TEX_COORD_4:
          case ChunkId.TEX_COORD_5:
          case ChunkId.TEX_COORD_6:
          case ChunkId.TEX_COORD_7:
            Mod.ReadGenericChunk_(reader, this.texcoords[(uint) chunkId - 0x18]);
            break;
          case ChunkId.TEXTURES:
            Mod.ReadGenericChunk_(reader, this.textures);
            for (var i = 0; i < this.textures.Count; ++i) {
              this.textures[i].index = i;
            }
            break;
          case ChunkId.TEXTURE_ATTRIBUTES:
            Mod.ReadGenericChunk_(reader, this.texattrs);
            break;
          case ChunkId.MATERIALS:
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
          case ChunkId.VERTEX_MATRIX:
            Mod.ReadGenericChunk_(reader, this.vtxMatrix);
            break;
          case ChunkId.MATRIX_ENVELOPE:
            Mod.ReadGenericChunk_(reader, this.envelopes);
            break;
          case ChunkId.MESH:
            Mod.ReadGenericChunk_(reader, this.meshes);
            break;
          case ChunkId.JOINTS:
            Mod.ReadGenericChunk_(reader, this.joints);
            break;
          case ChunkId.JOINT_NAMES:
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
          case ChunkId.COLLISION_PRISM:
            this.colltris.Read(reader);
            break;
          case ChunkId.COLLISION_GRID:
            this.collgrid.Read(reader);
            break;
          case ChunkId.END_OF_FILE:
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

        var afterPosition = reader.Position;

        /*Asserts.Equal(beforePosition + length,
                      afterPosition,
                      $"Read incorrect number of bytes for opcode: {opcodeName}");*/
      }

      ;
    }

    private static void ReadGenericChunk_<T>(
        EndianBinaryReader reader,
        List<T> vector) where T : IDeserializable, new() {
      var num = reader.ReadUInt32();
      vector.Clear();

      reader.Align(0x20);
      for (var i = 0; i < num; ++i) {
        vector.Add(Mod.ReadGeneric_<T>(reader));
      }
      reader.Align(0x20);
    }

    private static T ReadGeneric_<T>(EndianBinaryReader reader)
        where T : IDeserializable, new() {
      var instance = new T();
      instance.Read(reader);
      return instance;
    }

    public void Write(EndianBinaryWriter writer) {
      throw new NotImplementedException();
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