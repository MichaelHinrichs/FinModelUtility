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

    public readonly ModHeader m_header;
    public readonly List<Vector3f> m_vertices;
    public readonly List<Vector3f> m_vnormals;
    public readonly List<NBT> m_vertexnbt;
    public readonly List<ColourU8> m_vcolours;
    std::array<std::vector<Vector2f>, 8> m_texcoords;
    std::vector<Texture> m_textures;
    std::vector<TextureAttributes> m_texattrs;
    MaterialContainer m_materials;
    std::vector<VtxMatrix> m_vtxMatrix;
    std::vector<Envelope> m_envelopes;
    std::vector<Mesh> m_meshes;
    std::vector<Joint> m_joints;
    std::vector<std::string> m_jointNames;
    CollTriInfo m_colltris;
    CollGrid m_collgrid;
    std::vector<u8> m_eofBytes;
  }
}