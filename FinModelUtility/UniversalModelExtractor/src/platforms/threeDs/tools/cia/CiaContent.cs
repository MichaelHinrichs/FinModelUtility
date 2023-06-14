using schema.binary;
using schema.binary.attributes.child_of;

namespace uni.platforms.threeDs.tools.cia {
  public partial class CiaContent : IChildOf<Cia>, IBinaryDeserializable {
    public Cia Parent { get; set; }

    public IReadOnlyList<ContentInfo> ContentInfos { get; private set; }

    public void Read(IEndianBinaryReader er) {
      switch (this.Parent.Header.FormatVersion) {
        case CiaFormatVersion.DEFAULT: {
          this.ReadDefault_(er);
          break;
        }
        case CiaFormatVersion.SIMPLE: {
          this.ReadSimple_(er);
          break;
        }
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void ReadDefault_(IEndianBinaryReader er) { }

    private void ReadSimple_(IEndianBinaryReader er) {
      this.ContentInfos = new[] {
          new ContentInfo {
              Offset = er.Position,
              Size = this.Parent.Header.ContentSize,
              Id = 0,
              Index = 0,
              IsEncrypted = false,
              IsHashed = false,
              HashCode = new(),
              ValidState = ValidState.Unchecked,
          },
      };
    }
  }

  public class ContentInfo {
    public required long Offset { get; init; }
    public required long Size { get; init; }
    public required uint Id { get; init; }
    public required ushort Index { get; init; }
    public required bool IsEncrypted { get; init; }
    public required bool IsHashed { get; init; }
    public required HashCode HashCode { get; init; }
    public required ValidState ValidState { get; init; }
  }

  public enum ValidState {
    Unchecked,
    Good,
    Fail,
  }
}