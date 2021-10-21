using System.IO;
using System.Linq;

using fin.exporter.assimp.indirect;
using fin.io;

using zar.format.cmb;

namespace zar.api {
  public class ManualZar2FbxApi {
    public void Run(IDirectory outputDirectory, IFile[] cmbFiles) {
      var filesAndCmbs =
          cmbFiles.Select(cmbFile => (cmbFile,
                                      new Cmb(
                                          new EndianBinaryReader(
                                              cmbFile.OpenRead(),
                                              Endianness.LittleEndian))))
                  .ToList();

      foreach (var (cmbFile, cmb) in filesAndCmbs) {
        var model = new ModelConverter().Convert(cmb);

        new AssimpIndirectExporter().Export(
            new FinFile(Path.Join(outputDirectory.FullName,
                                  cmbFile.NameWithoutExtension + ".fbx")),
            model);
      }
    }
  }
}