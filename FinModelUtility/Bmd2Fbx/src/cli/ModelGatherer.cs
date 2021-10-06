using System;
using System.Collections.Generic;
using System.Linq;

using fin.io;

namespace bmd.cli {
  public record ModelFiles (
      string Name,
      IEnumerable<IFile> BmdFiles,
      IEnumerable<IFile> BcxFiles,
      IEnumerable<IFile> BtiFiles);

  public class FileComparer : IComparer<IFile> {
    public int Compare(IFile x, IFile y) {
      if (ReferenceEquals(x, y)) {
        return 0;
      }
      if (ReferenceEquals(null, y)) {
        return 1;
      }
      if (ReferenceEquals(null, x)) {
        return -1;
      }
      return string.Compare(x.FullName,
                            y.FullName,
                            StringComparison.OrdinalIgnoreCase);
    }
  }

  public class ModelFilesComparer : IComparer<ModelFiles> {
    public int Compare(ModelFiles x, ModelFiles y) {
      if (ReferenceEquals(x, y)) {
        return 0;
      }
      if (ReferenceEquals(null, y)) {
        return 1;
      }
      if (ReferenceEquals(null, x)) {
        return -1;
      }
      return string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
    }
  }

  public class ModelFilesInDirectory {
    public ModelFilesInDirectory(IDirectory directory) {
      this.Impl = new ModelFiles(directory.Name,
                                 directory.SearchForFiles("*.bmd"),
                                 directory.SearchForFiles("*.bc?"),
                                 directory.SearchForFiles("*.bti"));
    }

    public ModelFiles Impl { get; }
  }

  public class ModelFileTreeNode {
    private readonly IDirectory directory_;

    public ModelFileTreeNode(IDirectory directory) {
      this.directory_ = directory;
      this.Impl = new ModelFilesInDirectory(directory);
      this.Children = directory.GetExistingSubdirs()
                               .Select(subdir => new ModelFileTreeNode(subdir))
                               .ToList();
    }

    public ModelFilesInDirectory Impl { get; }
    public IReadOnlyList<ModelFileTreeNode> Children { get; }

    public bool HasOnlyOneBmdInSubdirs() {
      var queue = new Queue<ModelFileTreeNode>();
      queue.Enqueue(this);

      var total = 0;
      while (queue.Count > 0) {
        var node = queue.Dequeue();

        total += node.Impl.Impl.BmdFiles.Count();

        if (total > 1) {
          return false;
        }

        foreach (var child in node.Children) {
          queue.Enqueue(child);
        }
      }

      return total == 1;
    }

    public ModelFiles GatherAllFilesInSubdirs() {
      List<IFile> bmdFiles = new();
      List<IFile> bcxFiles = new();
      List<IFile> btiFiles = new();

      var queue = new Queue<ModelFileTreeNode>();
      queue.Enqueue(this);

      var total = 0;
      while (queue.Count > 0) {
        var node = queue.Dequeue();

        var impl = node.Impl.Impl;
        bmdFiles.AddRange(impl.BmdFiles);
        bcxFiles.AddRange(impl.BcxFiles);
        btiFiles.AddRange(impl.BtiFiles);

        foreach (var child in node.Children) {
          queue.Enqueue(child);
        }
      }

      FileComparer fileComparer = new();
      bmdFiles.Sort(fileComparer);
      bcxFiles.Sort(fileComparer);
      btiFiles.Sort(fileComparer);

      return new ModelFiles(this.directory_.Name, bmdFiles, bcxFiles, btiFiles);
    }
  }

  public static class ModelGatherer {
    public static IList<ModelFiles> GatherModels(IDirectory rootDir) {
      List<ModelFiles> models = new();

      var rootNode = new ModelFileTreeNode(rootDir);

      var queue = new Queue<ModelFileTreeNode>();
      queue.Enqueue(rootNode);

      var total = 0;
      while (queue.Count > 0) {
        var node = queue.Dequeue();

        if (node.HasOnlyOneBmdInSubdirs()) {
          models.Add(node.GatherAllFilesInSubdirs());
        } else {
          foreach (var child in node.Children) {
            queue.Enqueue(child);
          }
        }
      }

      models.Sort(new ModelFilesComparer());

      return models;
    }
  }
}