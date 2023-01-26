using System.IO;

namespace f3dzex2.io {
  public interface IBankManager {
    IEndianBinaryReader this[byte bankIndex] { get; set; }
  }
}
