using System.IO;

using fin.util.asserts;

namespace fin.io {
  public class FinFile : IFile {
    public FinFile(FileInfo fileInfo) => this.Info = fileInfo;
    public FinFile(string fullName) => this.Info = new FileInfo(fullName);

    public FileInfo Info { get; }

    public string Name => this.Info.Name;
    public string FullName => this.Info.FullName;

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

    public IDirectory? GetParent() => new FinDirectory(this.Info.Directory);
    public bool Exists => this.Info.Exists;

    public StreamReader ReadAsText() => File.OpenText(this.FullName);
    public byte[] SkimAllBytes() => File.ReadAllBytes(this.FullName);

    public FileStream OpenRead() => File.OpenRead(this.FullName);
    public FileStream OpenWrite() => File.OpenWrite(this.FullName);
  }
}