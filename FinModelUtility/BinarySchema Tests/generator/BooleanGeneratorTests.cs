using NUnit.Framework;


namespace schema.text {
  internal class BooleanGeneratorTests {
    [Test] public void TestByte() {
      SchemaTestUtil.AssertGenerated(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class ByteWrapper {
    [Format(SchemaNumberType.BYTE)]
    public bool Field { get; set; }

    [Format(SchemaNumberType.BYTE)]
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
    public void Write(EndianBinaryWriter ew) {
      ew.WriteByte((byte) (this.Field ? 1 : 0));
      ew.WriteByte((byte) (this.ReadonlyField ? 1 : 0));
    }
  }
}
");
    }
  }
}