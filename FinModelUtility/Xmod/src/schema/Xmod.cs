using System.Numerics;

using schema;

namespace xmod.schema {
  public class Xmod : IDeserializable {
    public void Read(IEndianBinaryReader er) {
      var version = ReadKeyValue_(er, "version:");
      
      var numVertices = ReadKeyValue_<int>(er, "verts:");
      var numNormals = ReadKeyValue_<int>(er, "normals:");
      var numColors = ReadKeyValue_<int>(er, "colors:");
      var numUv1s = ReadKeyValue_<int>(er, "tex1s:");
      var numUv2s = ReadKeyValue_<int>(er, "tex2s:");
      var numTangents = ReadKeyValue_<int>(er, "tangents:");
      var numMaterials = ReadKeyValue_<int>(er, "materials:");
      var numAdjuncts = ReadKeyValue_<int>(er, "adjuncts:");
      var numPrimitives = ReadKeyValue_<int>(er, "primitives:");
      var numMatrices = ReadKeyValue_<int>(er, "matrices:");
      var numReskins = ReadKeyValue_<int>(er, "reskins:");
      ReadEmptyLines_(er);

      var vertices = ReadVector3s_(er, "v", numVertices);
      ReadEmptyLines_(er);

      var normals = ReadVector3s_(er, "n", numNormals);
      ReadEmptyLines_(er);

      var colors = ReadVector4s_(er, "c", numColors);
      ReadEmptyLines_(er);

      var uv1s = ReadVector2s_(er, "t1", numUv1s);
      ReadEmptyLines_(er);
      var uv2s = ReadVector2s_(er, "t2", numUv1s);
      ReadEmptyLines_(er);

      {
        er.AssertString("packet");


      }

    }

    private static string ReadKeyValue_(IEndianBinaryReader er, string prefix) {
      er.AssertString(prefix);
      return er.ReadLine().Trim();
    }

    private static TNumber ReadKeyValue_<TNumber>(IEndianBinaryReader er, string prefix) where TNumber : INumber<TNumber> {
      return TNumber.Parse(ReadKeyValue_(er, prefix), null);
    }

    private static void ReadEmptyLines_(IEndianBinaryReader er) {
      long startPos;
      char c;
      do {
        startPos = er.Position;
        c = er.ReadChar();
        if (c == '\r') {
          c = er.ReadChar();
        }
      } while (c == '\n');
      er.Position = startPos;
    }

    private static string ReadWhiteSpace(IEndianBinaryReader er) {
      while ()
    }


      private static string ReadValue(IEndianBinaryReader er) {
      var values = new List<Vector2>();
      for (var i = 0; i < count; ++i) {
        var numbers = ReadKeyValue_(er, prefix).Split('\t');
        values.Add(new Vector2(float.Parse(numbers[0]),
          float.Parse(numbers[1])));
      }
      return values;
    }

    private static TNumber ReadValue<TNumber>(IEndianBinaryReader er) where TNumber : INumber<TNumber> {
      var values = new List<Vector2>();
      for (var i = 0; i < count; ++i) {
        var numbers = ReadKeyValue_(er, prefix).Split('\t');
        values.Add(new Vector2(float.Parse(numbers[0]),
          float.Parse(numbers[1])));
      }
      return values;
    }


    private static IList<Vector2> ReadValues_<TNumber>(IEndianBinaryReader er, string prefix, int count) {
      var values = new List<Vector2>();
      for (var i = 0; i < count; ++i) {
        var numbers = ReadKeyValue_(er, prefix).Split('\t');
        values.Add(new Vector2(float.Parse(numbers[0]),
          float.Parse(numbers[1])));
      }
      return values;
    }


    private static IList<Vector2> ReadVector2s_(IEndianBinaryReader er, string prefix, int count) {
      var values = new List<Vector2>();
      for (var i = 0; i < count; ++i) {
        var numbers = ReadKeyValue_(er, prefix).Split('\t');
        values.Add(new Vector2(float.Parse(numbers[0]),
          float.Parse(numbers[1])));
      }
      return values;
    }

    private static IList<Vector3> ReadVector3s_(IEndianBinaryReader er, string prefix, int count) {
      var values = new List<Vector3>();
      for (var i = 0; i < count; ++i) {
        var numbers = ReadKeyValue_(er, prefix).Split('\t');
        values.Add(new Vector3(float.Parse(numbers[0]),
          float.Parse(numbers[1]),
          float.Parse(numbers[2])));
      }
      return values;
    }

    private static IList<Vector4> ReadVector4s_(IEndianBinaryReader er, string prefix, int count) {
      var values = new List<Vector4>();
      for (var i = 0; i < count; ++i) {
        var numbers = ReadKeyValue_(er, prefix).Split('\t');
        values.Add(new Vector4(float.Parse(numbers[0]),
          float.Parse(numbers[1]),
          float.Parse(numbers[2]),
          float.Parse(numbers[3])));
      }
      return values;
    }
  }
}