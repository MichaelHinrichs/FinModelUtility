using OpenTK;


namespace level5 {
  public enum AnimNodeHashType {
    Name = 0,
    CRC32C = 1,
  }

  /// <summary>
  /// Stores animation data for transformations
  /// Information is stored in tracks <see cref="GenericTransformTrack"/>
  /// </summary>
  public class GenericAnimationTransform {
    public string Name { get; set; }

    public uint Hash { get; set; }

    public AnimNodeHashType HashType { get; set; } = AnimNodeHashType.Name;

    public List<GenericTransformTrack> Tracks { get; } =
      new List<GenericTransformTrack>();

    /// <summary>
    /// adds a new key frame to the animation
    /// </summary>
    /// <param name="frame"></param>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <param name="interpolationType"></param>
    public void AddKey(float frame,
                       float value,
                       AnimationTrackFormat type,
                       InterpolationType interpolationType =
                           InterpolationType.Linear) {
      var track = Tracks.Find(e => e.Type == type);
      if (track == null) {
        track = new GenericTransformTrack(type);
        Tracks.Add(track);
      }
      track.AddKey(frame, value, interpolationType);
    }

    /// <summary>
    /// gets the interpolated values for track type at frame
    /// </summary>
    /// <param name="frame"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public float GetTrackValueAt(float frame, AnimationTrackFormat type) {
      var track = Tracks.Find(e => e.Type == type);

      if (track == null)
        return 0;

      return track.Keys.GetValue(frame);
    }
  }


  public enum AnimationTrackFormat {
    TranslateX,
    TranslateY,
    TranslateZ,
    RotateX,
    RotateY,
    RotateZ,
    ScaleX,
    ScaleY,
    ScaleZ,
    CompensateScale
  }

  /// <summary>
  /// A track for <see cref="SBTransformAnimation"/>
  /// See <see cref="AnimationTrackFormat"/> for supported types
  /// </summary>
  public class GenericTransformTrack {
    public AnimationTrackFormat Type { get; internal set; }

    public GenericKeyGroup<float> Keys { get; } = new GenericKeyGroup<float>();

    public GenericTransformTrack(AnimationTrackFormat type) {
      Type = type;
    }

    public float GetValueAt(float frame) {
      return Keys.GetValue(frame);
    }

    public void AddKey(float frame,
                       float value,
                       InterpolationType interpolationType =
                           InterpolationType.Linear,
                       float InTan = 0,
                       float OutTan = float.MaxValue) {
      Keys.AddKey(frame, value, interpolationType, InTan, OutTan);
    }

    public void Optimize() {
      Keys.Optimize();
    }
  }
}