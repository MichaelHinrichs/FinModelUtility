using NUnit.Framework;


namespace schema.text {
  internal class ArrayLengthSourceGeneratorTests {
    [Test]
    public void TestConstLength() {
      SchemaTestUtil.AssertGenerated(@"
using schema;

namespace foo.bar {
  [BinarySchema]
  public partial class ConstLengthWrapper : IBiSerializable {
    [ArrayLengthSource(3)]
    public int[] Field { get; set; }
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class ConstLengthWrapper {
    public void Read(EndianBinaryReader er) {
      this.Field = new System.Int32[3];
      er.ReadInt32s(this.Field);
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class ConstLengthWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteInt32s(this.Field);
    }
  }
}
");
    }
  }
}