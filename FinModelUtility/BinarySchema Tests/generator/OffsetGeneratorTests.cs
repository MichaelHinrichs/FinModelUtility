using NUnit.Framework;


namespace schema.text {
  internal class OffsetGeneratorTests {
    [Test] public void TestOffset() {
      SchemaTestUtil.AssertGenerated(@"
using schema;
using schema.attributes.offset;

namespace foo.bar {
  [Schema]
  public partial class OffsetWrapper : IBiSerializable {
    public uint BaseLocation { get; set; }

    public uint Offset { get; set; }

    [Offset(nameof(BaseLocation), nameof(Offset))]
    public byte Field { get; set; }
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class OffsetWrapper {
    public void Read(EndianBinaryReader er) {
      this.BaseLocation = er.ReadUInt32();
      this.Offset = er.ReadUInt32();
      {
        var tempLocation = er.Position;
        er.Position = this.BaseLocation + this.Offset;
        this.Field = er.ReadByte();
        er.Position = tempLocation;
      }
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class OffsetWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteUInt32(this.BaseLocation);
      ew.WriteUInt32(this.Offset);
      throw new NotImplementedException();
    }
  }
}
");
    }

    [Test]
    public void TestOffsetFromParent() {
      SchemaTestUtil.AssertGenerated(@"
using schema;
using schema.attributes.child_of;
using schema.attributes.offset;

namespace foo.bar {
  [Schema]
  public partial class OffsetWrapper : IBiSerializable, IChildOf<Parent> {
    public Parent Parent { get; set; }

    public uint Offset { get; set; }

    [Offset($""{nameof(Parent)}.{nameof(Parent.BaseLocation)}"", nameof(Offset))]
    public byte Field { get; set; }
  }

  public partial class Parent : IBiSerializable {
    public OffsetWrapper Child { get; set; }

    public uint BaseLocation { get; set; }
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class OffsetWrapper {
    public void Read(EndianBinaryReader er) {
      this.Offset = er.ReadUInt32();
      {
        var tempLocation = er.Position;
        er.Position = this.Parent.BaseLocation + this.Offset;
        this.Field = er.ReadByte();
        er.Position = tempLocation;
      }
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class OffsetWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteUInt32(this.Offset);
      throw new NotImplementedException();
    }
  }
}
");
    }
  }
}