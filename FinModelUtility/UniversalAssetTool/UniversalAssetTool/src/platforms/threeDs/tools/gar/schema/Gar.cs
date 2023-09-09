using schema.binary;

namespace uni.platforms.threeDs.tools.gar.schema {
  /// <summary>
  ///   Based on the following:
  ///   - https://github.com/xdanieldzd/Scarlet/blob/master/Scarlet.IO.ContainerFormats/GARv2.cs
  ///   - https://github.com/xdanieldzd/Scarlet/blob/master/Scarlet.IO.ContainerFormats/GARv5.cs
  /// </summary>
  public class Gar {
    public GarHeader Header { get; }
    public IGarFileType[] FileTypes { get; }

    public Gar(IEndianBinaryReader er) {
      this.Header = new GarHeader(er);

      this.FileTypes = new IGarFileType[this.Header.FileTypeCount];
      for (var i = 0; i < this.FileTypes.Length; ++i) {
        this.FileTypes[i] = this.Header.Version switch {
            2 => new Gar2FileType(er, this.Header, i),
            5 => new Gar5FileType(er, this.Header, i),
            _ => throw new NotImplementedException()
        };
      }
    }
  }
}