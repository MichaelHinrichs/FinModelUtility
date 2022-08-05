using NUnit.Framework;

using schema.util;


namespace schema.memory {
  public class MemoryRangeTest {
    [Test]
    public void TestCreateSubranges() {
      var memoryRange = new MemoryBlock(MemoryBlockType.FILE, 10);
      var firstRange =
          memoryRange.ClaimBlockWithin(MemoryBlockType.SECTION, 5, 5);
      var secondRange = memoryRange.ClaimBlockAtEnd(MemoryBlockType.SECTION, 5);

      AssertLengthAndOffsets(memoryRange, 15, 0, 0);
      AssertLengthAndOffsets(firstRange, 5, 5, 5);
      AssertLengthAndOffsets(secondRange, 5, 10, 10);
    }

    public static void AssertLengthAndOffsets(
        IMemoryRange range,
        long length,
        long relativeOffset,
        long absoluteOffset) {
      Assert.AreEqual(length, range.SizeInBytes);
      Assert.AreEqual(relativeOffset, range.GetRelativeOffsetInBytes());
      Assert.AreEqual(absoluteOffset, range.GetAbsoluteOffsetInBytes());
    }
  }
}