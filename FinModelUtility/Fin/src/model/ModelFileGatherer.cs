using System;
using System.Collections.Generic;
using System.Linq;

using fin.io;


namespace fin.model {
  public interface IModelDirectory {
    string Name { get; }

    IEnumerable<IModelDirectory> Subdirs { get; }
    IEnumerable<IModelFileBundle> FileBundles { get; }
  }

  public interface IModelDirectory<TModelFileBundle> : IModelDirectory
      where TModelFileBundle : IModelFileBundle {
    IEnumerable<IModelDirectory<TModelFileBundle>> TypedSubdirs { get; }
    IEnumerable<TModelFileBundle> TypedFileBundles { get; }

    IModelDirectory<TModelFileBundle> AddSubdir(string name);
    void AddSubdir(IModelDirectory<TModelFileBundle> subdir);
    void AddFileBundle(TModelFileBundle fileBundle);

    void ForEachTyped(Action<TModelFileBundle> callback);
  }


  public class RootModelDirectory : IModelDirectory {
    private readonly List<IModelDirectory> subdirs_ = new();

    public string Name => "(root)";


    public IEnumerable<IModelDirectory> Subdirs => this.subdirs_;

    public IEnumerable<IModelFileBundle> FileBundles { get; } =
      Array.Empty<IModelFileBundle>();

    public void AddSubdirIfNotNull(IModelDirectory? subdir) {
      if (subdir != null) {
        this.subdirs_.Add(subdir);
      }
    }
  }


  public class
      ModelDirectory<TModelFileBundle> : IModelDirectory<TModelFileBundle>
      where TModelFileBundle : IModelFileBundle {
    private readonly List<IModelDirectory<TModelFileBundle>> subdirs_ = new();
    private readonly List<TModelFileBundle> fileBundles_ = new();

    public ModelDirectory(string name) {
      this.Name = name;
    }

    public string Name { get; }


    public IEnumerable<IModelDirectory> Subdirs => this.subdirs_;

    public IEnumerable<IModelDirectory<TModelFileBundle>> TypedSubdirs =>
        this.subdirs_;

    public IEnumerable<IModelFileBundle> FileBundles =>
        this.fileBundles_.Cast<IModelFileBundle>();

    public IEnumerable<TModelFileBundle> TypedFileBundles =>
        this.fileBundles_;

    public IModelDirectory<TModelFileBundle> AddSubdir(string name) {
      var subdir = new ModelDirectory<TModelFileBundle>(name);
      this.AddSubdir(subdir);
      return subdir;
    }

    public void AddSubdir(IModelDirectory<TModelFileBundle> subdir)
      => this.subdirs_.Add(subdir);

    public void AddFileBundle(TModelFileBundle fileBundle)
      => this.fileBundles_.Add(fileBundle);

    public void ForEachTyped(Action<TModelFileBundle> callback) {
      foreach (var fileBundle in this.TypedFileBundles) {
        callback(fileBundle);
      }

      foreach (var subdir in this.TypedSubdirs) {
        subdir.ForEachTyped(callback);
      }
    }
  }


  public interface IModelFileBundle : IUiFile { }

  public interface IModelFileGatherer<TModelFileBundle>
      where TModelFileBundle : IModelFileBundle {
    IModelDirectory<TModelFileBundle>? GatherModelFileBundles();
  }

  public interface IModelLoader<in TModelFileBundle>
      where TModelFileBundle : IModelFileBundle {
    IModel LoadModel(TModelFileBundle modelFileBundle);
  }
}