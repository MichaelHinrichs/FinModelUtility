using System.Collections.Generic;
using System.Linq;


namespace fin.io.bundles {
  public interface IFileBundleDirectory {
    string Name { get; }

    IReadOnlyList<IFileBundleDirectory> Subdirs { get; }
    IReadOnlyList<IFileBundle> FileBundles { get; }

    IFileBundleDirectory AddSubdir(string name);
    void AddFileBundle(IFileBundle fileBundle);

    void RemoveEmptyChildren();
  }

  public class FileBundleDirectory : IFileBundleDirectory {
    private readonly List<IFileBundleDirectory> subdirs_ = new();
    private readonly List<IFileBundle> fileBundles_ = new();

    public FileBundleDirectory(string name) {
      this.Name = name;
    }

    public string Name { get; }

    public IReadOnlyList<IFileBundleDirectory> Subdirs => this.subdirs_;
    public IReadOnlyList<IFileBundle> FileBundles => this.fileBundles_;

    public IFileBundleDirectory AddSubdir(string name) {
      var subdir = new FileBundleDirectory(name);
      this.subdirs_.Add(subdir);
      return subdir;
    }

    public void AddFileBundle(IFileBundle fileBundle)
      => this.fileBundles_.Add(fileBundle);

    public void RemoveEmptyChildren() {
      var subdirsToRemove = new List<IFileBundleDirectory>();
      foreach (var subdir in this.subdirs_) {
        subdir.RemoveEmptyChildren();
        if (!subdir.Subdirs.Any() && !subdir.FileBundles.Any()) {
          subdirsToRemove.Add(subdir);
        }
      }

      foreach (var subdir in subdirsToRemove) {
        this.subdirs_.Remove(subdir);
      }
    }
  }
}