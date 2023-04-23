using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

using fin.io;

namespace UoT.api {
  public enum ZFileType {
    OBJECT,
    CODE,
    SCENE,
    MAP,

    /// <summary>
    ///   A set of objects in a given map. These seem to be used to switch
    ///   between different versions of rooms.
    /// </summary>
    OBJECT_SET,
    OTHER,
  }

  public interface IZFile {
    ZFileType Type { get; }

    uint Offset { get; }
    uint Length { get; }
  }


  public abstract class BZFile : IZFile {
    protected BZFile(uint offset, uint length) {
      this.Offset = offset;
      this.Length = length;
    }

    public abstract ZFileType Type { get; }

    public string FileName { get; set; }

    public uint Offset { get; }
    public uint Length { get; }
  }


  public class ZObject : BZFile {
    public ZObject(uint offset, uint length) : base(offset, length) { }
    public override ZFileType Type => ZFileType.OBJECT;
  }


  public class ZCodeFiles : BZFile {
    public ZCodeFiles(uint offset, uint length) : base(offset, length) { }
    public override ZFileType Type => ZFileType.CODE;
  }


  public class ZScene : BZFile {
    public ZScene(uint offset, uint length) : base(offset, length) { }
    public override ZFileType Type => ZFileType.SCENE;

    // TODO: Make nonnull via init, C#9.
    public ZMap[]? Maps;
  }

  public class ZMap : BZFile {
    public ZMap(uint offset, uint length) : base(offset, length) { }
    public override ZFileType Type => ZFileType.MAP;

    // TODO: Make nonnull via init, C#9.
    public ZScene? Scene { get; set; }
  }

  public class ZObjectSet : BZFile {
    public ZObjectSet(uint offset, uint length) : base(offset, length) { }
    public override ZFileType Type => ZFileType.OBJECT_SET;
  }

  public class ZOtherData : BZFile {
    public ZOtherData(uint offset, uint length) : base(offset, length) { }
    public override ZFileType Type => ZFileType.OTHER;
  }

  public class ZSegments {
    public IReadOnlyList<ZObject> Objects { get; }
    public IReadOnlyList<ZCodeFiles> ActorCode { get; }
    public IReadOnlyList<ZScene> Scenes { get; }
    public IReadOnlyList<ZOtherData> Others { get; }

    private ZSegments(
        IList<ZObject> objects,
        IList<ZCodeFiles> actorCode,
        IList<ZScene> levels,
        IList<ZOtherData> others) {
      this.Objects = new ReadOnlyCollection<ZObject>(objects);
      this.ActorCode = new ReadOnlyCollection<ZCodeFiles>(actorCode);
      this.Scenes = new ReadOnlyCollection<ZScene>(levels);
      this.Others = new ReadOnlyCollection<ZOtherData>(others);
    }

    public static ZSegments GetFiles(IGenericFile romFile) {
      using var er =
          new EndianBinaryReader(romFile.OpenRead(), Endianness.BigEndian);

      for (long i = 0; i < er.Length; i += 16) {
        var romId = er.ReadStringAtOffset(i, 6);
        if (romId != "zelda@") {
          continue;
        }

        i += 0xd;

        er.Position = i;
        while ((er.ReadByte() >> 4) != 3) {
          ;
        }

        i = (er.Position -= 1);

        var buildDate = er.ReadStringAtOffset(i, 17);
        var segmentOffset = (int) (i + 0x20);

        int nameOffset;

        switch (buildDate) {
          case "00-07-31 17:04:16": {
            nameOffset = -1;
            break;
          }
          case "03-02-21 00:16:31": {
            nameOffset = 0xBE80;
            break;
          }
          default: throw new NotSupportedException();
        }

        return ZSegments.GetFiles(er, segmentOffset, nameOffset);
      }

      throw new NotSupportedException();
    }

    public static ZSegments GetFiles(
        IEndianBinaryReader er,
        int segmentOffset,
        int nameOffset) {
      var segments = ZSegments.GetSegments_(er, segmentOffset, nameOffset);

      var objects = new List<ZObject>();
      var actorCode = new List<ZCodeFiles>();
      var scenes = new LinkedList<ZScene>();
      var others = new List<ZOtherData>();

      foreach (var segment in segments) {
        var fileName = segment.FileName;
        var offset = segment.Offset;
        var length = segment.Length;

        BZFile file;
        if (fileName.StartsWith("object_")) {
          var obj = new ZObject(offset, length);
          file = obj;

          objects.Add(obj);
        } else if (fileName.StartsWith("ovl_")) {
          var ovl = new ZCodeFiles(offset, length);
          file = ovl;

          actorCode.Add(ovl);
        } else if (fileName.EndsWith("_scene")) {
          var scene = new ZScene(offset, length);
          file = scene;

          scenes.AddLast(scene);
        } else if (fileName.Contains("_room")) {
          var scene = scenes.Last.Value;

          var map = new ZMap(offset, length) { Scene = scene };
          file = map;

          var mapCount = scene.Maps?.Length ?? 0;
          Array.Resize(ref scene.Maps, mapCount + 1);
          scene.Maps[mapCount] = map;
        } else {
          var other = new ZOtherData(offset, length);
          file = other;

          others.Add(other);
        }

        file.FileName = fileName;
      }

      return new ZSegments(objects, actorCode, scenes.ToArray(), others);
    }

    private static IEnumerable<Segment> GetSegments_(
        IEndianBinaryReader er,
        int segmentOffset,
        long nameOffset) {
      var segments = new LinkedList<Segment>();

      er.Subread(
          segmentOffset,
          ser => {
            while (true) {
              var startAddress = ser.ReadUInt32();
              var endAddress = ser.ReadUInt32();

              if (startAddress == 0 && endAddress == 0) {
                break;
              }

              var unk0 = ser.ReadUInt32();
              var unk1 = ser.ReadUInt32();

              var fileNameBuilder = new StringBuilder();
              var pos = ser.Position;
              ser.Position = nameOffset;
              var inName = false;
              while (true) {
                var c = ser.ReadChar();

                if (c == '\0') {
                  if (inName) {
                    break;
                  }
                } else {
                  inName = true;
                  fileNameBuilder.Append(c);
                }
              }
              nameOffset = ser.Position;
              ser.Position = pos;

              var fileName = fileNameBuilder.ToString();

              segments.AddLast(new Segment {
                  FileName = fileName,
                  Offset = startAddress,
                  Length = endAddress - startAddress,
              });
            }
          });

      return segments;
    }

    private class Segment {
      public required string FileName { get; init; }
      public required uint Offset { get; init; }
      public required uint Length { get; init; }
    }
  }
}