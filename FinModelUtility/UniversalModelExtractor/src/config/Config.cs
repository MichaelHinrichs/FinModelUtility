using uni.platforms;


namespace uni.config {
  public class Config {
    public static Config Instance { get; } =
      DirectoryConstants.CONFIG_FILE.Deserialize<Config>();

    public bool IncludeFbx { get; set; }
    public bool AutomaticallyPlayGameAudioForModel { get; set; }

    public void SaveSettings()
      => DirectoryConstants.CONFIG_FILE.Serialize(Config.Instance);
  }
}