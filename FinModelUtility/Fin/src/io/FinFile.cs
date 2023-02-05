using System.IO;
using System.IO.Abstractions;

using fin.util.asserts;
using fin.util.json;

using schema.binary;


namespace fin.io {
  public class FinFile : BIoObject, IFile {
    public FinFile(string fullName) : base(fullName) {}

    public override bool Exists => FinFileSystem.File.Exists(this.FullName);

    public bool Delete() {
      if (!this.Exists) {
        return false;
      }

      FinFileSystem.File.Delete(this.FullName);
      return true;
    }

    public string Extension {
      get {
        var fullName = FullName;
        int length = fullName.Length;
        for (int i = length; --i >= 0;) {
          char ch = fullName[i];
          if (ch == '.')
            return fullName.Substring(i, length - i);
          if (ch == '\\' || ch == '/' || ch == Path.VolumeSeparatorChar)
            break;
        }
        return string.Empty;
      }
    }

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

    public T ReadNew<T>() where T : IDeserializable, new()
      => FileUtil.ReadNew<T>(this.FullName);

    public T ReadNew<T>(Endianness endianness) where T : IDeserializable, new()
      => FileUtil.ReadNew<T>(this.FullName, endianness);

    public byte[] ReadAllBytes()
      => FinFileSystem.File.ReadAllBytes(this.FullName);

    public string ReadAllText()
      => FinFileSystem.File.ReadAllText(this.FullName);

    public void WriteAllBytes(byte[] bytes)
      => File.WriteAllBytes(this.FullName, bytes);

    public StreamReader OpenReadAsText()
      => FileUtil.OpenReadAsText(this.FullName);

    public StreamWriter OpenWriteAsText()
      => FileUtil.OpenWriteAsText(this.FullName);

    public FileSystemStream OpenRead() => FileUtil.OpenRead(this.FullName);
    public FileSystemStream OpenWrite() => FileUtil.OpenWrite(this.FullName);

    public T Deserialize<T>() {
      var text = this.ReadAllText();
      return JsonUtil.Deserialize<T>(text);
    }

    public void Serialize<T>(T instance) where T : notnull {
      using var writer = this.OpenWriteAsText();
      writer.Write(JsonUtil.Serialize(instance));
    }


    public override bool Equals(object? other) {
      if (object.ReferenceEquals(this, other)) {
        return true;
      }

      if (other is not IFile otherFile) {
        return false;
      }

      return this.Equals(otherFile);
    }

    public bool Equals(IFile other) => this.FullName == other.FullName;

    public static bool operator ==(FinFile lhs, IFile rhs)
      => lhs.Equals(rhs);

    public static bool operator !=(FinFile lhs, IFile rhs)
      => !lhs.Equals(rhs);
  }
}