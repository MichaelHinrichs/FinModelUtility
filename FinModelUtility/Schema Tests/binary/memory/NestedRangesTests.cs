
using NUnit.Framework;


namespace schema.binary.memory {
  public class NestedRangesTests {
    [Test]
    public void TestClaimingChildrenWithin() {
      var nestedRanges = new NestedRanges<int>(-1, -1, 10);
      Assert.AreEqual(-1, nestedRanges.Data);
      Assert.AreEqual(10, nestedRanges.Length);

      var zeroethChild = nestedRanges.ClaimSubrangeWithin(0, 0, 0);
      Assert.AreEqual(0, zeroethChild.Data);
      Assert.AreEqual(0, zeroethChild.GetAbsoluteOffset());
      Assert.AreEqual(0, zeroethChild.GetRelativeOffset());
      Assert.AreEqual(0, zeroethChild.Length);

      var firstChild = nestedRanges.ClaimSubrangeWithin(1, 0, 1);
      Assert.AreEqual(1, firstChild.Data);
      Assert.AreEqual(0, firstChild.GetAbsoluteOffset());
      Assert.AreEqual(0, firstChild.GetRelativeOffset());
      Assert.AreEqual(1, firstChild.Length);

      var secondChild = nestedRanges.ClaimSubrangeWithin(2, 1, 2);
      Assert.AreEqual(2, secondChild.Data);
      Assert.AreEqual(1, secondChild.GetAbsoluteOffset());
      Assert.AreEqual(1, secondChild.GetRelativeOffset());
      Assert.AreEqual(2, secondChild.Length);

      var thirdChild = nestedRanges.ClaimSubrangeWithin(3, 3, 3);
      Assert.AreEqual(3, thirdChild.Data);
      Assert.AreEqual(3, thirdChild.GetAbsoluteOffset());
      Assert.AreEqual(3, thirdChild.GetRelativeOffset());
      Assert.AreEqual(3, thirdChild.Length);

      var fourthChild = nestedRanges.ClaimSubrangeWithin(4, 6, 4);
      Assert.AreEqual(4, fourthChild.Data);
      Assert.AreEqual(6, fourthChild.GetAbsoluteOffset());
      Assert.AreEqual(6, fourthChild.GetRelativeOffset());
      Assert.AreEqual(4, fourthChild.Length);

      Assert.AreEqual(10, nestedRanges.Length);
    }

    [Test]
    public void TestClaimingChildrenAtEnd() {
      var nestedRanges = new NestedRanges<int>(-1, -1);
      Assert.AreEqual(-1, nestedRanges.Data);
      Assert.AreEqual(0, nestedRanges.Length);

      var zeroethChild = nestedRanges.ClaimSubrangeAtEnd(0, 0);
      Assert.AreEqual(0, zeroethChild.Data);
      Assert.AreEqual(0, zeroethChild.GetAbsoluteOffset());
      Assert.AreEqual(0, zeroethChild.GetRelativeOffset());
      Assert.AreEqual(0, zeroethChild.Length);

      var firstChild = nestedRanges.ClaimSubrangeAtEnd(1, 1);
      Assert.AreEqual(1, firstChild.Data);
      Assert.AreEqual(0, firstChild.GetAbsoluteOffset());
      Assert.AreEqual(0, firstChild.GetRelativeOffset());
      Assert.AreEqual(1, firstChild.Length);

      var secondChild = nestedRanges.ClaimSubrangeAtEnd(2, 2);
      Assert.AreEqual(2, secondChild.Data);
      Assert.AreEqual(1, secondChild.GetAbsoluteOffset());
      Assert.AreEqual(1, secondChild.GetRelativeOffset());
      Assert.AreEqual(2, secondChild.Length);

      var thirdChild = nestedRanges.ClaimSubrangeAtEnd(3, 3);
      Assert.AreEqual(3, thirdChild.Data);
      Assert.AreEqual(3, thirdChild.GetAbsoluteOffset());
      Assert.AreEqual(3, thirdChild.GetRelativeOffset());
      Assert.AreEqual(3, thirdChild.Length);

      var fourthChild = nestedRanges.ClaimSubrangeAtEnd(4, 4);
      Assert.AreEqual(4, fourthChild.Data);
      Assert.AreEqual(6, fourthChild.GetAbsoluteOffset());
      Assert.AreEqual(6, fourthChild.GetRelativeOffset());
      Assert.AreEqual(4, fourthChild.Length);

      Assert.AreEqual(10, nestedRanges.Length);
    }

