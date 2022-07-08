using System.Collections.Generic;
using System.IO;

using fin.exporter.assimp.indirect;
using fin.io;


namespace cmb.api {
  public class ManualZar2FbxApi {
    public void Run(
        IDirectory outputDirectory,
        IFileHierarchyFile cmbFile,
        IReadOnlyList<IFileHierarchyFile>? csabFiles,
        IReadOnlyList<IFileHierarchyFile>? ctxbFiles,
        IReadOnlyList<IFileHierarchyFile>? shpaFiles) {
      var modelFileBundle =
          new CmbModelFileBundle(cmbFile, csabFiles, ctxbFiles, shpaFiles);

      var model =
          new CmbModelLoader().LoadModel(modelFileBundle);

      new AssimpIndirectExporter().Export(
          new FinFile(Path.Join(outputDirectory.FullName,
                                cmbFile.NameWithoutExtension + ".fbx")),
          model);
    }
  }
}