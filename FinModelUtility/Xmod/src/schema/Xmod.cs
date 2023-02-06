using System.Numerics;

using schema.text;

namespace xmod.schema {
  public class Xmod : ITextDeserializable {
    public void Read(ITextReader tr) {
      var version = TextReaderUtils.ReadKeyValue(tr, "version");

      var numVertices = TextReaderUtils.ReadKeyValueNumber<int>(tr, "verts");
      var numNormals = TextReaderUtils.ReadKeyValueNumber<int>(tr, "normals");
      var numColors = TextReaderUtils.ReadKeyValueNumber<int>(tr, "colors");
      var numUv1s = TextReaderUtils.ReadKeyValueNumber<int>(tr, "tex1s");
      var numUv2s = TextReaderUtils.ReadKeyValueNumber<int>(tr, "tex2s");
      var numTangents =
          TextReaderUtils.ReadKeyValueNumber<int>(tr, "tangents");
      var numMaterials =
          TextReaderUtils.ReadKeyValueNumber<int>(tr, "materials");
      var numAdjuncts =
          TextReaderUtils.ReadKeyValueNumber<int>(tr, "adjuncts");
      var numPrimitives =
          TextReaderUtils.ReadKeyValueNumber<int>(tr, "primitives");
      var numMatrices =
          TextReaderUtils.ReadKeyValueNumber<int>(tr, "matrices");
      var numReskins = TextReaderUtils.ReadKeyValueNumber<int>(tr, "reskins");
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);

      var vertices = TextReaderUtils.ReadInstances<Vector3>(tr, "v", numVertices);
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);

      var normals = TextReaderUtils.ReadInstances<Vector3>(tr, "n", numNormals);
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);

      var colors = TextReaderUtils.ReadInstances<Vector4>(tr, "c", numColors);
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);

      var uv1s =
          TextReaderUtils.ReadInstances<Vector2>(tr, "t1", numUv1s);
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);
      var uv2s =
          TextReaderUtils.ReadInstances<Vector2>(tr, "t2", numUv2s);
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);

      tr.ReadNewArray<Material>(out var materials, numMaterials);
    }
  }
}