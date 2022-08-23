using NUnit.Framework;


namespace schema.attributes.ignore {
  internal class IgnoreGeneratorTests {
    [Test] public void TestAlign() {
      SchemaTestUtil.AssertGenerated(@"
using schema;
using schema.attributes.ignore;

namespace foo.bar {
  [BinarySchema]
  public partial class IgnoreWrapper : IBiSerializable {
    [Ignore]
    public byte Field { get; set; }
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class IgnoreWrapper {
    public void Read(EndianBinaryReader er) {
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class IgnoreWrapper {
    public void Write(EndianBinaryWriter ew) {
    }
  }
}
");
    }
  }
}