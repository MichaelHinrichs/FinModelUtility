using fin.schema.data;

using NUnit.Framework;

using schema.binary;
using schema.binary.testing;

using System.IO;
using System.Threading.Tasks;


namespace fin.schema {
  internal partial class SizedSectionTests {
    [BinarySchema]
    public partial class A : IBinaryConvertible {
      public byte Field1 { get; set; }
      public byte Field2 { get; set; }
      public byte Field3 { get; set; }
    }

    [Test]
    public async Task TestSizedSection() {
      var sizedSection = new AutoUInt32SizedSection<A>();
      sizedSection.Data.Field1 = 12;
      sizedSection.Data.Field2 = 23;
      sizedSection.Data.Field3 = 34;

      await BinarySchemaAssert.WritesAndReadsIdentically(sizedSection);

      var ew = new EndianBinaryWriter(Endianness.LittleEndian);
      sizedSection.Write(ew);
      var bytes = await BinarySchemaAssert.GetEndianBinaryWriterBytes(ew);
      BinarySchemaAssert.AssertSequence(bytes,
                                        new byte[] { 3, 0, 0, 0, 12, 23, 34 });
    }
  }
}