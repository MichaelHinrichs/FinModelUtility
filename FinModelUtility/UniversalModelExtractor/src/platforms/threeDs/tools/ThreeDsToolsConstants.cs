using fin.io;

namespace uni.platforms.gcn.tools {
  public static class ThreeDsToolsConstants {
    public static ISystemDirectory HACKING_TOOLKIT_9DS_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.GetSubdir(
            "HackingToolkit9DSv12");

    public static ISystemFile HACKING_TOOLKIT_9DS_EXE =
        ThreeDsToolsConstants.HACKING_TOOLKIT_9DS_DIRECTORY.GetExistingFile(
            "HackingToolkit9DS.exe");


    public static ISystemDirectory THREEDS_XSFATOOL_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.GetSubdir(
            "3ds-xfsatool");

    public static ISystemFile THREEDS_XSFATOOL_EXE =
        ThreeDsToolsConstants.THREEDS_XSFATOOL_DIRECTORY.GetExistingFile(
            "3ds-xfsatool.exe");
  }
}