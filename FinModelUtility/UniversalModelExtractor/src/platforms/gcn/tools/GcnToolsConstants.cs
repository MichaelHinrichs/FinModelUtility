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


    public static IDirectory BMD2FBX_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.TryToGetSubdir("bmd2fbx");

    public static IFile BMD2FBX_EXE =
        GcnToolsConstants.BMD2FBX_DIRECTORY.TryToGetFile("bmd2fbx.exe");


    public static IDirectory MOD2FBX_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.TryToGetSubdir("mod2fbx");

    public static IFile MOD2FBX_EXE =
        GcnToolsConstants.MOD2FBX_DIRECTORY.TryToGetFile("mod2fbx.exe");

  }
}