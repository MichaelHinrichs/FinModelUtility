using fin.io;
using fin.util.asserts;
using fin.util.linq;

using Gameloop.Vdf;
using Gameloop.Vdf.Linq;

using Microsoft.Win32;


namespace uni.platforms.desktop {
  internal static class SteamUtils {
    public const string INSTALL_PATH_32_BIT_REGISTRY_KEY =
        @"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam";

    public const string INSTALL_PATH_64_BIT_REGISTRY_KEY =
        @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam";


    public static string? InstallPath { get; } =
      (Registry.GetValue(SteamUtils.INSTALL_PATH_64_BIT_REGISTRY_KEY,
                         "InstallPath",
                         null) ??
       Registry.GetValue(SteamUtils.INSTALL_PATH_32_BIT_REGISTRY_KEY,
                         "InstallPath",
                         null)) as string;

    public static ISystemDirectory? InstallDirectory { get; } =
      SteamUtils.InstallPath != null
          ? new FinDirectory(SteamUtils.InstallPath)
          : null;

    public static ISystemDirectory[] CommonDirectories { get; } =
      SteamUtils.InstallDirectory == null
          ? Array.Empty<ISystemDirectory>()
          : VdfConvert
            .Deserialize(
                SteamUtils.InstallDirectory
                          .GetExistingFile("config/libraryfolders.vdf")
                          .OpenReadAsText())
            .Value
            .Children()
            .Select(section => section.Value<VProperty>().Value)
            .Select(section => section["path"])
            .Select(path => new FinDirectory(path.ToString()))
            .Where(steamDirectory
                       => steamDirectory
                           .Exists) // A steam directory may not exist if it corresponds to an external hard drive
            .Select(
                libraryFolder => libraryFolder.GetSubdir("steamapps"))
            .Select(steamApps => steamApps.GetSubdir("common"))
            .ToArray();

    public static ISystemDirectory[] GameDirectories { get; }
      = CommonDirectories
        .SelectMany(common => common.GetExistingSubdirs())
        .ToArray();

    public static bool TryGetGameDirectory(
        string name,
        out ISystemDirectory directory,
        bool assert = false) {
      if (GameDirectories.TryGetFirst(game => game.Name == name,
                                      out directory)) {
        return true;
      }

      Asserts.False(assert, $"Could not find \"{name}\" installed in Steam.");
      return false;
    }

    public static ISystemDirectory? GetGameDirectory(
        string name,
        bool assert = false)
      => TryGetGameDirectory(name, out var directory, assert)
          ? directory
          : null;
  }
}