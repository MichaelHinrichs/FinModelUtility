using fin.util.enumerables;

using Microsoft.Win32;


namespace uni.platforms.desktop {
  public static class RegistryExtensions {
    public static object? GetSoftwareValueEither32Or64Bit(
        string relativeKeyName,
        string valueName,
        object? defaultValue = null) {
      var softwareKeyNames =
          GetSoftwareKeyNames32Or64Bit(relativeKeyName).ToArray();
      var values = softwareKeyNames
                   .Select(
                       keyName => {
                         using var subkey =
                             Registry.LocalMachine.OpenSubKey(keyName);
                         return subkey.GetValue(valueName);
                       })
                   .ToArray();
      return values.Where(value => value != null).FirstOrDefault(defaultValue);
    }

    public static RegistryKey? OpenSoftwareSubkeyEither32Or64Bit(
        string relativeKeyName)
      => GetSoftwareKeyNames32Or64Bit(relativeKeyName)
         .Select(Registry.LocalMachine.OpenSubKey)
         .FirstOrDefault();

    public static IEnumerable<string> GetSoftwareKeyNames32Or64Bit(
        string relativeKeyName)
      => Path.Join(@"SOFTWARE", relativeKeyName)
             .Yield()
             .Concat(Path.Join(@"SOFTWARE\Wow6432Node", relativeKeyName));
  }
}
