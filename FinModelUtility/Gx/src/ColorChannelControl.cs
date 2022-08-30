namespace gx {
  public enum GxColorSrc : byte {
    Register = 0, // Use Register Colors
    Vertex = 1 // Use Vertex Colors
  }

  public enum GxAttenuationFunction : byte {
    // No attenuation
    None = 2,

    // Specular Computation
    Spec = 0,

    // Spot Light Attenuation
    Spot = 1
  }

  [Flags]
  public enum GxLightMask : byte {
    Light0 = 0x01,
    Light1 = 0x02,
    Light2 = 0x04,
    Light3 = 0x08,
    Light4 = 0x10,
    Light5 = 0x20,
    Light6 = 0x40,
    Light7 = 0x80,
    None = 0x00
  }

  public enum GxDiffuseFunction : byte {
    None = 0,
    Signed = 1,
    Clamp = 2
  }
}