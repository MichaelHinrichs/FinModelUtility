using System.Collections.Generic;
using System.Linq;

namespace fin.io {
  public static class FileExtensions {
    public static IEnumerable<TFile> WithName<TFile>(
        this IEnumerable<TFile> files,
        string name) where TFile : IReadOnlyTreeFile {
      name = name.ToLower();
      return files.Where(file => file.Name.ToLower() == name);
    }

    public static IEnumerable<TFile> WithFileType<TFile>(
        this IEnumerable<TFile> files,
        string fileType) where TFile : IReadOnlyTreeFile {
      fileType = fileType.ToLower();
      return files.Where(file => file.FileType.ToLower() == fileType);
    }

    public static IEnumerable<TFile> WithFileTypes<TFile>(
        this IEnumerable<TFile> files,
        params string[] fileTypes) where TFile : IReadOnlyTreeFile {
      for (var i = 0; i < fileTypes.Length; ++i) {
        fileTypes[i] = fileTypes[i].ToLower();
      }

      return files.Where(file => fileTypes.Contains(file.FileType.ToLower()));
    }
  }
}