using fin.exporter.assimp.indirect;
using fin.io;

using glo.debug;
using glo.schema;


namespace glo.api {
  public class ManualGloApi {
    public void Run(
        IDirectory outputDirectory,
        GloModelFileBundle gloModelFileBundle) {
      var model = new GloModelLoader().LoadModel(gloModelFileBundle);

      new AssimpIndirectExporter().Export(
          new FinFile(Path.Join(outputDirectory.FullName,
                                gloModelFileBundle.GloFile
                                                  .NameWithoutExtension +
                                ".fbx")),
          model);
    }
  }
}