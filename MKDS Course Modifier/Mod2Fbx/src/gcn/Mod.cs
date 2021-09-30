using System.Collections.Generic;
using System.IO;

namespace mod.gcn {
  public class DateTime {
    public ushort year = 2021;
    public byte month = 9;
    public byte day = 18;
  }

  public class ModHeader {
    public readonly DateTime dateTime = new();

    public uint m_flags = 0;
  }

  public enum ModFlags {
    UseNBT = 0x01
  }

  public class Mod : IGcnSerializable {
    public Mod() {}
    public Mod(EndianBinaryReader reader) => this.Read(reader);

    public void Read(EndianBinaryReader reader);
    public void Write(EndianBinaryWriter writer);
    public void Reset();

    public static string GetChunkName(uint opcode);

    public readonly ModHeader m_header = new();
    public readonly List<Vector3f> m_vertices = new();
    public readonly List<Vector3f> m_vnormals = new();
    public readonly List<NBT> m_vertexnbt = new();
    public readonly List<ColourU8> m_vcolours = new();
    public readonly List<Vector2f>[] m_texcoords = new List<Vector2f>[8];
    public readonly List<Texture> m_textures = new();
    public readonly List<TextureAttributes> m_texattrs = new();
    public readonly MaterialContainer m_materials;
    public readonly List<VtxMatrix> m_vtxMatrix = new();
    public readonly List<Envelope> m_envelopes = new();
    public readonly List<Mesh> m_meshes = new();
    public readonly List<Joint> m_joints = new();
    public readonly List<string> m_jointNames = new();
    public readonly CollTriInfo m_colltris;
    public readonly CollGrid m_collgrid;
    public readonly List<byte> m_eofBytes = new();
  }
}