using System;
using System.IO;

using fin.io;

namespace zar.format.cmb {
  public class Vatr : IDeserializable {
    public void Read(EndianBinaryReader r) {
      r.AssertMagicText("vatr");

      throw new NotImplementedException();
    }
  }
}
