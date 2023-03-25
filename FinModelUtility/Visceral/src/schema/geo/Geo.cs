﻿using fin.schema.matrix;
using fin.schema.vector;

using schema.binary;


namespace visceral.schema.geo {
  public class Geo : IBinaryDeserializable {
    public string ModelName { get; set; }

    public IReadOnlyList<Bone> Bones { get; set; }
    public IReadOnlyList<Mesh> Meshes { get; set; }

    public void Read(IEndianBinaryReader er) {
      er.AssertString("MGAE");

      er.Position += 8;
      er.AssertUInt32((uint) er.Length);

      er.Position += 0x10;

      this.ModelName = er.ReadStringNTAtOffset(er.ReadUInt32());

      er.Position += 0x10;
      var meshCount = er.ReadUInt32();
      var boneCount = er.ReadUInt32();

      er.Position += 0xc;

      var refCount = er.ReadUInt32();
      var refTableOffset = er.ReadUInt32();

      var tableOffset = er.ReadUInt32();
      var unkOffset = er.ReadUInt32();

      er.Position += 8;

      var boneDataOffset = er.ReadUInt32();
      var boneOffset = er.ReadUInt32();
      var uvBufferInfoOffset = er.ReadUInt32();
      var faceBufferInfoOffset = er.ReadUInt32();


      er.Position = uvBufferInfoOffset;
      er.Position += 0x14;
      var uvBufferLength = er.ReadUInt32();
      var totalUvBufferCount = er.ReadUInt32();
      var uvSize = er.ReadUInt16();
      er.Position += 2;
      var uvBufferOffset = er.ReadUInt32();


      er.Position = boneDataOffset;
      var bones = new List<Bone>();
      for (var i = 0; i < boneCount; ++i) {
        var boneName = er.ReadStringNTAtOffset(er.ReadUInt32());
        er.Position += 8;
        var someId = er.ReadUInt32();

        var matrix =
            er.ReadNewAtOffset<Matrix4x4f>(boneOffset + 16 * (someId - 1));

        bones.Add(new Bone {
            Name = boneName, 
            Matrix = matrix,
            Id = someId,
        });
      }
      this.Bones = bones;


      var meshes = new List<Mesh>();
      for (var i = 0; i < meshCount; i++) {
        er.Position = tableOffset + 0xC0 * i;

        var meshName = er.ReadStringNTAtOffset(er.ReadUInt32());

        er.Position += 0x1c;
        er.Position += 0x10;

        var polyInfoOffset = er.ReadUInt32();
        er.Position += 0x4;

        var vertOffset = er.ReadUInt32();
        er.Position += 0x4;

        var faceOffset = er.ReadUInt32();
        er.Position += 0x4;


        er.Position = polyInfoOffset;
        var faceCount = er.ReadUInt32();
        er.Position += 4;
        var baseVertexIndex = er.ReadUInt16();
        var vertexCount = er.ReadUInt16();


        er.Position = vertOffset;
        var vertices = new List<Vertex>();
        for (var v = 0; v < vertexCount; v++) {
          var position = er.ReadNew<Vector3f>();
          er.Position += 8;

          var boneIds = er.ReadBytes(4);
          var weights = er.ReadUn16s(4);

          vertices.Add(new Vertex {
            Position = position,
            Bones = boneIds,
            Weights = weights,
          });
        }


        er.Position = faceOffset;
        var faces = new List<Face>();
        for (var f = 0; f < faceCount / 3; ++f) {
          var vertexIndices = er.ReadUInt16s(3);
          faces.Add(new Face {
            Indices = vertexIndices,
          });
        }


        meshes.Add(new Mesh {
            Name = meshName,
            BaseVertexIndex = baseVertexIndex,
            Vertices = vertices,
            Faces = faces,
        });
      }

      this.Meshes = meshes;

      ;
    }

    public class Bone {
      public required string Name { get; init; }
      public required Matrix4x4f Matrix { get; init; }
      public required uint Id { get; init; }
    }

    public class Mesh {
      public required string Name { get; init; }
      public required ushort BaseVertexIndex { get; init; }
      public required IReadOnlyList<Vertex> Vertices { get; init; }
      public required IReadOnlyList<Face> Faces { get; init; }
    }

    public class Vertex {
      public required Vector3f Position { get; init; }
      public required IReadOnlyList<byte> Bones { get; init; }
      public required IReadOnlyList<float> Weights { get; init; }
    }

    public class Face {
      public required IReadOnlyList<ushort> Indices { get; init; }
    }
  }
}