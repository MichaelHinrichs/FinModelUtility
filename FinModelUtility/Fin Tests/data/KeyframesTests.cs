using NUnit.Framework;
using schema.util;

namespace fin.data {
  public class KeyframesTests {
    [Test]
    public void TestAddToEnd() {
      var impl = new Keyframes<string>();

      impl.SetKeyframe(0, "first");
      impl.SetKeyframe(1, "second");
      impl.SetKeyframe(2, "third");

      AssertKeyframes_(impl,
        new Keyframe<string>(0, "first"),
        new Keyframe<string>(1, "second"),
        new Keyframe<string>(2, "third")
      );
    }

    [Test]
    public void TestReplace() {
      var impl = new Keyframes<string>();

      impl.SetKeyframe(0, "first");
      impl.SetKeyframe(0, "second");
      impl.SetKeyframe(0, "third");

      AssertKeyframes_(impl, new Keyframe<string>(0, "third"));
    }

    [Test]
    public void TestInsertAtFront() {
      var impl = new Keyframes<string>();

      impl.SetKeyframe(2, "third");
      impl.SetKeyframe(1, "second");
      impl.SetKeyframe(0, "first");

      AssertKeyframes_(impl,
        new Keyframe<string>(0, "first"),
        new Keyframe<string>(1, "second"),
        new Keyframe<string>(2, "third")
      );
    }

    [Test]
    public void TestInsertInMiddle() {
      var impl = new Keyframes<string>();

      impl.SetKeyframe(0, "0");
      impl.SetKeyframe(9, "9");
      impl.SetKeyframe(5, "5");
      impl.SetKeyframe(2, "2");
      impl.SetKeyframe(7, "7");

      AssertKeyframes_(impl,
        new Keyframe<string>(0, "0"),
        new Keyframe<string>(2, "2"),
        new Keyframe<string>(5, "5"),
        new Keyframe<string>(7, "7"),
        new Keyframe<string>(9, "9")
      );
    }

    [Test]
    public void TestGetKeyframes() {
      var impl = new Keyframes<string>();

      impl.SetKeyframe(0, "first");
      impl.SetKeyframe(2, "second");
      impl.SetKeyframe(4, "third");

      Assert.AreEqual(null, impl.GetKeyframeAtFrame(-1));
      Assert.AreEqual(new Keyframe<string>(0, "first"), impl.GetKeyframeAtFrame(0));
      Assert.AreEqual(new Keyframe<string>(2, "second"), impl.GetKeyframeAtFrame(3));
      Assert.AreEqual(new Keyframe<string>(4, "third"), impl.GetKeyframeAtFrame(5));
    }

    private void AssertKeyframes_(Keyframes<string> actual,
      params Keyframe<string>[] expected) {
      Asserts.Equal(expected, actual.Definitions);
    }
  }
}
