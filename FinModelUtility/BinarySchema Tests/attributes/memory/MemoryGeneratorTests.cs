using NUnit.Framework;


namespace schema.attributes.memory {
  internal class MemoryGeneratorTests {
    [Test]
    public void TestOtherFieldBlock() {
      SchemaTestUtil.AssertGenerated(@"
using schema;
using schema.attributes.memory;
using schema.memory;

namespace foo.bar {
  [Schema]
  public partial class BlockWrapper {
    public long Size;

    [Block(nameof(Size))]
    public IMemoryBlock Block;

    public long Offset;

    [Pointer(nameof(Block), nameof(Offset))]
    public float Field;
  }
}",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class BlockWrapper {
    public void Read(EndianBinaryReader er) {
      this.Size = er.ReadInt64();
      this.Block = new MemoryBlock(MemoryBlockType.DATA, this.Size);
      this.Offset = er.ReadInt64();
      this.Pointer = this.Block.ClaimPointerWithin(
        this.Offset,
        er => {
          this.Field = ew.ReadSingle();
        },
        ew => {
          ew.WriteSingle(this.Field);
        });
      this.Pointer.Read(er);
    }
  }
}
",
                                     @"using System;
using System.IO;
namespace foo.bar {
  public partial class BlockWrapper {
    public void Write(EndianBinaryWriter ew) {
      ew.WriteInt64(this.Size);
      er.WriteInt64(this.Offset);
      this.Pointer.Write(ew);
    }
  }
}
");
    }
  }
}