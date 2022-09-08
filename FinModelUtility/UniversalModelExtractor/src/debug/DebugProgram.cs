using System.Security.Cryptography;
using System.Text;
using System.Xml;

using fin.data;
using fin.util.asserts;

using Force.Crc32;

using NullFX.CRC;


namespace uni.debug {
  public class DebugProgram {
    public void Run() {
      using var xmlStream =
          File.OpenRead(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\halo_wars\data\soundtable.xml");

      var xmlReader = XmlReader.Create(xmlStream);

      xmlReader.MoveToContent();

      xmlReader.ReadStartElement("Table");

      var sounds = new List<(string cueName, long cueIndex)>();
      while (true) {
        try {
          xmlReader.ReadStartElement("Sound");
        } catch {
          break;
        }

        xmlReader.ReadStartElement("CueName");
        var cueName = xmlReader.ReadContentAsString();
        xmlReader.ReadEndElement();

        xmlReader.ReadStartElement("CueIndex");

        var longCueIndex = xmlReader.ReadContentAsLong();
        var cueIndex = (uint) longCueIndex;
        Asserts.Equal(longCueIndex, cueIndex);

        xmlReader.ReadEndElement();

        sounds.Add((cueName, cueIndex));

        xmlReader.ReadEndElement();
      }

      sounds.Sort((lhs, rhs) => {
        var (lhsCueName, lhsCueIndex) = lhs;
        var (rhsCueName, rhsCueIndex) = rhs;

        return lhsCueIndex.CompareTo(rhsCueIndex);
      });

      const int bits32 = 32;
      const uint prime32 = 16777619;
      const uint offsetBasis32 = 2166136261U;

      var calcHash32A = (byte[] bytes) => {
        var hval = offsetBasis32;
        foreach (var b in bytes) {
          hval *= prime32;
          hval ^= b;
        }

        return hval;
      };

      var calcHash32B = (byte[] bytes) => {
        ulong hval = offsetBasis32;
        foreach (var b in bytes) {
          hval *= prime32;
          hval ^= b;
        }

        var mask = (1UL << bits32) - 1;
        return (uint) ((hval >> bits32) ^ (hval & mask));
      };

      var calcHashLowercase32A = (string str) => {
        var hval = offsetBasis32;
        foreach (var c in str) {
          hval *= prime32;
          var cc = c is >= 'A' and <= 'Z' ? c - 'A' + 'a' : c;
          hval ^= (uint) cc;
        }
        return hval;
      };

      var calcHashLowercase32B = (string str) => {
        ulong hval = offsetBasis32;
        foreach (var c in str) {
          hval *= prime32;
          var cc = c is >= 'A' and <= 'Z' ? c - 'A' + 'a' : c;
          hval ^= (uint) cc;
        }

        var mask = (1UL << bits32) - 1;
        return (uint) ((hval >> bits32) ^ (hval & mask));
      };


      var offsetBasis30 = offsetBasis32;
      var prime30 = prime32;
      var bits30 = 30;

      var calcHash30A = (byte[] bytes) => {
        var hval = offsetBasis30;
        foreach (var b in bytes) {
          hval *= prime30;
          hval ^= b;
        }

        return hval;
      };

      var calcHash30B = (byte[] bytes) => {
        ulong hval = offsetBasis30;
        foreach (var b in bytes) {
          hval *= prime30;
          hval ^= b;
        }

        var mask = (1UL << bits30) - 1;
        return (uint)((hval >> bits30) ^ (hval & mask));
      };

      var calcHashLowercase30A = (string str) => {
        var hval = offsetBasis30;
        foreach (var c in str) {
          hval *= prime30;
          var cc = c is >= 'A' and <= 'Z' ? c - 'A' + 'a' : c;
          hval ^= (uint)cc;
        }
        return hval;
      };

      var calcHashLowercase30B = (string str) => {
        ulong hval = offsetBasis30;
        foreach (var c in str) {
          hval *= prime30;
          var cc = c is >= 'A' and <= 'Z' ? c - 'A' + 'a' : c;
          hval ^= (uint)cc;
        }

        var mask = (1UL << bits30) - 1;
        return (uint)((hval >> bits30) ^ (hval & mask));
      };



      var soundNamesByCrc32 =
          sounds
              .Select(
                  cueNameAndIndex => cueNameAndIndex.cueName)
              .SelectMany(cueName => {
                var allBytes = new List<byte[]>();

                var allStrings = new[] {
                    cueName,
                    cueName + '\0'
                };

                var strChecksums = allStrings.SelectMany(str => {
                  var allChecksums = new List<uint>();

                  allChecksums.Add(calcHashLowercase32A(str));
                  allChecksums.Add(calcHashLowercase32B(str));

                  allChecksums.Add(calcHashLowercase30A(str));
                  allChecksums.Add(calcHashLowercase30B(str));

                  return allChecksums;
                });

                allBytes.AddRange(
                    allStrings.Select(str => Encoding.ASCII.GetBytes(str)));
                allBytes.AddRange(
                    allStrings.Select(str => Encoding.UTF8.GetBytes(str)));
                allBytes.AddRange(
                    allStrings.Select(str => Encoding.Unicode.GetBytes(str)));
                allBytes.AddRange(
                    allStrings.Select(
                        str => Encoding.BigEndianUnicode.GetBytes(str)));

                var n = allBytes.Count;

                for (var i = 0; i < 4; ++i) {
                  var totalNameBytes = new byte[0x40];
                  var currentBytes = allBytes[i];

                  for (var b = 0; b < currentBytes.Length; ++b) {
                    totalNameBytes[b] = currentBytes[b];
                  }

                  allBytes.Add(totalNameBytes);
                }

                for (var i = 0; i < n; ++i) {
                  var totalNameBytes = new byte[0x80];
                  var currentBytes = allBytes[i];

                  for (var b = 0; b < currentBytes.Length; ++b) {
                    totalNameBytes[b] = currentBytes[b];
                  }

                  allBytes.Add(totalNameBytes);
                }

                n = allBytes.Count;
                for (var i = 0; i < n; ++i) {
                  allBytes.Add(allBytes[i].Reverse().ToArray());
                }

                var byteChecksums = allBytes.SelectMany(bytes => {
                  var allChecksums = new List<uint>();

                  allChecksums.Add(calcHash32A(bytes));
                  allChecksums.Add(calcHash32B(bytes));

                  allChecksums.Add(calcHash30A(bytes));
                  allChecksums.Add(calcHash30B(bytes));

                  allChecksums.Add(Crc32.ComputeChecksum(bytes));

                  allChecksums.Add(Crc32Algorithm.Compute(bytes));
                  allChecksums.Add(Crc32CAlgorithm.Compute(bytes));

                  var md5 = MD5.HashData(bytes);
                  using var er =
                      new EndianBinaryReader(new MemoryStream(md5),
                                             Endianness.LittleEndian);
                  foreach (var val in er.ReadUInt32s(4)) {
                    allChecksums.Add(val);
                  }

                  allChecksums.Add(
                      Crc16.ComputeChecksum(Crc16Algorithm.Standard, bytes));
                  allChecksums.Add(
                      Crc16.ComputeChecksum(Crc16Algorithm.Ccitt, bytes));
                  allChecksums.Add(
                      Crc16.ComputeChecksum(
                          Crc16Algorithm.CcittInitialValue0x1D0F, bytes));
                  allChecksums.Add(
                      Crc16.ComputeChecksum(
                          Crc16Algorithm.CcittInitialValue0xFFFF, bytes));
                  allChecksums.Add(
                      Crc16.ComputeChecksum(Crc16Algorithm.CcittKermit, bytes));
                  allChecksums.Add(
                      Crc16.ComputeChecksum(Crc16Algorithm.Dnp, bytes));
                  allChecksums.Add(
                      Crc16.ComputeChecksum(Crc16Algorithm.Modbus, bytes));

                  return allChecksums;
                });

                var allChecksums = strChecksums.Concat(byteChecksums).ToList();
                var allChecksumCount = allChecksums.Count;
                for (var i = 0; i < allChecksumCount; ++i) {
                  var checksum = allChecksums[i];
                  allChecksums.Add(checksum - 1);
                  allChecksums.Add(checksum + 1);
                }

                return allChecksums.Select(checksum => (cueName, checksum));
              })
              .Select(cueNameAndChecksum => cueNameAndChecksum.checksum)
              .ToHashSet();

      var cueIndices = sounds.Select(sound => sound.cueIndex).ToHashSet();
      var cueIndexMatchCount = 0;

      var filesByLang = new ListDictionary<uint, SoundFile>();
      {
        using var fs =
            File.OpenRead(
                @"R:\Leaks\Wwise-Unpacker-1.0.3\Game Files\Sounds.pck");
        var er = new EndianBinaryReader(fs, Endianness.LittleEndian);

        er.AssertString("AKPK");

        var headerSize = er.ReadUInt32();
        var headerStart = er.Position;
        er.AssertUInt32(1);

        var languageListSize = er.ReadUInt32();
        var bankTableSize = er.ReadUInt32();
        var soundTableSize = er.ReadUInt32();
        var externalsSize = er.ReadUInt32();

        // Read langs
        var langDict = new Dictionary<uint, Lang>();
        {
          var stringsOffset = er.Position;

          var langCount = er.ReadUInt32();
          for (var i = 0; i < langCount; ++i) {
            var langOffset = er.ReadUInt32();
            var langId = er.ReadUInt32();

            var langName = "";
            er.Subread(stringsOffset + langOffset,
                       ser => {
                         langName = ser.ReadStringNT(Encoding.Unicode);
                       });

            var lang = new Lang {
                Id = langId,
                Name = langName
            };

            langDict[langId] = lang;
          }

          er.Position = stringsOffset + languageListSize;
        }

        var maxSoundCrc = 0UL;
        var maxBankCrc = 0UL;

        var readTable = (uint sectionSize, bool isBank) => {
          var sectionOffset = er.Position;
          var fileCount = er.ReadUInt32();

          for (var f = 0; f < fileCount; ++f) {
            var nameCrc32 = er.ReadUInt32();

            if (isBank) {
              maxBankCrc = Math.Max(nameCrc32, maxBankCrc);
            } else {
              maxSoundCrc = Math.Max(nameCrc32, maxSoundCrc);
            }

            var blockSize = er.ReadUInt32();
            var size = er.ReadUInt32();
            var offset = er.ReadUInt32();
            var langId = er.ReadUInt32();

            if (soundNamesByCrc32.Contains(nameCrc32)) {
              ++cueIndexMatchCount;
            }

            er.Subread(offset, ser => {
              var unkId = ser.ReadUInt32();

              if (soundNamesByCrc32.Contains(unkId)) {
                ++cueIndexMatchCount;
              }

              var unk1 = ser.ReadUInt32();
              var unk2 = ser.ReadUInt32();

              uint unk3;
              if (isBank) {
                ser.AssertUInt32(nameCrc32);
              } else {
                unk3 = ser.ReadUInt32();
              }
              var unk4 = ser.ReadUInt32();

              var codecUint = ser.ReadUInt16();

              ;
            });

            var soundFile = new SoundFile {
                Id = nameCrc32,
                BlockSize = blockSize,
                Size = size,
                Offset = offset,
                LangId = langId,
            };

            filesByLang.Add(langId, soundFile);
          }

          er.Position = sectionOffset + sectionSize;
        };

        // Read banks
        readTable(bankTableSize, true);

        ;

        readTable(soundTableSize, false);

        var bankF = 1f * maxBankCrc / uint.MaxValue;
        var soundF = 1f * maxSoundCrc / Math.Pow(2, 30);

        ;
      }

      ;
    }
  }

  public class Lang {
    public uint Id { get; set; }
    public string Name { get; set; }
  }

  public class SoundFile {
    public uint Id { get; set; }
    public uint BlockSize { get; set; }
    public uint Size { get; set; }
    public uint Offset { get; set; }
    public uint LangId { get; set; }
  }

  public enum Codec {
    UNDEFINED,
    XMA,
    OGG,
    WAV
  }
}