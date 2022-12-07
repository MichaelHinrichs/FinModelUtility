using NUnit.Framework;


namespace schema.attributes.size {
  internal class SizeOfMemberInBytesGeneratorTests {
    [Test]
    public void TestSizeOfInStructure() {
      SchemaTestUtil.AssertGenerated(@"
using schema;
using schema.attributes.size;

namespace foo.bar {
  [BinarySchema]
  public partial class SizeWrapper : IBiSerializable {
    [SizeOfMemberInBytes(nameof(Foo)]
    public uint FooSize { get; set; }

    public byte Foo;
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class SizeWrapper {
    public void Read(EndianBinaryReader er) {
      this.FooSize = er.ReadUInt32();
      this.Foo = er.ReadByte();
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class SizeWrapper {
    public void Write(ISubEndianBinaryWriter ew) {
      ew.WriteUInt32Delayed(ew.GetSizeOfMemberRelativeToScope(""Foo"").ContinueWith(task => (uint) task.Result));
      ew.MarkStartOfMember(""Foo"");
      ew.WriteByte(this.Foo);
      ew.MarkEndOfMember();
    }
  }
}
");
    }

    [Test]
    public void TestSizeOfThroughChild() {
      SchemaTestUtil.AssertGenerated(@"
using schema;
using schema.attributes.size;

namespace foo.bar {
  [BinarySchema]
  public partial class SizeWrapper : IBiSerializable {
    [SizeOfMemberInBytes($""{nameof(Foo)}.{nameof(Foo.Bar}"")]
    public uint FooBarSize { get; set; }

    public Child Foo;
  }

  [BinarySchema]
  public partial class Child : IBiSerializable {
    public byte Bar;
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class SizeWrapper {
    public void Read(EndianBinaryReader er) {
      this.FooBarSize = er.ReadUInt32();
      this.Foo.Read(er);
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class SizeWrapper {
    public void Write(ISubEndianBinaryWriter ew) {
      ew.WriteUInt32Delayed(ew.GetSizeOfMemberRelativeToScope(""Foo.Bar"").ContinueWith(task => (uint) task.Result));
      ew.MarkStartOfMember(""Foo"");
      this.Foo.Write(ew);
      ew.MarkEndOfMember();
    }
  }
}
");
    }
  }
}