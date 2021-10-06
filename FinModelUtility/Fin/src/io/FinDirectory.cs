﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fin.io {
  public class FinDirectory : IDirectory {
    public static IDirectory GetCwd()
      => new FinDirectory(Directory.GetCurrentDirectory());


    public FinDirectory(DirectoryInfo directoryInfo)
      => this.Info = directoryInfo;

    public FinDirectory(string fullName)
      => this.Info = new DirectoryInfo(fullName);
    
    public DirectoryInfo Info { get; }

    public string Name => this.Info.Name;
    public string FullName => this.Info.FullName;

    public IDirectory? GetParent() => new FinDirectory(this.Info.Parent);

    public bool Exists => this.Info.Exists;

    public bool Create() {
      if (this.Exists) {
        return false;
      }

      this.Info.Create();
      return true;
    }

    public IEnumerable<IDirectory> GetExistingSubdirs()
      => this.Info.EnumerateDirectories()
             .Select(subdir => new FinDirectory(subdir));

    public IDirectory TryToGetSubdir(string relativePath, bool create = false)
      => this.GetSubdirImpl_(relativePath.Split('/', '\\'), create);

    private FinDirectory GetSubdirImpl_(
        IEnumerable<string> subdirs,
        bool create) {
      var current = this.Info;

      foreach (var subdir in subdirs) {
        if (subdir == "..") {
          current = current.Parent;
          continue;
        }

        var matches = current.GetDirectories(subdir);

        if (!create || matches.Length == 1) {
          current = matches.Single();
        } else {
          current = current.CreateSubdirectory(subdir);
        }
      }

      return new FinDirectory(current);
    }


    public IEnumerable<IFile> GetExistingFiles()
      => this.Info.EnumerateFiles().Select(file => new FinFile(file));

    public IEnumerable<IFile> SearchForFiles(
        string searchPattern,
        bool includeSubdirs = false)
      => this.Info
             .GetFiles(searchPattern,
                       includeSubdirs
                           ? SearchOption.AllDirectories
                           : SearchOption.TopDirectoryOnly)
             .Select(file => new FinFile(file));

    public IFile TryToGetFile(string path) {
      // TODO: Handle subdirectories automatically.
      try {
        return new FinFile(this.Info.GetFiles(path).Single());
      } catch (Exception e) {
        throw new Exception($"Expected to find {path}", e);
      }
    }

    public override string ToString() => this.FullName;
  }
}