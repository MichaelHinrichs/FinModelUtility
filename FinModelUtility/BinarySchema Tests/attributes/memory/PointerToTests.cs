using NUnit.Framework;
using schema.attributes.child_of;
using schema.testing;
using System.IO;
using System.Threading.Tasks;


namespace schema.attributes.memory {
  internal partial class PointerToTests {
    [BinarySchema]
    public partial class ParentImpl : IBiSerializable {
      public Child Child { get; } = new();

      public int Field { get; set; }
    }

    [BinarySchema]
    public partial class Child : IChildOf<ParentImpl>, IBiSerializable {
      public ParentImpl Parent { get; set; }

      [PointerTo($"{nameof(Parent)}.{nameof(ParentImpl.Field)}")]
      private byte fieldPointer_;
    }


    [Test]
    public async Task TestPointerToThroughParent() {
      var parent = new ParentImpl();
      parent.Field = 12;

      var ew = new EndianBinaryWriter();
      parent.Write(ew);

      var bytes = await BinarySchemaAssert.GetEndianBinaryWriterBytes(ew);
      BinarySchemaAssert.AssertSequence(
          bytes,
          new byte[] {1, 12, 0, 0, 0});
    }
  }
}