using fin.io;

namespace uni.platforms.gcn.tools {
  public static class GcnToolsConstants {
    public static IDirectory SZSTOOLS_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.GetSubdir("szstools");

    public static IFile GCMDUMP_EXE =
        GcnToolsConstants.SZSTOOLS_DIRECTORY.GetExistingFile("gcmdump.exe");

    public static IFile RARCDUMP_EXE =
        GcnToolsConstants.SZSTOOLS_DIRECTORY.GetExistingFile("rarcdump.exe");

    public static IFile YAZ0DEC_EXE =
        GcnToolsConstants.SZSTOOLS_DIRECTORY.GetExistingFile("yaz0dec.exe");


    public static IDirectory YAY0DEC_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.GetSubdir("yay0dec");

    public static IFile YAY0DEC_EXE =
        GcnToolsConstants.YAY0DEC_DIRECTORY.GetExistingFile("yay0dec.exe");


    public static IDirectory BMD2FBX_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.GetSubdir("bmd2fbx");

    public static IFile BMD2FBX_EXE =
        GcnToolsConstants.BMD2FBX_DIRECTORY.GetExistingFile("bmd2fbx.exe");


    public static IDirectory MOD2FBX_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.GetSubdir("mod2fbx");

    public static IFile MOD2FBX_EXE =
        GcnToolsConstants.MOD2FBX_DIRECTORY.GetExistingFile("mod2fbx.exe");
  }
}