﻿using System.Numerics;

using fin.math.matrix.two;
using fin.util.asserts;

using OpenTK.Graphics.OpenGL;


namespace fin.ui.rendering.gl {
  public partial class GlShaderProgram {
    public IShaderUniform<Vector2> GetUniformVec2(string name) {
      if (!this.cachedUniforms_.TryGetValue(name, out var uniform)) {
        this.cachedUniforms_[name] = uniform =
            new Vec2ShaderUniform(this.GetUniformLocation_(name));
      }

      return Asserts.AsA<IShaderUniform<Vector2>>(uniform);
    }

    private class Vec2ShaderUniform : BShaderUniform, IShaderUniform<Vector2> {
      private readonly int location_;
      private Vector2 value_;

      public Vec2ShaderUniform(int location) {
        this.location_ = location;
      }

      public void SetAndMarkDirty(in Vector2 value) {
        this.value_ = value;
        this.MarkDirty();
      }

      public void SetAndMaybeMarkDirty(in Vector2 value) {
        if (this.value_.Equals(value)) {
          return;
        }

        this.value_ = value;
        this.MarkDirty();
      }

      protected override unsafe void PassValueToProgram() {
        fixed (float* ptr = &this.value_.X) {
          GL.Uniform2(this.location_, 1, ptr);
        }
      }
    }
  }
}