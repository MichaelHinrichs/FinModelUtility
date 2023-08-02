using f3dzex2.image;

using SuperMario64.memory;
using SuperMario64.Scripts;
using SuperMario64.JSON;
using SuperMario64.LevelInfo;


namespace SuperMario64.api {
  public static class Sm64LevelLoader {
    public static Level LoadLevel(Sm64LevelFileBundle levelFileBundle) {
      ROM rom = ROM.Instance;

      rom.clearSegments();
      rom.readFile(levelFileBundle.Sm64Rom.FullName);

      Globals.objectComboEntries.Clear();
      Globals.behaviorNameEntries.Clear();
      BehaviorNameFile.parseBehaviorNames(
          Globals.getDefaultBehaviorNamesPath());
      ModelComboFile.parseObjectCombos(Globals.getDefaultObjectComboPath());
      var memoryConstants = Globals.MemoryConstants;
      rom.setSegment(0x15,
                     memoryConstants.Segment15.Offset,
                     memoryConstants.Segment15.Offset + memoryConstants.Segment15.Length,
                     false,
                     null);
      rom.setSegment(0x02,
                     memoryConstants.Segment2.Offset,
                     memoryConstants.Segment2.Offset + memoryConstants.Segment2.Length,
                     rom.isSegmentMIO0(0x02, null),
                     rom.Seg02_isFakeMIO0,
                     rom.Seg02_uncompressedOffset,
                     null);

      var sm64Hardware = new N64Hardware<ISm64Memory>();
      sm64Hardware.Memory = new Sm64Memory();
      sm64Hardware.Rdp = new Rdp { Tmem = new JankTmem(sm64Hardware) };
      sm64Hardware.Rsp = new Rsp();

      var level = new Level((ushort) levelFileBundle.LevelId, 1);
      LevelScripts.parse(sm64Hardware, ref level, 0x15, 0);
      level.sortAndAddNoModelEntries();
      level.CurrentAreaID = level.Areas[0].AreaID;

      return level;
    }
  }
}