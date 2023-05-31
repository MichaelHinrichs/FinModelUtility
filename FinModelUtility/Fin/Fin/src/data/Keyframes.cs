using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fin.data {
  public readonly struct Keyframe<T> : IComparable<Keyframe<T>> {
    public int Frame { get; }
    public T Value { get; }

    public Keyframe(int frame, T value) {
      this.Frame = frame;
      this.Value = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(Keyframe<T> other)
      => this.Frame.CompareTo(other.Frame);

    public override string ToString()
      => $"{{ Frame: {this.Frame}, Value: {this.Value} }}";
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


    private int lastAccessedIndex_ = -1;

    public bool FindIndexOfKeyframe(
        int frame,
        out int keyframeIndex,
        out Keyframe<T> keyframe,
        out bool isLastKeyframe) {
      // Try to optimize the case where the next frame is being accessed.
      var keyframeCount = this.impl_.Count;
      if (this.lastAccessedIndex_ >= 0 &&
          this.lastAccessedIndex_ < keyframeCount) {
        keyframeIndex = this.lastAccessedIndex_;
        keyframe = this.impl_[keyframeIndex];

        if (frame >= keyframe.Frame) {
          isLastKeyframe = keyframeIndex == keyframeCount - 1;

          if (isLastKeyframe || frame == keyframe.Frame) {
            return true;
          }

          var nextKeyframe = this.impl_[this.lastAccessedIndex_ + 1];
          if (nextKeyframe.Frame > frame) {
            return true;
          } else if (nextKeyframe.Frame == frame) {
            this.lastAccessedIndex_ = ++keyframeIndex;
            keyframe = nextKeyframe;
            isLastKeyframe = keyframeIndex == keyframeCount - 1;
            return true;
          }
        }
      }

      // Perform a binary search for the current frame.
      var result = this.impl_.BinarySearch(new Keyframe<T>(frame, default!));
      if (result >= 0) {
        this.lastAccessedIndex_ = keyframeIndex = result;
        keyframe = this.impl_[keyframeIndex];
        isLastKeyframe = keyframeIndex == keyframeCount - 1;
        return true;
      }

      var i = ~result;
      if (i == keyframeCount) {
        this.lastAccessedIndex_ = keyframeIndex = keyframeCount - 1;
        isLastKeyframe = true;
        if (keyframeCount > 0) {
          keyframe = this.impl_[keyframeIndex];
          return true;
        }

        keyframe = default;
        return false;
      }

      this.lastAccessedIndex_ = keyframeIndex = Math.Max(0, i - 1);
      keyframe = this.impl_[keyframeIndex];
      var keyframeExists = keyframe.Frame <= frame;
      if (!keyframeExists) {
        keyframe = default;
      }

      isLastKeyframe = false;
      return keyframeExists;
    }
  }
}