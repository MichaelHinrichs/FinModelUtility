using System.Collections.Generic;
using System.Linq;

using fin.io;

namespace fin.model.io.importer.assimp {
  public class AssimpModelImporterPlugin : IModelImporterPlugin {
    public string DisplayName => "Assimp";
    public string Description => "Loads standard model formats via Assimp.";

    public IReadOnlyList<string> KnownPlatforms { get; } = new string[] { };
    public IReadOnlyList<string> KnownGames { get; } = new string[] { };

    public IReadOnlyList<string> MainFileExtensions { get; }
      = new[] { ".glb", ".gltf", ".fbx", ".obj" };

    public IReadOnlyList<string> FileExtensions => this.MainFileExtensions;

    public IModel ImportModel(IEnumerable<IReadOnlySystemFile> files,
                              float frameRate = 30)
      => new AssimpModelImporter().ImportModel(new AssimpModelFileBundle {
          MainFile = files.Single(),
      });
  }
}