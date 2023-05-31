using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace fin.data {
  public readonly struct Keyframe<T> {
    public int Frame { get; }
    public T Value { get; }

    public Keyframe(int frame,
                    T value) {
      this.Frame = frame;
      this.Value = value;
    }
  }

  public interface IKeyframes<T> {
    IReadOnlyList<Keyframe<T>> Definitions { get; }

    bool IsDefined { get; }

    void SetKeyframe(int frame, T value);

    Keyframe<T> GetKeyframeAtIndex(int index);
    Keyframe<T>? GetKeyframeAtFrame(int frame);

    bool FindIndexOfKeyframe(
        int frame,
        out int keyframeIndex,
        out Keyframe<T> keyframe,
        out bool isLastKeyframe);
  }

  public class Keyframes<T> : IKeyframes<T> {
    private readonly List<Keyframe<T>> impl_;

    public Keyframes(int initialCapacity = 0) {
      this.impl_ = new(initialCapacity);
    }

    public IReadOnlyList<Keyframe<T>> Definitions => this.impl_;

    public bool IsDefined { get; set; }

    public void SetKeyframe(int frame, T value) {
      this.IsDefined = true;

      var keyframeExists = this.FindIndexOfKeyframe(frame,
                                                    out var keyframeIndex,
                                                    out var existingKeyframe,
                                                    out var isLastKeyframe);

      var newKeyframe = new Keyframe<T>(frame, value);

      if (keyframeExists && existingKeyframe.Frame == frame) {
        this.impl_[keyframeIndex] = newKeyframe;
      } else if (isLastKeyframe) {
        this.impl_.Add(newKeyframe);
      } else if (keyframeExists && existingKeyframe.Frame < frame) {
        this.impl_.Insert(keyframeIndex + 1, newKeyframe);
      } else {
        this.impl_.Insert(keyframeIndex, newKeyframe);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Keyframe<T> GetKeyframeAtIndex(int index) => this.impl_[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Keyframe<T>? GetKeyframeAtFrame(int frame) {
      this.FindIndexOfKeyframe(frame,
                               out _,
                               out var keyframe,
                               out _);
      return keyframe;
    }


    public bool FindIndexOfKeyframe(
        int frame,
        out int keyframeIndex,
        out Keyframe<T> keyframe,
        out bool isLastKeyframe) {
      // Optimizes the "finding last keyframe" state, which is expected to be
      // each call when creating an animation.
      var keyframeCount = this.impl_.Count;
      if (keyframeCount > 0) {
        keyframeIndex = keyframeCount - 1;
        keyframe = this.impl_[keyframeIndex];
        if (frame > keyframe.Frame) {
          isLastKeyframe = true;
          return true;
        }
      }

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

      if (i < keyframeCount) {
        var kf = this.impl_[i];
        if (kf.Frame <= frame) {
          keyframe = kf;
          keyframeIndex = i;
          isLastKeyframe = keyframeIndex == keyframeCount - 1;
          return true;
        }
      }

      {
        keyframeIndex = Math.Max(min, max);
        keyframe = i == keyframeCount ? this.impl_.LastOrDefault() : default;
        isLastKeyframe = keyframeIndex == keyframeCount;
      }
      return false;
    }
  }
}