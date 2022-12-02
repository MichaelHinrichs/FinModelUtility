using NUnit.Framework;


namespace schema.text {
  internal class PositionGeneratorTests {
    [Test]
    public void TestPosition() {
      SchemaTestUtil.AssertGenerated(@"
using schema;
using schema.attributes.position;

namespace foo.bar {
  [BinarySchema]
  public partial class PositionWrapper : IBiSerializable {
    [Position]
    public long Position { get; set; }

    public byte Value { get; set; }

    [Position]
    public long ExpectedPosition { get; }
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class PositionWrapper {
    public void Read(EndianBinaryReader er) {
      this.Position = er.Position;
      this.Value = er.ReadByte();
      er.AssertPosition(this.ExpectedPosition);
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class PositionWrapper {
    public void Write(ISubEndianBinaryWriter ew) {
      ew.WriteByte(this.Value);
    }
  }
}
");
    }
  }
}