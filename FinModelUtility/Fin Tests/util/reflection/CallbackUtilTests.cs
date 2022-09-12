using fin.util.asserts;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace fin.util.reflection {
  [TestClass]
  public class CallbackUtilTests {
    [TestMethod]
    public void TestGet1ParameterNameFromFunc() {
      Expect.AreArraysEqual(new[] {"test"},
                            CallbackUtil.GetParameterNames((byte test) => true));
    }

    [TestMethod]
    public void TestGet2ParameterNamesFromFunc() {
      Expect.AreArraysEqual(new[] {"foo", "bar"},
                            CallbackUtil.GetParameterNames(
                                (int foo, float bar) => true));
    }
  }
}