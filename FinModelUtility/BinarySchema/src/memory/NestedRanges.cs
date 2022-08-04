﻿using System;
using System.Collections.Generic;

using schema.util;


namespace schema.memory {
  public interface IReadonlyNestedRanges<T> {
    INestedRanges<T>? Parent { get; }

    T Data { get; }

    long Length { get; }
    bool IsLengthValid { get; }

    long GetRelativeOffset();
    long GetAbsoluteOffset();
  }

  public interface INestedRanges<T> : IReadonlyNestedRanges<T> {
    new T Data { get; set; }

    void Free();

    void InvalidateLengthOfSelfAndParents();

    /// <summary>
    ///   Resizes the range while also affecting any parent(s). Siblings that come after this will be pushed back.
    /// </summary>
    void ResizeSelfAndParents(long length);

    /// <summary>
    ///   Resizes the range by growing into the next unclaimed sibling. The parent's length will be unaffected.
    /// </summary>
    void ResizeInPlace(long length);

    INestedRanges<T> ClaimSubrangeWithin(T data, long offset, long length);
    INestedRanges<T> ClaimSubrangeAtEnd(T data, long length);

    INestedRanges<T> ClaimSubrangeWithin(T data, long offset);
    INestedRanges<T> ClaimSubrangeAtEnd(T data);
  }

  public class NestedRanges<T> : INestedRanges<T> {
    private enum MemoryRangeType {
      ROOT,
      UNCLAIMED,
      CLAIMED,
    }

    private readonly T nullData_;

    private MemoryRangeType type_;
    private readonly NestedRanges<T>? parent_;
    private IList<NestedRanges<T>>? children_;

    public NestedRanges(
        T nullData,
        T data,
        long length) {
      this.type_ = MemoryRangeType.ROOT;
      this.nullData_ = nullData;
      this.Data = data;
      this.Length = length;
    }

    public NestedRanges(
        T nullData,
        T data) : this(nullData, data, 0) { }

    private NestedRanges(
        MemoryRangeType type,
        NestedRanges<T> parent,
        T nullData,
        T data,
        long length) {
      this.type_ = type;
      this.parent_ = parent;
      this.nullData_ = nullData;
      this.Data = data;
      this.Length = length;
    }

    private NestedRanges(
        MemoryRangeType type,
        NestedRanges<T> parent,
        T nullData,
        T data) {
      this.type_ = type;
      this.parent_ = parent;
      this.nullData_ = nullData;
      this.Data = data;
    }


    public INestedRanges<T>? Parent => parent_;
    public T Data { get; set; }


    public void Free() {
      if (this.parent_ == null) {
        this.children_?.Clear();
        this.children_ = null;
        this.ResizeInPlace(0);
        return;
      }

      this.type_ = MemoryRangeType.UNCLAIMED;
      this.parent_.MergeUnclaimedRegions();
    }

    private void MergeUnclaimedRegions() {
      throw new NotImplementedException();
    }


    private long length_ = -1;

    public long Length {
      get {
        Asserts.True(this.IsLengthValid);
        return this.length_;
      }
      private set {
        this.length_ = value;
        this.IsLengthValid = true;
      }
    }

    public bool IsLengthValid { get; private set; }

    public void InvalidateLengthOfSelfAndParents() {
      this.IsLengthValid = false;
      this.parent_?.InvalidateLengthOfSelfAndParents();
    }


    public void ResizeSelfAndParents(long length) {
      this.Length = length;

      var parent = this.parent_;
      while (parent != null) {
        parent.RecalculateLengthFromChildren_();
        parent = parent.parent_;
      }
    }

    private void RecalculateLengthFromChildren_() {
      var totalLength = 0L;
      if (this.children_ != null) {
        foreach (var child in this.children_) {
          if (child.IsLengthValid) {
            totalLength += child.Length;
          } else {
            this.IsLengthValid = false;
          }
        }
      }
      this.Length = totalLength;
    }


    public void ResizeInPlace(long length) {
      if (this.parent_ != null) {
        var childrenOfParent = this.parent_!.children_;

        var indexOfSelf = childrenOfParent.IndexOf(this);
        var nextSibling = childrenOfParent[indexOfSelf + 1];
        Asserts.Equal(MemoryRangeType.UNCLAIMED, nextSibling.type_);
        Asserts.Equal(MemoryRangeType.UNCLAIMED, nextSibling.type_);

        var deltaLength = length - this.Length;
        nextSibling.Length += deltaLength;
      }

      this.Length = length;
    }


    public long GetRelativeOffset() {
      if (this.parent_ == null) {
        return 0;
      }

      var totalOffset = 0L;
      foreach (var child in this.parent_.children_!) {
        if (child == this) {
          return totalOffset;
        }
        totalOffset += child.Length;
      }

      throw new NotSupportedException();
    }

