using fin.io;

using modl.schema.anim;
using modl.schema.modl.bw1;


namespace uni.games.battalion_wars {
  public class DebugBattalionWars {
    public void Main() {
      var fileName = "FGRUN.anim";
      //fileName = "FGRIGHTTCROUCHTURN.anim";
      //fileName = "FGSTEERAFRONT.anim";

      var file =
          new FinFile(
              $@"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\battalion_wars\Data\CompoundFiles\C1_Bonus_Level\{fileName}");
      var er = new EndianBinaryReader(file.OpenRead(), Endianness.BigEndian);

      var anim = er.ReadNew<Anim>();
    }
  }
}