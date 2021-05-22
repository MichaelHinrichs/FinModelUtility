using fin.model;

using SharpGLTF.Schema2;

namespace fin.exporter.gltf {
  public class GltfExporter : IExporter {
    public void Export(IModel model) {
      var modelRoot = ModelRoot.CreateModel();

      var scene = modelRoot.UseScene("default");
      var skin = modelRoot.CreateSkin();

      new GltfBoneGatherer().GatherBones(scene,
                                         skin,
                                         model.Skeleton,
                                         model.Animations);
    }
  }
}