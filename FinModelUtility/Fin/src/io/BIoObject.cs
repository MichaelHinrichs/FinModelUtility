using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace fin.io {
  public abstract class BIoObject : IIoObject {
    protected BIoObject(string fullName)
      => this.FullName = Path.GetFullPath(fullName);

    public string Name => Path.GetFileName(this.FullName);
    public string FullName { get; }


    public abstract bool Exists { get; }

    public string? GetParentFullName() => Path.GetDirectoryName(this.FullName);

    public IDirectory? GetParent() {
      var parentFullName = GetParentFullName();
      return parentFullName != null
          ? new FinDirectory(parentFullName)
          : null;
    }

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

    public override string ToString() => this.FullName;
    public override int GetHashCode() => this.FullName.GetHashCode();
  }
}