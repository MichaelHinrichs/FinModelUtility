using fin.io;

namespace uni.platforms.wii.tools {
  public static class WiiToolsConstants {
    public static IDirectory WIT_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.GetSubdir("wit");

    public static IFile WIT_EXE =
        WiiToolsConstants.WIT_DIRECTORY.GetExistingFile("wit.exe");
  }
}