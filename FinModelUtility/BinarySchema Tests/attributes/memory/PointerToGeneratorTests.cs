﻿using NUnit.Framework;


namespace schema.attributes.memory {
  internal class PointerToGeneratorTests {
    [Test]
    public void TestPointerToInStructure() {
      SchemaTestUtil.AssertGenerated(@"
using schema;
using schema.attributes.memory;

namespace foo.bar {
  [BinarySchema]
  public partial class SizeWrapper : IBiSerializable {
    [PointerTo(nameof(Foo)]
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
      ew.WriteUInt32Delayed(ew.GetPointerToMemberRelativeToScope(""Foo"").ContinueWith(task => (uint) task.Result));
      ew.MarkStartOfMember(""Foo"");
      ew.WriteByte(this.Foo);
      ew.MarkEndOfMember();
    }
  }
}
");
    }

    [Test]
    public void TestPointerToThroughChild() {
      SchemaTestUtil.AssertGenerated(@"
using schema;
using schema.attributes.memory;

namespace foo.bar {
  [BinarySchema]
  public partial class SizeWrapper : IBiSerializable {
    [PointerTo(nameof(Foo.Bar)]
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
      ew.WriteUInt32Delayed(ew.GetPointerToMemberRelativeToScope(""Foo.Bar"").ContinueWith(task => (uint) task.Result));
      ew.MarkStartOfMember(""Foo"");
      this.Foo.Write(ew);
      ew.MarkEndOfMember();
    }
  }
}
");
    }

    [Test]
    public void TestPointerToThroughParent() {
      SchemaTestUtil.AssertGeneratedForAll(@"
using schema;
using schema.attributes.child_of;
using schema.attributes.memory;

namespace foo.bar {
  [BinarySchema]
  public partial class SizeWrapper : IChildOf<ParentImpl>, IBiSerializable {
    public ParentImpl Parent;

    [PointerTo(nameof(Parent.Foo)]
    public uint FooSize { get; set; }
  }

  [BinarySchema]
  public partial class ParentImpl : IBiSerializable {
    public SizeWrapper Child;

    public byte Foo;
  }
}",
// Size Wrapper                                           
                                           (@"using System;
using System.IO;
namespace foo.bar {
  public partial class SizeWrapper {
    public void Read(EndianBinaryReader er) {
      this.FooSize = er.ReadUInt32();
    }
  }
}
",
                                            @"using System;
using System.IO;
namespace foo.bar {
  public partial class SizeWrapper {
    public void Write(ISubEndianBinaryWriter ew) {
      ew.WriteUInt32Delayed(ew.GetPointerToMemberRelativeToScope(""Foo"").ContinueWith(task => (uint) task.Result));
    }
  }
}
"),
// Parent Impl
                                           (@"using System;
using System.IO;
namespace foo.bar {
  public partial class ParentImpl {
    public void Read(EndianBinaryReader er) {
      this.Child.Parent = this;
      this.Child.Read(er);
      this.Foo = er.ReadByte();
    }
  }
}
",
                                            @"using System;
using System.IO;
namespace foo.bar {
  public partial class ParentImpl {
    public void Write(ISubEndianBinaryWriter ew) {
      this.Child.Write(ew);
      ew.MarkStartOfMember(""Foo"");
      ew.WriteByte(this.Foo);
      ew.MarkEndOfMember();
    }
  }
}
"));
    }
  }
}