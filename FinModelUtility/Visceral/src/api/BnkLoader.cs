using System.Collections;

using fin.data.queue;
using fin.io;
using fin.model;


namespace visceral.api {
  public class BnkLoader {
    public enum MaybeBoneType {
      ROOT = 0x2,
      PARENT = 0x5,
      ANIMATED = 0x16,
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
          finAnimation.Name = bnkEr.ReadStringNTAtOffset(animationNameOffset);
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
              var singleFrameCommandPrefix = bnkEr.ReadUInt32();

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
                  if (command >> 4 == singleFrameCommandPrefix) {
                    var singleCommandType = command & 0xF;

                    float value;
                    if (singleCommandType == 0xC) {
                      value = 0;
                    } else if (singleCommandType == 0xD) {
                      value = 1;
                    } else if (singleCommandType == 0x0) {
                      bnkEr.Position -= 1;
                      value = bnkEr.ReadSingle();
                    } else {
                      throw new NotImplementedException();
                    }

                    setKeyframe(0, value);
                  } else if ((command & 0xF) == 5) {
                    var keyframeCount = command >> 4;

                    var frame = 0;
                    for (var k = 0; k < keyframeCount; ++k) {
                      var lengthAndKeyframeType = bnkEr.ReadByte();
                      var length = lengthAndKeyframeType >> 4;
                      var keyframeType = lengthAndKeyframeType & 0xF;

                      float value;
                      switch (keyframeType) {
                        case 0: {
                          value = bnkEr.ReadSingle();
                          break;
                        }
                        case 1: {
                          bnkEr.Position += 3;
                          value = bnkEr.ReadSingle();
                          break;
                        }
                        case 2: {
                          bnkEr.Position += 6;
                          value = bnkEr.ReadSingle();
                          break;
                        }
                        case 3: {
                          bnkEr.Position += 9;
                          value = bnkEr.ReadSingle();
                          break;
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