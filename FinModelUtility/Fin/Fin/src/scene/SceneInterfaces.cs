using System;
using System.Collections.Generic;
using System.Drawing;

using fin.animation;
using fin.io.bundles;
using fin.math;
using fin.model;

namespace fin.scene {
  public interface ISceneFileBundle : IFileBundle { }

  public interface ISceneImporter<in TSceneFileBundle>
      where TSceneFileBundle : ISceneFileBundle {
    IScene ImportScene(TSceneFileBundle sceneFileBundle);
  }

  /// <summary>
  ///   A single scene from a game. These can be thought of as the parts of the
  ///   game that are each separated by a loading screen.
  /// </summary>
  public interface IScene : ITickable, IDisposable {
    IReadOnlyList<ISceneArea> Areas { get; }
    ISceneArea AddArea();

    float ViewerScale { get; set; }
  }

  /// <summary>
  ///   A single area in a scene. This is used to split out the different
  ///   regions into separate pieces that can be loaded separately; for
  ///   example, in Ocarina of Time, this is used to represent a single room in
  ///   a dungeon.
  /// </summary>
  public interface ISceneArea : ITickable, IDisposable {
    IReadOnlyList<ISceneObject> Objects { get; }
    ISceneObject AddObject();

    float ViewerScale { get; set; }

    Color? BackgroundColor { get; set; }
    ISceneObject? CustomSkyboxObject { get; set; }
    ISceneObject CreateCustomSkyboxObject();
  }

  /// <summary>
  ///   An instance of an object in a scene. This can be used for anything that
  ///   appears in the scene, such as the level geometry, scenery, or
  ///   characters.
  /// </summary>
  public interface ISceneObject : ITickable, IDisposable {
    Position Position { get; }
    IRotation Rotation { get; }
    Scale Scale { get; }

    ISceneObject SetPosition(float x, float y, float z);
    ISceneObject SetRotationRadians(float xRadians, float yRadians, float zRadians);
    ISceneObject SetRotationDegrees(float xDegrees, float yDegrees, float zDegrees);
    ISceneObject SetScale(float x, float y, float z);

    public delegate void OnTick(ISceneObject self);

    ISceneObject SetOnTickHandler(OnTick handler);

    IReadOnlyList<ISceneModel> Models { get; }
    ISceneModel AddSceneModel(IModel model);

    float ViewerScale { get; set; }
  }

  /// <summary>
  ///   An instance of a model rendered in a scene. This will automatically
  ///   take care of rendering animations, and also supports adding sub-models
  ///   onto bones.
  /// </summary>
  public interface ISceneModel : IDisposable {
    IReadOnlyList<ISceneModel> Children { get; }
    ISceneModel AddModelOntoBone(IModel model, IBone bone);

    IModel Model { get; }

    IBoneTransformManager BoneTransformManager { get; }

    IModelAnimation? Animation { get; set; }
    IAnimationPlaybackManager AnimationPlaybackManager { get; }

    float ViewerScale { get; set; }
  }

  public interface ITickable {
    void Tick();
  }
}