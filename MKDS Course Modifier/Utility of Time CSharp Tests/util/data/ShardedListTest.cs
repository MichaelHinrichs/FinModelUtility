using System.Linq;

using UoT.util;

using NUnit.Framework;

namespace UoT.util.data {
  public class ShardedListTest {
    [Test]
    public void ShardingNegativeThrows() {
      var shardedList = ShardedList<int>.From(1, 2, 3, 4, 5, 6);
      Assert.Throws<AssertException>(() => shardedList.Shard(-1, 1));
    }

    [Test]
    public void ShardingZeroThrows() {
      var shardedList = ShardedList<int>.From(1, 2, 3, 4, 5, 6);
      Assert.Throws<AssertException>(() => shardedList.Shard(0, 0));
    }

    [Test]
    public void ShardingLargeThrows() {
      var shardedList = ShardedList<int>.From(1, 2, 3, 4, 5, 6);
      Assert.Throws<AssertException>(() => shardedList.Shard(0, 7));
    }

    [Test]
    public void ShardingFullListReturnsSelf() {
      var shardedList = ShardedList<int>.From(1, 2, 3, 4, 5, 6);
      var subRegion = shardedList.Shard(0, 6);

      Assert.AreSame(shardedList, subRegion);
    }


    [Test]
    public void DoubleShardingThrows() {
      var shardedList = ShardedList<int>.From(1, 2, 3, 4, 5, 6);
      shardedList.Shard(2, 4);

      Assert.Throws<AssertException>(() => shardedList.Shard(2, 4));
    }

    [Test]
    public void IteratingOverSingleShard() {
      var shardedList = ShardedList<int>.From(1, 2, 3, 4, 5, 6);
      Assert.AreEqual(new[] {1, 2, 3, 4, 5, 6}, shardedList.ToArray());
    }

    [Test]
    public void IteratingOverSubRegion() {
      var shardedList = ShardedList<int>.From(1, 2, 3, 4, 5, 6);
      var subRegion = shardedList.Shard(1, 4);

      Assert.AreEqual(new[] {1, 2, 3, 4, 5, 6}, shardedList.ToArray());
      Assert.AreEqual(new[] {2, 3, 4, 5}, subRegion.ToArray());
    }

    [Test]
    public void IteratingOverSubSubRegion() {
      var shardedList = ShardedList<int>.From(1, 2, 3, 4, 5, 6);
      var subRegion = shardedList.Shard(1, 4);
      var subSubRegion = subRegion.Shard(1, 2);

      Assert.AreEqual(new[] {1, 2, 3, 4, 5, 6}, shardedList.ToArray());
      Assert.AreEqual(new[] {3, 4}, subSubRegion.ToArray());
    }


    [Test]
    public void IteratingOverNeighboringRegions() {
      var shardedList = ShardedList<int>.From(1, 2, 3, 4, 5, 6);
      var subRegion1 = shardedList.Shard(0, 2);
      var subRegion2 = shardedList.Shard(2, 2);
      var subRegion3 = shardedList.Shard(4, 2);

      Assert.AreEqual(new[] {1, 2, 3, 4, 5, 6}, shardedList.ToArray());
      Assert.AreEqual(new[] {1, 2}, subRegion1.ToArray());
      Assert.AreEqual(new[] {3, 4}, subRegion2.ToArray());
      Assert.AreEqual(new[] {5, 6}, subRegion3.ToArray());
    }

    [Test]
    public void ShardingOverRegionThrows() {
      var shardedList = ShardedList<int>.From(1, 2, 3, 4, 5, 6);
      shardedList.Shard(0, 2);
      shardedList.Shard(4, 2);

      Assert.Throws<AssertException>(() => shardedList.Shard(2, 3));
    }


    [Test]
    public void ResizingSingleShardDown() {
      var shardedList = ShardedList<int>.From(1, 2, 3, 4, 5, 6);

      shardedList.Resize(5);

      Assert.AreEqual(new[] { 1, 2, 3, 4, 5 }, shardedList.ToArray());
    }

    [Test]
    public void ResizingSingleShardUp() {
      var shardedList = ShardedList<int>.From(1, 2, 3, 4, 5, 6);
      shardedList.Resize(7);
      Assert.AreEqual(new[] { 1, 2, 3, 4, 5, 6, 0 }, shardedList.ToArray());
    }

    [Test]
    public void ResizingSubRegionDown() {
      var shardedList = ShardedList<int>.From(1, 2, 3, 4, 5, 6);
      var subRegion = shardedList.Shard(1, 4);

      subRegion.Resize(3);

      Assert.AreEqual(new[] { 1, 2, 3, 4, 6 }, shardedList.ToArray());
      Assert.AreEqual(new[] { 2, 3, 4 }, subRegion.ToArray());
    }

    [Test]
    public void ResizingSubRegionUp() {
      var shardedList = ShardedList<int>.From(1, 2, 3, 4, 5, 6);
      var subRegion = shardedList.Shard(1, 4);

      subRegion.Resize(5);

      Assert.AreEqual(new[] { 1, 2, 3, 4, 5, 0, 6 }, shardedList.ToArray());
      Assert.AreEqual(new[] { 2, 3, 4, 5, 0 }, subRegion.ToArray());
    }


    [Test]
    public void ShardsAsExpected() {
      var shardedList = ShardedList<int>.From(1, 2, 3, 4, 5, 6);
      Assert.AreEqual(1, shardedList.RegionCount);

      var subRegion1 = shardedList.Shard(0, 2);
      Assert.AreEqual(1, subRegion1.RegionCount);
      var subRegion2 = shardedList.Shard(2, 2);
      Assert.AreEqual(1, subRegion2.RegionCount);
      var subRegion3 = shardedList.Shard(4, 2);
      Assert.AreEqual(1, subRegion3.RegionCount);

      Assert.AreEqual(3, shardedList.RegionCount);

      var subRegion1a = subRegion1.Shard(0, 1);
      var subRegion1b = subRegion1.Shard(1, 1);
      Assert.AreEqual(2, subRegion1.RegionCount);

      Assert.AreEqual(3, shardedList.RegionCount);
    }
  }
}