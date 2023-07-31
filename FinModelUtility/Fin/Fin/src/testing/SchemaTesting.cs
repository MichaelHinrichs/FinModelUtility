using System.IO;
using System.Threading.Tasks;

using fin.util.asserts;

using schema.binary;
using schema.binary.testing;

namespace fin.testing {
  public static class SchemaTesting {
    public static Task<byte[]> GetEndianBinaryWriterBytes(
        EndianBinaryWriter ew)
      => BinarySchemaAssert.GetEndianBinaryWriterBytes(ew);

    public static async Task WritesAndReadsIdentically<T>(
        T value,
        Endianness endianess = Endianness.LittleEndian,
        bool assertExactEndPositions = true)
        where T : IBinaryConvertible, new() {
      var ew = new EndianBinaryWriter(endianess);
      value.Write(ew);

      var actualBytes = await GetEndianBinaryWriterBytes(ew);

      var er = new EndianBinaryReader(actualBytes, endianess);
      await ReadsAndWritesIdentically<T>(er, assertExactEndPositions);
    }

    public static async Task ReadsAndWritesIdentically<T>(
        IEndianBinaryReader er,
        bool assertExactEndPositions = true)
        where T : IBinaryConvertible, new() {
      var readerStartPos = er.Position;
      var instance = er.ReadNew<T>();
      var expectedReadLength = er.Position - readerStartPos;

      var ew = new EndianBinaryWriter(er.Endianness);
      instance.Write(ew);

      var actualBytes = await GetEndianBinaryWriterBytes(ew);

      er.Position = readerStartPos;
      var expectedBytes = er.ReadBytes(actualBytes.Length);
      Asserts.Equal(expectedBytes, actualBytes);

      if (assertExactEndPositions) {
        Asserts.Equal(expectedReadLength, actualBytes.Length);
      }
    }
  }
}