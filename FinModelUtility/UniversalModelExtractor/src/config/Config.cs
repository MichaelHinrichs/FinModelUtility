using uni.platforms;


namespace uni.config {
  public class Config {
    public static Config Instance { get; } =
      DirectoryConstants.CONFIG_FILE.Deserialize<Config>();

    public string[] ExportedFormats { get; set; } = Array.Empty<string>();
    public bool AutomaticallyPlayGameAudioForModel { get; set; }
    public bool ShowSkeleton { get; set; }
    public bool ShowGrid { get; set; }

    public void SaveSettings()
      => DirectoryConstants.CONFIG_FILE.Serialize(Config.Instance);
  }
}