namespace glo.schema {
  [Flags]
  public enum GloMeshFlags : ushort {
    GOURAUD_SHADED = 1 << 0,
    PRELIT = 1 << 3,
    FACE_COLOR = 1 << 10,
  }
}