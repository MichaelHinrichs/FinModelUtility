using fin.io;

using modl.schema.modl.bw1;


namespace uni.games.battalion_wars {
  public class DebugBatallionWars {
    public void Main() {
      var file = new FinFile(@"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\battalion_wars\Data\CompoundFiles\C1_Gauntlet_Level\TVET.modl");

      using var er =
          new EndianBinaryReader(file.OpenRead(), Endianness.LittleEndian);

      var bw1Model = er.ReadNew<Bw1Model>();
    }
  }
}
