using System.IO.Abstractions;
using System.Runtime.CompilerServices;

using schema.defaultinterface;

namespace fin.io {
  [IncludeDefaultInterfaceMethods]
  public readonly partial struct FinFile : ISystemFile {
    public string FullName { get; }
    public string DisplayFullName => this.FullName;

    public FinFile(string fullName) {
      this.FullName = fullName;
    }

    public override string ToString() => this.FullName;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FileSystemStream OpenRead()
      => FinFileStatic.OpenRead(this.FullName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FileSystemStream OpenWrite()
      => FinFileStatic.OpenWrite(this.FullName);
  }
}