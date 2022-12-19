using OpenTK;

using Quad64.src.LevelInfo;
using sm64.scripts;
using sm64.scripts.geo;


namespace Quad64.src.Scripts {
  class GeoScriptsV2 {
    private static GeoScriptNode rootNode;
    private static GeoScriptNode nodeCurrent;

    private static Vector3 getTotalOffset() {
      Vector3 newOffset = Vector3.Zero;
      GeoScriptNode n = nodeCurrent;
      while (n.parent != null) {
        newOffset += n.offset;
        n = n.parent;
      }
      return newOffset;
    }

    public static void resetNodes() {
      rootNode = new GeoScriptNode();
      nodeCurrent = rootNode;
    }

    public static void parse(Model3DLods mdlLods,
                             ref Level lvl,
                             byte seg,
                             uint off,
                             byte? areaID) {
      var commandList =
          new GeoScriptParser().Parse(GeoScriptParser.MergeAddress(seg, off),
                                      areaID);
      if (commandList == null) {
        return;
      }

      Add_(mdlLods, lvl, commandList);
    }

    private static void Add_(Model3DLods mdlLods,
                             Level lvl,
                             IGeoCommandList commandList) {
      foreach (var command in commandList.Commands) {
        switch (command) {
          case GeoBackgroundCommand geoBackgroundCommand: break;
          case GeoBillboardCommand geoBillboardCommand: break;
          case GeoBranchAndStoreCommand geoBranchAndStoreCommand: {
            Add_(mdlLods, lvl, commandList);
            break;
          }
          case GeoBranchCommand geoBranchCommand: {
            Add_(mdlLods, lvl, commandList);
            break;
          }
          case GeoCameraFrustumCommand geoCameraFrustumCommand: break;
          case GeoCameraLookAtCommand geoCameraLookAtCommand: break;
          case GeoCloseNodeCommand geoCloseNodeCommand: break;
          case GeoCullingRadiusCommand geoCullingRadiusCommand: break;
          case GeoDisplayListCommand geoDisplayListCommand: break;
          case GeoDisplayListFromAsm geoDisplayListFromAsm: break;
          case GeoDisplayListWithOffsetCommand geoDisplayListWithOffsetCommand: break;
          case GeoHeldObjectCommand geoHeldObjectCommand: break;
          case GeoNoopCommand geoNoopCommand: break;
          case GeoObjectListCommand geoObjectListCommand: break;
          case GeoOpenNodeCommand geoOpenNodeCommand: {
            GeoScriptNode newNode = new GeoScriptNode();
            newNode.ID = nodeCurrent.ID + 1;
            newNode.parent = nodeCurrent;
            nodeCurrent = newNode;
            break;
          }
          case GeoOrthoMatrixCommand geoOrthoMatrixCommand: break;
          case GeoReturnFromBranchCommand geoReturnFromBranchCommand: break;
          case GeoRotationCommand geoRotationCommand: break;
          case GeoScaleCommand geoScaleCommand: {
            mdlLods.Current.builder.currentScale =
                geoScaleCommand.Scale / 65536.0f;
            break;
          }
          case GeoSetRenderRangeCommand geoSetRenderRangeCommand: break;
          case GeoShadowCommand geoShadowCommand: break;
          case GeoStartLayoutCommand geoStartLayoutCommand: break;
          case GeoSwitchCommand geoSwitchCommand: break;
          case GeoTerminateCommand geoTerminateCommand: break;
          case GeoToggleDepthBufferCommand geoToggleDepthBufferCommand: break;
          case GeoTranslateAndRotateCommand geoTranslateAndRotateCommand: break;
          case GeoTranslationCommand geoTranslationCommand: break;
          case GeoViewportCommand geoViewportCommand: break;
          default: throw new ArgumentOutOfRangeException(nameof(command));
        }
      }
    }
  }
}