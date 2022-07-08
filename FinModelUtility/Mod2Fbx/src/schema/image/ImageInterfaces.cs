using System.Collections.Generic;
using System.Drawing;

namespace mod.schema.image {
  public interface IImageDecoder {
    public Bitmap Decode(IList<byte> data, int width, int height);
  }
}
