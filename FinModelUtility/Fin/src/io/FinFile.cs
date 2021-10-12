using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.util.asserts;

namespace fin.io {
  public class FinFile : IFile {
    public FinFile(FileInfo fileInfo) => this.Info = fileInfo;
    public FinFile(string fullName) => this.Info = new FileInfo(fullName);

    public FileInfo Info { get; }

    public string Name => this.Info.Name;
    public string FullName => this.Info.FullName;

    public bool Exists => File.Exists(this.FullName);

    public string Extension => this.Info.Extension;

    public IFile CloneWithExtension(string newExtension) {
      Asserts.True(newExtension.StartsWith("."),
                   $"'{newExtension}' is not a valid extension!");

      var oldExtension = this.Extension;

      var newFullName = this.FullName;
      var i = newFullName.LastIndexOf(oldExtension);
      if (i >= 0) {
        newFullName = newFullName.Substring(0, i) + newExtension;
      }

      return new FinFile(newFullName);
    }

    public IDirectory? GetParent()
      => this.Info.Directory != null
             ? new FinDirectory(this.Info.Directory)
             : null;

    public IDirectory[] GetAncestry() {
      var parents = new LinkedList<IDirectory>();
      IDirectory? parent = null;
      do {
        parent = parent == null ? this.GetParent() : parent.GetParent();
        if (parent != null) {
          parents.AddLast(parent);
        }
      } while (parent != null);
      return parents.ToArray();
    }

    public StreamReader ReadAsText() => File.OpenText(this.FullName);
    public byte[] SkimAllBytes() => File.ReadAllBytes(this.FullName);

    public FileStream OpenRead() => File.OpenRead(this.FullName);
    public FileStream OpenWrite() => File.OpenWrite(this.FullName);

    public override string ToString() => this.FullName;


    public override bool Equals(object? other) {
      if (object.ReferenceEquals(this, other)) {
        return true;
      }
      if (other is not IFile otherFile) {
        return false;
      }
      return this.Equals(otherFile);
    }

    public bool Equals(IFile other) {
      return Path.GetFullPath(this.FullName) ==
             Path.GetFullPath(other.FullName);
    }

    public override int GetHashCode() => this.FullName.GetHashCode();

    public static bool operator ==(FinFile lhs, IFile rhs)
      => lhs.Equals(rhs);

    public static bool operator !=(FinFile lhs, IFile rhs)
      => !lhs.Equals(rhs);
  }
}