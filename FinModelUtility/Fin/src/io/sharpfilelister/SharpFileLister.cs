using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using static fin.io.sharpfilelister.Interop;

using fin.io;

namespace fins.io.sharpDirLister {
  public class DirectoryInformation : ISubdirPaths {
    public string AbsoluteSubdirPath { get; set; }

    public IReadOnlyCollection<string> AbsoluteFilePaths => AbsoluteFilePathsImpl;
    public IReadOnlyCollection<ISubdirPaths> Subdirs => SubdirsImpl;

    public LinkedList<string> AbsoluteFilePathsImpl { get; } = new();
    public LinkedList<DirectoryInformation> SubdirsImpl { get; } = new();
  }

  public interface IFileLister {
    DirectoryInformation FindNextFilePInvokeRecursiveParalleled(
        string path);
  }

  public class SharpFileLister : IFileLister {
    public const IntPtr INVALID_HANDLE_VALUE = -1;

    public DirectoryInformation FindNextFilePInvokeRecursiveParalleled(
        string path) {
      var directoryInfo = new DirectoryInformation { AbsoluteSubdirPath = path };
      this.FindNextFilePInvokeRecursive_(directoryInfo);
      return directoryInfo;
    }

    //Code based heavily on https://stackoverflow.com/q/47471744
    private void FindNextFilePInvokeRecursive_(
        DirectoryInformation directoryInfo) {
      var path = directoryInfo.AbsoluteSubdirPath;
      var fileList = directoryInfo.AbsoluteFilePathsImpl;
      var directoryList = directoryInfo.SubdirsImpl;

      IntPtr fileSearchHandle = INVALID_HANDLE_VALUE;

      try {
        fileSearchHandle =
            FindFirstFileW(path + @"\*", out WIN32_FIND_DATAW findData);

        if (fileSearchHandle != INVALID_HANDLE_VALUE) {
          do {
            if (findData.cFileName != "." && findData.cFileName != "..") {
              string fullPath = path + @"\" + findData.cFileName;

              if (!findData.dwFileAttributes.HasFlag(FileAttributes.Directory)) {
                fileList.AddLast(fullPath);
              } else if (!findData.dwFileAttributes.HasFlag(FileAttributes.ReparsePoint)) {
                var dirdata =
                    new DirectoryInformation { AbsoluteSubdirPath = fullPath, };
                directoryList.AddLast(dirdata);
                this.FindNextFilePInvokeRecursive_(dirdata);
              } 
            }
          } while (FindNextFile(fileSearchHandle, out findData));
        }
      } finally {
        if (fileSearchHandle != INVALID_HANDLE_VALUE) {
          FindClose(fileSearchHandle);
        }
      }
    }
  }
}