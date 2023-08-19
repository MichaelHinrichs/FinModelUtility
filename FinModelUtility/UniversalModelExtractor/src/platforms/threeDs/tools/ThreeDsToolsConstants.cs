using fin.io;

namespace uni.platforms.gcn.tools {
  public static class ThreeDsToolsConstants {
    public static ISystemDirectory CTRTOOL_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.AssertGetExistingSubdir("ctrtool");

    public static ISystemFile EXTRACT_CIA_BAT =
        ThreeDsToolsConstants.CTRTOOL_DIRECTORY.AssertGetExistingFile(
            "extract_cia.bat");

    public static ISystemFile EXTRACT_CCI_BAT =
        ThreeDsToolsConstants.CTRTOOL_DIRECTORY.AssertGetExistingFile(
            "extract_cci.bat");


    public static ISystemDirectory THREEDS_XSFATOOL_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.AssertGetExistingSubdir(
            "3ds-xfsatool");

    public static ISystemFile THREEDS_XSFATOOL_EXE =
        ThreeDsToolsConstants.THREEDS_XSFATOOL_DIRECTORY.AssertGetExistingFile(
            "3ds-xfsatool.exe");
  }
}