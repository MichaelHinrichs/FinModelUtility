using UoT.memory.files;
using UoT.util;

namespace UoT.ui.main.files {
  public class ZFileTreeView : FileTreeView<IZFile, ZFiles> {
    protected override void PopulateImpl(ZFiles zFiles, FileNode root) {
      var modelsNode = root.AddChild("Actor models");
      foreach (var model in zFiles.Objects) {
        modelsNode.AddChild(model);
      }

      var actorCodeNode = root.AddChild("Actor code");
      foreach (var code in zFiles.ActorCode) {
        actorCodeNode.AddChild(code);
      }

      var scenesNode = root.AddChild("Scenes");
      foreach (var scene in zFiles.Scenes) {
        var sceneNode = scenesNode.AddChild(scene);

        foreach (var map in Asserts.Assert(scene.Maps)) {
          sceneNode.AddChild(map);
        }
      }

      var othersNode = root.AddChild("Others");
      foreach (var other in zFiles.Others) {
        othersNode.AddChild(other);
      }
    }
  }
}