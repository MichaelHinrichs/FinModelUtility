using fin.io;

namespace uni.platforms.gcn.tools {
  public static class GcnToolsConstants {
    public static IDirectory SZSTOOLS_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.TryToGetSubdir("szstools");

    public static IFile GCMDUMP_EXE =
        GcnToolsConstants.SZSTOOLS_DIRECTORY.TryToGetFile("gcmdump.exe");

    public static IFile RARCDUMP_EXE =
        GcnToolsConstants.SZSTOOLS_DIRECTORY.TryToGetFile("rarcdump.exe");

    public static IFile YAZ0DEC_EXE =
        GcnToolsConstants.SZSTOOLS_DIRECTORY.TryToGetFile("yaz0dec.exe");
  }
}