    [Test]
    public void TestResizingSingleRange() {
      var nestedRanges = new NestedRanges<int>(-1, 0, 25);
      Assert.AreEqual(25, nestedRanges.Length);

      {
        nestedRanges.ResizeInPlace(50);
        Assert.AreEqual(50, nestedRanges.Length);

        nestedRanges.ResizeInPlace(25);
        Assert.AreEqual(25, nestedRanges.Length);
      }

      {
        nestedRanges.ResizeSelfAndParents(50);
        Assert.AreEqual(50, nestedRanges.Length);

        nestedRanges.ResizeSelfAndParents(25);
        Assert.AreEqual(25, nestedRanges.Length);
      }
    }

    [Test]
    public void TestResizingNestedRanges() {
      var outerRanges = new NestedRanges<int>(-1, 0);
      var middleRanges1 = outerRanges.ClaimSubrangeAtEnd(-1);
      var innerRanges1_1 = middleRanges1.ClaimSubrangeAtEnd(-1);
      var innerRanges1_2 = middleRanges1.ClaimSubrangeAtEnd(-1);
      var middleRanges2 = outerRanges.ClaimSubrangeAtEnd(-1);
      var innerRanges2_1 = middleRanges2.ClaimSubrangeAtEnd(-1);
      var innerRanges2_2 = middleRanges2.ClaimSubrangeAtEnd(-1);

      AssertLengthAndOffsets(outerRanges, 0, 0, 0);
      AssertLengthAndOffsets(middleRanges1, 0, 0, 0);
      AssertLengthAndOffsets(innerRanges1_1, 0, 0, 0);
      AssertLengthAndOffsets(innerRanges1_2, 0, 0, 0);
      AssertLengthAndOffsets(middleRanges2, 0, 0, 0);
      AssertLengthAndOffsets(innerRanges2_1, 0, 0, 0);
      AssertLengthAndOffsets(innerRanges2_2, 0, 0, 0);

      innerRanges1_1.ResizeSelfAndParents(1);
      innerRanges1_2.ResizeSelfAndParents(2);
      innerRanges2_1.ResizeSelfAndParents(3);
      innerRanges2_2.ResizeSelfAndParents(4);

      AssertLengthAndOffsets(outerRanges, 10, 0, 0);
      AssertLengthAndOffsets(middleRanges1, 3, 0, 0);
      AssertLengthAndOffsets(innerRanges1_1, 1, 0, 0);
      AssertLengthAndOffsets(innerRanges1_2, 2, 1, 1);
      AssertLengthAndOffsets(middleRanges2, 7, 3, 3);
      AssertLengthAndOffsets(innerRanges2_1, 3, 0, 3);
      AssertLengthAndOffsets(innerRanges2_2, 4, 3, 6);
    }

    [Test]
    public void TestUnclaimedRanges() {
      var parentRanges = new NestedRanges<int>(-1, 0, 10);

      var range1 = parentRanges.ClaimSubrangeWithin(1, 1, 1);
      var range2 = parentRanges.ClaimSubrangeWithin(2, 3, 1);
      var range3 = parentRanges.ClaimSubrangeWithin(3, 5, 1);
      var range4 = parentRanges.ClaimSubrangeWithin(4, 7, 1);

      AssertLengthAndOffsets(parentRanges, 10, 0, 0);
      AssertLengthAndOffsets(range1, 1, 1, 1);
      AssertLengthAndOffsets(range2, 1, 3, 3);
      AssertLengthAndOffsets(range3, 1, 5, 5);
      AssertLengthAndOffsets(range4, 1, 7, 7);

      range1.ResizeInPlace(2);
      range2.ResizeSelfAndParents(2);
      range3.ClaimSubrangeAtEnd(-1, 1);
      range4.FreeAndMarkAsUnclaimed();

      AssertLengthAndOffsets(parentRanges, 12, 0, 0);
      AssertLengthAndOffsets(range1, 2, 1, 1);
      AssertLengthAndOffsets(range2, 2, 3, 3);
      AssertLengthAndOffsets(range3, 2, 6, 6);

      parentRanges.RemoveAllUnclaimedSpace();

      AssertLengthAndOffsets(parentRanges, 6, 0, 0);
      AssertLengthAndOffsets(range1, 2, 0, 0);
      AssertLengthAndOffsets(range2, 2, 2, 2);
      AssertLengthAndOffsets(range3, 2, 4, 4);
    }

