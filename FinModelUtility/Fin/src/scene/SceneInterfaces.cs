using fin.model;
using System.Collections.Generic;


namespace fin.scene {
  /// <summary>
  ///   A single scene from a game. These can be thought of as the parts of the
  ///   game that are each separated by a loading screen.
  /// </summary>
  public interface IScene {
    IReadOnlyList<ISceneArea> Areas { get; }
    ISceneArea AddArea();
  }

  /// <summary>
  ///   A single area in a scene. This is used to split out the different
  ///   regions into separate pieces that can be loaded separately; for
  ///   example, in Ocarina of Time, this is used to represent a single room in
  ///   a dungeon.
  /// </summary>
  public interface ISceneArea {
    IReadOnlyList<ISceneObject> Objects { get; }
    ISceneObject AddObject();
  }

  /// <summary>
  ///   An instance of an object in a scene. This can be used for anything that
  ///   appears in the scene, such as the level geometry, scenery, or
  ///   characters.
  /// </summary>
  public interface ISceneObject {
    ISceneObject SetPosition(IPosition position);
    ISceneObject SetRotation(IRotation rotation);

    ISceneModel AddSceneModel(IModel model);


    public delegate void OnTick(ISceneObject self);

    event OnTick Tick;


    void Render();
  }

  /// <summary>
  ///   An instance of a model rendered in a scene. This will automatically
  ///   take care of rendering animations, and also supports adding sub-models
  ///   onto bones.
  /// </summary>
  public interface ISceneModel {
    IModel Model { get; }
    IAnimation? Animation { get; set; }
    ISceneModel AddModelOntoBone(IModel model, IBone bone);
  }
}