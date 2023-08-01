using System;
using System.IO;

using fin.schema.color;
using fin.util.array;

using schema.binary;


namespace cmb.schema.cmb.mats {
  public class Material : IBinaryConvertible {
    public bool isFragmentLightingEnabled;
    public bool isVertexLightingEnabled;
    public bool isHemiSphereLightingEnabled;
    public bool isHemiSphereOcclusionEnabled;
    public CullMode faceCulling;
    public bool isPolygonOffsetEnabled;
    public float polygonOffset;
    public uint unk0;
    public uint textureMappersUsed;
    public uint textureCoordsUsed;
    public readonly TexMapper[] texMappers = new TexMapper[3];
    public readonly TexCoords[] texCoords = new TexCoords[3];

    public Rgba32 emissionColor { get; private set; } = new();
    public Rgba32 ambientColor { get; private set; } = new();
    public Rgba32 diffuseColor { get; private set; } = new();
    public Rgba32 specular0Color { get; private set; } = new();
    public Rgba32 specular1Color { get; private set; } = new();

    public Rgba32[] constantColors { get; } =
      Arrays.From(6, () => new Rgba32());

    public readonly float[] bufferColor = new float[4];

    public BumpTexture bumpTexture;
    public BumpMode bumpMode;
    public bool isBumpRenormalized;

    public LayerConfig layerConfig;
    public FresnelConfig fresnelSelector;
    public bool isClampHighlight;
    public bool isDistribution0Enabled;
    public bool isDistribution1Enabled;
    public bool isGeometricFactor0Enabled;
    public bool isGeometricFactor1Enabled;
    public bool isReflectionEnabled;

    public readonly Sampler reflectanceRSampler = new();
    public readonly Sampler reflectanceGSampler = new();
    public readonly Sampler reflectanceBSampler = new();
    public readonly Sampler distibution0Sampler = new();
    public readonly Sampler distibution1Sampler = new();
    public readonly Sampler fresnelSampler = new();

    public uint texEnvStageCount;
    public short[] texEnvStagesIndices = new short[6];

    public bool alphaTestEnabled;
    public float alphaTestReferenceValue;
    public TestFunc alphaTestFunction;
    public bool depthTestEnabled;
    public bool depthWriteEnabled;
    public TestFunc depthTestFunction;
    public BlendMode blendMode;

    public BlendFactor alphaSrcFunc;
    public BlendFactor alphaDstFunc;
    public BlendEquation alphaEquation;
    public BlendFactor colorSrcFunc;
    public BlendFactor colorDstFunc;
    public BlendEquation colorEquation;
    public readonly float[] blendColor = new float[4];

    public bool stencilEnabled;
    public byte stencilReferenceValue;
    public byte bufferMask;
    public byte buffer;
    public TestFunc stencilFunc;
    public StencilTestOp failOP;
    public StencilTestOp zFailOP;
    public StencilTestOp zPassOP;
    public uint unk1; // CRC32 of something

    public void Read(IEndianBinaryReader r) {
      this.isFragmentLightingEnabled = r.ReadByte() != 0;
      this.isVertexLightingEnabled = r.ReadByte() != 0;
      this.isHemiSphereLightingEnabled = r.ReadByte() != 0;
      this.isHemiSphereOcclusionEnabled = r.ReadByte() != 0;

      this.faceCulling = (CullMode) r.ReadByte();

      this.isPolygonOffsetEnabled = r.ReadByte() != 0;
      this.polygonOffset = r.ReadInt16() / 65534f;

      if (CmbHeader.Version > Version.MAJORAS_MASK_3D) {
        this.unk0 = r.ReadUInt32();
        this.textureMappersUsed = (uint) r.ReadInt16();
        this.textureCoordsUsed = (uint) r.ReadInt16();
      } else {
        this.textureMappersUsed = r.ReadUInt32();
        this.textureCoordsUsed = r.ReadUInt32();
      }

      for (var i = 0; i < 3; ++i) {
        var texMapper = new TexMapper();
        texMapper.Read(r);
        this.texMappers[i] = texMapper;
      }
      for (var i = 0; i < 3; ++i) {
        var texCoord = new TexCoords();
        texCoord.Read(r);
        this.texCoords[i] = texCoord;
      }

      this.emissionColor.Read(r);
      this.ambientColor.Read(r);
      this.diffuseColor.Read(r);
      this.specular0Color.Read(r);
      this.specular1Color.Read(r);
      foreach (var constantColor in this.constantColors) {
        constantColor.Read(r);
      }
      r.ReadSingles(this.bufferColor);

      this.bumpTexture = (BumpTexture) r.ReadUInt16();
      this.bumpMode = (BumpMode) r.ReadUInt16();
      this.isBumpRenormalized = r.ReadUInt32() != 0;

      this.layerConfig = (LayerConfig) r.ReadUInt32();
      this.fresnelSelector = (FresnelConfig) r.ReadUInt16();
      this.isClampHighlight = r.ReadByte() != 0;
      this.isDistribution0Enabled = r.ReadByte() != 0;
      this.isDistribution1Enabled = r.ReadByte() != 0;
      this.isGeometricFactor0Enabled = r.ReadByte() != 0;
      this.isGeometricFactor1Enabled = r.ReadByte() != 0;
      this.isReflectionEnabled = r.ReadByte() != 0;

      this.reflectanceRSampler.Read(r);
      this.reflectanceGSampler.Read(r);
      this.reflectanceBSampler.Read(r);
      this.distibution0Sampler.Read(r);
      this.distibution1Sampler.Read(r);
      this.fresnelSampler.Read(r);

      this.texEnvStageCount = r.ReadUInt32();
      for (var i = 0; i < 6; ++i) {
        this.texEnvStagesIndices[i] = r.ReadInt16();
      }

      this.alphaTestEnabled = r.ReadByte() != 0;
      this.alphaTestReferenceValue = r.ReadByte() / 255f;
      this.alphaTestFunction = (TestFunc) r.ReadUInt16();
      this.depthTestEnabled = r.ReadByte() != 0;
      this.depthWriteEnabled = r.ReadByte() != 0;
      this.depthTestFunction = (TestFunc) r.ReadUInt16();
      this.blendMode = (BlendMode) (r.ReadByte());
      r.Align(4);

      this.alphaSrcFunc = (BlendFactor) (r.ReadUInt16());
      this.alphaDstFunc = (BlendFactor) (r.ReadUInt16());
      this.alphaEquation = (BlendEquation) (r.ReadUInt32());
      this.colorSrcFunc = (BlendFactor) (r.ReadUInt16());
      this.colorDstFunc = (BlendFactor) (r.ReadUInt16());
      this.colorEquation = (BlendEquation) (r.ReadUInt32());
      r.ReadSingles(this.blendColor);

      if (CmbHeader.Version.SupportsStencilBuffer()) {
        this.stencilEnabled = r.ReadByte() != 0;
        this.stencilReferenceValue = r.ReadByte();
        this.bufferMask = r.ReadByte();
        this.buffer = r.ReadByte();
        this.stencilFunc = (TestFunc) r.ReadUInt16();
        this.failOP = (StencilTestOp) r.ReadUInt16();
        this.zFailOP = (StencilTestOp) r.ReadUInt16();
        this.zPassOP = (StencilTestOp) r.ReadUInt16();
        this.unk1 = r.ReadUInt32();
      }
    }

    public void Write(ISubEndianBinaryWriter ew) {
      throw new NotImplementedException();
    }
  }
}