    [Test]
    public void TestMergingUnclaimedRanges() {
      var parentRanges = new NestedRanges<int>(-1, 0);

      var range1 = parentRanges.ClaimSubrangeAtEnd(1, 1);
      var range2 = parentRanges.ClaimSubrangeAtEnd(2, 1);
      var range3 = parentRanges.ClaimSubrangeAtEnd(3, 1);
      var range4 = parentRanges.ClaimSubrangeAtEnd(4, 1);

      AssertLengthAndOffsets(parentRanges, 4, 0, 0);
      AssertLengthAndOffsets(range1, 1, 0, 0);
      AssertLengthAndOffsets(range2, 1, 1, 1);
      AssertLengthAndOffsets(range3, 1, 2, 2);
      AssertLengthAndOffsets(range4, 1, 3, 3);

      range2.FreeAndMarkAsUnclaimed();
      range3.FreeAndMarkAsUnclaimed();
      range4.FreeAndMarkAsUnclaimed();

      AssertLengthAndOffsets(parentRanges, 4, 0, 0);
      AssertLengthAndOffsets(range1, 1, 0, 0);

      range1.ResizeInPlace(4);

      AssertLengthAndOffsets(parentRanges, 4, 0, 0);
      AssertLengthAndOffsets(range1, 4, 0, 0);
    }

    [Test]
    public void TestInvalidatingLengthsAllClaimed() {
      var outerRanges = new NestedRanges<int>(-1, 0);
      var middleRanges1 = outerRanges.ClaimSubrangeAtEnd(-1);
      var innerRanges1_1 = middleRanges1.ClaimSubrangeAtEnd(-1);
      var innerRanges1_2 = middleRanges1.ClaimSubrangeAtEnd(-1);
      var middleRanges2 = outerRanges.ClaimSubrangeAtEnd(-1);
      var innerRanges2_1 = middleRanges2.ClaimSubrangeAtEnd(-1);
      var innerRanges2_2 = middleRanges2.ClaimSubrangeAtEnd(-1);

      outerRanges.InvalidateLengthOfSelfAndChildren();

      Assert.False(outerRanges.IsLengthValid);
      Assert.False(middleRanges1.IsLengthValid);
      Assert.False(innerRanges1_1.IsLengthValid);
      Assert.False(innerRanges1_2.IsLengthValid);
      Assert.False(middleRanges2.IsLengthValid);
      Assert.False(innerRanges2_1.IsLengthValid);
      Assert.False(innerRanges2_2.IsLengthValid);

      innerRanges1_1.ResizeSelfAndParents(1);
      innerRanges1_2.ResizeSelfAndParents(2);
      innerRanges2_1.ResizeSelfAndParents(3);
      innerRanges2_2.ResizeSelfAndParents(4);

      AssertLengthAndOffsets(outerRanges, 10, 0, 0);
      AssertLengthAndOffsets(middleRanges1, 3, 0, 0);
      AssertLengthAndOffsets(innerRanges1_1, 1, 0, 0);
      AssertLengthAndOffsets(innerRanges1_2, 2, 1, 1);
      AssertLengthAndOffsets(middleRanges2, 7, 3, 3);
      AssertLengthAndOffsets(innerRanges2_1, 3, 0, 3);
      AssertLengthAndOffsets(innerRanges2_2, 4, 3, 6);
      Assert.True(outerRanges.IsLengthValid);
      Assert.True(middleRanges1.IsLengthValid);
      Assert.True(innerRanges1_1.IsLengthValid);
      Assert.True(innerRanges1_2.IsLengthValid);
      Assert.True(middleRanges2.IsLengthValid);
      Assert.True(innerRanges2_1.IsLengthValid);
      Assert.True(innerRanges2_2.IsLengthValid);
    }

    public static void AssertLengthAndOffsets<T>(
        IReadonlyNestedRanges<T> range,
        long length,
        long relativeOffset,
        long absoluteOffset) {
      Assert.AreEqual(length, range.Length);
      Assert.AreEqual(relativeOffset, range.GetRelativeOffset());
      Assert.AreEqual(absoluteOffset, range.GetAbsoluteOffset());
    }
  }
}