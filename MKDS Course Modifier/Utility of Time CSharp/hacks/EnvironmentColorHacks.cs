namespace UoT {
  /// <summary>
  ///   Hack class for setting specific environment colors for given models.
  /// </summary>
  public class EnvironmentColorHacks {
    // TODO: Add a tab for these toggles.
    // TODO: Add toggle for Link's tunic.

    public static byte[]? GetColorForObject(string filename) {
      if (filename == "object_link_boy" || filename == "object_link_child") {
        return Tunic.TUNIC_COLOR_KOKIRI;
      }

      return null;
    }
  }
}