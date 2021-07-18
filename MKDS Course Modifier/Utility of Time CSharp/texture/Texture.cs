using System.Drawing;
using System.IO;

using Tao.OpenGl;

namespace UoT {
  /// <summary>
  ///   Instance of a valid texture.
  /// </summary>
  public class Texture {
    public TileDescriptor TileDescriptor { get; }
    public byte[] Rgba { get; }

    public Texture(TileDescriptor tileDescriptor, byte[] rgba, bool save = false) {
      this.TileDescriptor = tileDescriptor;
      this.Rgba = rgba;

      if (save) {
        var format = tileDescriptor.ColorFormat;
        var size = tileDescriptor.BitSize;
        var address = tileDescriptor.Address;
        var filename = "R:/Noesis/Output/" +
                       format +
                       "_" +
                       size +
                       "_" +
                       address.ToString("X8") +
                       ".png";
        this.SaveToFile(filename);
      }
    }

    public bool TileMirroredS
      => (this.TileDescriptor.CMS & (int) RDP.G_TX_MIRROR) != 0;
    public bool TileMirroredT
      => (this.TileDescriptor.CMT & (int) RDP.G_TX_MIRROR) != 0;

    public bool GlMirroredS { get; set; }
    public bool GlMirroredT { get; set; }

    public bool GlClampedS { get; set; }
    public bool GlClampedT { get; set; }

    // OpenGL-specific logic.
    public int GlId => this.TileDescriptor.ID;

    public void Bind() => Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.GlId);

    public void Destroy() {
      var id = this.GlId;
      Gl.glDeleteTextures(1, ref id);
    }

    public void SaveToFile(string filename) {
      if (File.Exists(filename)) {
        return;
      }

      var width = this.TileDescriptor.LoadWidth;
      var height = this.TileDescriptor.LoadHeight;

      var rgba = this.Rgba;

      var bmp = new Bitmap(width, height);
      for (var y = 0; y < height; y++) {
        for (var x = 0; x < width; x++) {
          var i = 4 * ((y * width) + x);

          var r = rgba[i];
          var g = rgba[i + 1];
          var b = rgba[i + 2];
          var a = rgba[i + 3];

          var color = Color.FromArgb(a, r, g, b);
          bmp.SetPixel(x, y, color);
        }
      }

      bmp.Save(filename);
    }

    // TODO: Add method for destroying.
    // TODO: Add method for saving to a file.
  }
}
