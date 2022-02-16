using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;

using Dxt;


namespace HaloWarsTools {
  public class HWXttResource : HWBinaryResource {
    public Bitmap AlbedoTexture =>
        ValueCache.Get(() => ExtractEmbeddedDXT1(
                           GetFirstChunkOfType(
                               HWBinaryResourceChunkType
                                   .XTT_AtlasChunkAlbedo)));

    public static new HWXttResource
        FromFile(HWContext context, string filename) {
      return GetOrCreateFromFile(context, filename, HWResourceType.Xtt) as
                 HWXttResource;
    }

    private Bitmap ExtractEmbeddedDXT1(HWBinaryResourceChunk chunk) {
      // Decompress DXT1 texture and turn it into a Bitmap
      var width =
          BinaryUtils.ReadInt32BigEndian(RawBytes, (int) chunk.Offset + 4);
      var height =
          BinaryUtils.ReadInt32BigEndian(RawBytes, (int) chunk.Offset + 8);
      return DxtDecoder.DecompressDXT1(RawBytes, 
                                       (int) chunk.Offset + 16, 
                                       width,
                                       height);
    }
  }
}