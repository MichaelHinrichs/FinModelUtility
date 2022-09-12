using System.IO;
using System.Threading.Tasks;

using schema.util;


namespace schema.testing {
  public static class BinarySchemaAssert {
    public static async Task ReadsAndWritesIdentically<T>(EndianBinaryReader er)
        where T : IBiSerializable, new() {
      var readerStartPos = er.Position;
      var instance = er.ReadNew<T>();
      var readerEndPos = er.Position;

      var expectedLength = (int) (readerEndPos - readerStartPos);

      er.Position = readerStartPos;
      var expectedBytes = er.ReadBytes(expectedLength);

      var ew = new EndianBinaryWriter(er.Endianness);
      instance.Write(ew);

      var actualOutputStream = new MemoryStream();
      await ew.CompleteAndCopyToDelayed(actualOutputStream);

      Asserts.Equal(expectedLength, actualOutputStream.Length);
      Asserts.Equal(expectedBytes, actualOutputStream.ToArray());
    }
  }
}