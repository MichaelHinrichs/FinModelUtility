using fin.image;
using fin.io;
using level5.schema;

namespace level5.debug {
  public class DebugLevel5 {
    public void Run() {
      var file = new FinFile(@"C:\Users\Ryan\Documents\CSharpWorkspace\FinModelUtility\cli\roms\professor_layton_vs_phoenix_wright\vs1\chr\c101.xc");

      var endianness = Endianness.LittleEndian;
      var xc = file.ReadNew<Xc>(endianness);

      xc.FilesByExtension.TryGetList(".xi", out var xiFiles);
      foreach (var xiFile in xiFiles) {
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(xiFile.Name);
        var outPath = Path.Combine(@"C:\Users\Ryan\Documents\CSharpWorkspace\FinModelUtility\cli\out",
          nameWithoutExtension + ".png");

        var xi = new Xi();
        xi.Open(xiFile.Data);

        var image = xi.ToBitmap();
        image.ExportToStream(File.OpenWrite(outPath), LocalImageFormat.PNG);
      }
    }
  }
}
