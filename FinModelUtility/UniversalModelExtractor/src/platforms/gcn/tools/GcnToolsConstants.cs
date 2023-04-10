using fin.io;

namespace uni.platforms.gcn.tools {
  public static class GcnToolsConstants {
    public static ISystemDirectory SZSTOOLS_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.GetSubdir("szstools");

    public static ISystemFile GCMDUMP_EXE =
        GcnToolsConstants.SZSTOOLS_DIRECTORY.GetExistingFile("gcmdump.exe");

    public static ISystemFile RARCDUMP_EXE =
        GcnToolsConstants.SZSTOOLS_DIRECTORY.GetExistingFile("rarcdump.exe");

    public static ISystemFile YAZ0DEC_EXE =
        GcnToolsConstants.SZSTOOLS_DIRECTORY.GetExistingFile("yaz0dec.exe");


    public static ISystemDirectory YAY0DEC_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.GetSubdir("yay0dec");

    public static ISystemFile YAY0DEC_EXE =
        GcnToolsConstants.YAY0DEC_DIRECTORY.GetExistingFile("yay0dec.exe");


    public static ISystemDirectory BMD2FBX_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.GetSubdir("bmd2fbx");

    public static ISystemFile BMD2FBX_EXE =
        GcnToolsConstants.BMD2FBX_DIRECTORY.GetExistingFile("bmd2fbx.exe");


    public static ISystemDirectory MOD2FBX_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.GetSubdir("mod2fbx");

    public static ISystemFile MOD2FBX_EXE =
        GcnToolsConstants.MOD2FBX_DIRECTORY.GetExistingFile("mod2fbx.exe");
  }
}