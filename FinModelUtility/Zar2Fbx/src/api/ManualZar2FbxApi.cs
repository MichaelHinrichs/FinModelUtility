using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.exporter.assimp.indirect;
using fin.io;
using fin.model;
using fin.util.asserts;

using zar.format.cmb;
using zar.format.csab;
using zar.format.ctxb;
using zar.format.shpa;


namespace zar.api {
  public class ManualZar2FbxApi {
    public void Run(
        IDirectory outputDirectory,
        IFileHierarchyFile cmbFile,
        IReadOnlyList<IFileHierarchyFile>? csabFiles,
        IReadOnlyList<IFileHierarchyFile>? ctxbFiles,
        IReadOnlyList<IFileHierarchyFile>? shpaFiles) {
      var modelFileBundle =
          new ZarModelFileBundle(cmbFile, csabFiles, ctxbFiles, shpaFiles);

      var model =
          new ZarModelLoader().LoadModel(modelFileBundle);

      new AssimpIndirectExporter().Export(
          new FinFile(Path.Join(outputDirectory.FullName,
                                cmbFile.NameWithoutExtension + ".fbx")),
          model);
    }
  }
}