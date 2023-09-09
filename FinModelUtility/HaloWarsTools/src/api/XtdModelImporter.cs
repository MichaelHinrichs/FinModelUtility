using fin.model;
using fin.model.io;
using fin.model.io.importer;

using HaloWarsTools;

namespace hw.api {
  public class XtdModelImporter : IModelImporter<XtdModelFileBundle> {
    public IModel ImportModel(XtdModelFileBundle modelFileBundle,
                              IModelParameters? modelParameters = null) {
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