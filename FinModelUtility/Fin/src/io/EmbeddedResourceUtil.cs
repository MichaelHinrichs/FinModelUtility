using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

using fin.util.asserts;


namespace fin.io {
  public static class EmbeddedResourceUtil {
    public static Stream GetStream(Assembly assembly, string embeddedResourceName) {
      var resourceNames = assembly.GetManifestResourceNames();
      Asserts.True(resourceNames.Contains(embeddedResourceName));

      return assembly.GetManifestResourceStream(embeddedResourceName)!;
    }

    public static Image Load(Assembly assembly, string embeddedResourceName) {
      using var stream = EmbeddedResourceUtil.GetStream(assembly, embeddedResourceName);
      return Bitmap.FromStream(stream);
    }
  }
}
