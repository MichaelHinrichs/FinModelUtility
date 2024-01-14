using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace fin.animation {
  public readonly record struct Keyframe<T>(int Frame, T Value) : IComparable<Keyframe<T>> {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(Keyframe<T> other)
      => this.Frame - other.Frame;
  }

  public class Keyframes<T> : IKeyframes<T> {
    private List<Keyframe<T>> impl_;

    public Keyframes(int initialCapacity = 0) {
      this.impl_ = new(initialCapacity);
    }

    public IReadOnlyList<Keyframe<T>> Definitions => this.impl_;

    public bool HasAtLeastOneKeyframe { get; set; }

    public void SetKeyframe(int frame, T value)
      => SetKeyframe(frame, value, out _);

    public void SetKeyframe(int frame,
                            T value,
                            out bool performedBinarySearch) {
      this.HasAtLeastOneKeyframe = true;

      var keyframeExists = this.FindIndexOfKeyframe(frame,
                                                    out var keyframeIndex,
                                                    out var existingKeyframe,
                                                    out var isLastKeyframe,
                                                    out performedBinarySearch);

      var newKeyframe = new Keyframe<T>(frame, value);

      if (keyframeExists && existingKeyframe.Frame == frame) {
        this.lastAccessedKeyframeIndex_ = keyframeIndex;
        this.impl_[keyframeIndex] = newKeyframe;
      } else if (isLastKeyframe) {
        this.lastAccessedKeyframeIndex_ = this.impl_.Count;
        this.impl_.Add(newKeyframe);
      } else if (keyframeExists && existingKeyframe.Frame < frame) {
        this.impl_.Insert(keyframeIndex + 1, newKeyframe);
      } else {
        this.impl_.Insert(keyframeIndex, newKeyframe);
      }
    }

    public void SetAllKeyframes(IEnumerable<T> values) {
      this.impl_ = values
                   .Select((value, frame) => new Keyframe<T>(frame, value))
                   .ToList();
      this.HasAtLeastOneKeyframe = this.impl_.Count > 0;
      this.lastAccessedKeyframeIndex_ = this.impl_.Count - 1;
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


    private int lastAccessedKeyframeIndex_ = -1;

    public bool FindIndexOfKeyframe(
        int frame,
        out int keyframeIndex,
        out Keyframe<T> keyframe,
        out bool isLastKeyframe)
      => this.FindIndexOfKeyframe(frame,
                                  out keyframeIndex,
                                  out keyframe,
                                  out isLastKeyframe,
                                  out _);

    public bool FindIndexOfKeyframe(
        int frame,
        out int keyframeIndex,
        out Keyframe<T> keyframe,
        out bool isLastKeyframe,
        out bool performedBinarySearch) {
      performedBinarySearch = false;

      // Try to optimize the case where no frames have been processed yet.
      var keyframeCount = this.impl_.Count;
      if (this.lastAccessedKeyframeIndex_ == -1 || keyframeCount == 0) {
        this.lastAccessedKeyframeIndex_ = keyframeIndex = 0;
        keyframe = default;
        isLastKeyframe = false;
        return false;
      }

      // Try to optimize the case where the next frame is being accessed.
      if (this.lastAccessedKeyframeIndex_ >= 0 &&
          this.lastAccessedKeyframeIndex_ < keyframeCount) {
        keyframeIndex = this.lastAccessedKeyframeIndex_;
        keyframe = this.impl_[keyframeIndex];

        if (frame >= keyframe.Frame) {
          isLastKeyframe = keyframeIndex == keyframeCount - 1;

          if (isLastKeyframe || frame == keyframe.Frame) {
            return true;
          }

          var nextKeyframe = this.impl_[this.lastAccessedKeyframeIndex_ + 1];
          if (nextKeyframe.Frame > frame) {
            return true;
          } else if (nextKeyframe.Frame == frame) {
            this.lastAccessedKeyframeIndex_ = ++keyframeIndex;
            keyframe = nextKeyframe;
            isLastKeyframe = keyframeIndex == keyframeCount - 1;
            return true;
          }
        }
      }

      // Perform a binary search for the current frame.
      var result = this.impl_.BinarySearch(new Keyframe<T>(frame, default!));
      performedBinarySearch = true;

      if (result >= 0) {
        this.lastAccessedKeyframeIndex_ = keyframeIndex = result;
        keyframe = this.impl_[keyframeIndex];
        isLastKeyframe = keyframeIndex == keyframeCount - 1;
        return true;
      }

      var i = ~result;
      if (i == keyframeCount) {
        this.lastAccessedKeyframeIndex_ = keyframeIndex = keyframeCount - 1;
        isLastKeyframe = true;
        if (keyframeCount > 0) {
          keyframe = this.impl_[keyframeIndex];
          return true;
        }

        keyframe = default;
        return false;
      }

      this.lastAccessedKeyframeIndex_ = keyframeIndex = Math.Max(0, i - 1);
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