using System;
using System.Collections.Generic;


namespace schema.memory {
  public interface IMemoryRange<T> {
    IMemoryRange<T>? Parent { get; }

    T Data { get; set; }

    void Clear();

    long Length { get; }
    void Resize(long length);

    long GetOffset();


    void ClaimWithin(long offset, long sublength);
    void ClaimAtEnd(long sublength);

    IMemoryRange<T> ClaimSubrangeWithin(T data, long offset, long length);
    IMemoryRange<T> ClaimSubrangeAtEnd(T data, long length);
  }

  public class MemoryRange<T> : IMemoryRange<T> {
    private enum MemoryRangeType {
      ROOT,
      UNCLAIMED,
      CLAIMED,
    }

    private readonly T nullData_;

    private MemoryRangeType type_;
    private readonly MemoryRange<T>? parent_;
    private IList<MemoryRange<T>>? children_;

    public MemoryRange(
        T nullData,
        T data,
        long length) {
      this.type_ = MemoryRangeType.ROOT;
      this.nullData_ = nullData;
      this.Data = data;
      this.Length = length;
    }

    private MemoryRange(
        MemoryRangeType type,
        MemoryRange<T> parent,
        T nullData,
        T data,
        long length) {
      this.type_ = type;
      this.parent_ = parent;
      this.nullData_ = nullData;
      this.Data = data;
      this.Length = length;
    }


    public IMemoryRange<T>? Parent => parent_;
    public T Data { get; set; }


    public void Clear() {
      if (this.parent_ == null) {
        this.children_?.Clear();
        this.children_ = null;
        return;
      }

      this.Resize(0);
    }


    public long Length { get; private set; }

    public void Resize(long length) {
      var deltaLength = length - this.Length;
      this.Length = length;

      var parent = this.parent_;
      while (parent != null) {
        parent.Length += deltaLength;
        parent = parent.parent_;
      }
    }


    public long GetOffset() {
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


    public void ClaimWithin(long offset, long sublength)
      => this.ClaimSubrangeWithin(this.nullData_, offset, sublength);

    public void ClaimAtEnd(long sublength)
      => this.ClaimSubrangeAtEnd(this.nullData_, sublength);


    public IMemoryRange<T> ClaimSubrangeWithin(
        T data,
        long offset,
        long length) {
      if (offset < 0) {
        throw new ArgumentOutOfRangeException(
            nameof(offset),
            "Memory range offset must be a nonzero number!");
      }
      if (length <= 0) {
        throw new ArgumentOutOfRangeException(
            nameof(length),
            "Memory range length must be a positive number!");
      }
      if (offset + length > this.Length) {
        throw new Exception(
            "Memory range offset + length must less than the parent length!");
      }

      var (index, absoluteOffset, child) =
          this.FindRangeContainingRelativeOffset_(offset);

      if (child == null) {
        this.children_ = new List<MemoryRange<T>>();

        var beforeLength = offset;
        if (beforeLength > 0) {
          this.children_.Add(
              new MemoryRange<T>(
                  MemoryRangeType.UNCLAIMED,
                  this,
                  this.nullData_,
                  this.nullData_,
                  beforeLength));
        }

        var newRange = new MemoryRange<T>(
            MemoryRangeType.CLAIMED,
            this,
            this.nullData_,
            data,
            length);
        this.children_.Add(newRange);

        var afterLength = this.Length - (beforeLength + length);
        if (afterLength > 0) {
          this.children_.Add(
              new MemoryRange<T>(
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
        return child;
      }

      {
        this.children_.RemoveAt(index);

        var beforeLength = offset - absoluteOffset;
        if (beforeLength > 0) {
          this.children_.Insert(
              index,
              new MemoryRange<T>(
                  MemoryRangeType.UNCLAIMED,
                  this,
                  this.nullData_,
                  this.nullData_,
                  beforeLength));
        }

        var newRange = new MemoryRange<T>(
            MemoryRangeType.CLAIMED,
            this,
            this.nullData_,
            data,
            length);
        this.children_.Insert(index + 1, newRange);

        var afterLength = this.Length - (beforeLength + length);
        if (afterLength > 0) {
          this.children_.Insert(
              index + 2,
              new MemoryRange<T>(
                  MemoryRangeType.UNCLAIMED,
                  this,
                  this.nullData_,
                  this.nullData_,
                  afterLength));
        }

        return newRange;
      }
    }

    public IMemoryRange<T> ClaimSubrangeAtEnd(T data, long length) {
      if (length <= 0) {
        throw new ArgumentOutOfRangeException(
            nameof(length),
            "Memory range length must be a positive number!");
      }
      if (length > this.Length) {
        throw new ArgumentOutOfRangeException(
            nameof(length),
            "Memory range length must less than the parent length!");
      }


      if (this.children_ == null) {
        this.children_ = new List<MemoryRange<T>>();
        this.children_.Add(
            new MemoryRange<T>(
                MemoryRangeType.UNCLAIMED,
                this,
                this.nullData_,
                this.nullData_,
                this.Length));
      }

      var newRange = new MemoryRange<T>(
          MemoryRangeType.CLAIMED,
          this,
          this.nullData_,
          this.nullData_,
          length);
      this.children_.Add(newRange);

      this.Resize(this.Length + length);

      return newRange;
    }


    private (int index, long absoluteOffset, MemoryRange<T>? range)
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

        if (start <= relativeOffset && relativeOffset <= end) {
          return (i, start, child);
        }
      }

      throw new NotSupportedException();
    }
  }
}