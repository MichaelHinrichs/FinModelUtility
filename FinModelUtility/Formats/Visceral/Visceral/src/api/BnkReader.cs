using System.Collections;

using fin.data.queues;
using fin.io;
using fin.math.floats;
using fin.model;

using schema.binary;

namespace visceral.api {
  public class BnkReader {
    public enum MaybeBoneType {
      ROOT = 0x2,
      PARENT = 0x5,
      ANIMATED = 0x16,
    }

    public enum KeyframeType : byte {
      ONLY_KEYFRAME = 0x0,
      KEYFRAME_AND_3_BYTES = 0x1,
      KEYFRAME_AND_6_BYTES = 0x2,
      KEYFRAME_AND_9_BYTES = 0x3,
      BYTE_GRADIENT = 0x6,
      SHORT_GRADIENT = 0x7,
      SINGLETON_0 = 0xC,
      SINGLETON_1 = 0xD,
    }

    public void ReadBnk(IModel model,
                        IReadOnlyGenericFile bnkFile,
                        IReadOnlyGenericFile? rcbFile,
                        IBone[] bones) {
      using var bnkBr =
          new SchemaBinaryReader(bnkFile.OpenRead(), Endianness.LittleEndian);

      bnkBr.Position = 0x24;
      var headerCount = bnkBr.ReadUInt32();
      var headerOffsetOffset = bnkBr.ReadUInt32();

      bnkBr.Position = headerOffsetOffset;
      var headerOffsets = bnkBr.ReadUInt32s(headerCount);

      foreach (var headerOffset in headerOffsets) {
        var finAnimation = model.AnimationManager.AddAnimation();
        finAnimation.FrameRate = 60;

        bnkBr.Position = headerOffset;

        if (GeoModelImporter.STRICT_DAT) {
          this.ReadIntoAnimation_(bnkBr, rcbFile, bones, finAnimation);
        } else {
          try {
            this.ReadIntoAnimation_(bnkBr, rcbFile, bones, finAnimation);
          } catch {
            model.AnimationManager.RemoveAnimation(finAnimation);
          }
        }
      }
    }

