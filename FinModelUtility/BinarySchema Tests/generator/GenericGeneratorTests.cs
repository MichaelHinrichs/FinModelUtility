using NUnit.Framework;


namespace schema.text {
  internal class GenericGeneratorTests {
    [Test]
    public void TestGenericStructure() {
      SchemaTestUtil.AssertGenerated(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class GenericWrapper<T> : IBiSerializable where T : IBiSerializable, new() {
    public T Data { get; } = new();
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class GenericWrapper<T> {
    public void Read(EndianBinaryReader er) {
      this.Data.Read(er);
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class GenericWrapper<T> {
    public void Write(EndianBinaryWriter ew) {
      this.Data.Write(ew);
    }
  }
}
");
    }

    [Test]
    public void TestGenericStructureArray() {
      SchemaTestUtil.AssertGenerated(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class GenericWrapper<T> : IBiSerializable where T : IBiSerializable, new() {
    public T[] Data { get; } = {};
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class GenericWrapper<T> {
    public void Read(EndianBinaryReader er) {
      this.Data.Read(er);
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class ByteWrapper {
    public void Write(EndianBinaryWriter ew) {
      this.Data.Write(er);
    }
  }
}
");
    }
  }
}