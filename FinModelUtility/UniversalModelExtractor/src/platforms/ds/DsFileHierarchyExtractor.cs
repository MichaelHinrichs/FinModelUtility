using fin.data.queue;
using fin.io;

using SceneGate.Ekona.Containers.Rom;

using Yarhl.FileSystem;

namespace uni.platforms.ds {
  internal class DsFileHierarchyExtractor {
    public IFileHierarchy ExtractFromRom(
        ISystemFile romFile) {
      var outDir = DirectoryConstants.ROMS_DIRECTORY.GetOrCreateSubdir(
          romFile.NameWithoutExtension);

      if (outDir.IsEmpty) {
        var game = NodeFactory.FromFile(romFile.FullPath);
        game.TransformWith<Binary2NitroRom>();

        var nodeQueue = new FinTuple2Queue<string, Node>(("", game));
        while (nodeQueue.TryDequeue(out var path, out var node)) {
          path += node.Name;


          nodeQueue.Enqueue(node.Children.Select(child => (path, child)));
        }
      }

      return new FileHierarchy(outDir);
    }
  }
}