using NUnit.Framework;


namespace schema.text {
  internal class BooleanGeneratorTests {
    [Test] public void TestByte() {
      SchemaTestUtil.AssertGenerated(@"
using schema;

namespace foo.bar {
  [BinarySchema]
  public partial class ByteWrapper {
    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool Field { get; set; }

    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool ReadonlyField { get; }
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class ByteWrapper {
    public void Read(EndianBinaryReader er) {
      this.Field = er.ReadByte() != 0;
      er.AssertByte(this.ReadonlyField ? 1 : 0);
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class ByteWrapper {
    public void Write(ISubEndianBinaryWriter ew) {
      ew.WriteByte((byte) (this.Field ? 1 : 0));
      ew.WriteByte((byte) (this.ReadonlyField ? 1 : 0));
    }
  }
}
");
    }
  }
}