using System.Linq;

namespace fin.model.extensions {
  public static class SkinExtensions {
    public static bool HasNormals(this ISkin skin)
      => skin.Meshes
             .SelectMany(mesh => mesh.Primitives)
             .SelectMany(primitive => primitive.Vertices)
             .Any(vertex => vertex is IReadOnlyNormalVertex {
                 LocalNormal: not null
             });

    public static bool HasNormalsForMaterial(this ISkin skin,
                                             IReadOnlyMaterial material)
      => skin.Meshes
             .SelectMany(mesh => mesh.Primitives)
             .Where(primitive => primitive.Material == material)
             .SelectMany(primitive => primitive.Vertices)
             .Any(vertex => vertex is IReadOnlyNormalVertex {
                 LocalNormal: not null
             });
  }
}