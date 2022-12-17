using fin.animation.playback;
using fin.gl;
using fin.gl.model;
using fin.io.bundles;
using fin.model;
using System.Collections.Generic;


namespace fin.scene {
  public interface ISceneFileBundle : IFileBundle { }

  public interface ISceneLoader<in TSceneFileBundle>
      where TSceneFileBundle : ISceneFileBundle {
    IScene LoadScene(TSceneFileBundle sceneFileBundle);
  }

  /// <summary>
  ///   A single scene from a game. These can be thought of as the parts of the
  ///   game that are each separated by a loading screen.
  /// </summary>
  public interface IScene : ITickable, IRenderable {
    IReadOnlyList<ISceneArea> Areas { get; }
    ISceneArea AddArea();
  }

  /// <summary>
  ///   A single area in a scene. This is used to split out the different
  ///   regions into separate pieces that can be loaded separately; for
  ///   example, in Ocarina of Time, this is used to represent a single room in
  ///   a dungeon.
  /// </summary>
  public interface ISceneArea : ITickable, IRenderable {
    IReadOnlyList<ISceneObject> Objects { get; }
    ISceneObject AddObject();
  }

  /// <summary>
  ///   An instance of an object in a scene. This can be used for anything that
  ///   appears in the scene, such as the level geometry, scenery, or
  ///   characters.
  /// </summary>
  public interface ISceneObject : ITickable, IRenderable {
    IPosition Position { get; }
    IRotation Rotation { get; }

    ISceneObject SetPosition(IPosition position);
    ISceneObject SetRotation(IRotation rotation);

    public delegate void OnTick(ISceneObject self);
    ISceneObject SetOnTickHandler(OnTick handler);

    IReadOnlyList<ISceneModel> Models { get; }
    ISceneModel AddSceneModel(IModel model);
  }

  /// <summary>
  ///   An instance of a model rendered in a scene. This will automatically
  ///   take care of rendering animations, and also supports adding sub-models
  ///   onto bones.
  /// </summary>
  public interface ISceneModel : IRenderable {
    IReadOnlyList<ISceneModel> Children { get; }
    ISceneModel AddModelOntoBone(IModel model, IBone bone);

    IModel Model { get; }
    IModelRenderer ModelRenderer { get; }

    IAnimation? Animation { get; set; }
    IAnimationPlaybackManager AnimationPlaybackManager { get; }

    bool ShowSkeleton { get; set; }
    ISkeletonRenderer SkeletonRenderer { get; }
  }
}