using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using UoT.memory.map;

namespace UoT.memory.files {
  public class ZFiles {
    public IReadOnlyList<ZObj> Objects;
    public IReadOnlyList<ZCodeFiles> ActorCode;
    public IReadOnlyList<ZSc> Scenes;
    public IReadOnlyList<ZOtherData> Others;

    private ZFiles(
        IList<ZObj> objects,
        IList<ZCodeFiles> actorCode,
        IList<ZSc> levels,
        IList<ZOtherData> others) {
      this.Objects = new ReadOnlyCollection<ZObj>(objects);
      this.ActorCode = new ReadOnlyCollection<ZCodeFiles>(actorCode);
      this.Scenes = new ReadOnlyCollection<ZSc>(levels);
      this.Others = new ReadOnlyCollection<ZOtherData>(others);
    }


    public static ZFiles? FromRom(string filename) {
      var romBytes = ZFiles.LoadRomBytes(filename);

      //var segments = ZFiles.GetSegments_(romBytes);

      return null;
    }

    // TODO: Make private.
    public static byte[] LoadRomBytes(string filename)
      => File.ReadAllBytes(filename);

    public class Header {}

    // TODO: Make private.
    public static Header? GetHeader() => null;

    private class Segment {
      // Make nonnull via init, C#9
      public string FileName { get; }
      public IShardedMemory Region { get; }

      public Segment(string filename, IShardedMemory region) {
        this.FileName = filename;
        this.Region = region;
      }
    }

    private static IEnumerable<Segment> GetSegments_(
        IShardedMemory romMemory,
        uint segmentOffset,
        int nameOffset) {
      var segments = new List<Segment>();

      bool bothZero;
      do {
        var startAddress = (int) IoUtil.ReadUInt32(romMemory, segmentOffset);
        var endAddress = (int) IoUtil.ReadUInt32(romMemory, segmentOffset + 4);

        bothZero = startAddress == 0 && endAddress == 0;
        if (!bothZero) {
          var fileNameBytes = new List<byte>();
          while (romMemory[nameOffset] == 0) {
            nameOffset++;
          }
          while (romMemory[nameOffset] != 0) {
            fileNameBytes.Add(romMemory[nameOffset++]);
          }
          var fileName =
              System.Text.Encoding.UTF8.GetString(
                  fileNameBytes.ToArray(),
                  0,
                  fileNameBytes.Count);

          segments.Add(new Segment(fileName,
                                   romMemory.Shard(
                                       startAddress,
                                       endAddress - startAddress)));

          segmentOffset += 16;
        }
      } while (!bothZero);

      return segments;
    }

    public static ZFiles GetFiles(
        IShardedMemory romMemory,
        uint segmentOffset,
        int nameOffset) {
      var segments = ZFiles.GetSegments_(romMemory, segmentOffset, nameOffset);

      var objects = new List<ZObj>();
      var actorCode = new List<ZCodeFiles>();
      var scenes = new LinkedList<ZSc>();
      var others = new List<ZOtherData>();

      foreach (var segment in segments) {
        var fileName = segment.FileName;
        var betterFileName = BetterFileNames.Get(fileName);
        var region = segment.Region;

        IZFile file;
        if (fileName.StartsWith("object_")) {
          region.ShardType = ShardedMemoryType.OBJECT;
          var obj = new ZObj(region);
          file = obj;

          objects.Add(obj);
        } else if (fileName.StartsWith("ovl_")) {
          region.ShardType = ShardedMemoryType.CODE;
          var ovl = new ZCodeFiles(region);
          file = ovl;

          actorCode.Add(ovl);
        } else if (fileName.EndsWith("_scene")) {
          region.ShardType = ShardedMemoryType.SCENE;
          var scene = new ZSc(region);
          file = scene;

          scenes.AddLast(scene);
        } else if (fileName.Contains("_room")) {
          var scene = scenes.Last.Value;

          region.ShardType = ShardedMemoryType.MAP;
          var map = new ZMap(region) {Scene = scene};
          file = map;

          var mapCount = scene.Maps?.Length ?? 0;
          Array.Resize(ref scene.Maps, mapCount + 1);
          scene.Maps[mapCount] = map;
        } else {
          region.ShardType = ShardedMemoryType.OTHER_DATA;
          var other = new ZOtherData(region);
          file = other;

          others.Add(other);
        }

        file.FileName = fileName;
        file.BetterFileName = betterFileName;
      }

      return new ZFiles(objects, actorCode, scenes.ToArray(), others);
    }
  }
}