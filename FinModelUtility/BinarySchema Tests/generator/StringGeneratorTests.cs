using NUnit.Framework;


namespace schema.text {
  internal class StringGeneratorTests {
    [Test]
    public void TestConstString() {
      SchemaTestUtil.AssertGenerated(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class StringWrapper {
    public readonly string Field = ""foo"";
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class StringWrapper {
    public void Read(EndianBinaryReader er) {
      er.AssertString(this.Field);
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class StringWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteString(this.Field);
    }
  }
}
");
    }

    [Test]
    public void TestConstEndianString() {
      SchemaTestUtil.AssertGenerated(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class StringWrapper {
    [EndianOrdered]
    public readonly string Field = ""foo"";
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class StringWrapper {
    public void Read(EndianBinaryReader er) {
      er.AssertStringEndian(this.Field);
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class StringWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteStringEndian(this.Field);
    }
  }
}
");
    }
  }
}