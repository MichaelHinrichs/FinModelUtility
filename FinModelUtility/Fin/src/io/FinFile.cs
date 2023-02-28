using schema.defaultinterface;

namespace fin.io {
  [IncludeDefaultInterfaceMethods]
  public readonly partial struct FinFile : IFile {
    public string FullName { get; }

    public FinFile(string fullName) {
      this.FullName = fullName;
    }

    public override string ToString() => this.FullName;
  }
}