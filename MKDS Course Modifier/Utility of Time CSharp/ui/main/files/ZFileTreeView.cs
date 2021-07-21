using UoT.memory.files;
using UoT.ui.common.component;
using UoT.util;

namespace UoT.ui.main.files {
  public class ZFileTreeView : FileTreeView<IZFile, ZFiles> {
    protected override void PopulateImpl(ZFiles zFiles, BetterTreeNode<FileNode> root) {
      var modelsNode = root.Add("Actor models");
      this.AddFileNodeFor(null, modelsNode);
      foreach (var model in zFiles.Objects) {
        var modelNode = modelsNode.Add(model.BetterFileName!);
        this.AddFileNodeFor(model, modelNode);
      }

      var actorCodeNode = root.Add("Actor code");
      this.AddFileNodeFor(null, actorCodeNode);
      foreach (var code in zFiles.ActorCode) {
        var codeNode = actorCodeNode.Add(code.BetterFileName!);
        this.AddFileNodeFor(code, codeNode);
      }

      var scenesNode = root.Add("Scenes");
      this.AddFileNodeFor(null, scenesNode);
      foreach (var scene in zFiles.Scenes) {
        var sceneNode = scenesNode.Add(scene.BetterFileName!);
        this.AddFileNodeFor(scene, sceneNode);

        foreach (var map in Asserts.Assert(scene.Maps)) {
          var mapNode = sceneNode.Add(map.BetterFileName!);
          this.AddFileNodeFor(map, mapNode);
        }
      }

      var othersNode = root.Add("Others");
      this.AddFileNodeFor(null, othersNode);
      foreach (var other in zFiles.Others) {
        var otherNode = othersNode.Add(other.BetterFileName!);
        this.AddFileNodeFor(other, otherNode);
      }
    }
  }
}