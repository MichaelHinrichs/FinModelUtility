using System.IO;
using System.Linq;

using fin.exporter.assimp.indirect;
using fin.io;
using fin.util.asserts;

using zar.format.cmb;
using zar.format.csab;

namespace zar.api {
  public class ManualZar2FbxApi {
    public void Run(
        IDirectory outputDirectory,
        IFile[] cmbFiles,
        IFile[]? csabFiles,
        float fps) {
      Asserts.True(cmbFiles.Length == 1 || csabFiles.Length == 0);

      var filesAndCmbs =
          cmbFiles.Select(cmbFile => (cmbFile,
                                      new Cmb(
                                          new EndianBinaryReader(
                                              cmbFile.OpenRead(),
                                              Endianness.LittleEndian))))
                  .ToList();

      var filesAndCsabs =
          csabFiles.Select(csabFile => {
                     var csab = new Csab();
                     csab.Read(new EndianBinaryReader(
                                   csabFile.OpenRead(),
                                   Endianness.LittleEndian));
                     return (csabFile, csab);
                   })
                   .ToList();

      foreach (var (cmbFile, cmb) in filesAndCmbs) {
        using var r =
            new EndianBinaryReader(cmbFile.OpenRead(), Endianness.LittleEndian);
        var model =
            new ModelConverter().Convert(r,
                                         cmb,
                                         filesAndCsabs,
                                         outputDirectory,
                                         fps);

        new AssimpIndirectExporter().Export(
            new FinFile(Path.Join(outputDirectory.FullName,
                                  cmbFile.NameWithoutExtension + ".fbx")),
            model);
      }
    }
  }
}