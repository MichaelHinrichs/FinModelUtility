using NUnit.Framework;


namespace schema.text {
  internal class IChildOfGeneratorTests {
    [Test]
    public void TestChild() {
      SchemaTestUtil.AssertGenerated(@"
using schema;
using schema.attributes.child_of;

namespace foo.bar {
  [Schema]
  public partial class ChildOfWrapper : IBiSerializable, IChildOf<Parent> {
    public Parent Parent { get; set; }

    public byte Field { get; set; }
  }

  public partial class Parent {
    public ChildOfWrapper Child { get; set; }
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class ChildOfWrapper {
    public void Read(EndianBinaryReader er) {
      this.Field = er.ReadByte();
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class ChildOfWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteByte(this.Field);
    }
  }
}
");
    }

    [Test]
    public void TestParent() {
      SchemaTestUtil.AssertGenerated(@"
using schema;
using schema.attributes.child_of;

namespace foo.bar {
  [Schema]
  public partial class Parent {
    public ChildOfWrapper Child { get; set; }
  }

  public partial class ChildOfWrapper : IBiSerializable, IChildOf<Parent> {
    public Parent Parent { get; set; }

    public byte Field { get; set; }
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class Parent {
    public void Read(EndianBinaryReader er) {
      this.Child.Parent = this;
      this.Child.Read(er);
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class Parent {
    public void Write(EndianBinaryWriter ew) {
      this.Child.Write(ew);
    }
  }
}
");
    }
  }
}