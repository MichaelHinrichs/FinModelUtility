using System;
using System.IO;

using Newtonsoft.Json;


namespace fin.util.json {
  public static class JsonUtil {
    public static string Serialize(object obj) {
      var jsonSerializer = new JsonSerializer {
          Formatting = Formatting.Indented
      };
      var jsonTextWriter = new StringWriter();
      jsonSerializer.Serialize(jsonTextWriter, obj);
      return jsonTextWriter.ToString();
    }
  }
}
