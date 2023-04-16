using f3dzex2.image;
using f3dzex2.io;

using Quad64.memory;
using Quad64.schema;
using Quad64.src.JSON;
using Quad64.src.Scripts;


namespace Quad64.src.LevelInfo {
  public class AreaBackgroundInfo {
    public uint address = 0;
    public ushort id_or_color = 0;
    public bool isEndCakeImage = false;
    public uint romLocation = 0;
    public bool usesFog = false;
    public Color fogColor = Color.White;
    public List<uint> fogColor_romLocation = new List<uint>();
  }

  public class Area {
    public Level parent;
    private ushort areaID;

    public ushort AreaID {
      get { return areaID; }
    }

    private uint geoLayoutPointer;

    public uint GeometryLayoutPointer {
      get { return geoLayoutPointer; }
    }

    public AreaBackgroundInfo bgInfo = new AreaBackgroundInfo();

    public Model3DLods AreaModel;
    public CollisionMap collision = new CollisionMap();

    public List<Object3D> Objects = new List<Object3D>();
    public List<Object3D> MacroObjects = new List<Object3D>();
    public List<Object3D> SpecialObjects = new List<Object3D>();
    public List<Warp> Warps = new List<Warp>();
    public List<Warp> PaintingWarps = new List<Warp>();
    public List<WarpInstant> InstantWarps = new List<WarpInstant>();

    private byte? defaultTerrainType_;

    public byte DefaultTerrainType {
      get => defaultTerrainType_ ?? 0;
      set {
        if (this.defaultTerrainType_.HasValue) {
          throw new Exception();
        }
        this.defaultTerrainType_ = value;
      }
    }

    public Area(IN64Hardware<ISm64Memory> sm64Hardware, ushort areaID, uint geoLayoutPointer, Level parent) {
      this.AreaModel = new Model3DLods(sm64Hardware);
      this.areaID = areaID;
      this.geoLayoutPointer = geoLayoutPointer;
      this.parent = parent;
    }
  }

  public class Level {
    private ushort levelID;

    public ushort LevelID {
      get { return levelID; }
    }

    private ushort currentAreaID;

    public ushort CurrentAreaID {
      get { return currentAreaID; }
      set { currentAreaID = value; }
    }

    public List<Area> Areas = new List<Area>();
    public AreaBackgroundInfo temp_bgInfo = new AreaBackgroundInfo();

    public Dictionary<ushort, Model3DLods> ModelIDs =
        new Dictionary<ushort, Model3DLods>();

    public List<ObjectComboEntry> LevelObjectCombos =
        new List<ObjectComboEntry>();

    public List<PresetMacroEntry> MacroObjectPresets =
        new List<PresetMacroEntry>();

    public List<PresetMacroEntry> SpecialObjectPresets_8 =
        new List<PresetMacroEntry>();

    public List<PresetMacroEntry> SpecialObjectPresets_10 =
        new List<PresetMacroEntry>();

    public List<PresetMacroEntry> SpecialObjectPresets_12 =
        new List<PresetMacroEntry>();

    public List<ScriptDumpCommandInfo> LevelScriptCommands_ForDump =
        new List<ScriptDumpCommandInfo>();


    public ObjectComboEntry? getObjectComboFromData(
        byte modelID,
        uint modelAddress,
        uint behavior,
        out int index) {
      for (int i = 0; i < LevelObjectCombos.Count; i++) {
        ObjectComboEntry oce = LevelObjectCombos[i];
        if (oce.ModelID == modelID && oce.ModelSegmentAddress == modelAddress
                                   && oce.Behavior == behavior) {
          index = i;
          return oce;
        }
      }
      index = -1;
      return null;
    }

