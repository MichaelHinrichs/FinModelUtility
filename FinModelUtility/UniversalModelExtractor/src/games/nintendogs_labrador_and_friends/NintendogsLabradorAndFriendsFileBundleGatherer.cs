using uni.platforms;

using fin.io.bundles;

using SceneGate.Ekona.Containers.Rom;

using Yarhl.FileSystem;

namespace uni.games.nintendogs_labrador_and_friends {
  public class NintendogsLabradorAndFriendsFileBundleGatherer
      : IFileBundleGatherer<IFileBundle> {
    public IEnumerable<IFileBundle> GatherFileBundles(bool assert) {
      if (!DirectoryConstants.ROMS_DIRECTORY.PossiblyAssertExistingFile(
              "nintendogs_labrador_and_friends.nds",
              assert,
              out var nintendogsRom)) {
        yield break;
      }

      var game = NodeFactory.FromFile(nintendogsRom.FullName);
      game.TransformWith<Binary2NitroRom>();

      var names = new List<string>();

      foreach (var node in Navigator.IterateNodes(game)) {
        names.Add(node.Name);
      }

      yield break;
    }
  }
}