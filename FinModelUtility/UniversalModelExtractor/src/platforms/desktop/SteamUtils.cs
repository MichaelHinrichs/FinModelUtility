using fin.io;
using fin.util.asserts;

using Gameloop.Vdf;
using Gameloop.Vdf.Linq;

using Microsoft.Win32;


namespace uni.platforms.desktop {
  internal static class SteamUtils {
    public const string INSTALL_PATH_32_BIT_REGISTRY_KEY =
        @"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam";

    public const string INSTALL_PATH_64_BIT_REGISTRY_KEY =
        @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam";


    public static string InstallPath { get; } =
      (Registry.GetValue(SteamUtils.INSTALL_PATH_64_BIT_REGISTRY_KEY,
                         "InstallPath",
                         null) ??
       Registry.GetValue(SteamUtils.INSTALL_PATH_32_BIT_REGISTRY_KEY,
                         "InstallPath",
                         null)) as string;

    public static IDirectory InstallDirectory { get; } =
      new FinDirectory(Asserts.CastNonnull(SteamUtils.InstallPath));

    public static IDirectory[] CommonDirectories { get; } =
      VdfConvert
          .Deserialize(
              SteamUtils.InstallDirectory
                        .GetExistingFile("config/libraryfolders.vdf")
                        .OpenReadAsText())
          .Value.Skip(1)
          .Select(section => section.Value<VProperty>().Value)
          .Select(section => section["path"])
          .Select(path => new FinDirectory(path.ToString()))
          .Select(
              libraryFolder => libraryFolder.GetSubdir("steamapps"))
          .Select(steamApps => steamApps.GetSubdir("common"))
          .ToArray();

    public static IDirectory?
        GetGameDirectory(string name, bool assert = false) {
      var gameDir = CommonDirectories
                   .SelectMany(common => common.GetExistingSubdirs())
                   .FirstOrDefault(game => game.Name == name);
      return !assert
                 ? gameDir
                 : Asserts.CastNonnull(
                     gameDir, $"Could not find \"{name}\" installed in Steam.");
    }
  }
}