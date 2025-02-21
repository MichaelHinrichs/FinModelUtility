﻿using fin.language.equations.fixedFunction;
using fin.model;

namespace fin.shaders.glsl {
  public class FixedFunctionShaderSourceGlsl(IModel model,
                                             IFixedFunctionMaterial material,
                                             bool useBoneMatrices)
      : IShaderSourceGlsl {
    public string VertexShaderSource { get; } =
      GlslUtil.GetVertexSrc(model, useBoneMatrices);

    public string FragmentShaderSource { get; } =
      new FixedFunctionEquationsGlslPrinter()
          .Print(material);
  }
}