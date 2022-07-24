﻿using NUnit.Framework;


namespace schema.text {
  internal class AlignGeneratorTests {
    [Test] public void TestAlign() {
      SchemaTestUtil.AssertGenerated(@"
using schema;
using schema.attributes.align;

namespace foo.bar {
  [Schema]
  public partial class AlignWrapper : IBiSerializable {
    [Align(0x2)]
    public byte Field { get; set; }
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class AlignWrapper {
    public void Read(EndianBinaryReader er) {
      er.Align(2);
      this.Field = er.ReadByte();
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class AlignWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.Align(2);
      ew.WriteByte(this.Field);
    }
  }
}
");
    }
  }
}