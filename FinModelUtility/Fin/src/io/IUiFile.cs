namespace fin.io {
  public interface IUiFile {
    // TODO: Make these nonnull via init setters in C#9.
    string? FileName { get; set; }
    string? BetterFileName { get; set; }
  }
}
