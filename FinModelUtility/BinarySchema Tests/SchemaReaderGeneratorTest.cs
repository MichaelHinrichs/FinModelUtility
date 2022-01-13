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
                            @"using System.IO;
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
                            @"using System.IO;
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
                            @"using System.IO;
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
                            @"using System.IO;
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
    public void TestField() {
      this.AssertGenerated_(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class ShortWrapper {
    public short Field { get; }
  }
}",
                            @"using System.IO;
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
                            @"using System.IO;
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
                            @"using System.IO;
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
                            @"using System.IO;
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
    public enum ShortEnum : short {
      A, B, C
    }

    [Schema]
    public class EverythingWrapper {
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
    }
  }
}",
                            @"using System.IO;
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
      er.AssertInt32((int) this.constIntField);
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