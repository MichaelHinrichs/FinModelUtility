using fin.io;

namespace uni.platforms.gcn.tools {
  public static class ThreeDsToolsConstants {
    public static IDirectory HACKING_TOOLKIT_9DS_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.TryToGetSubdir(
            "HackingToolkit9DSv12");

    public static IFile HACKING_TOOLKIT_9DS_EXE =
        ThreeDsToolsConstants.HACKING_TOOLKIT_9DS_DIRECTORY.TryToGetFile(
            "HackingToolkit9DS.exe");
  }
}