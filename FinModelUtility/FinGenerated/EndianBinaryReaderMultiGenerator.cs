using System.Runtime.InteropServices;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace System.IO {
  [Generator]
  public class EndianBinaryReaderMultiGenerator : ISourceGenerator {
    public void Initialize(GeneratorInitializationContext context) {}

    public void Execute(GeneratorExecutionContext context) {
      var stringBuilder = new StringBuilder(@"
using fin.util.asserts;

namespace System.IO {
  public sealed partial class EndianBinaryReader {
");

      var types = new[] {
          ("Byte", typeof(byte), "byte"),
          ("SByte", typeof(sbyte), "sbyte"),
          ("Int16", typeof(short), "short"),
          ("UInt16", typeof(ushort), "ushort"),
          ("Int32", typeof(int), "int"),
          ("UInt32", typeof(uint), "uint"),
          ("Int64", typeof(long), "long"),
          ("UInt64", typeof(ulong), "ulong"),
          ("Single", typeof(float), "float"),
          ("Double", typeof(double), "double"),
          ("Sn16", typeof(short), "float"),
          ("Un16", typeof(ushort), "float"),
      };

      foreach (var (name, inType, outTypeName) in types) {
        var inTypeSize = Marshal.SizeOf(inType);

        stringBuilder.Append($@"
    public void Assert{name}({outTypeName} expectedValue)
      => Asserts.Equal(expectedValue, this.Read{name}());

    public {outTypeName} Read{name}() {{
      this.FillBuffer_({inTypeSize});
      return this.Convert{name}_(0);
    }}

    public {outTypeName}[] Read{name}s(int count)
      => this.Read{name}s(new {outTypeName}[count]);

    public {outTypeName}[] Read{name}s({outTypeName}[] dst) {{
      var size = {inTypeSize};
      this.FillBuffer_(size * dst.Length, size);
      for (var i = 0; i < dst.Length; ++i) {{
        dst[i] = this.Convert{name}_(i);
      }}
      return dst;
    }}
");
      }

      stringBuilder.Append(@"
  }
}
");

      context.AddSource("EndianBinaryReader",
                        stringBuilder.ToString());
    }
  }
}