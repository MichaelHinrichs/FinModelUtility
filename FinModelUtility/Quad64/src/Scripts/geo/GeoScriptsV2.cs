using fin.math.matrix;
using fin.model.impl;
using Quad64.src.LevelInfo;
using sm64.scripts;
using sm64.scripts.geo;


namespace Quad64.src.Scripts {
  public class GeoScriptsV2 : IGeoScripts {
    private GeoScriptNode rootNode;
    private GeoScriptNode nodeCurrent;

    public GeoScriptsV2() {
      rootNode = new GeoScriptNode(null);
      nodeCurrent = rootNode;
    }

    public void parse(Model3DLods mdlLods,
                      ref Level lvl,
                      byte seg,
                      uint off,
                      byte? areaID) {
      var commandList =
          new GeoScriptParser().Parse(GeoUtils.MergeAddress(seg, off),
                                      areaID);
      if (commandList == null) {
        return;
      }

      mdlLods.Current.Node = nodeCurrent;

      Add_(mdlLods, lvl, areaID, commandList);
    }

    private void Add_(Model3DLods mdlLods,
                      Level lvl,
                      byte? areaID,
                      IGeoCommandList commandList) {
      foreach (var command in commandList.Commands) {
        switch (command) {
          case GeoAnimatedPartCommand geoAnimatedPartCommand: {
            var translation = geoAnimatedPartCommand.Offset;
            this.nodeCurrent.matrix.MultiplyInPlace(
                MatrixTransformUtil.FromTranslation(
                    translation.X, translation.Y, translation.Z));
            AddDisplayList(
                mdlLods,
                lvl,
                geoAnimatedPartCommand.DisplayListSegmentedAddress,
                areaID);
            break;
          }
          case GeoBillboardCommand geoBillboardCommand: break;
          case GeoBranchAndStoreCommand geoBranchAndStoreCommand: {
            if (geoBranchAndStoreCommand.GeoCommandList != null) {
              var currentNode = nodeCurrent;
              Add_(mdlLods, lvl, areaID,
                   geoBranchAndStoreCommand.GeoCommandList);
              mdlLods.Current.Node = currentNode;
            }
            break;
          }
          case GeoBranchCommand geoBranchCommand: {
            if (geoBranchCommand.GeoCommandList != null) {
              var currentNode = nodeCurrent;
              Add_(mdlLods, lvl, areaID, geoBranchCommand.GeoCommandList);
              if (geoBranchCommand.StoreReturnAddress) {
                mdlLods.Current.Node = currentNode;
              }
            }
            break;
          }
          case GeoCloseNodeCommand geoCloseNodeCommand: {
            if (nodeCurrent != rootNode) {
              nodeCurrent = nodeCurrent.parent;
              mdlLods.Current.Node = nodeCurrent;
            }
            break;
          }
          case GeoDisplayListCommand geoDisplayListCommand: {
            AddDisplayList(
                mdlLods,
                lvl,
                geoDisplayListCommand.DisplayListSegmentedAddress,
                areaID);
            break;
          }
          case GeoDisplayListFromAsm geoDisplayListFromAsm: break;
          case GeoHeldObjectCommand geoHeldObjectCommand:   break;
          case GeoObjectListCommand geoObjectListCommand:   break;
          case GeoOpenNodeCommand geoOpenNodeCommand: {
            GeoScriptNode newNode = new GeoScriptNode(nodeCurrent);
            newNode.ID = nodeCurrent.ID + 1;
            newNode.parent = nodeCurrent;
            nodeCurrent = newNode;
            mdlLods.Current.Node = nodeCurrent;
            break;
          }
          case GeoRotationCommand geoRotationCommand: {
            var rotation = geoRotationCommand.Rotation;
            this.nodeCurrent.matrix.MultiplyInPlace(
                MatrixTransformUtil.FromRotation(
                    new ModelImpl.RotationImpl().SetDegrees(
                        rotation.X, rotation.Y, rotation.Z)));
            AddDisplayList(
                mdlLods,
                lvl,
                geoRotationCommand.DisplayListSegmentedAddress,
                areaID);
            break;
          }
          case GeoScaleCommand geoScaleCommand: {
            var scale = (geoScaleCommand.Scale / 65536.0f);
            this.nodeCurrent.matrix.MultiplyInPlace(
                MatrixTransformUtil.FromScale(
                    new ModelImpl.ScaleImpl {X = scale, Y = scale, Z = scale}));
            AddDisplayList(
                mdlLods,
                lvl,
                geoScaleCommand.DisplayListSegmentedAddress,
                areaID);
            break;
          }
          case GeoSetRenderRangeCommand geoSetRenderRangeCommand: {
            mdlLods.Add(nodeCurrent!);
            break;
          }
          case GeoShadowCommand geoShadowCommand: break;
          case GeoSwitchCommand geoSwitchCommand: break;
          case GeoTranslateAndRotateCommand geoTranslateAndRotateCommand: {
            var translation = geoTranslateAndRotateCommand.Translation;
            var rotation = geoTranslateAndRotateCommand.Rotation;
            this.nodeCurrent.matrix.MultiplyInPlace(
                MatrixTransformUtil.FromTrs(
                    new ModelImpl.PositionImpl {
                        X = translation.X, Y = translation.Y, Z = translation.Z
                    },
                    new ModelImpl.RotationImpl().SetDegrees(
                        rotation.X, rotation.Y, rotation.Z),
                    null));
            AddDisplayList(
                mdlLods,
                lvl,
                geoTranslateAndRotateCommand.DisplayListSegmentedAddress,
                areaID);
            break;
          }
          case GeoTranslationCommand geoTranslationCommand: {
            var translation = geoTranslationCommand.Translation;
            this.nodeCurrent.matrix.MultiplyInPlace(
                MatrixTransformUtil.FromTranslation(
                    translation.X, translation.Y, translation.Z));
            AddDisplayList(
                mdlLods,
                lvl,
                geoTranslationCommand.DisplayListSegmentedAddress,
                areaID);
            break;
          }
          case GeoBackgroundCommand geoBackgroundCommand: break;
          case GeoCameraFrustumCommand geoCameraFrustumCommand: break;
          case GeoCameraLookAtCommand geoCameraLookAtCommand: break;
          case GeoCullingRadiusCommand geoCullingRadiusCommand: break;
          case GeoNoopCommand geoNoopCommand: break;
          case GeoOrthoMatrixCommand geoOrthoMatrixCommand: break;
          case GeoReturnFromBranchCommand geoReturnFromBranchCommand: break;
          case GeoStartLayoutCommand geoStartLayoutCommand: break;
          case GeoTerminateCommand geoTerminateCommand: break;
          case GeoToggleDepthBufferCommand geoToggleDepthBufferCommand: break;
          case GeoViewportCommand geoViewportCommand: break;
          default: throw new ArgumentOutOfRangeException(nameof(command));
        }
      }
    }

    public void AddDisplayList(Model3DLods mdlLods,
                               Level lvl,
                               uint? displayListAddress,
                               byte? areaID) {
      var mdl = mdlLods.Current;

      // Don't bother processing duplicate display lists.
      if ((displayListAddress ?? 0) != 0) {
        GeoUtils.SplitAddress(displayListAddress!.Value,
                              out var seg,
                              out var off);

        if (!mdl.hasGeoDisplayList(displayListAddress!.Value)) {
          Fast3DScripts.parse(ref mdl, ref lvl, seg, off, areaID, 0);
        }
        lvl.temp_bgInfo.usesFog = mdl.builder.UsesFog;
        lvl.temp_bgInfo.fogColor = mdl.builder.FogColor;
        lvl.temp_bgInfo.fogColor_romLocation = mdl.builder.FogColor_romLocation;
      }
    }
  }
}