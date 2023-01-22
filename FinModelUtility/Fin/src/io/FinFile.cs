using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.util.asserts;
using fin.util.json;

using schema;


namespace fin.io {
  public class FinFile : IFile {
    public FinFile(FileInfo fileInfo) => this.Info = fileInfo;
    public FinFile(string fullName) => this.Info = new FileInfo(fullName);

    public FileInfo Info { get; }

    public string Name => this.Info.Name;
    public string FullName => this.Info.FullName;

    private string? absolutePath_ = null;

    public string GetAbsolutePath() {
      if (this.absolutePath_ == null) {
        this.absolutePath_ = Path.GetFullPath(this.FullName);
      }

      return this.absolutePath_;
    }

    public bool Exists => File.Exists(this.FullName);

    public string Extension => this.Info.Extension;

    public string FullNameWithoutExtension
      => this.FullName.Substring(0,
                                 this.FullName.Length -
                                 this.Extension.Length);

    public string NameWithoutExtension
      => this.Name.Substring(0, this.Name.Length - this.Extension.Length);

    public IFile CloneWithExtension(string newExtension) {
      Asserts.True(newExtension.StartsWith("."),
                   $"'{newExtension}' is not a valid extension!");
      return new FinFile(this.FullNameWithoutExtension + newExtension);
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

    public T ReadNew<T>() where T : IDeserializable, new()
      => FileUtil.ReadNew<T>(this.FullName);

    public T ReadNew<T>(Endianness endianness) where T : IDeserializable, new()
      => FileUtil.ReadNew<T>(this.FullName, endianness);

    public byte[] ReadAllBytes() => FileUtil.ReadAllBytes(this.FullName);
    public string ReadAllText() => FileUtil.ReadAllText(this.FullName);

    public void WriteAllBytes(byte[] bytes)
      => File.WriteAllBytes(this.FullName, bytes);

    public StreamReader OpenReadAsText()
      => FileUtil.OpenReadAsText(this.FullName);

    public StreamWriter OpenWriteAsText()
      => FileUtil.OpenWriteAsText(this.FullName);

    public FileStream OpenRead() => FileUtil.OpenRead(this.FullName);
    public FileStream OpenWrite() => FileUtil.OpenWrite(this.FullName);

    public T Deserialize<T>() {
      var text = this.ReadAllText();
      return JsonUtil.Deserialize<T>(text);
    }

    public void Serialize<T>(T instance) where T : notnull {
      using var writer = this.OpenWriteAsText();
      writer.Write(JsonUtil.Serialize(instance));
    }

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

    public bool Equals(IFile other)
      => this.GetAbsolutePath() == other.GetAbsolutePath();

    public override int GetHashCode() => this.FullName.GetHashCode();

    public static bool operator ==(FinFile lhs, IFile rhs)
      => lhs.Equals(rhs);

    public static bool operator !=(FinFile lhs, IFile rhs)
      => !lhs.Equals(rhs);
  }
}