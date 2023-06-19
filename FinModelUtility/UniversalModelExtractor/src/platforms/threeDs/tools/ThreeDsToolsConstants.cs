using fin.io;

namespace uni.platforms.gcn.tools {
  public static class ThreeDsToolsConstants {
    public static ISystemDirectory CTRTOOL_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.GetSubdir("ctrtool");

    public static ISystemFile EXTRACT_CIA_BAT =
        ThreeDsToolsConstants.CTRTOOL_DIRECTORY.GetExistingFile(
            "extract_cia.bat");


    public static ISystemDirectory THREEDS_XSFATOOL_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.GetSubdir(
            "3ds-xfsatool");

    public static ISystemFile THREEDS_XSFATOOL_EXE =
        ThreeDsToolsConstants.THREEDS_XSFATOOL_DIRECTORY.GetExistingFile(
            "3ds-xfsatool.exe");
  }
}