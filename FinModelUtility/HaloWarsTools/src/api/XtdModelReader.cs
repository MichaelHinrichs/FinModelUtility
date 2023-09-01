using fin.io;
using fin.model;

using HaloWarsTools;

namespace hw.api {
  public class XtdModelFileBundle : IHaloWarsModelFileBundle {
    public XtdModelFileBundle(IFileHierarchyFile xtdFile,
                              IFileHierarchyFile xttFile) {
      this.XtdFile = xtdFile;
      this.XttFile = xttFile;
    }

    public string GameName => "halo_wars";
    public IFileHierarchyFile MainFile => this.XtdFile;
    public IFileHierarchyFile XttFile { get; }
    public IFileHierarchyFile XtdFile { get; }

    public bool UseLowLevelExporter => true;
    public bool ForceGarbageCollection => true;
  }

  public class XtdModelReader : IModelReader<XtdModelFileBundle> {
    public IModel ReadModel(XtdModelFileBundle modelFileBundle) {
      var xtdFile = modelFileBundle.XtdFile;
      var xttFile = modelFileBundle.XttFile;

      var mapName = xtdFile.Parent.Name;

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