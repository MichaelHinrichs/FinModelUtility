using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.exporter.assimp.indirect;
using fin.io;
using fin.util.asserts;

using zar.format.cmb;
using zar.format.csab;
using zar.format.ctxb;
using zar.format.shpa;

namespace zar.api {
  public class ManualZar2FbxApi {
    public void Run(
        IDirectory outputDirectory,
        IFile[] cmbFiles,
        IFile[]? csabFiles,
        IFile[]? ctxbFiles,
        IFile[]? shpaFiles,
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
          csabFiles?.Select(csabFile => {
                     var csab = new Csab();
                     csab.Read(new EndianBinaryReader(
                                   csabFile.OpenRead(),
                                   Endianness.LittleEndian));
                     return (csabFile, csab);
                   })
                   .ToList() ??
          new List<(IFile shpaFile, Csab csab)>();

      var filesAndCtxbs =
          ctxbFiles?.Select(ctxbFile => {
                     var ctxb = new Ctxb();
                     ctxb.Read(new EndianBinaryReader(
                                   ctxbFile.OpenRead(),
                                   Endianness.LittleEndian));
                     return (ctxbFile, ctxb);
                   })
                   .ToList() ??
          new List<(IFile shpaFile, Ctxb ctxb)>();

      var filesAndShpas =
          shpaFiles?.Select(shpaFile => {
                     var shpa = new Shpa();
                     shpa.Read(new EndianBinaryReader(
                                   shpaFile.OpenRead(),
                                   Endianness.LittleEndian));
                     return (shpaFile, shpa);
                   })
                   .ToList() ??
          new List<(IFile shpaFile, Shpa shpa)>();

      foreach (var (cmbFile, cmb) in filesAndCmbs) {
        using var r =
            new EndianBinaryReader(cmbFile.OpenRead(), Endianness.LittleEndian);
        var model =
            new ModelConverter().Convert(r,
                                         cmb,
                                         filesAndCsabs,
                                         filesAndCtxbs,
                                         filesAndShpas,
                                         outputDirectory,
                                         fps);

        // TODO: Move this to the indirect model exporter
        foreach (var material in model.MaterialManager.All) {
          foreach (var texture in material.Textures) {
            //texture.ImageData.Save(Path.Combine(outputDirectory.FullName,$"{texture.Name}.png"));
            ;
          }
        }

        new AssimpIndirectExporter().Export(
            new FinFile(Path.Join(outputDirectory.FullName,
                                  cmbFile.NameWithoutExtension + ".fbx")),
            model);
      }
    }
  }
}