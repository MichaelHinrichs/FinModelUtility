using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using static fin.io.sharpfilelister.Interop;

using fin.io;

namespace fins.io.sharpDirLister {
  public class DirectoryInformation : ISubdirPaths {
    public string AbsoluteSubdirPath { get; set; }

    public IReadOnlyList<string> AbsoluteFilePaths => AbsoluteFilePathsImpl;
    public IReadOnlyList<ISubdirPaths> Subdirs => SubdirsImpl;

    public List<string> AbsoluteFilePathsImpl { get; } = new();
    public List<DirectoryInformation> SubdirsImpl { get; } = new();
  }

  public class SharpFileLister {
    static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

    //Code based heavily on https://stackoverflow.com/q/47471744
    static void FindNextFilePInvokeRecursive(
        DirectoryInformation directoryInfo) {
      var path = directoryInfo.AbsoluteSubdirPath;
      var fileList = directoryInfo.AbsoluteFilePathsImpl;
      var directoryList = directoryInfo.SubdirsImpl;

      IntPtr findHandle = INVALID_HANDLE_VALUE;
      List<Tuple<string, DateTime>> info = new List<Tuple<string, DateTime>>();

      try {
        findHandle =
            FindFirstFileW(path + @"\*", out WIN32_FIND_DATAW findData);

        if (findHandle != INVALID_HANDLE_VALUE) {
          do {
            if (findData.cFileName != "." && findData.cFileName != "..") {
              string fullPath = path + @"\" + findData.cFileName;

              if (findData.dwFileAttributes.HasFlag(FileAttributes.Directory) &&
                  !findData.dwFileAttributes.HasFlag(
                      FileAttributes.ReparsePoint)) {
                var dirdata =
                    new DirectoryInformation { AbsoluteSubdirPath = fullPath, };
                directoryList.Add(dirdata);

                FindNextFilePInvokeRecursive(dirdata);
              } else if (!findData.dwFileAttributes.HasFlag(
                             FileAttributes.Directory)) {
                fileList.Add(fullPath);
              }
            }
          } while (FindNextFile(findHandle, out findData));
        }
      } finally {
        if (findHandle != INVALID_HANDLE_VALUE) {
          FindClose(findHandle);
        }
      }
    }

    public static DirectoryInformation FindNextFilePInvokeRecursiveParalleled(
        string path) {
      var directoryInfo = new DirectoryInformation { AbsoluteSubdirPath = path };
      var fileList = directoryInfo.AbsoluteFilePathsImpl;
      var directoryList = directoryInfo.SubdirsImpl;

      object fileListLock = new object();
      object directoryListLock = new object();
      IntPtr findHandle = INVALID_HANDLE_VALUE;
      List<Tuple<string, DateTime>> info = new List<Tuple<string, DateTime>>();

      try {
        path = path.EndsWith(@"\") ? path : path + @"\";
        findHandle = FindFirstFileW(path + @"*", out WIN32_FIND_DATAW findData);

        if (findHandle != INVALID_HANDLE_VALUE) {
          do {
            if (findData.cFileName != "." && findData.cFileName != "..") {
              string fullPath = path + findData.cFileName;

              if (findData.dwFileAttributes.HasFlag(FileAttributes.Directory) &&
                  !findData.dwFileAttributes.HasFlag(
                      FileAttributes.ReparsePoint)) {
                var dirdata = new DirectoryInformation { AbsoluteSubdirPath = fullPath, };
                directoryList.Add(dirdata);
              } else if (!findData.dwFileAttributes.HasFlag(
                             FileAttributes.Directory)) {
                fileList.Add(fullPath);
              }
            }
          } while (FindNextFile(findHandle, out findData));

          directoryList.AsParallel()
                       .ForAll(x => FindNextFilePInvokeRecursive(x));
        }
      } finally {
        if (findHandle != INVALID_HANDLE_VALUE) {
          FindClose(findHandle);
        }
      }

      return directoryInfo;
    }
  }
}