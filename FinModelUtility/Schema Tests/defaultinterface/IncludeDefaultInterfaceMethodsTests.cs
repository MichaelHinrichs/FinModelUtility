using NUnit.Framework;

using schema.binary;


namespace schema.defaultinterface {
  internal class IncludeDefaultInterfaceMethodsTests {
    [Test]
    public void TestHandlesNamespaces() {
      DefaultInterfaceMethodsTestUtil.AssertGenerated(@"
using schema.defaultinterface;

namespace foo.bar {
  interface IInterface {
    void Something() => 1;
  }

  [IncludeDefaultInterfaceMethods]
  public partial class Class : IInterface {
  }
}
",
        @"using schema.defaultinterface;

namespace foo.bar {
  public partial class Class {
    public void Something() => 1;
  }
}
");
    }

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
",
        @"public partial class Class {
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
",
        @"using schema.defaultinterface;

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
",
        @"public partial class Class {
}
");
    }

    [Test]
    public void TestIgnoresGenericMethodWithImplementation() {
      DefaultInterfaceMethodsTestUtil.AssertGenerated(@"
using schema.defaultinterface;

interface IInterface<T> {
  T Something() {
    var a = 0;
  }
}

[IncludeDefaultInterfaceMethods]
public partial class Class : IInterface<int> {
  public int Something() {
    // Do nothing
  }
}
",
        @"public partial class Class {
}
");
    }

    [Test]
    public void TestIncludesGenericMethodWithoutImplementation() {
      DefaultInterfaceMethodsTestUtil.AssertGenerated(@"
using schema.defaultinterface;

interface IInterface<T> {
  T Something() {
    var a = 0;
  }
}

[IncludeDefaultInterfaceMethods]
public partial class Class : IInterface<int> {
}
",
        @"using schema.defaultinterface;

public partial class Class {
  public int Something() {
    var a = 0;
  }
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
",
        @"public partial class Class {
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
",
        @"using schema.defaultinterface;

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
",
        @"public partial class Class {
}
");
    }

    [Test]
    public void TestIgnoresGenericPropertyWithImplementation() {
      DefaultInterfaceMethodsTestUtil.AssertGenerated(@"
using schema.defaultinterface;

interface IInterface<T> {
  T Something => default;
}

[IncludeDefaultInterfaceMethods]
public partial class Class : IInterface<int> {
  public int Something => 1;
}
",
        @"public partial class Class {
}
");
    }

    [Test]
    public void TestIncludesGenericPropertyWithoutImplementation() {
      DefaultInterfaceMethodsTestUtil.AssertGenerated(@"
using schema.defaultinterface;

interface IInterface<T> {
  T Something => default;
}

[IncludeDefaultInterfaceMethods]
public partial class Class : IInterface<int> {
}
",
        @"using schema.defaultinterface;

public partial class Class {
  public int Something => default;
}
");
    }

    [Test]
    public void TestHandlesPropertiesWithAttributes() {
      DefaultInterfaceMethodsTestUtil.AssertGenerated(@"
using schema.defaultinterface;

interface IInterface<T> {
  [Something]
  T Something => default;
}

[IncludeDefaultInterfaceMethods]
public partial class Class : IInterface<int> {
}
",
        @"using schema.defaultinterface;

public partial class Class {
  [Something]
  public int Something => default;
}
");
    }
  }
}