using System.Collections.Generic;

namespace fin.model {
  public interface IMaterialManager {
    IReadOnlyList<IMaterial> All { get; }
    IMaterial AddMaterial();
  }

  public interface IMaterial {
    string Name { get; set; }

    // TODO: Setting texture layer(s).
    // TODO: Setting logic for combining texture layers.
  }
}
