using System.Collections.Generic;

namespace fin.animation {
  public interface IReadOnlyKeyframes<T> {
    bool HasAtLeastOneKeyframe { get; }

    IReadOnlyList<Keyframe<T>> Definitions { get; }

    Keyframe<T> GetKeyframeAtIndex(int index);
    Keyframe<T>? GetKeyframeAtFrame(int frame);

    bool FindIndexOfKeyframe(
        int frame,
        out int keyframeIndex,
        out Keyframe<T> keyframe,
        out bool isLastKeyframe);
  }

  public interface IKeyframes<T> : IReadOnlyKeyframes<T> {
    void SetKeyframe(int frame, T value);
    void SetAllKeyframes(IEnumerable<T> value);
  }
}