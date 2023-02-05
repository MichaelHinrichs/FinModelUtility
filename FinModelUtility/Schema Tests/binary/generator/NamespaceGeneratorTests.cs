using NUnit.Framework;


namespace schema.binary.text {
  internal class NamespaceGeneratorTests {
    [Test]
    public void TestFromSameNamespace() {
      SchemaTestUtil.AssertGenerated(@"
using schema.binary;

namespace foo.bar {
  public enum A : byte {
  }

  [BinarySchema]
  public partial class Wrapper : IBiSerializable {
    public A Field { get; set; }
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class Wrapper {
    public void Read(IEndianBinaryReader er) {
      this.Field = (A) er.ReadByte();
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class Wrapper {
    public void Write(ISubEndianBinaryWriter ew) {
      ew.WriteByte((byte) this.Field);
    }
  }
}
");
    }

    [Test]
    public void TestFromHigherNamespace() {
      SchemaTestUtil.AssertGenerated(@"
using schema.binary;

namespace foo {
  public enum A : byte {
  }

  namespace bar {
    [BinarySchema]
    public partial class Wrapper : IBiSerializable {
      public A Field { get; set; }
    }
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class Wrapper {
    public void Read(IEndianBinaryReader er) {
      this.Field = (A) er.ReadByte();
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class Wrapper {
    public void Write(ISubEndianBinaryWriter ew) {
      ew.WriteByte((byte) this.Field);
    }
  }
}
");
    }

    [Test]
    public void TestFromLowerNamespace() {
      SchemaTestUtil.AssertGenerated(@"
using schema.binary;

namespace foo.bar {
  namespace goo {
    public enum A : byte {
    }
  }

  [BinarySchema]
  public partial class Wrapper : IBiSerializable {
    public goo.A Field { get; set; }
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class Wrapper {
    public void Read(IEndianBinaryReader er) {
      this.Field = (goo.A) er.ReadByte();
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class Wrapper {
    public void Write(ISubEndianBinaryWriter ew) {
      ew.WriteByte((byte) this.Field);
    }
  }
}
");
    }

    [Test]
    public void TestFromSimilarNamespace() {
      SchemaTestUtil.AssertGenerated(@"
using schema.binary;

namespace foo.bar {
  namespace goo {
    public enum A : byte {
    }
  }

  namespace gar {
    [BinarySchema]
    public partial class Wrapper : IBiSerializable {
      public goo.A Field { get; set; }
    }
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar.gar {
  public partial class Wrapper {
    public void Read(IEndianBinaryReader er) {
      this.Field = (goo.A) er.ReadByte();
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar.gar {
  public partial class Wrapper {
    public void Write(ISubEndianBinaryWriter ew) {
      ew.WriteByte((byte) this.Field);
    }
  }
}
");
    }
  }
}