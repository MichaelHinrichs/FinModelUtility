using System.IO;

using fin.util.asserts;

namespace fin.io {
  public class FinFile : IFile {
    private readonly FileInfo impl_;

    public FinFile(FileInfo fileInfo) => this.impl_ = fileInfo;
    public FinFile(string fullName) => this.impl_ = new FileInfo(fullName);

    public string Name => this.impl_.Name;
    public string FullName => this.impl_.FullName;

    public string Extension => this.impl_.Extension;

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

    public IDirectory? GetParent() => new FinDirectory(this.impl_.Directory);
    public bool Exists => this.impl_.Exists;

    public StreamReader ReadAsText() => File.OpenText(this.FullName);
    public byte[] SkimAllBytes() => File.ReadAllBytes(this.FullName);
  }
}