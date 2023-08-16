using fin.util.asserts;

using NUnit.Framework;

namespace fin.util.reflection {
  public class CallbackUtilTests {
    [Test]
    public void TestGet1ParameterNameFromFunc() {
      Expect.AreArraysEqual(new[] { "test" },
                            CallbackUtil.GetParameterNames(
                                (byte test) => true));
    }

    [Test]
    public void TestGet2ParameterNamesFromFunc() {
      Expect.AreArraysEqual(new[] { "foo", "bar" },
                            CallbackUtil.GetParameterNames(
                                (int foo, float bar) => true));
    }
  }
}