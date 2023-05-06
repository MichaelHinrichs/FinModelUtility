﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


namespace fin.io.bundles {
  public interface IFileBundleDirectory {
    IFileHierarchyDirectory? Directory { get; }
    string Name { get; }

    IReadOnlyList<IFileBundleDirectory> Subdirs { get; }
    IReadOnlyList<IFileBundle> FileBundles { get; }

    IFileBundleDirectory AddSubdir(IFileHierarchyDirectory directory);
    IFileBundleDirectory AddSubdir(string name);
    void AddFileBundle(IFileBundle fileBundle);

    void CleanUp();
  }

  public class FileBundleDirectory : IFileBundleDirectory {
    private readonly List<IFileBundleDirectory> subdirs_ = new();
    private readonly List<IFileBundle> fileBundles_ = new();

    public FileBundleDirectory(IFileHierarchyDirectory? directory) {
      this.Directory = directory;
      this.Name = directory.Name;
    }

    public FileBundleDirectory(string name) {
      this.Name = name;
    }

    public IFileHierarchyDirectory? Directory { get; }
    public string Name { get; }

    public IReadOnlyList<IFileBundleDirectory> Subdirs => this.subdirs_;
    public IReadOnlyList<IFileBundle> FileBundles => this.fileBundles_;

    public IFileBundleDirectory AddSubdir(IFileHierarchyDirectory directory) {
      var subdir = new FileBundleDirectory(directory);
      this.subdirs_.Add(subdir);
      return subdir;
    }

    public IFileBundleDirectory AddSubdir(string name) {
      var subdir = new FileBundleDirectory(name);
      this.subdirs_.Add(subdir);
      return subdir;
    }

    public void AddFileBundle(IFileBundle fileBundle)
      => this.fileBundles_.Add(fileBundle);

    public void CleanUp() {
      var subdirsToRemove = new List<IFileBundleDirectory>();
      foreach (var subdir in this.subdirs_) {
        subdir.CleanUp();
        if (!subdir.Subdirs.Any() && !subdir.FileBundles.Any()) {
          subdirsToRemove.Add(subdir);
        }
      }

      foreach (var subdir in subdirsToRemove) {
        this.subdirs_.Remove(subdir);
      }

      this.subdirs_.Sort((lhs, rhs) => lhs.Name.CompareTo(rhs.Name));
      this.fileBundles_.Sort((lhs, rhs)
                                 => lhs.DisplayShortName.CompareTo(
                                     rhs.DisplayShortName));
    }
  }
}