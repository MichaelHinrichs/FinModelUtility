using NUnit.Framework;


namespace schema.memory {
  public class NestedRangesTest {
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
      var nestedRanges = new NestedRanges<int>(-1, -1, 0);
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

      var thirdChild = nestedRanges.ClaimSubrangeAtEnd(3,  3);
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
  }
}