    private void AddMacroObjectEntries() {
      MacroObjectPresets.Clear();
      ROM rom = ROM.Instance;

      using var er = new EndianBinaryReader(rom.Bytes);
      er.Position = Globals.macro_preset_table;

      ushort pID = 0x1F;
      for (int i = 0; i < 366; i++) {
        var presetMacroEntry = er.ReadNew<PresetMacroEntry>();
        presetMacroEntry.PresetId = pID++;
        this.MacroObjectPresets.Add(presetMacroEntry);
      }
    }

    public void AddSpecialObjectPreset_8(ushort presetID,
                                         byte modelId,
                                         uint behavior) {
      SpecialObjectPresets_8.Add(
          new PresetMacroEntry(presetID, modelId, behavior));
    }

    public void AddSpecialObjectPreset_10(ushort presetID,
                                          byte modelId,
                                          uint behavior) {
      SpecialObjectPresets_10.Add(
          new PresetMacroEntry(presetID, modelId, behavior));
    }

    public void AddSpecialObjectPreset_12(ushort presetID,
                                          byte modelId,
                                          uint behavior,
                                          byte bp1,
                                          byte bp2) {
      SpecialObjectPresets_12.Add(
          new PresetMacroEntry(presetID, modelId, behavior, bp1, bp2));
    }

    public void AddObjectCombos(byte modelId, uint modelSegAddress) {
      for (int i = 0; i < Globals.objectComboEntries.Count; i++) {
        ObjectComboEntry oce = Globals.objectComboEntries[i];
        if (oce.ModelID == modelId &&
            oce.ModelSegmentAddress == modelSegAddress)
          LevelObjectCombos.Add(oce);
      }
    }

    public void sortAndAddNoModelEntries() {
      for (int i = 0; i < Globals.objectComboEntries.Count; i++) {
        ObjectComboEntry oce = Globals.objectComboEntries[i];
        if (oce.ModelID == 0x00)
          LevelObjectCombos.Add(oce);
      }
      LevelObjectCombos.Sort((x, y) => string.Compare(x.Name, y.Name));
    }

    public void printLevelObjectCombos() {
      for (int i = 0; i < LevelObjectCombos.Count; i++)
        Console.WriteLine(LevelObjectCombos[i].ToString());
    }

    public bool hasArea(ushort areaID) {
      foreach (Area a in Areas)
        if (a.AreaID == areaID)
          return true;
      return false;
    }

    public HashSet<ushort> getModelIDsUsedByObjects() {
      HashSet<ushort> modelIDs = new HashSet<ushort>();

      foreach (Area area in Areas) {
        foreach (Object3D obj in area.Objects)
          if (obj.ModelID > 0)
            modelIDs.Add(obj.ModelID);
        foreach (Object3D obj in area.MacroObjects)
          if (obj.ModelID > 0)
            modelIDs.Add(obj.ModelID);
        foreach (Object3D obj in area.SpecialObjects)
          if (obj.ModelID > 0)
            modelIDs.Add(obj.ModelID);
      }

      return modelIDs;
    }

    public Area getCurrentArea() {
      foreach (Area a in Areas)
        if (a.AreaID == currentAreaID)
          return a;
      return Areas[0]; // return default area
    }

    public void setAreaBackgroundInfo(ref Area area) {
      area.bgInfo.address = temp_bgInfo.address;
      area.bgInfo.id_or_color = temp_bgInfo.id_or_color;
      area.bgInfo.isEndCakeImage = temp_bgInfo.isEndCakeImage;
      area.bgInfo.romLocation = temp_bgInfo.romLocation;
      area.bgInfo.usesFog = temp_bgInfo.usesFog;
      area.bgInfo.fogColor = temp_bgInfo.fogColor;
      area.bgInfo.fogColor_romLocation = temp_bgInfo.fogColor_romLocation;
    }

    public Level(ushort levelID, ushort startArea) {
      ROM.Instance.clearSegments();
      this.levelID = levelID;
      currentAreaID = startArea;
      LevelObjectCombos.Clear();
      LevelScriptCommands_ForDump.Clear();
      AddMacroObjectEntries();
    }
  }
}