using NUnit.Framework;


namespace schema.text {
  internal class EnumGeneratorTests {
    [Test]
    public void TestEnum() {
      SchemaTestUtil.AssertGenerated(@"
using schema;

namespace foo.bar {
  enum A {}

  enum B : int {
  }
 
  [Schema]
  public partial class EnumWrapper {
    [Format(SchemaNumberType.BYTE)]
    public A fieldA;

    public B fieldB;
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class EnumWrapper {
    public void Read(EndianBinaryReader er) {
      this.fieldA = (A) er.ReadByte();
      this.fieldB = (B) er.ReadInt32();
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class EnumWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteByte((byte) this.fieldA);
      ew.WriteInt32((int) this.fieldB);
    }
  }
}
");
    }
  }
}