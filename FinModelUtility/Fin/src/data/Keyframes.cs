using fin.util.optional;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fin.data {
  public record Keyframe<T>(
    int Frame,
    T Value);

  public interface IKeyframes<T> {
    IReadOnlyList<Keyframe<T>> Definitions { get; }

    bool IsDefined { get; }

    void SetKeyframe(int frame, T value);

    Keyframe<T> GetKeyframeAtIndex(int index);
    Optional<Keyframe<T>> GetKeyframeAtFrame(int frame);

    bool FindIndexOfKeyframe(
      int frame,
      out int keyframeIndex,
      out Optional<Keyframe<T>> keyframe,
      out bool isLastKeyframe);
  }

  public class Keyframes<T> : IKeyframes<T> {
    private readonly List<Keyframe<T>> impl_ = new();

    public IReadOnlyList<Keyframe<T>> Definitions => this.impl_;

    public bool IsDefined { get; set; }

    public void SetKeyframe(
        int frame,
        T value) {
      this.IsDefined = true;

      var keyframeExists = this.FindIndexOfKeyframe(frame,
        out var keyframeIndex,
        out var maybeExistingKeyframe,
        out var isLastKeyframe);

      var newKeyframe = new Keyframe<T>(frame, value);

      if (maybeExistingKeyframe.Try(out var existingKeyframe) && existingKeyframe.Frame == frame) {
        this.impl_[frame] = newKeyframe;
      } else if (isLastKeyframe) {
        this.impl_.Add(newKeyframe);
      } else if (keyframeExists && existingKeyframe.Frame < frame) {
        this.impl_.Insert(keyframeIndex + 1, newKeyframe);
      } else {
        this.impl_.Insert(keyframeIndex, newKeyframe);
      }
    }

    public Keyframe<T> GetKeyframeAtIndex(int index) => this.impl_[index];

    public Optional<Keyframe<T>> GetKeyframeAtFrame(int frame) {
      this.FindIndexOfKeyframe(frame,
        out _,
        out var keyframe,
        out _);
      return keyframe;
    }


    public bool FindIndexOfKeyframe(
      int frame,
      out int keyframeIndex,
      out Optional<Keyframe<T>> keyframe,
      out bool isLastKeyframe) {
      var keyframeCount = this.impl_.Count;

      var min = 0;
      var max = keyframeCount - 1;

      var i = (min + max) / 2;

      while (min <= max) {
        var currentKeyframe = this.impl_[i];

        if (currentKeyframe.Frame == frame) {
          min = max = i;
          break;
        } else if (currentKeyframe.Frame < frame) {
          min = i + 1;
        } else {
          max = i - 1;
        }

        i = (min + max) / 2;
      }

      if (i < this.impl_.Count) {
        var kf = this.impl_[i];
        if (kf.Frame <= frame) {
          keyframe = Optional.Of(kf);
          keyframeIndex = i;
          isLastKeyframe = keyframeIndex == keyframeCount - 1;
          return true;
        }
      }
      {
        keyframeIndex = Math.Max(min, max);
        var kf = i == this.impl_.Count ? this.impl_.LastOrDefault() : default;
        keyframe = kf == null ? Optional.None<Keyframe<T>>() : Optional.Of(kf);
        isLastKeyframe = keyframeIndex == keyframeCount;
      }
      return false;
    }
  }
}
