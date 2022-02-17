using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

using Dxt;

using fin.model;
using fin.model.impl;


namespace HaloWarsTools {
  public class HWXtdResource : HWBinaryResource {
    public IModel Mesh { get; private set; }

    public Bitmap AmbientOcclusionTexture { get; private set; }
    public Bitmap OpacityTexture { get; private set; }

    public static new HWXtdResource FromFile(HWContext context, string filename)
      => GetOrCreateFromFile(context, filename, HWResourceType.Xtd) as
             HWXtdResource;

    protected override void Load(byte[] bytes) {
      base.Load(bytes);

      this.Mesh = this.ImportMesh(bytes);

      this.AmbientOcclusionTexture = ExtractEmbeddedDXT5A(bytes,
        GetFirstChunkOfType(HWBinaryResourceChunkType.XTD_AOChunk));
      this.OpacityTexture = ExtractEmbeddedDXT5A(bytes,
                                                 GetFirstChunkOfType(
                                                     HWBinaryResourceChunkType
                                                         .XTD_AlphaChunk));
    }

    private Bitmap ExtractEmbeddedDXT5A(byte[] bytes,
                                        HWBinaryResourceChunk chunk) {
      // Get raw embedded DXT5 texture from resource file
      var width = (int) Math.Sqrt(chunk.Size * 2);
      var height = width;

      // For some godforsaken reason, every pair of bytes is flipped so we need
      // to fix it here. This was really annoying to figure out, haha.
      for (var i = 0; i < chunk.Size; i += 2) {
        var offset = (int) chunk.Offset + i;

        var byte0 = bytes[offset + 0];
        var byte1 = bytes[offset + 1];

        bytes[offset + 0] = byte1;
        bytes[offset + 1] = byte0;
      }

      return DxtDecoder.DecompressDxt5a(bytes,
                                        (int) chunk.Offset,
                                        width,
                                        height);
    }

    private IModel ImportMesh(byte[] bytes) {
      int stride = 1;
      MeshNormalExportMode shadingMode = MeshNormalExportMode.Unchanged;

      HWBinaryResourceChunk headerChunk =
          GetFirstChunkOfType(HWBinaryResourceChunkType.XTD_XTDHeader);
      float tileScale =
          BinaryUtils.ReadFloatBigEndian(bytes,
                                         (int) headerChunk.Offset + 12);
      HWBinaryResourceChunk atlasChunk =
          GetFirstChunkOfType(HWBinaryResourceChunkType.XTD_AtlasChunk);

      int gridSize =
          (int) Math.Round(Math.Sqrt((atlasChunk.Size - 32) /
                                     8)); // Subtract the min/range vector sizes, divide by position + normal size, and sqrt for grid size
      int positionOffset = (int) atlasChunk.Offset + 32;
      int normalOffset = positionOffset + gridSize * gridSize * 4;

      if (gridSize % stride != 0) {
        throw new Exception(
            $"Grid size {gridSize} is not evenly divisible by stride {stride} - choose a different stride value.");
      }

      // These are stored as ZYX, 4 bytes per component
      Vector3 PosCompMin = BinaryUtils
                           .ReadVector3BigEndian(
                               bytes, (int) atlasChunk.Offset)
                           .ReverseComponents();
      Vector3 PosCompRange =
          BinaryUtils
              .ReadVector3BigEndian(bytes, (int) atlasChunk.Offset + 16)
              .ReverseComponents();

      var finModel = new ModelImpl(gridSize * gridSize);
      var finMesh = finModel.Skin.AddMesh();

      var finVertices = new IVertex[gridSize * gridSize];

      // Read vertex offsets/normals and add them to the mesh
      for (int x = 0; x < gridSize; x += stride) {
        for (int z = 0; z < gridSize; z += stride) {
          int index =
              ConvertGridPositionToIndex(new Tuple<int, int>(x, z), gridSize);
          int offset = index * 4;


          // Get offset position and normal for this vertex
          Vector3 position =
              ReadVector3Compressed(bytes, positionOffset + offset) *
              PosCompRange -
              PosCompMin;

          // Positions are relative to the terrain grid, so shift them by the grid position
          position += new Vector3(x, 0, z) * tileScale;

          Vector3 normal =
              ConvertDirectionVector(
                  Vector3.Normalize(
                      ReadVector3Compressed(bytes, normalOffset + offset) *
                      2.0f -
                      Vector3.One));

          // Simple UV based on original, non-warped terrain grid
          Vector3 texCoord = new Vector3(x / ((float) gridSize - 1),
                                         z / ((float) gridSize - 1), 0);

          var finVertex =
              finModel.Skin
                      .AddVertex(position.X, position.Y, position.Z)
                      .SetLocalNormal(normal.X, normal.Y, normal.Z)
                      .SetUv(texCoord.X, texCoord.Y);
          finVertices[GetVertexIndex(x, z, gridSize)] = finVertex;
        }
      }

      // Generate faces based on terrain grid
      for (int x = 0; x < gridSize - stride; x += stride) {
        var triangles = new List<(IVertex, IVertex, IVertex)>();

        for (int z = 0; z < gridSize - stride; z += stride) {
          var a = finVertices[GetVertexIndex(x, z, gridSize)];
          var b = finVertices[GetVertexIndex(x + stride, z, gridSize)];
          var c = finVertices[GetVertexIndex(x, z + stride, gridSize)];
          var d = finVertices[GetVertexIndex(x + stride, z + stride, gridSize)];

          triangles.Add((a, b, c));
          triangles.Add((d, c, b));
        }

        finMesh.AddTriangles(triangles.ToArray());
        triangles.Clear();
      }

      return finModel;
    }

    private static int GetVertexIndex(int x, int z, int gridSize)
      => z * gridSize + x;

    private static int GetVertexID(int x, int z, int gridSize, int stride) {
      return ConvertGridPositionToIndex(
          new Tuple<int, int>(x / stride, z / stride), gridSize / stride);
    }

    private static Tuple<int, int> ConvertIndexToGridPosition(
        int index,
        int gridSize) {
      return new Tuple<int, int>(index % gridSize, index / gridSize);
    }

    private static int ConvertGridPositionToIndex(
        Tuple<int, int> gridPosition,
        int gridSize) {
      return gridPosition.Item2 * gridSize + gridPosition.Item1;
    }

    private Vector3 ReadVector3Compressed(byte[] bytes, int offset) {
      // Inexplicably, position and normal vectors are encoded inside 4 bytes. ~10 bits per component
      // This seems okay for directions, but positions suffer from stairstepping artifacts
      uint kBitMask10 = (1 << 10) - 1;
      uint v = BinaryUtils.ReadUInt32LittleEndian(bytes, offset);
      uint x = (v >> 0) & kBitMask10;
      uint y = (v >> 10) & kBitMask10;
      uint z = (v >> 20) & kBitMask10;
      return new Vector3(x, y, z) / kBitMask10;
    }

    private static Vector3 ConvertPositionVector(Vector3 vector) {
      return new Vector3(vector.X, -vector.Z, vector.Y);
    }

    private static Vector3 ConvertDirectionVector(Vector3 vector) {
      return new Vector3(vector.Z, vector.X, vector.Y);
    }
  }
}