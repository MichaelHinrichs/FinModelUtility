using fin.model;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace fin.data {
  public record Keyframe<T>(
    int Frame,
    T Value);

  public interface IKeyframes<T> {
    int FrameCount { get; set; }
    IReadOnlyList<Keyframe<T>> Keyframes { get; }

    bool IsDefined { get; }

    void SetKeyframe(int frame, T value);

    public Keyframe<T>? GetKeyframe(int frame) {
      this.FindIndexOfKeyframe(frame,
        out _,
        out var keyframe,
        out _);
      return keyframe;
    }

    bool FindIndexOfKeyframe(
      int frame,
      out int keyframeIndex,
      out T? keyframeValue,
      out bool isLastKeyframe);
  }

  public class Keyframes<T> : IKeyframes<T> {
    private readonly List<Keyframe<T>> impl_ = new();

    public int FrameCount { get; set; }
    public IReadOnlyList<Keyframe<T>> Keyframes => this.impl_;

    public bool IsDefined { get; set; }

    public void SetKeyframe(
        int frame,
        T value) {
      this.IsDefined = true;

      var keyframeExists = this.FindIndexOfKeyframe(frame,
        out var keyframeIndex,
        out var existingKeyframe,
        out var isLastKeyframe);

      var newKeyframe = new Keyframe<T>(frame, value);

      if (keyframeExists && existingKeyframe.Frame == frame) {
        this.impl_[frame] = newKeyframe;
      } else if (isLastKeyframe) {
        this.impl_.Add(newKeyframe);
      } else {
        this.impl_.Insert(keyframeIndex, newKeyframe);
      }
    }

    public Keyframe<T>? GetKeyframe(int frame) {
      this.FindIndexOfKeyframe(frame,
        out _,
        out var keyframe,
        out _);
      return keyframe;
    }


    public bool FindIndexOfKeyframe(
      int frame,
      out int keyframeIndex,
      out Keyframe<T>? keyframe,
      out bool isLastKeyframe) {
      var keyframeCount = this.impl_.Count;

      var min = 0;
      var max = keyframeCount - 1;
      var i = (min + max) / 2;

      while (true) {
        var currentKeyframe = this.impl_[i];

        if (currentKeyframe.Frame <= frame) {
          if (min >= max || i == max || this.impl_[i + 1].Frame > frame) {
            keyframeIndex = i;
            keyframeValue = currentKeyframe.Value;
            isLastKeyframe = i == keyframeCount - 1;
            return true;
          } else {
            max = i;
          }
        } else {
          min = i + 1;
        }

        i = (min + max) / 2;
      }

      keyframeIndex = this.impl_.Count;
      keyframeValue = default;
      isLastKeyframe = true;
      return false;
    }
  }
}
