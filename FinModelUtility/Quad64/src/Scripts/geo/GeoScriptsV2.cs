using fin.math.matrix;
using fin.model;
using fin.model.impl;
using fin.schema.vector;

using Quad64.memory;
using Quad64.Scripts;
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

    public void parse(
        IReadOnlySm64Memory n64Memory,
        Model3DLods mdlLods,
        ref Level lvl,
        byte seg,
        uint off) {
      var commandList =
          new GeoScriptParser().Parse(GeoUtils.MergeAddress(seg, off),
                                      n64Memory.AreaId);
      if (commandList == null) {
        return;
      }

      mdlLods.Current.Node = nodeCurrent;

      Add_(n64Memory, mdlLods, lvl, commandList);
    }

    private void Add_(
        IReadOnlySm64Memory n64Memory,
        Model3DLods mdlLods,
        Level lvl,
        IGeoCommandList commandList) {
      foreach (var command in commandList.Commands) {
        switch (command) {
          case GeoAnimatedPartCommand geoAnimatedPartCommand: {
            var translation = geoAnimatedPartCommand.Offset;
            this.nodeCurrent.matrix.MultiplyInPlace(
                CreateTranslationMatrix_(translation));
            AddDisplayList(
                n64Memory,
                mdlLods,
                lvl,
                geoAnimatedPartCommand.DisplayListSegmentedAddress);
            break;
          }
          case GeoBillboardCommand geoBillboardCommand: break;
          case GeoBranchAndStoreCommand geoBranchAndStoreCommand: {
            if (geoBranchAndStoreCommand.GeoCommandList != null) {
              var currentNode = nodeCurrent;
              Add_(n64Memory, mdlLods, lvl,
                   geoBranchAndStoreCommand.GeoCommandList);
              mdlLods.Current.Node = currentNode;
            }
            break;
          }
          case GeoBranchCommand geoBranchCommand: {
            if (geoBranchCommand.GeoCommandList != null) {
              var currentNode = nodeCurrent;
              Add_(n64Memory, mdlLods, lvl, geoBranchCommand.GeoCommandList);
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
                n64Memory,
                mdlLods,
                lvl,
                geoDisplayListCommand.DisplayListSegmentedAddress);
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
                CreateRotationMatrix_(rotation));
            AddDisplayList(
                n64Memory,
                mdlLods,
                lvl,
                geoRotationCommand.DisplayListSegmentedAddress);
            break;
          }
          case GeoScaleCommand geoScaleCommand: {
            var scale = (geoScaleCommand.Scale / 65536.0f);
            this.nodeCurrent.matrix.MultiplyInPlace(
                MatrixTransformUtil.FromScale(new Scale (scale)));
            AddDisplayList(
                n64Memory,
                mdlLods,
                lvl,
                geoScaleCommand.DisplayListSegmentedAddress);
            break;
          }
          case GeoSetRenderRangeCommand geoSetRenderRangeCommand: {
            mdlLods.AddLod(nodeCurrent!);
            break;
          }
          case GeoShadowCommand geoShadowCommand: break;
          case GeoSwitchCommand geoSwitchCommand: break;
          case GeoTranslateAndRotateCommand geoTranslateAndRotateCommand: {
            var translation = geoTranslateAndRotateCommand.Translation;
            var rotation = geoTranslateAndRotateCommand.Rotation;
            this.nodeCurrent.matrix.MultiplyInPlace(
                CreateTranslationAndRotationMatrix_(translation, rotation));
            AddDisplayList(
                n64Memory,
                mdlLods,
                lvl,
                geoTranslateAndRotateCommand.DisplayListSegmentedAddress);
            break;
          }
          case GeoTranslationCommand geoTranslationCommand: {
            var translation = geoTranslationCommand.Translation;
            this.nodeCurrent.matrix.MultiplyInPlace(
                CreateTranslationMatrix_(translation));
            AddDisplayList(
                n64Memory,
                mdlLods,
                lvl,
                geoTranslationCommand.DisplayListSegmentedAddress);
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

    public IFinMatrix4x4 CreateTranslationAndRotationMatrix_(
        Vector3s position,
        Vector3s rotation)
      => CreateRotationMatrix_(rotation)
          .MultiplyInPlace(CreateTranslationMatrix_(position));

    public IFinMatrix4x4 CreateTranslationMatrix_(Vector3s position)
      => MatrixTransformUtil.FromTranslation(new Position(position.X, position.Y, position.Z));

    public IFinMatrix4x4 CreateRotationMatrix_(Vector3s rotation)
      => MatrixTransformUtil
         .FromRotation(
             new ModelImpl.RotationImpl().SetDegrees(0, 0, rotation.Z))
         .MultiplyInPlace(
             MatrixTransformUtil.FromRotation(
                 new ModelImpl.RotationImpl().SetDegrees(rotation.X, 0, 0)))
         .MultiplyInPlace(
             MatrixTransformUtil.FromRotation(
                 new ModelImpl.RotationImpl().SetDegrees(0, rotation.Y, 0)));

    public void AddDisplayList(
        IReadOnlySm64Memory n64Memory,
        Model3DLods mdlLods,
        Level lvl,
        uint? displayListAddress) {
      var mdl = mdlLods.Current;

      // Don't bother processing duplicate display lists.
      if ((displayListAddress ?? 0) != 0) {
        if (!mdl.hasGeoDisplayList(displayListAddress!.Value)) {
          mdlLods.AddDl(n64Memory, displayListAddress.Value);
        }
        lvl.temp_bgInfo.usesFog = mdl.builder.UsesFog;
        lvl.temp_bgInfo.fogColor = mdl.builder.FogColor;
        lvl.temp_bgInfo.fogColor_romLocation = mdl.builder.FogColor_romLocation;
      }
    }
  }
}