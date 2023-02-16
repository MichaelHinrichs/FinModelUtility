using schema.defaultinterface;

namespace fin.io {
  [IncludeDefaultInterfaceMethods]
  public readonly partial struct FinDirectory : IDirectory {
    public string FullName { get; }

    public static FinDirectory FromFullName(string fullName) => new(fullName);

    public FinDirectory(string fullName) {
      this.FullName = fullName;
    }
  }
}