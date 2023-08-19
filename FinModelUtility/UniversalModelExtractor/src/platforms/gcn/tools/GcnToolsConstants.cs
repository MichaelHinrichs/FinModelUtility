using fin.io;

namespace uni.platforms.gcn.tools {
  public static class GcnToolsConstants {
    public static ISystemDirectory SZSTOOLS_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.AssertGetExistingSubdir("szstools");

    public static ISystemFile GCMDUMP_EXE =
        GcnToolsConstants.SZSTOOLS_DIRECTORY.AssertGetExistingFile("gcmdump.exe");

    public static ISystemFile RARCDUMP_EXE =
        GcnToolsConstants.SZSTOOLS_DIRECTORY.AssertGetExistingFile("rarcdump.exe");

    public static ISystemFile YAZ0DEC_EXE =
        GcnToolsConstants.SZSTOOLS_DIRECTORY.AssertGetExistingFile("yaz0dec.exe");


    public static ISystemDirectory YAY0DEC_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.AssertGetExistingSubdir("yay0dec");

    public static ISystemFile YAY0DEC_EXE =
        GcnToolsConstants.YAY0DEC_DIRECTORY.AssertGetExistingFile("yay0dec.exe");
  }
}