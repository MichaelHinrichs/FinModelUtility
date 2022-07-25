using NUnit.Framework;


namespace schema.text {
  internal class IfBooleanGeneratorTests {
    [Test]
    public void TestIfBoolean() {
      SchemaTestUtil.AssertGenerated(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class ByteWrapper : IBiSerializable {
    [IfBoolean(SchemaIntType.BYTE)]
    public A? ImmediateValue { get; set; }

    [Format(SchemaNumberType.BYTE)]
    public bool Field { get; set; }

    [IfBoolean(nameof(Field))]
    public int? OtherValue { get; set; }
  }

  public class A : IBiSerializable { }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class ByteWrapper {
    public void Read(EndianBinaryReader er) {
      {
        var b = er.ReadByte() != 0;
        if (b) {
          this.ImmediateValue = new A();
          this.ImmediateValue.Read(er);
        }
        else {
          this.ImmediateValue = null;
        }
      }
      this.Field = er.ReadByte() != 0;
      if (this.Field) {
        this.OtherValue = er.ReadInt32();
      }
      else {
        this.OtherValue = null;
      }
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class ByteWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteByte((byte) (this.ImmediateValue != null ? 1 : 0));
      if (this.ImmediateValue != null) {
        this.ImmediateValue.Write(ew);
      }
      ew.WriteByte((byte) (this.Field ? 1 : 0));
      if (this.Field) {
        ew.WriteInt32(this.OtherValue);
      }
    }
  }
}
");
    }


    [Test]
    public void TestUsingBoolFromChild() {
      SchemaTestUtil.AssertGenerated(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class ByteWrapper : IBiSerializable {
    public ClassWith1Bool Field { get; set; }

    [IfBoolean($""{nameof(Field)}.{nameof(Field.Bool)}"")]
    public int? OtherValue { get; set; }
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class ByteWrapper {
    public void Read(EndianBinaryReader er) {
      this.Field.Read(er);
      if (this.Field.Bool) {
        this.OtherValue = er.ReadInt32();
      }
      else {
        this.OtherValue = null;
      }
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class ByteWrapper {
    public void Write(EndianBinaryWriter ew) {
      this.Field.Write(ew);
      if (this.Field.Bool) {
        ew.WriteInt32(this.OtherValue);
      }
    }
  }
}
");
    }
  }
}