using System.Collections;

using fin.data.queue;
using fin.io;
using fin.model;

using schema.binary;


namespace visceral.api {
  public class BnkLoader {
    public enum MaybeBoneType {
      ROOT = 0x2,
      PARENT = 0x5,
      ANIMATED = 0x16,
    }

    public enum AxisType : byte {
      SINGLETON_0 = 0xC,
      SINGLETON_1 = 0xD,
      SINGLETON_CONSTANT = 0x0,
      TYPE_2 = 0x2,
      TYPE_6 = 0x6,
      TYPE_7 = 0x7,
    }

    public enum MaybeMultipleKeyframeType : byte {
      ONLY_KEYFRAME = 0x0,
      KEYFRAME_AND_3_BYTES = 0x1,
      KEYFRAME_AND_6_BYTES = 0x2,
      KEYFRAME_AND_9_BYTES = 0x3,
      KEYFRAME_AND_BYTE_GRADIENT = 0x6,
      COMMAND_7 = 0x7,
    }

    public void LoadBnk(IModel model,
                        ISystemFile bnkFile,
                        ISystemFile rcbFile,
                        IBone[] bones) {
      using var bnkEr =
          new EndianBinaryReader(bnkFile.OpenRead(), Endianness.LittleEndian);

      bnkEr.Position = 0x24;
      var headerCount = bnkEr.ReadUInt32();
      var headerOffsetOffset = bnkEr.ReadUInt32();

      bnkEr.Position = headerOffsetOffset;
      var headerOffsets = bnkEr.ReadUInt32s(headerCount);

      foreach (var headerOffset in headerOffsets) {
        var finAnimation = model.AnimationManager.AddAnimation();

        bnkEr.Position = headerOffset;

        {
          var animationNameOffset = bnkEr.ReadUInt32();
          finAnimation.Name =
              bnkEr.Subread(animationNameOffset, ser => ser.ReadStringNT());
        }

        var totalFrames = 1;

        {
          var rootOffset = bnkEr.ReadUInt32();
          var maybeBoneOffsetQueue = new FinQueue<uint>(rootOffset);
          while (maybeBoneOffsetQueue.TryDequeue(out var maybeBoneOffset)) {
            bnkEr.Position = maybeBoneOffset;

            var boneType = (MaybeBoneType) bnkEr.ReadByte();
            var childCount = bnkEr.ReadByte();
            var maybeId = bnkEr.ReadByte();
            var unk1 = bnkEr.ReadByte();

            var someMetadataOffset = bnkEr.ReadUInt32();
            var unk2 = bnkEr.ReadUInt32();

            if (childCount > 0) {
              maybeBoneOffsetQueue.Enqueue(bnkEr.ReadUInt32s(childCount));
            }

            if (boneType == MaybeBoneType.ANIMATED) {
              var animationDataOffset = bnkEr.ReadUInt32();
              bnkEr.Position = animationDataOffset;

              var unkHash = bnkEr.ReadUInt32();
              var maybeVersion = bnkEr.ReadUInt32();
              var unk3 = bnkEr.ReadUInt32();
              var unk4 = bnkEr.ReadUInt32();
              var unk5 = bnkEr.ReadUInt32();

              var someHashFromRcb = bnkEr.ReadUInt32();
              var standaloneCommandPrefix = bnkEr.ReadUInt32();

              // These are unknown
              bnkEr.Position += 4 * 5;

              using var rcbEr =
                  new EndianBinaryReader(rcbFile.OpenRead(),
                                         Endianness.LittleEndian);
              rcbEr.Position = 0x24;
              var someDefinitionsOffset = rcbEr.ReadUInt32();
              rcbEr.Position = someDefinitionsOffset;
              while (rcbEr.ReadUInt32() != someHashFromRcb) ;
              var bitMaskOffsetOffset = rcbEr.ReadUInt32();
              rcbEr.Position = bitMaskOffsetOffset;
              var bitMaskOffset = rcbEr.ReadUInt32();
              var bitMaskLength = rcbEr.ReadUInt32();

              rcbEr.Position = bitMaskOffset;
              var bitMaskArray =
                  new BitArray(
                      rcbEr.ReadBytes((int) Math.Ceiling(bitMaskLength / 8f)));

              for (var b = 0; b < bitMaskLength; ++b) {
                var isActive = bitMaskArray[b];
                if (!isActive) {
                  continue;
                }

                var boneTracks = finAnimation.AddBoneTracks(bones[b]);
                var rotations = boneTracks.UseQuaternionAxesRotationTrack();
                var positions = boneTracks.UseSeparatePositionAxesTrack();

                for (var a = 0; a < 7; ++a) {
                  Action<int, float> setKeyframe = (frame, value) => {
                    switch (a) {
                      case 0:
                      case 1:
                      case 2:
                      case 3: {
                        rotations.Set(frame, a, value);
                        break;
                      }
                      case 4:
                      case 5:
                      case 6: {
                        positions.Set(frame, a - 4, value);
                        break;
                      }
                      default: throw new Exception();
                    }
                  };

                  var command = bnkEr.ReadUInt16();
                  if (command >> 4 == standaloneCommandPrefix) {
                    var axisType = (AxisType) (command & 0xF);

                    switch (axisType) {
                      case AxisType.SINGLETON_0: {
                        var value = 0;
                        setKeyframe(0, value);
                        break;
                      }
                      case AxisType.SINGLETON_1: {
                        var value = 1;
                        setKeyframe(0, value);
                        break;
                      }
                      case AxisType.SINGLETON_CONSTANT: {
                        bnkEr.Position -= 1;
                        var value = bnkEr.ReadSingle();
                        setKeyframe(0, value);
                        break;
                      }
                      case AxisType.TYPE_2: {
                        // TODO: Verify if this is correct
                        // Plasma cutter:
                        // D2   00 6A 0C B6 C3 AF   37 03 F7 3D
                        bnkEr.Position += 5;
                        var value = bnkEr.ReadSingle();
                        setKeyframe(0, value);
                        break;
                      }
                      case AxisType.TYPE_6: {
                        throw new NotImplementedException();
                      }
                      case AxisType.TYPE_7: {
                        // Plasma cutter:
                        /**
                         * D7 00
                         * E7 0C
                         * BD 0A 8A 35 FE FF BE FF 7F FF 3F FF FF FE FF CB FF 98 00 66 00 33 00 00 BD 77 5E BB FF FE 
                         */
                        bnkEr.Position += 1;
                        var frameCount = 1 + bnkEr.ReadByte();
                        var scale = bnkEr.ReadSingle();
                        for (var f = 0; f < frameCount; ++f) {
                          var raw = bnkEr.ReadUInt16();
                          setKeyframe(f, raw * scale);
                        }

                        break;
                      }
                      default:
                        throw new NotImplementedException();
                    }
                  } else if ((command & 0xF) == 5) {
                    var keyframeCount = command >> 4;

                    var frame = 0;
                    for (var k = 0; k < keyframeCount; ++k) {
                      var lengthAndKeyframeType = bnkEr.ReadByte();
                      var length = lengthAndKeyframeType >> 4;
                      var keyframeType =
                          (MaybeMultipleKeyframeType) (lengthAndKeyframeType &
                              0xF);

                      float value;
                      switch (keyframeType) {
                        case MaybeMultipleKeyframeType.ONLY_KEYFRAME: {
                          value = bnkEr.ReadSingle();
                          break;
                        }
                        case MaybeMultipleKeyframeType.KEYFRAME_AND_3_BYTES: {
                          // Security camera:
                          // 21   00 44 7C                     BB 44 7C 3B
                          // TODO: Add support for this. Is this easing?
                          bnkEr.Position += 3;
                          value = bnkEr.ReadSingle();
                          break;
                        }
                        case MaybeMultipleKeyframeType.KEYFRAME_AND_6_BYTES: {
                          // Security camera:
                          // 22   03 8F E0 B7 CB BA            3B CB F6 3D 
                          // TODO: Add support for this. Is this easing?
                          bnkEr.Position += 6;
                          value = bnkEr.ReadSingle();
                          break;
                        }
                        case MaybeMultipleKeyframeType.KEYFRAME_AND_9_BYTES: {
                          // TODO: Handle easing?
                          // Security camera:
                          // 23   03 39 63 B4 6C 92 37 92 AC   3B 50 1D BE 
                          bnkEr.Position += 9;
                          value = bnkEr.ReadSingle();
                          break;
                        }
                        case MaybeMultipleKeyframeType
                            .KEYFRAME_AND_BYTE_GRADIENT: {
                          // (Occurs in a Plasma Cutter animation)
                          // TODO: Add support, looks like a float followed by a smooth gradient of bytes
                          // Baby:
                          /**
                           * 76
                           * 06 53 42
                           * BC C5 A1 38
                           * 99 99 98 97 95 93 92 91 91 94 9A A3 AD B8 C2 CA
                           * D1 D5 D6 D4 CD C3 B6 A9 9B 8E 82 78 6F 6B 69 6B
                           * 6F 75 7C 82 88 8D 8F 8F 8D 88 81 79 6F 65 5A 4F
                           * 46 3E 36 2C 20 15 0B 03 00 01 09 19 33 56 7B 9C
                           * B8 CE DF EC F6 FB FE FE FE FD FA F7 F3 EF EA E5
                           * DF D9 D4 CE C9 C3 BF BC B9 BA BF C6 CE D5 DB DE
                           * DE DC D8 D3 CD C5 BD
                           */
                          /**
                           * 76
                           * 05 75 85
                           * BC BA 9C 38
                           * FF F5 EB E0 D6 CB C2 B8 AF A4 99 8D 81 75 68 5B
                           * 4E 42 36 2B 20 17 0F 08 03 00 00 01 04 09 0F 17
                           * 1F 27 30 38 41 49 50 56 5B 5F 63 65 68 6A 6C 6D
                           * 6F 70 71 72 73 74 75 76 77 78 79 7A 7A 79 79 7A
                           * 7B 7D 80 85 8C 94 9D A6 AF B8 C1 C8 CE D3 D7 DA
                           * DC DE E0 E0 E0 DF DD
                           */
                          // Plasma cutter:
                          /**
                           * D6
                           * 00 DD FE
                           * BB DD FF 37
                           * FF FF A4 00 00 00 00 00 64 C4 FF C8 FF BC 00
                           */
                          /**
                           * D6
                           * 00 00 00
                           * 00 58 CD 36
                           * 00 00 A0 7B 7B 7B 7B 7B FE B1 00 A8 00 BC 00
                           */
                          throw new NotImplementedException();
                        }
                        case MaybeMultipleKeyframeType.COMMAND_7: {
                          // TODO: Fix this, probably not right
                          throw new NotImplementedException();
                        }
                        default: throw new NotImplementedException();
                      }

                      setKeyframe(frame, value);

                      frame += length;

                      totalFrames = Math.Max(totalFrames, frame);
                    }
                  } else {
                    throw new NotImplementedException();
                  }
                }
              }
            }
          }
        }

        finAnimation.FrameRate = 60;
        finAnimation.FrameCount = totalFrames;
      }
    }
  }
}