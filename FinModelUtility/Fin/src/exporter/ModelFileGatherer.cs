using System;
using System.Collections.Generic;

using fin.io;
using fin.model;


namespace fin.games {
  public interface IModelDirectory {
    public string Name { get; set; }
    IReadOnlyList<IModelDirectory> Subdirectories { get; }
    IReadOnlyList<IModelFileBundle> FileBundles { get; }
  }

  public interface IModelFileBundle : IUiFile {
    ILazyFileReaderMap FileReaders { get; }
  }

  public interface IModelFileGatherer {
    IModelDirectory GatherModelFileBundlesFromHierarchy(IFileHierarchy fileHierarchy);
  }

  public interface IModelLoader {
    IModel LoadModel(ILazyFileReaderMap fileReaderMap);
  }
}
