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

    [Test]
    public void TestIgnoresMethodIfAlreadyImplemented() {
      DefaultInterfaceMethodsTestUtil.AssertGenerated(@"
using schema.defaultinterface;

interface IInterface {
  void Something() {
    var a = 0;
  }
}

[IncludeDefaultInterfaceMethods]
public partial class Class : IInterface {
  public void Something() {
    // Do nothing
  }
}
", @"public partial class Class {
}
");
    }

    [Test]
    public void TestIgnoresPropertyWithoutImplementation() {
      DefaultInterfaceMethodsTestUtil.AssertGenerated(@"
using schema.defaultinterface;

interface IInterface {
  byte Something { get; }
}

[IncludeDefaultInterfaceMethods]
public partial class Class : IInterface {
}
", @"public partial class Class {
}
");
    }

    [Test]
    public void TestIncludesPropertyWithImplementation() {
      DefaultInterfaceMethodsTestUtil.AssertGenerated(@"
using schema.defaultinterface;

interface IInterface {
  byte Something => 1;
}

[IncludeDefaultInterfaceMethods]
public partial class Class : IInterface {
}
", @"using schema.defaultinterface;

public partial class Class {
  public byte Something => 1;
}
");
    }

    [Test]
    public void TestIgnoresPropertyIfAlreadyImplemented() {
      DefaultInterfaceMethodsTestUtil.AssertGenerated(@"
using schema.defaultinterface;

interface IInterface {
  byte Something => 1;
}

[IncludeDefaultInterfaceMethods]
public partial class Class : IInterface {
  public byte Something => 3;
}
", @"public partial class Class {
}
");
    }
  }
}