using schema.binary.util;
using schema.text;

namespace xmod.schema.xmod {
  public class Vector3 : ITextDeserializable {
    public Vector3() { }

    public Vector3(float x, float y, float z) {
      X = x;
      Y = y;
      Z = z;
    }

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public void Read(ITextReader tr) {
      var values = tr.ReadSingles(TextReaderConstants.WHITESPACE_STRINGS,
                                  TextReaderConstants.NEWLINE_STRINGS);
      Asserts.Equal(3, values.Length);
      X = values[0];
      Y = values[1];
      Z = values[2];
    }
  }
}