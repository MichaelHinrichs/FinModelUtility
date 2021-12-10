using System.IO;

namespace f3dzex.io {
  public interface IBankManager {
    EndianBinaryReader this[byte bankIndex] { get; set; }
  }
}
