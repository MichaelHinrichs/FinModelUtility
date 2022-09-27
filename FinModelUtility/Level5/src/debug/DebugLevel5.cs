using fin.image;
using fin.io;
using level5.schema;


namespace level5.debug {
  public class DebugLevel5 {
    public void Run() {
      var fileHierarchy = new FileHierarchy(
          new FinDirectory(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\professor_layton_vs_phoenix_wright"));

      HashSet<string> filesWithModels = new();
      HashSet<string> filesWithAnimations = new();
      foreach (var directory in fileHierarchy) {
        var xcFiles = directory.FilesWithExtension(".xc");

        foreach (var xcFile in xcFiles) {
          try {
            var xc = xcFile.Impl.ReadNew<Xc>(Endianness.LittleEndian);

            if (xc.FilesByExtension.TryGetList(".prm", out _)) {
              filesWithModels.Add(xcFile.FullName);
            }

            if (xc.FilesByExtension.TryGetList(".mtn2", out _)) {
              filesWithAnimations.Add(xcFile.FullName);
            }
          } catch { }
        }
      }


      HashSet<string> filesFoundByName = new();
      foreach (var directory in fileHierarchy) {
        var xcFiles = directory.FilesWithExtension(".xc").ToHashSet();

        foreach (var xcFile in xcFiles) {
          if (!xcFile.Name.Contains("_")) {
            filesFoundByName.Add(xcFile.FullName);
          }
        }
      }


      var filesWithModelsAndAnimations =
          filesWithModels.Intersect(filesWithAnimations).ToHashSet();

      var truePositiveMatches =
          filesFoundByName.Where(
                              file =>
                                  filesWithModelsAndAnimations.Contains(file))
                          .ToArray();
      var falsePositiveMatches =
          filesFoundByName.Where(
                              file =>
                                  !filesWithModelsAndAnimations.Contains(file))
                          .ToArray();


      ;

      /*foreach (var xiFile in xiFiles) {
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(xiFile.Name);
        var outPath = Path.Combine(@"C:\Users\Ryan\Documents\CSharpWorkspace\FinModelUtility\cli\out",
          nameWithoutExtension + ".png");

        var xi = new Xi();
        xi.Open(xiFile.Data);

        var image = xi.ToBitmap();
        image.ExportToStream(File.OpenWrite(outPath), LocalImageFormat.PNG);
      }*/
    }
  }
}