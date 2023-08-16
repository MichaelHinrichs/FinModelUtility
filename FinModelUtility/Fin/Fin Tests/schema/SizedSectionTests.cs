using System.Threading.Tasks;

using fin.schema.data;

using NUnit.Framework;

using schema.binary;
using schema.binary.testing;

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

      var ew = new EndianBinaryWriter(Endianness.LittleEndian);
      sizedSection.Write(ew);
      var bytes = await BinarySchemaAssert.GetEndianBinaryWriterBytes(ew);
      BinarySchemaAssert.AssertSequence(bytes,
                                        new byte[] { 3, 0, 0, 0, 12, 23, 34 });

      await BinarySchemaAssert.WritesAndReadsIdentically(sizedSection);
    }


    [BinarySchema]
    public partial class TweakedASection : IBinaryConvertible {
      public AutoUInt32SizedSection<A> Section { get; } = new() {
          TweakReadSize = -4,
      };
    }

    [Test]
    public async Task TestSizedSectionWithTweakedLength() {
      var sizedSection = new TweakedASection();
      
      var data = sizedSection.Section.Data;
      data.Field1 = 12;
      data.Field2 = 23;
      data.Field3 = 34;

      var ew = new EndianBinaryWriter(Endianness.LittleEndian);
      sizedSection.Write(ew);
      var bytes = await BinarySchemaAssert.GetEndianBinaryWriterBytes(ew);
      BinarySchemaAssert.AssertSequence(bytes,
                                        new byte[] { 7, 0, 0, 0, 12, 23, 34 });

      await BinarySchemaAssert.WritesAndReadsIdentically(sizedSection);
    }
  }
}