    public long GetAbsoluteOffset() {
      var totalOffset = 0L;

      var range = this;
      while (range != null) {
        totalOffset += range.GetRelativeOffset();
        range = range.parent_;
      }

      return totalOffset;
    }


    public INestedRanges<T> ClaimSubrangeWithin(T data, long offset)
      => this.ClaimSubrangeWithin(data, offset, 0);

    public INestedRanges<T> ClaimSubrangeWithin(
        T data,
        long offset,
        long length) {
      if (offset < 0) {
        throw new ArgumentOutOfRangeException(
            nameof(offset),
            "Memory range offset must be a nonzero number!");
      }
      if (length < 0) {
        throw new ArgumentOutOfRangeException(
            nameof(length),
            "Memory range length must be a nonzero number!");
      }
      if (offset + length > this.Length) {
        throw new Exception(
            "Memory range offset + length must less than the parent length!");
      }

      var (index, absoluteOffset, child) =
          this.FindRangeContainingRelativeOffset_(offset);

      if (child == null) {
        this.children_ = new List<NestedRanges<T>>();

        var beforeLength = offset;
        if (beforeLength > 0) {
          this.children_.Add(
              new NestedRanges<T>(
                  MemoryRangeType.UNCLAIMED,
                  this,
                  this.nullData_,
                  this.nullData_,
                  beforeLength));
        }

        var newRange = new NestedRanges<T>(
            MemoryRangeType.CLAIMED,
            this,
            this.nullData_,
            data,
            length);
        this.children_.Add(newRange);

        var afterLength = this.Length - (beforeLength + length);
        if (afterLength > 0) {
          this.children_.Add(
              new NestedRanges<T>(
                  MemoryRangeType.UNCLAIMED,
                  this,
                  this.nullData_,
                  this.nullData_,
                  afterLength));
        }

        return newRange;
      }

      if (child.type_ == MemoryRangeType.CLAIMED) {
        throw new Exception("Range at offset is already claimed!");
      }

      if (absoluteOffset == offset && child.Length == length) {
        child.type_ = MemoryRangeType.CLAIMED;
        child.Data = data;
        return child;
      }

      {
        this.children_.RemoveAt(index);

        var newRangeIndex = index;
        var beforeLength = offset - absoluteOffset;
        if (beforeLength > 0) {
          newRangeIndex = index + 1;
          this.children_.Insert(
              index,
              new NestedRanges<T>(
                  MemoryRangeType.UNCLAIMED,
                  this,
                  this.nullData_,
                  this.nullData_,
                  beforeLength));
        }

        var newRange = new NestedRanges<T>(
            MemoryRangeType.CLAIMED,
            this,
            this.nullData_,
            data,
            length);
        this.children_.Insert(newRangeIndex, newRange);

        var afterLength = this.Length - (beforeLength + length);
        if (afterLength > 0) {
          this.children_.Insert(
              newRangeIndex + 1,
              new NestedRanges<T>(
                  MemoryRangeType.UNCLAIMED,
                  this,
                  this.nullData_,
                  this.nullData_,
                  afterLength));
        }

        return newRange;
      }
    }


    public INestedRanges<T> ClaimSubrangeAtEnd(T data)
      => this.ClaimSubrangeAtEnd(data, 0);

    public INestedRanges<T> ClaimSubrangeAtEnd(T data, long length) {
      if (length < 0) {
        throw new ArgumentOutOfRangeException(
            nameof(length),
            "Memory range length must be a nonnegative number!");
      }

      if (this.children_ == null) {
        this.children_ = new List<NestedRanges<T>>();
        this.children_.Add(
            new NestedRanges<T>(
                MemoryRangeType.UNCLAIMED,
                this,
                this.nullData_,
                data,
                this.Length));
      }

      var newRange = new NestedRanges<T>(
          MemoryRangeType.CLAIMED,
          this,
          this.nullData_,
          data,
          length);
      this.children_.Add(newRange);

      this.ResizeSelfAndParents(this.Length + length);

      return newRange;
    }


    private (int index, long absoluteOffset, NestedRanges<T>? range)
        FindRangeContainingRelativeOffset_(
            long relativeOffset) {
      if (this.children_ == null) {
        return (-1, -1, null);
      }

      var totalOffset = 0L;
      for (var i = 0; i < this.children_.Count; ++i) {
        var child = this.children_[i];

        var start = totalOffset;
        var end = totalOffset + child.Length;

        if (start <= relativeOffset && relativeOffset < end) {
          return (i, start, child);
        }

        totalOffset = end;
      }

      throw new NotSupportedException();
    }
  }
}