    private void ReadIntoAnimation_(IBinaryReader bnkBr,
                                    IReadOnlyGenericFile? rcbFile,
                                    IBone[] bones,
                                    IModelAnimation finAnimation) {
      {
        var animationNameOffset = bnkBr.ReadUInt32();
        finAnimation.Name =
            bnkBr.SubreadAt(animationNameOffset, ser => ser.ReadStringNT());
      }

      var totalFrames = 1;

      var commands = new List<ushort>();

      {
        var rootOffset = bnkBr.ReadUInt32();
        var maybeBoneOffsetQueue = new FinQueue<uint>(rootOffset);
        while (maybeBoneOffsetQueue.TryDequeue(out var maybeBoneOffset)) {
          bnkBr.Position = maybeBoneOffset;

          var boneType = (MaybeBoneType) bnkBr.ReadByte();
          var childCount = bnkBr.ReadByte();
          var maybeId = bnkBr.ReadByte();
          var unk1 = bnkBr.ReadByte();

          var someMetadataOffset = bnkBr.ReadUInt32();
          var unk2 = bnkBr.ReadUInt32();

          if (childCount > 0) {
            maybeBoneOffsetQueue.Enqueue(bnkBr.ReadUInt32s(childCount));
          }

          if (boneType == MaybeBoneType.ANIMATED) {
            var animationDataOffset = bnkBr.ReadUInt32();
            bnkBr.Position = animationDataOffset;

            var unkHash = bnkBr.ReadUInt32();
            var maybeVersion = bnkBr.ReadUInt32();
            var unk3 = bnkBr.ReadUInt32();
            var unk4 = bnkBr.ReadUInt32();
            var unk5 = bnkBr.ReadUInt32();

            var someHashFromRcb = bnkBr.ReadUInt32();
            var standaloneCommandPrefix = bnkBr.ReadUInt32();

            // These are unknown
            bnkBr.Position += 4 * 5;

            using var rcbEr =
                new SchemaBinaryReader(rcbFile.OpenRead(),
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
                    rcbEr.ReadBytes(
                        (int) Math.Ceiling(bitMaskLength / 8f)));

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

                var command = bnkBr.ReadUInt16();
                commands?.Add(command);

                var upper = command >> 4;
                var lower = command & 0xF;

                if (upper == standaloneCommandPrefix) {
                  var axisType = (KeyframeType) lower;

                  switch (axisType) {
                    case KeyframeType.SINGLETON_0: {
                      var value = 0;
                      setKeyframe(0, value);
                      break;
                    }
                    case KeyframeType.SINGLETON_1: {
                      var value = 1;
                      setKeyframe(0, value);
                      break;
                    }
                    case KeyframeType.ONLY_KEYFRAME: {
                      bnkBr.Position -= 1;
                      var value = bnkBr.ReadSingle();
                      setKeyframe(0, value);
                      break;
                    }
                    case KeyframeType.KEYFRAME_AND_3_BYTES: {
                      bnkBr.Position += 2;
                      var value = bnkBr.ReadSingle();
                      setKeyframe(0, value);
                      break;
                    }
                    case KeyframeType.KEYFRAME_AND_6_BYTES: {
                      bnkBr.Position += 5;
                      var value = bnkBr.ReadSingle();
                      setKeyframe(0, value);
                      break;
                    }
                    case KeyframeType.KEYFRAME_AND_9_BYTES: {
                      bnkBr.Position += 8;
                      var value = bnkBr.ReadSingle();
                      setKeyframe(0, value);
                      break;
                    }
                    case KeyframeType.BYTE_GRADIENT:
                    case KeyframeType.SHORT_GRADIENT: {
                      // TODO: What are these values?
                      var fromFraction = bnkBr.ReadUn8();
                      var toFraction = bnkBr.ReadUn8();

                      if (fromFraction.IsRoughly(0) &&
                          toFraction.IsRoughly(0)) {
                        fromFraction = 0;
                        toFraction = 1;
                      } else if (fromFraction.IsRoughly(toFraction)) {
                        ;
                      }

                      var frameCount = (int) standaloneCommandPrefix;
                      var fractions = axisType == KeyframeType.BYTE_GRADIENT
                          ? bnkBr.ReadUn8s(frameCount)
                          : bnkBr.ReadUn16s(frameCount);

                      var value = bnkBr.ReadSingle();
                      for (var f = 0; f < frameCount; ++f) {
                        var fraction = fractions[f];
                        var keyframeAmount = fromFraction * (1 - fraction) +
                                             toFraction * fraction;
                        var keyframe = keyframeAmount * value;

                        setKeyframe(f, keyframe);
                      }

                      totalFrames = Math.Max(totalFrames, frameCount);

                      break;
                    }
                    default:
                      throw new NotImplementedException();
                  }
                } else if (lower == 5) {
                  var keyframeCount = upper;

                  var frame = 0;
                  for (var k = 0; k < keyframeCount; ++k) {
                    var lengthAndKeyframeType = bnkBr.ReadByte();
                    var length = lengthAndKeyframeType >> 4;
                    var keyframeType =
                        (KeyframeType) (lengthAndKeyframeType & 0xF);

                    switch (keyframeType) {
                      case KeyframeType.SINGLETON_0: {
                        var value = 0;
                        setKeyframe(frame, value);
                        frame += length;
                        break;
                      }
                      case KeyframeType.SINGLETON_1: {
                        var value = 1;
                        setKeyframe(frame, value);
                        frame += length;
                        break;
                      }
                      case KeyframeType.ONLY_KEYFRAME: {
                        var value = bnkBr.ReadSingle();

                        setKeyframe(frame, value);
                        frame += length;

                        break;
                      }
                      case KeyframeType.KEYFRAME_AND_3_BYTES: {
                        // Security camera:
                        // 21   00 44 7C                     BB 44 7C 3B
                        // TODO: Add support for this. Is this easing?
                        bnkBr.Position += 3;
                        var value = bnkBr.ReadSingle();

                        setKeyframe(frame, value);
                        frame += length;

                        break;
                      }
                      case KeyframeType.KEYFRAME_AND_6_BYTES: {
                        // Security camera:
                        // 22   03 8F E0 B7 CB BA            3B CB F6 3D 
                        // TODO: Add support for this. Is this easing?
                        bnkBr.Position += 6;
                        var value = bnkBr.ReadSingle();

                        setKeyframe(frame, value);
                        frame += length;

                        break;
                      }
                      case KeyframeType.KEYFRAME_AND_9_BYTES: {
                        // TODO: Handle easing?
                        // Security camera:
                        // 23   03 39 63 B4 6C 92 37 92 AC   3B 50 1D BE 
                        bnkBr.Position += 9;
                        var value = bnkBr.ReadSingle();

                        setKeyframe(frame, value);
                        frame += length;

                        break;
                      }
                      case KeyframeType.BYTE_GRADIENT:
                      case KeyframeType.SHORT_GRADIENT: {
                        var rowCount = bnkBr.ReadByte();

                        // TOOD: Is this actually right???
                        var fromFraction = bnkBr.ReadUn8();
                        var toFraction = bnkBr.ReadUn8();

                        if (fromFraction.IsRoughly(0) &&
                            toFraction.IsRoughly(0)) {
                          fromFraction = 0;
                          toFraction = 1;
                        } else if (fromFraction.IsRoughly(toFraction)) {
                          ;
                        }

                        var value = bnkBr.ReadSingle();

                        // TODO: Is this right????
                        var frameCount = (rowCount << 4) | length;
                        var fractions =
                            keyframeType == KeyframeType.BYTE_GRADIENT
                                ? bnkBr.ReadUn8s(frameCount)
                                : bnkBr.ReadUn16s(frameCount);

                        foreach (var fraction in fractions) {
                          var keyframeAmount = fromFraction * (1 - fraction) +
                                               toFraction * fraction;
                          var keyframe = keyframeAmount * value;

                          setKeyframe(frame++, keyframe);
                        }

                        frame += fractions.Length;

                        break;
                      }
                      default: throw new NotImplementedException();
                    }

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

      finAnimation.FrameCount = totalFrames;
    }
  }
}