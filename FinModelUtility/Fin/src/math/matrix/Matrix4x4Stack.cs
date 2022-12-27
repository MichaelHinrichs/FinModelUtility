using System.Collections.Generic;
using System.Numerics;

namespace fin.math.matrix {
  public interface IMatrix4x4Stack {
    Matrix4x4 Top { get; set; }

    Matrix4x4 Pop();
    void Push(Matrix4x4 value);
    void Push();

    void SetIdentity();
    void MultiplyInPlace(Matrix4x4 other);
  }

  public class Matrix4x4Stack : IMatrix4x4Stack {
    private readonly Stack<Matrix4x4> impl_ = new(new[] { Matrix4x4.Identity });

    public Matrix4x4 Top {
      get => this.impl_.Peek();
      set {
        this.impl_.Pop();
        this.impl_.Push(value);
      }
    }

    public Matrix4x4 Pop() => this.impl_.Pop();
    public void Push(Matrix4x4 value) => this.impl_.Push(value);
    public void Push() => this.impl_.Push(this.impl_.Peek());

    public void SetIdentity() => this.Top = Matrix4x4.Identity;
    public void MultiplyInPlace(Matrix4x4 other) {
      this.Top = other * this.Top;
    }
  }
}
