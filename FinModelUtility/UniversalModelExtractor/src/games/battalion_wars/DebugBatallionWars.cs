using fin.io;

using modl.schema.modl.bw1;


namespace uni.games.battalion_wars {
  public class DebugBatallionWars {
    public void Main() {
      var file =
          new FinFile(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\battalion_wars\Data\CompoundFiles\C1_Gauntlet_Level\TVET.modl");
      var bytes = file.ReadAllBytes();

      using var memoryUsageStream = new MemoryUsageStream(bytes);
      var er =
          new EndianBinaryReader(memoryUsageStream, Endianness.LittleEndian);

      var bw1Model = er.ReadNew<Bw1Model>();

      var untouchedRanges = memoryUsageStream.GetUntouchedRanges().ToList();

      foreach (var node in bw1Model.Nodes) {
        var mesh = node.Meshes[1];

        foreach (var strip in mesh.TriangleStrips) {
          var vertices = strip.VertexAttributeIndicesList;

          var nodes = vertices.Select(vertex => vertex.NodeIndex).ToArray();

          for (var i = 0; i < vertices.Count; ++i) {
            var magic = 15;

            var vertex = vertices[i];
            if (vertex.PositionIndex == magic) {
              ;
            }

            if (vertex.NodeIndex == magic) {
              ;
            }

            if (vertex.NormalIndex == magic) {
              ;
            }

            foreach (var texCoordIndex in vertex.TexCoordIndices) {
              if (texCoordIndex == magic) {
                ;
              }
            }
          }
        }
      }

      ;
    }
  }
}