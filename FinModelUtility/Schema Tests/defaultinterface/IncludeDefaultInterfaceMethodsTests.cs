using NUnit.Framework;

using schema.binary;


namespace schema.defaultinterface {
  internal class IncludeDefaultInterfaceMethodsTests {
    [Test]
    public void TestIgnoresMethodWithoutImplementation() {
      DefaultInterfaceMethodsTestUtil.AssertGenerated(@"
using schema.defaultinterface;

interface IInterface {
  void Something();
}

[IncludeDefaultInterfaceMethods]
public partial class Class : IInterface {
}
", @"public partial class Class {
}
");
    }

    [Test]
    public void TestIncludesMethodWithImplementation() {
      DefaultInterfaceMethodsTestUtil.AssertGenerated(@"
using schema.defaultinterface;

interface IInterface {
  void Something() {
    var a = 0;
  }
}

[IncludeDefaultInterfaceMethods]
public partial class Class : IInterface {
}
", @"using schema.defaultinterface;

public partial class Class {
  public void Something() {
    var a = 0;
  }
}
");
    }
  }
}