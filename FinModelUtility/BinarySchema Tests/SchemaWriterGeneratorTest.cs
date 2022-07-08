using Microsoft.CodeAnalysis;

using NUnit.Framework;

namespace schema.text {
  public class SchemaWriterGeneratorTest {
    [SetUp]
    public void Setup() {}

    [Test]
    public void TestByte() {
      this.AssertGenerated_(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class ByteWrapper {
    public byte Field { get; }
  }
}",
                            @"using System;
using System.IO;
namespace foo.bar {
  public partial class ByteWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteByte(this.Field);
    }
  }
}
");
    }

    [Test]
    public void TestSByte() {
      this.AssertGenerated_(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class SByteWrapper {
    public sbyte Field { get; }
  }
}",
                            @"using System;
using System.IO;
namespace foo.bar {
  public partial class SByteWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteSByte(this.Field);
    }
  }
}
");
    }

    [Test]
    public void TestInt16() {
      this.AssertGenerated_(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class ShortWrapper {
    public short Field { get; }
  }
}",
                            @"using System;
using System.IO;
namespace foo.bar {
  public partial class ShortWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteInt16(this.Field);
    }
  }
}
");
    }

    [Test]
    public void TestConstArray() {
      this.AssertGenerated_(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class ArrayWrapper {
    public readonly int[] field;
  }
}",
                            @"using System;
using System.IO;
namespace foo.bar {
  public partial class ArrayWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteInt32s(this.field);
    }
  }
}
");
    }

    [Test]
    public void TestArrayOtherMemberLength() {
      this.AssertGenerated_(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class ArrayWrapper {
    public int length;

    [ArrayLengthSource(nameof(ArrayWrapper.length))]
    public int[] field;
  }
}",
                            @"using System;
using System.IO;
namespace foo.bar {
  public partial class ArrayWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteInt32(this.length);
      ew.WriteInt32s(this.field);
    }
  }
}
");
    }

    [Test]
    public void TestNestedClass() {
      this.AssertGenerated_(@"
using schema;

namespace foo.bar {
  static internal partial class Parent {
    protected partial class Middle {
      public enum ValueEnum {
        A, B
      }

      [Schema]
      private partial class Wrapper {
        public int length;
        [Format(SchemaNumberType.INT32)]
        public ValueEnum value;
      }
    }
  }
}",
                            @"using System;
using System.IO;
namespace foo.bar {
  static internal partial class Parent {
    protected partial class Middle {
      private partial class Wrapper {
        public void Write(EndianBinaryWriter ew) {
          ew.WriteInt32(this.length);
          ew.WriteInt32((int) this.value);
        }
      }
    }
  }
}
");
    }

    [Test]
    public void TestConstCharArray() {
      this.AssertGenerated_(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class CharWrapper {
    public char[] Array { get; }
  }
}",
                            @"using System;
using System.IO;
namespace foo.bar {
  public partial class CharWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteChars(this.Array);
    }
  }
}
");
    }

    [Test]
    public void TestField() {
      this.AssertGenerated_(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class ShortWrapper {
    public short Field { get; }
  }
}",
                            @"using System;
using System.IO;
namespace foo.bar {
  public partial class ShortWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteInt16(this.Field);
    }
  }
}
");
    }

    [Test]
    public void TestProperty() {
      this.AssertGenerated_(@"
using schema;

namespace foo.bar {
  [Schema]
  public class ByteWrapper {
    public byte field { get; set; }
  }
}",
                            @"using System;
using System.IO;
namespace foo.bar {
  public partial class ByteWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteByte(this.field);
    }
  }
}
");
    }

    [Test]
    public void TestReadonlyPrimitiveField() {
      this.AssertGenerated_(@"
using schema;

namespace foo.bar {
  [Schema]
  public class ByteWrapper {
    public readonly byte field;
  }
}",
                            @"using System;
using System.IO;
namespace foo.bar {
  public partial class ByteWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteByte(this.field);
    }
  }
}
");
    }

    [Test]
    public void TestReadonlyPrimitiveProperty() {
      this.AssertGenerated_(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class ByteWrapper {
    public byte Field { get; }
  }
}",
                            @"using System;
using System.IO;
namespace foo.bar {
  public partial class ByteWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteByte(this.Field);
    }
  }
}
");
    }

    [Test]
    public void TestEverything() {
      this.AssertGenerated_(@"
using schema;

namespace foo {
  namespace bar {
    [Schema]
    public partial class EverythingWrapper : IBiSerializable {
      public readonly string magicText = ""foobar"";

      public byte byteField;
      public sbyte sbyteProperty { get; set; }
      public readonly short constShortField;
      public ushort constUshortProperty { get; }
     
      public ShortEnum nakedShortField;
      public readonly ShortEnum constNakedShortField;
      [Format(SchemaNumberType.INT32)]
      public ShortEnum intField;
      [Format(SchemaNumberType.INT32)]
      public readonly ShortEnum constIntField;

      public readonly int[] constLengthIntValues;
      [ArrayLengthSource(IntType.UINT32)]
      public int[] intValues;

      public Other other;
      [ArrayLengthSource(IntType.INT32)]
      public Other[] others;

      [Format(SchemaNumberType.UN16)]
      public float normalized;
      [Format(SchemaNumberType.UN16)]
      public readonly float constNormalized = 0;
    }

    public enum ShortEnum : short {
      A, B, C
    }

    [Schema]
    public partial class Other : IBiSerializable {
    }
  }
}",
                            @"using System;
using System.IO;
namespace foo.bar {
  public partial class EverythingWrapper {
    public void Write(EndianBinaryWriter ew) {" +
                            @"
      ew.WriteString(this.magicText);" +
                            @"
      ew.WriteByte(this.byteField);
      ew.WriteSByte(this.sbyteProperty);
      ew.WriteInt16(this.constShortField);
      ew.WriteUInt16(this.constUshortProperty);" +
                            @"
      ew.WriteInt16((short) this.nakedShortField);
      ew.WriteInt16((short) this.constNakedShortField);
      ew.WriteInt32((int) this.intField);
      ew.WriteInt32((int) this.constIntField);" +
                            @"
      ew.WriteInt32s(this.constLengthIntValues);
      ew.WriteUInt32((uint) this.intValues.Length);
      ew.WriteInt32s(this.intValues);" +
                            @"
      this.other.Write(ew);
      ew.WriteInt32((int) this.others.Length);
      foreach (var e in this.others) {
        e.Write(ew);
      }
      ew.WriteUn16(this.normalized);
      ew.WriteUn16(this.constNormalized);
    }
  }
}
");
    }

    private void AssertGenerated_(string src, string expectedGenerated) {
      var structure = SchemaTestUtil.Parse(src);
      Assert.IsEmpty(structure.Diagnostics);

      var actualGenerated = new SchemaWriterGenerator().Generate(structure);
      Assert.AreEqual(expectedGenerated, actualGenerated.ReplaceLineEndings());
    }
  }
}