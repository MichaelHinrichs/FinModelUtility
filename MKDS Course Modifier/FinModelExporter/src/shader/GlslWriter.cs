using fin.language.writer;
using fin.shader;

namespace fin.src.shader {
  public class GlslWriter {
    public string Write(IGlslShader shader) {
      var impl = new NestedStringBuilderImpl("  ");

      // TODO: Support different versions.
      impl.WriteLine("#version 110");

      foreach (var inVar in shader.Ins) {
        GlslWriter.WriteHeaderVariable_(impl, HeaderVariableType.IN, inVar);
      }
      impl.Write("\n");

      foreach (var outVar in shader.Outs) {
        GlslWriter.WriteHeaderVariable_(impl, HeaderVariableType.OUT, outVar);
      }
      impl.Write("\n");

      foreach (var uniformVar in shader.Uniforms) {
        GlslWriter.WriteHeaderVariable_(impl,
                                        HeaderVariableType.UNIFORM,
                                        uniformVar);
      }
      impl.Write("\n\n");

      foreach (var method in shader.Methods) {
        GlslWriter.WriteMethod_(impl, method);
        impl.Write("\n");
      }

      return impl.ToString();
    }

    private enum HeaderVariableType {
      IN,
      OUT,
      UNIFORM,
    }

    private static void WriteHeaderVariable_(
        BStringBuilder impl,
        HeaderVariableType type,
        IGlslVariable var) {
      impl.Write(type switch {
          HeaderVariableType.IN      => "in",
          HeaderVariableType.OUT     => "out",
          HeaderVariableType.UNIFORM => "uniform",
      });
      impl.Write(" ");

      impl.Write(GlslWriter.GetValueTypeName_(var.UntypedValue.Type));
      impl.Write(" ");

      impl.Write(var.Name);
      impl.WriteLine(";");
    }

    private static void WriteMethod_(
        BNestedStringBuilder impl,
        IGlslMethod method) {
      if (method.Return != null) {
        impl.Write(GlslWriter.GetValueTypeName_(method.Return.Type));
      } else {
        impl.Write("void");
      }
      impl.Write(" ");

      impl.Write(method.Name);
      impl.Write("(");

      for (var p = 0; p < method.Parameters.Count; ++p) {
        var parameter = method.Parameters[p];

        if (p > 0) {
          impl.Write(", ");
        }

        impl.Write(GlslWriter.GetValueTypeName_(parameter.UntypedValue.Type));
        impl.Write(" ");
        impl.Write(parameter.Name);
      }
      
      impl.WriteLine(") {");
      
      // TODO: Find shortest path between return value and inputs
      // TODO: Write path

      impl.WriteLine("}");
    }

    private static string GetValueTypeName_(GlslValueType type)
      => type switch {
          GlslValueType.BOOL   => "bool",
          GlslValueType.INT    => "int",
          GlslValueType.UINT   => "uint",
          GlslValueType.FLOAT  => "float",
          GlslValueType.DOUBLE => "double",

          GlslValueType.BVEC2 => "bvec2",
          GlslValueType.BVEC3 => "bvec3",
          GlslValueType.BVEC4 => "bvec4",

          GlslValueType.IVEC2 => "ivec2",
          GlslValueType.IVEC3 => "ivec3",
          GlslValueType.IVEC4 => "ivec4",

          GlslValueType.UVEC2 => "uvec2",
          GlslValueType.UVEC3 => "uvec3",
          GlslValueType.UVEC4 => "uvec4",

          GlslValueType.VEC2 => "vec2",
          GlslValueType.VEC3 => "vec3",
          GlslValueType.VEC4 => "vec4",

          GlslValueType.DVEC2 => "dvec2",
          GlslValueType.DVEC3 => "dvec3",
          GlslValueType.DVEC4 => "dvec4",

          GlslValueType.MAT2   => "mat2",
          GlslValueType.MAT2X3 => "mat2x3",
          GlslValueType.MAT2X4 => "mat2x4",

          GlslValueType.MAT3x2 => "mat3x2",
          GlslValueType.MAT3   => "mat3",
          GlslValueType.MAT3x4 => "mat3x4",

          GlslValueType.MAT4X2 => "mat4x2",
          GlslValueType.MAT4X3 => "mat4x3",
          GlslValueType.MAT4   => "mat4",

          GlslValueType.SAMPLER2D => "sampler2d",
      };
  }
}