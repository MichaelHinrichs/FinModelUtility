using fin.exporter.assimp.indirect;
using fin.io;

using glo.debug;
using glo.schema;


namespace glo.api {
  public class ManualGloApi {
    public void Run(
        IDirectory outputDirectory,
        IList<IDirectory> textureDirectories,
        IFile gloFile,
        float fps) {
      using var er =
          new EndianBinaryReader(gloFile.OpenRead(), Endianness.LittleEndian);

      var glo = new Glo();
      glo.Read(er);

      new MeshCsvWriter().WriteToFile(
          glo, new FinFile(Path.Join(outputDirectory.FullName, "mesh.csv")));
      new FaceCsvWriter().WriteToFile(
          glo, new FinFile(Path.Join(outputDirectory.FullName, "face.csv")));
      new VertexCsvWriter().WriteToFile(
          glo, new FinFile(Path.Join(outputDirectory.FullName, "vertex.csv")));

      var model = new ModelConverter().Convert(glo,
                                               outputDirectory,
                                               textureDirectories,
                                               fps);

      new AssimpIndirectExporter().Export(
          new FinFile(Path.Join(outputDirectory.FullName,
                                gloFile.NameWithoutExtension + ".fbx")),
          model);
    }
  }
}