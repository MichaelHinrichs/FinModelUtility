using System;

using fin.model;
using fin.util.hash;

using schema.binary;

namespace fin.schema.vector {
  public abstract class BVector3<T> {
    public T X { get; set; }
    public T Y { get; set; }
    public T Z { get; set; }

    public T this[int index] {
      get => index switch {
          0 => X,
          1 => Y,
          2 => Z,
      };
      set {
        switch (index) {
          case 0: {
            this.X = value;
            break;
          }
          case 1: {
            this.Y = value;
            break;
          }
          case 2: {
            this.Z = value;
            break;
          }
          default: throw new ArgumentOutOfRangeException();
        }
      }
    }

    public void Set(T x, T y, T z) {
      this.X = x;
      this.Y = y;
      this.Z = z;
    }

    public override string ToString() => $"{{{this.X}, {this.Y}, {this.Z}}}";
  }

  [BinarySchema]
  public sealed partial class Vector3f : BVector3<float>,
                                         IVector3,
                                         IBinaryConvertible {
    public static bool operator ==(Vector3f lhs, Vector3f rhs)
      => lhs.Equals(rhs);

    public static bool operator !=(Vector3f lhs, Vector3f rhs)
      => !lhs.Equals(rhs);

    public override bool Equals(object? obj) {
      if (Object.ReferenceEquals(this, obj)) {
        return true;
      }

      if (obj is Vector3f other) {
        return this.X == other.X &&
               this.Y == other.Y &&
               this.Z == other.Z;
      }

      return false;
    }

    public override int GetHashCode()
      => FluentHash.Start().With(X).With(Y).With(Z).Hash;
  }

  [BinarySchema]
  public sealed partial class Vector3i : BVector3<int>, IBinaryConvertible { }

  [BinarySchema]
  public sealed partial class Vector3s : BVector3<short>, IBinaryConvertible { }
}