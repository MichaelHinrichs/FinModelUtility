using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using schema.util;
using System.Collections;


namespace schema.testing {
  public static class BinarySchemaAssert {
    public static async Task<byte[]> GetEndianBinaryWriterBytes(
        EndianBinaryWriter ew) {
      var outputStream = new MemoryStream();
      await ew.CompleteAndCopyToDelayed(outputStream);
      return outputStream.ToArray();
    }

    public static async Task WritesAndReadsIdentically<T>(
        T value,
        Endianness endianess = Endianness.LittleEndian)
        where T : IBiSerializable, new() {
      var ew = new EndianBinaryWriter(endianess);
      value.Write(ew);

      var actualBytes = await GetEndianBinaryWriterBytes(ew);

      var er = new EndianBinaryReader(new MemoryStream(actualBytes), endianess);
      await ReadsAndWritesIdentically<T>(er);
    }

    public static async Task ReadsAndWritesIdentically<T>(EndianBinaryReader er)
        where T : IBiSerializable, new() {
      var readerStartPos = er.Position;
      var instance = er.ReadNew<T>();
      var readerEndPos = er.Position;

      var expectedLength = (int)(readerEndPos - readerStartPos);

      er.Position = readerStartPos;
      var expectedBytes = er.ReadBytes(expectedLength);

      var ew = new EndianBinaryWriter(er.Endianness);
      instance.Write(ew);

      var actualBytes = await GetEndianBinaryWriterBytes(ew);

      Asserts.Equal(expectedLength, actualBytes.Length);
      Asserts.Equal(expectedBytes, actualBytes);
    }

    public static void AssertSequence<T>(
        IEnumerable<T> enumerableA,
        IEnumerable<T> enumerableB) {
      var enumeratorA = enumerableA.GetEnumerator();
      var enumeratorB = enumerableB.GetEnumerator();

      var hasA = enumeratorA.MoveNext();
      var hasB = enumeratorB.MoveNext();

      var index = 0;
      while (hasA && hasB) {
        var currentA = enumeratorA.Current;
        var currentB = enumeratorB.Current;

        if (!object.Equals(currentA, currentB)) {
          Asserts.Fail(
              $"Expected {currentA} to equal {currentB} at index ${index}.");
        }
        index++;

        hasA = enumeratorA.MoveNext();
        hasB = enumeratorB.MoveNext();
      }

      Asserts.True(!hasA && !hasB,
                   "Expected enumerables to be equal:\n" +
                   $"  A: {ConvertSequenceToString_(enumerableA)}\n" +
                   $"  B: {ConvertSequenceToString_(enumerableB)}");
    }

    private static string ConvertSequenceToString_(IEnumerable enumerable) {
      var str = new StringBuilder();
      foreach (var value in enumerable) {
        if (str.Length > 0) {
          str.Append(", ");
        }
        str.Append(value);
      }
      return str.ToString();
    }
  }
}