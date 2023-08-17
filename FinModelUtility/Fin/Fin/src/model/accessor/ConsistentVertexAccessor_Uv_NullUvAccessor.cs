using System.Runtime.CompilerServices;

namespace fin.model {
  public partial class ConsistentVertexAccessor : IVertexAccessor {
    private sealed class NullUvAccessor
        : BAccessor, IVertexUvAccessor {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Target(IReadOnlyVertex vertex) { }

      public int UvCount => 0;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public TexCoord? GetUv() => null;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public TexCoord? GetUv(int uvIndex) => null;
    }
  }
}