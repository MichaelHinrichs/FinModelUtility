using System.IO;

namespace f3dzex2.io {
  public interface IBankManager {
    EndianBinaryReader this[byte bankIndex] { get; set; }
  }
}
