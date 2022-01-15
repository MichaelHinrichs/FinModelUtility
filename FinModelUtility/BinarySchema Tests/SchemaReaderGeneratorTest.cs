using Microsoft.CodeAnalysis;

using NUnit.Framework;

namespace schema.text {
  public class SchemaReaderGeneratorTest {
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
    public void Read(EndianBinaryReader er) {
      er.AssertByte(this.Field);
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
    public void Read(EndianBinaryReader er) {
      er.AssertSByte(this.Field);
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
    public void Read(EndianBinaryReader er) {
      er.AssertInt16(this.Field);
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
    public void Read(EndianBinaryReader er) {
      er.ReadInt32s(this.field);
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
    public void Read(EndianBinaryReader er) {
      this.length = er.ReadInt32();
      if (this.length < 0) {
        throw new Exception(""Expected length to be nonnegative!"");
      }
      this.field = new System.Int32[this.length];
      er.ReadInt32s(this.field);
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
      [Schema]
      private partial class Wrapper {
        public int length;
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
        public void Read(EndianBinaryReader er) {
          this.length = er.ReadInt32();
        }
      }
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
    public void Read(EndianBinaryReader er) {
      er.AssertInt16(this.Field);
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
    public void Read(EndianBinaryReader er) {
      this.field = er.ReadByte();
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
    public void Read(EndianBinaryReader er) {
      er.AssertByte(this.field);
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
    public void Read(EndianBinaryReader er) {
      er.AssertByte(this.Field);
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
    public partial class EverythingWrapper : IDeserializable {
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
    }

    public enum ShortEnum : short {
      A, B, C
    }

    [Schema]
    public partial class Other : IDeserializable {
    }
  }
}",
                            @"using System;
using System.IO;
namespace foo.bar {
  public partial class EverythingWrapper {
    public void Read(EndianBinaryReader er) {" +
                            @"
      er.AssertString(this.magicText);" +
                            @"
      this.byteField = er.ReadByte();
      this.sbyteProperty = er.ReadSByte();
      er.AssertInt16(this.constShortField);
      er.AssertUInt16(this.constUshortProperty);" +
                            @"
      this.nakedShortField = (foo.bar.ShortEnum) er.ReadInt16();
      er.AssertInt16((short) this.constNakedShortField);
      this.intField = (foo.bar.ShortEnum) er.ReadInt32();
      er.AssertInt32((int) this.constIntField);" +
                            @"
      er.ReadInt32s(this.constLengthIntValues);
      {
        var c = er.ReadUInt32();
        if (c < 0) {
          throw new Exception(""Expected length to be nonnegative!"");
        }
        this.intValues = new System.Int32[c];
      }
      er.ReadInt32s(this.intValues);" +
                            @"
      this.other.Read(er);
      {
        var c = er.ReadInt32();
        if (c < 0) {
          throw new Exception(""Expected length to be nonnegative!"");
        }
        this.others = new foo.bar.Other[c];
        for (var i = 0; i < c; ++i) {
          this.others[i] = new foo.bar.Other();
        }
      }
      foreach (var e in this.others) {
        e.Read(er);
      }
    }
  }
}
");
    }

    private void AssertGenerated_(string src, string expectedGenerated) {
      var structure = SchemaTestUtil.Parse(src);
      Assert.IsEmpty(structure.Diagnostics);

      var actualGenerated = new SchemaReaderGenerator().Generate(structure);
      Assert.AreEqual(expectedGenerated, actualGenerated);
    }
  }
}