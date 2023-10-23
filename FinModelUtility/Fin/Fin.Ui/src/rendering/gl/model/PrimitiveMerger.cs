using System.Linq;

using fin.model;
using fin.util.enumerables;

namespace fin.ui.rendering.gl.model {
  public readonly struct MergedPrimitive {
    public required PrimitiveType PrimitiveType { get; init; }
    public required IReadOnlyList<IReadOnlyVertex> Vertices { get; init; }
    public required bool IsFlipped { get; init; }
  }

  public class PrimitiveMerger {
    public bool TryToMergePrimitives(
        IReadOnlyList<IPrimitive> primitives,
        out MergedPrimitive mergedPrimitive) {
      mergedPrimitive = default;
      if (primitives.Count == 0) {
        return false;
      }

      if (primitives.Count == 1) {
        var primitive = primitives[0];
        mergedPrimitive = new MergedPrimitive {
            PrimitiveType = primitive.Type,
            Vertices = primitive.Vertices,
            IsFlipped = primitive.VertexOrder == VertexOrder.FLIP,
        };
        return true;
      }

      var primitiveTypes = primitives.Select(primitive => primitive.Type)
                                     .Distinct()
                                     .ToArray();
      if (primitiveTypes is [PrimitiveType.LINES or PrimitiveType.POINTS]) {
        var primitiveType = primitiveTypes.First();
        mergedPrimitive = new MergedPrimitive {
            PrimitiveType = primitiveType,
            Vertices = primitives.SelectMany(primitive => primitive.Vertices)
                                 .ToArray(),
            IsFlipped = false,
        };

        return true;
      }

      var flippedType =
          primitives.Select(primitive
                                => primitive.VertexOrder == VertexOrder.FLIP)
                    .Distinct()
                    .ToArray();
      if (primitiveTypes is [PrimitiveType.TRIANGLE_STRIP] &&
          flippedType.Length == 1) {
        var totalVertexCount =
            primitives.Sum(primitive => primitive.Vertices.Count) +
            2 * primitives.Count;

        var i = 0;
        var mergedVertices = new IReadOnlyVertex[totalVertexCount];
        for (var p = 0; p < primitives.Count; ++p) {
          var primitive = primitives[p];
          var vertices = primitive.Vertices;

          if (p > 0) {
            mergedVertices[i++] = vertices[0];
          }

          foreach (var vertex in vertices) {
            mergedVertices[i++] = vertex;
          }

          if (p < primitives.Count - 1) {
            mergedVertices[i++] = vertices[^1];
          }
        }

        mergedPrimitive = new MergedPrimitive {
            PrimitiveType = PrimitiveType.TRIANGLE_STRIP,
            Vertices = primitives.SelectMany(primitive => primitive.Vertices)
                                 .ToArray(),
            IsFlipped = flippedType[0],
        };

        return true;
      }

      mergedPrimitive = new MergedPrimitive {
          PrimitiveType = PrimitiveType.TRIANGLES,
          Vertices = primitives
                     .SelectMany(primitive
                                     => primitive.GetOrderedTriangleVertices())
                     .ToArray(),
          IsFlipped = false,
      };

      return true;
    }
  }
}