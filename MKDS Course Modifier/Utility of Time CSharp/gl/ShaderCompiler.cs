using System;
using System.Linq;
using System.Text;

using Tao.OpenGl;

namespace UoT {
  public static class ShaderCompiler {

    public static int Compile(int shaderType, string shaderLines) {
      var shader = Gl.glCreateShader(shaderType);

      Gl.glShaderSource(shader,
                        1,
                        new[] { shaderLines },
                        new[] { shaderLines.Length });
      Gl.glCompileShader(shader);

      Gl.glGetShaderiv(shader, Gl.GL_COMPILE_STATUS, out var success);
      if (success == Gl.GL_FALSE) {
        Gl.glGetShaderiv(shader,
                         Gl.GL_INFO_LOG_LENGTH,
                         out var logSize);

        var logBuilder = new StringBuilder(logSize);
        Gl.glGetShaderInfoLog(shader, logSize, out _, logBuilder);

        var i = 0;
        var formattedShaderLines =
            string.Join(
                "\n",
                shaderLines
                    .Split('\n')
                    .Select(rawLine => rawLine.Trim())
                    .Select(
                        line => (++i).ToString("00") + "| " + line));

        throw new Exception(Environment.NewLine +
                            logBuilder +
                            Environment.NewLine +
                            formattedShaderLines);
      }

      return shader;
    }
  }
}
