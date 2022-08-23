using NUnit.Framework;


namespace schema.text {
  internal class StringGeneratorTests {
    [Test]
    public void TestConstString() {
      SchemaTestUtil.AssertGenerated(@"
using schema;

namespace foo.bar {
  [BinarySchema]
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
  [BinarySchema]
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

    [Test]
    public void TestConstLengthString() {
      SchemaTestUtil.AssertGenerated(@"
using schema;

namespace foo.bar {
  [BinarySchema]
  public partial class StringWrapper {
    [StringLengthSource(3)]
    public string Field;
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class StringWrapper {
    public void Read(EndianBinaryReader er) {
      this.Field = er.ReadString(3);
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class StringWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteStringWithExactLength(this.Field, 3);
    }
  }
}
");
    }

    [Test]
    public void TestConstLengthEndianString() {
      SchemaTestUtil.AssertGenerated(@"
using schema;

namespace foo.bar {
  [BinarySchema]
  public partial class StringWrapper {
    [StringLengthSource(3)]
    [EndianOrdered]
    public string Field;
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class StringWrapper {
    public void Read(EndianBinaryReader er) {
      this.Field = er.ReadStringEndian(3);
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