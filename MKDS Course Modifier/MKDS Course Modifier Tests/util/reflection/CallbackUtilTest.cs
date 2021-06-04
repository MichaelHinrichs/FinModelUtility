using fin.util.asserts;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.util.reflection {
  [TestClass]
  public class CallbackUtilTest {
    [TestMethod]
    public void TestGet1ParameterNameFromFunc() {
      Expect.AreEqual(new[] {"test"},
                      CallbackUtil.GetParameterNames((byte test) => true));
    }

    [TestMethod]
    public void TestGet2ParameterNamesFromFunc() {
      Expect.AreEqual(new[] {"foo", "bar"},
                      CallbackUtil.GetParameterNames(
                          (int foo, float bar) => true));
    }


    /*[TestMethod]
    public void TestGet1ParametersNameFromAction() {
      Expect.AreEqual(new[] {"test"},
                      CallbackUtil.GetParameterNames(
                          (byte test) => {}));
    }

    [TestMethod]
    public void TestGet2ParametersFromAction() {
      Expect.AreEqual(new[] {"foo", "bar"},
                      CallbackUtil.GetParameterNames(
                          (int foo, float bar) => {}
                      ));
    }*/
  }
}