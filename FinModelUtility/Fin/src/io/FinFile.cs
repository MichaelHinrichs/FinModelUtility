using schema.defaultinterface;

namespace fin.io {
  [IncludeDefaultInterfaceMethods]
  public readonly partial struct FinFile : IFile {
    public string FullName { get; }

    public static FinFile FromFullName(string fullName) => new(fullName);

    public FinFile(string fullName) {
      this.FullName = fullName;
    }
  }
}