using System;
using System.Collections.Generic;
using System.Linq;
using fin.util.strings;


namespace fin.io.bundles {
  public interface IFileBundleDirectory {
    string Name { get; }

    IEnumerable<IFileBundleDirectory> Subdirs { get; }
    IEnumerable<IFileBundle> FileBundles { get; }

    void RemoveEmptyChildren();
  }

  public interface IFileBundleDirectory<TFileBundle> : IFileBundleDirectory
      where TFileBundle : IFileBundle {
    IEnumerable<IFileBundleDirectory<TFileBundle>> TypedSubdirs { get; }
    IEnumerable<TFileBundle> TypedFileBundles { get; }

    IFileBundleDirectory<TFileBundle> AddSubdir(string name);
    void AddSubdir(IFileBundleDirectory<TFileBundle> subdir);
    void AddFileBundle(TFileBundle fileBundle);

    void AddFileBundleRelative(TFileBundle fileBundle,
                               IEnumerable<string>? segments = null);

    void ForEachTyped(Action<TFileBundle> callback);
  }


  public class RootFileBundleDirectory : IFileBundleDirectory {
    private readonly List<IFileBundleDirectory> subdirs_ = new();

    public string Name => "(root)";


    public IEnumerable<IFileBundleDirectory> Subdirs => this.subdirs_;

    public IEnumerable<IFileBundle> FileBundles { get; } =
      Array.Empty<IFileBundle>();

    public void AddSubdirIfNotNull(IFileBundleDirectory? subdir) {
      if (subdir != null) {
        this.subdirs_.Add(subdir);
      }
    }

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


  public class FileBundleDirectory<TFileBundle>
      : IFileBundleDirectory<TFileBundle>
      where TFileBundle : IFileBundle {
    private readonly List<IFileBundleDirectory<TFileBundle>> subdirs_ = new();
    private readonly List<TFileBundle> fileBundles_ = new();

    public FileBundleDirectory(string name) {
      this.Name = name;
    }

    public string Name { get; }


    public IEnumerable<IFileBundleDirectory> Subdirs => this.subdirs_;

    public IEnumerable<IFileBundleDirectory<TFileBundle>> TypedSubdirs =>
        this.subdirs_;

    public IEnumerable<IFileBundle> FileBundles =>
        this.fileBundles_.Cast<IFileBundle>();

    public IEnumerable<TFileBundle> TypedFileBundles =>
        this.fileBundles_;

    public IFileBundleDirectory<TFileBundle> AddSubdir(string name) {
      var subdir = new FileBundleDirectory<TFileBundle>(name);
      this.AddSubdir(subdir);
      return subdir;
    }

    public void AddSubdir(IFileBundleDirectory<TFileBundle> subdir)
      => this.subdirs_.Add(subdir);

    public void AddFileBundle(TFileBundle fileBundle)
      => this.fileBundles_.Add(fileBundle);

    public void AddFileBundleRelative(TFileBundle fileBundle,
                                      IEnumerable<string>? segments = null) {
      segments ??= StringUtil.UpTo(fileBundle.MainFile.LocalPath,
                                   fileBundle.MainFile.Name)
                             .Split('\\', '/',
                                    StringSplitOptions.RemoveEmptyEntries);

      var firstSegment = segments.FirstOrDefault();
      if (firstSegment == null) {
        this.AddFileBundle(fileBundle);
        return;
      }

      var subdirName = firstSegment;
      segments = segments.Skip(1);

      var subdir =
          this.TypedSubdirs.Where(typedSubdir => typedSubdir.Name == subdirName)
              .SingleOrDefault(
                  (IFileBundleDirectory<TFileBundle>?)null);
      subdir ??= this.AddSubdir(subdirName);

      subdir.AddFileBundleRelative(fileBundle, segments);
    }


    public void ForEachTyped(Action<TFileBundle> callback) {
      foreach (var fileBundle in this.TypedFileBundles) {
        callback(fileBundle);
      }

      foreach (var subdir in this.TypedSubdirs) {
        subdir.ForEachTyped(callback);
      }
    }

    public void RemoveEmptyChildren() {
      var subdirsToRemove = new List<IFileBundleDirectory<TFileBundle>>();
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


  public interface IFileBundle : IUiFile {
    IFileHierarchyFile MainFile { get; }
    string IUiFile.FileName => this.MainFile.NameWithoutExtension;
  }

  public interface IFileBundleGatherer {
    IFileBundleDirectory? GatherFileBundles(bool assert);
  }

  public interface IFileBundleGatherer<TFileBundle> : IFileBundleGatherer
      where TFileBundle : IFileBundle {
    new IFileBundleDirectory<TFileBundle>? GatherFileBundles(bool assert);

    IFileBundleDirectory? IFileBundleGatherer.GatherFileBundles(
        bool assert)
      => this.GatherFileBundles(assert);
  }
}