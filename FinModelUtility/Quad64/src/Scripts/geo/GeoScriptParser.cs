using Quad64;
using schema.util;


namespace sm64.scripts.geo {
  public class GeoScriptParser {
    private class GeoCommandList : IGeoCommandList {
      private readonly List<IGeoCommand> commands_ = new();

      public void AddCommand(IGeoCommand command)
        => this.commands_.Add(command);

      public IReadOnlyList<IGeoCommand> Commands => this.commands_;
    }

    public enum ReturnType {
      UNDEFINED,
      TERMINATED,
      RETURNED
    }

    public IGeoCommandList Parse(uint address, byte? areaId)
      => Asserts.CastNonnull(ParseImpl_(address, areaId).Value).Item1;

    private (IGeoCommandList, ReturnType)?
        ParseImpl_(uint address, byte? areaId) {
      GeoUtils.SplitAddress(address, out var seg, out var off);

      ROM rom = ROM.Instance;
      byte[] data = rom.getSegment(seg, areaId)!;

      if (data == null) {
        return null;
      }

      if (off >= data.Length) {
        return null;
      }


      {
        var d = new byte[] {1, 2, 3, 4};

        var r = new EndianBinaryReader(d, GeoUtils.Endianness);

        var adr = r.ReadUInt32();

        GeoUtils.SplitAddress(adr, out var sgm, out var ofst);

        ;
      }


      using var er = new EndianBinaryReader(data, GeoUtils.Endianness);
      er.Position = off;

      var commands = new GeoCommandList();

      var returnType = ReturnType.UNDEFINED;
      while (returnType == ReturnType.UNDEFINED) {
        var cmdIdByte = er.ReadByte();
        var cmdId = (GeoCommandId)cmdIdByte;
        er.Position--;

        var startPos = er.Position;
        var expectedLen = GetCmdLength_(cmdIdByte);

        IGeoCommand? command = null;

        switch (cmdId) {
          case GeoCommandId.BRANCH_AND_STORE: {
            var branchAndStoreCommand = er.ReadNew<GeoBranchAndStoreCommand>();
            command = branchAndStoreCommand;

            ReturnType branchReturnType = ReturnType.UNDEFINED;
            var commandListAndReturnValue = this.ParseImpl_(
                branchAndStoreCommand.GeoCommandSegmentedAddress, areaId);
            if (commandListAndReturnValue != null) {
              (branchAndStoreCommand.GeoCommandList, branchReturnType) =
                  commandListAndReturnValue.Value;
            }

            if (branchReturnType == ReturnType.TERMINATED) {
              returnType = ReturnType.TERMINATED;
            }
            break;
          }
          case GeoCommandId.TERMINATE: {
            command = er.ReadNew<GeoTerminateCommand>();
            returnType = ReturnType.TERMINATED;
            break;
          }
          case GeoCommandId.BRANCH: {
            var branchCommand = er.ReadNew<GeoBranchCommand>();
            command = branchCommand;

            ReturnType branchReturnType = ReturnType.UNDEFINED;
            var commandListAndReturnValue =
                this.ParseImpl_(branchCommand.GeoCommandSegmentedAddress,
                                areaId);

            if (commandListAndReturnValue != null) {
              (branchCommand.GeoCommandList, branchReturnType) =
                  commandListAndReturnValue.Value;
            }

            if (branchReturnType == ReturnType.TERMINATED) {
              returnType = ReturnType.TERMINATED;
            }

            if (!branchCommand.StoreReturnAddress) {
              returnType = ReturnType.RETURNED;
            }
            break;
          }
          case GeoCommandId.RETURN_FROM_BRANCH: {
            command = er.ReadNew<GeoReturnFromBranchCommand>();
            returnType = ReturnType.RETURNED;
            break;
          }
          case GeoCommandId.OPEN_NODE: {
            command = er.ReadNew<GeoOpenNodeCommand>();
            break;
          }
          case GeoCommandId.CLOSE_NODE: {
            command = er.ReadNew<GeoCloseNodeCommand>();
            break;
          }
          case GeoCommandId.VIEWPORT: {
            command = er.ReadNew<GeoViewportCommand>();
            break;
          }
          case GeoCommandId.ORTHO_MATRIX: {
            command = er.ReadNew<GeoOrthoMatrixCommand>();
            break;
          }
          case GeoCommandId.CAMERA_FRUSTUM: {
            command = er.ReadNew<GeoCameraFrustumCommand>();
            break;
          }
          case GeoCommandId.START_LAYOUT: {
            command = er.ReadNew<GeoStartLayoutCommand>();
            break;
          }
          case GeoCommandId.TOGGLE_DEPTH_BUFFER: {
            command = er.ReadNew<GeoToggleDepthBufferCommand>();
            break;
          }
          case GeoCommandId.SET_RENDER_RANGE: {
            command = er.ReadNew<GeoSetRenderRangeCommand>();
            break;
          }
          case GeoCommandId.SWITCH: {
            command = er.ReadNew<GeoSwitchCommand>();
            // TODO: How does getting cases work??
            break;
          }
          case GeoCommandId.CAMERA_LOOK_AT: {
            command = er.ReadNew<GeoCameraLookAtCommand>();
            break;
          }
          case GeoCommandId.TRANSLATE_AND_ROTATE: {
            var translateAndRotateCommand = new GeoTranslateAndRotateCommand();
            er.ReadByte();
            translateAndRotateCommand.Params = er.ReadByte();

            switch (translateAndRotateCommand.Format) {
              case GeoTranslateAndRotateFormat.TRANSLATION_AND_ROTATION: {
                er.ReadUInt16();
                translateAndRotateCommand.Translation.Read(er);
                translateAndRotateCommand.Rotation.Read(er);
                break;
              }
              case GeoTranslateAndRotateFormat.TRANSLATION: {
                translateAndRotateCommand.Translation.Read(er);
                break;
              }
              case GeoTranslateAndRotateFormat.ROTATION: {
                translateAndRotateCommand.Rotation.Read(er);
                break;
              }
              case GeoTranslateAndRotateFormat.YAW: {
                translateAndRotateCommand.Rotation.Y = er.ReadInt16();
                break;
              }
              default: throw new ArgumentOutOfRangeException();
            }

            if (translateAndRotateCommand.HasDisplayList) {
              translateAndRotateCommand.DisplayListSegmentedAddress =
                  er.ReadUInt32();
            }

            command = translateAndRotateCommand;
            break;
          }
          case GeoCommandId.TRANSLATE: {
            command = er.ReadNew<GeoTranslationCommand>();
            break;
          }
          case GeoCommandId.ROTATE: {
            command = er.ReadNew<GeoRotationCommand>();
            break;
          }
          case GeoCommandId.ANIMATED_PART: {
            command = er.ReadNew<GeoAnimatedPartCommand>();
            break;
          }
          case GeoCommandId.BILLBOARD: {
            command = er.ReadNew<GeoBillboardCommand>();
            break;
          }
          case GeoCommandId.DISPLAY_LIST: {
            command = er.ReadNew<GeoDisplayListCommand>();
            break;
          }
          case GeoCommandId.SHADOW: {
            command = er.ReadNew<GeoShadowCommand>();
            break;
          }
          case GeoCommandId.OBJECT_LIST: {
            command = er.ReadNew<GeoObjectListCommand>();
            break;
          }
          case GeoCommandId.DISPLAY_LIST_FROM_ASM: {
            command = er.ReadNew<GeoDisplayListFromAsm>();
            break;
          }
          case GeoCommandId.BACKGROUND: {
            command = er.ReadNew<GeoBackgroundCommand>();
            break;
          }
          case GeoCommandId.NOOP_1A: {
            command = er.ReadNew<GeoNoopCommand>();
            break;
          }
          case GeoCommandId.HELD_OBJECT: {
            command = er.ReadNew<GeoHeldObjectCommand>();
            break;
          }
          case GeoCommandId.SCALE: {
            command = er.ReadNew<GeoScaleCommand>();
            break;
          }
          case GeoCommandId.CULLING_RADIUS: {
            command = er.ReadNew<GeoCullingRadiusCommand>();
            break;
          }
          default: {
            throw new NotImplementedException();
          }
        }

        var actualLen = er.Position - startPos;
        if (expectedLen != actualLen) {
          var translateAndRotateCommand =
              command as GeoTranslateAndRotateCommand;
          if (translateAndRotateCommand == null) {
            Asserts.Fail();
          }
        }

        commands.AddCommand(Asserts.CastNonnull(command));
      }

      return (commands, returnType);
    }

    private static byte GetCmdLength_(byte cmd) {
      switch (cmd) {
        case 0x00:
        case 0x02:
        case 0x0D:
        case 0x0E:
        case 0x11:
        case 0x12:
        case 0x14:
        case 0x15:
        case 0x16:
        case 0x18:
        case 0x19:
        case 0x1A:
        case 0x1D:
        case 0x1E:
          return 0x08;
        case 0x08:
        case 0x0A:
        case 0x13:
        case 0x1C:
          return 0x0C;
        case 0x1F:
          return 0x10;
        case 0x0F:
        case 0x10:
          return 0x14;
        default:
          return 0x04;
      }
    }
  }
}