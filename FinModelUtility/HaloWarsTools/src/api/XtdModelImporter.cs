using fin.io;
using fin.model;
using fin.model.io.importer;

using HaloWarsTools;

namespace hw.api {
  public class XtdModelFileBundle : IHaloWarsModelFileBundle {
    public XtdModelFileBundle(IReadOnlyTreeFile xtdFile,
                              IReadOnlyTreeFile xttFile) {
      this.XtdFile = xtdFile;
      this.XttFile = xttFile;
    }

    public string GameName => "halo_wars";
    public IReadOnlyTreeFile MainFile => this.XtdFile;
    public IReadOnlyTreeFile XttFile { get; }
    public IReadOnlyTreeFile XtdFile { get; }

    public bool UseLowLevelExporter => true;
    public bool ForceGarbageCollection => true;
  }

  public class XtdModelImporter : IModelImporter<XtdModelFileBundle> {
    public IModel ImportModel(XtdModelFileBundle modelFileBundle) {
      var xtdFile = modelFileBundle.XtdFile;
      var xttFile = modelFileBundle.XttFile;

      var mapName = xtdFile.AssertGetParent().Name;

      var xtd = HWXtdResource.FromFile(null, xtdFile.FullPath);
      var xtt = HWXttResource.FromFile(null, xttFile.FullPath);

      var finModel = xtd.Mesh;
      var xttMaterial = finModel.MaterialManager.AddStandardMaterial();

      xttMaterial.DiffuseTexture = finModel.MaterialManager.CreateTexture(
          xtt.AlbedoTexture);
      xttMaterial.DiffuseTexture.Name = $"{mapName}_albedo";

      xttMaterial.AmbientOcclusionTexture =
          finModel.MaterialManager.CreateTexture(
              xtd.AmbientOcclusionTexture);
      xttMaterial.AmbientOcclusionTexture.Name = $"{mapName}_ao";

      foreach (var primitive in finModel.Skin.Meshes[0].Primitives) {
        primitive.SetMaterial(xttMaterial);
      }

      return xtd.Mesh;
    }
  }
}