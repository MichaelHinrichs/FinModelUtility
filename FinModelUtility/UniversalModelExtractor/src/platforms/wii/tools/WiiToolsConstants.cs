﻿using fin.io;

namespace uni.platforms.wii.tools {
  public static class WiiToolsConstants {
    public static ISystemDirectory WIT_DIRECTORY =
        DirectoryConstants.TOOLS_DIRECTORY.GetSubdir("wit");

    public static ISystemFile WIT_EXE =
        WiiToolsConstants.WIT_DIRECTORY.GetExistingFile("wit.exe");
  }
}