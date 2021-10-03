﻿using System.Collections.Generic;
using System.IO;

namespace mod.gcn {
  public enum ChunkId {
    HEADER = 0x00,
    VERTICES = 0x10,
    VERTEX_NORMALS = 0x11,
    VERTEX_NBTS = 0x12,
    VERTEX_COLOURS = 0x13,

    TEX_COORD_0 = 0x18,
    TEX_COORD_1 = 0x19,
    TEX_COORD_2 = 0x1A,
    TEX_COORD_3 = 0x1B,
    TEX_COORD_4 = 0x1C,
    TEX_COORD_5 = 0x1D,
    TEX_COORD_6 = 0x1E,
    TEX_COORD_7 = 0x1F,

    TEXTURES = 0x20,
    TEXTURE_ATTRIBUTES = 0x22,
    MATERIALS = 0x30,

    VERTEX_MATRIX = 0x40,
    MATRIX_ENVELOPE = 0x41,

    MESH = 0x50,
    JOINTS = 0x60,
    JOINT_NAMES = 0x61,

    COLLISION_PRISM = 0x100,
    COLLISION_GRID = 0x110,
    END_OF_FILE = 0xFFFF,
  }

  public static class Chunk {
    public static string GetName(ChunkId id)
      => id switch {
          ChunkId.HEADER => "Header",
          ChunkId.VERTICES => "Vertices",
          ChunkId.VERTEX_NORMALS => "Vertex Normals",
          ChunkId.VERTEX_NBTS => "Vertex Normal/Binormal/Tangent Descriptors",
          ChunkId.VERTEX_COLOURS => "Vertex Colours",

          ChunkId.TEX_COORD_0 => "Texture Coordinate 0",
          ChunkId.TEX_COORD_1 => "Texture Coordinate 1",
          ChunkId.TEX_COORD_2 => "Texture Coordinate 2",
          ChunkId.TEX_COORD_3 => "Texture Coordinate 3",
          ChunkId.TEX_COORD_4 => "Texture Coordinate 4",
          ChunkId.TEX_COORD_5 => "Texture Coordinate 5",
          ChunkId.TEX_COORD_6 => "Texture Coordinate 6",
          ChunkId.TEX_COORD_7 => "Texture Coordinate 7",

          ChunkId.TEXTURES           => "Textures",
          ChunkId.TEXTURE_ATTRIBUTES => "Texture Attributes",
          ChunkId.MATERIALS          => "Materials",

          ChunkId.VERTEX_MATRIX   => "Vertex Matrix",
          ChunkId.MATRIX_ENVELOPE => "Matrix Envelope",

          ChunkId.MESH        => "Mesh",
          ChunkId.JOINTS      => "Joints",
          ChunkId.JOINT_NAMES => "Joint Names",

          ChunkId.COLLISION_PRISM => "Collision Prism",
          ChunkId.COLLISION_GRID  => "Collision Grid",
          ChunkId.END_OF_FILE     => "End Of File",
      };
  }

  public class ChunkData : IGcnSerializable {
    public ChunkId Id { get; private set; }
    public string Name => Chunk.GetName(this.Id);

    public long Offset { get; private set; }
    public int Length { get; private set; }

    public IReadOnlyList<byte> Data { get; private set; }

    public void Read(EndianBinaryReader reader) {
      this.Offset = reader.Position;

      this.Id = (ChunkId) reader.ReadUInt32();
      this.Length = reader.ReadInt32();

      this.Data = reader.ReadBytes(this.Length);
    }

    public void Write(EndianBinaryWriter writer) {
      throw new System.NotImplementedException();
    }
  }

  public class ModSectionData : IGcnSerializable {
    public List<ChunkData> Chunks { get; } = new();
    public List<byte> EofBytes { get; } = new();

    public void Read(EndianBinaryReader reader) {
      var opcodes = new HashSet<uint>();

      while (!opcodes.Contains(0xFFFF)) {
        var chunkData = new ChunkData();
        chunkData.Read(reader);

        this.Chunks.Add(chunkData);

        opcodes.Add((uint) chunkData.Id);
      }

      while (!reader.Eof) {
        this.EofBytes.Add(reader.ReadByte());
      }
    }

    public void Write(EndianBinaryWriter writer) {
      throw new System.NotImplementedException();
    }
  }
}