import sys, os
from .io_utils import (align, readDataType, readString, readArray, readFloat,
                    readUInt32, readInt32, readUShort,
                    readShort, readByte, readUByte)
from .cmbEnums import *
from .common import GLOBAL_SCALE

Version = CmbVersion.OoT3D

class Cmb(object):
    def __init__(self):
        self.version = Version
        self.name = "Dummy CMB"
        self.texDataOfs = 0
        self.indicesOfs = 0
        self.vatrOfs = 0
        self.skeleton = [Bone()]
        self.materials = [Material()]
        self.textures = [Texture()]
        self.meshes = [Mesh()]
        self.shapes = [Sepd()]
        self.vatr = Vatr()

    def read(self, f, startOff):
        f.seek(startOff)

        header = CmbHeader().read(f)
        skl = Skl().read(f)# Skeleton
        #qtrs = Qtrs().read(f) if (Version > 6) else Qtrs()
        f.seek(header.matsOfs + startOff)
        mat = Mat().read(f)# Materials
        tex = Tex().read(f)# Textures
        sklm = Sklm().read(f)# Skeleton Meshes
        f.seek(header.vatrOfs + startOff)
        vatr = Vatr().read(f)# Vertex Attributes

        # Add face indices to primitive sets
        for shape in sklm.shapes:
            for pset in shape.primitiveSets:
                f.seek((header.faceIndicesOfs + pset.primitive.offset * 2) + startOff)# Always * 2 even if ubyte is used...
                pset.primitive.indices = [int(readDataType(f, pset.primitive.dataType)) for _ in range(pset.primitive.indicesCount)]

        self.skeleton = skl.bones
        self.materials = mat.materials# TODO: Combiners
        self.textures = tex.textures
        self.meshes = sklm.meshes
        self.shapes = sklm.shapes
        self.vatr = vatr

        self.texDataOfs = header.textureDataOfs
        self.indicesOfs = header.faceIndicesOfs
        self.vatrOfs = header.vatrOfs
        self.name = header.name
        self.version = Version

        return self


class Sepd(object):
    def __init__(self):
        self.magic = "sepd"# SEParateDataShape
        self.chunkSize = 0
        self.primSetCount = 1
        self.vertFlags = 1

        self.meshCenter = [0.0, 0.0, 0.0]
        self.positionOffset = [0.0, 0.0, 0.0]
        self.Mix = [-1.0, -1.0, -1.0]
        self.Max = [0.0, 0.0, 0.0]

        self.position = VertexAttribute()
        self.normal = VertexAttribute()
        self.tangents = VertexAttribute()
        self.color = VertexAttribute()
        self.uv0 = VertexAttribute()
        self.uv1 = VertexAttribute()
        self.uv2 = VertexAttribute()
        self.bIndices = VertexAttribute()
        self.bWeights = VertexAttribute()

        self.boneDimensions = 1
        self.constantFlags = 0
        self.primitiveSets = [PrimitiveSet()]

    def read(self,f):
        self.magic = readString(f,4)
        self.chunkSize = readUInt32(f)
        self.primSetCount = readUShort(f)# PrimitiveSet Count

    #Bit Flags: (HasTangents was added in versions > OoT:3D (aka 6))
        # HasPosition : 00000001
        # HasNormals  : 00000010
        # HasTangents : 00000100 (MM3D/LM3D/EO only)
        # HasColors   : 00000100
        # HasUV0      : 00001000
        # HasUV1      : 00010000
        # HasUV2      : 00100000
        # HasIndices  : 01000000
        # HasWeights  : 10000000
        self.vertFlags = readUShort(f)

        self.meshCenter = readArray(f, 3)
        self.positionOffset = readArray(f, 3)

        if(Version > 12):
            self.Min = readArray(f, 3)# Max coordinate of the shape
            self.Max = readArray(f, 3)# Min coordinate of the shape

        self.position = VertexAttribute().read(f)

        self.normal = VertexAttribute().read(f)
        self.tangents = VertexAttribute().read(f) if (Version > 6) else self.tangents
        self.color = VertexAttribute().read(f)
        self.uv0 = VertexAttribute().read(f)
        self.uv1 = VertexAttribute().read(f)
        self.uv2 = VertexAttribute().read(f)
        self.bIndices = VertexAttribute().read(f)
        self.bWeights = VertexAttribute().read(f)

        self.boneDimensions = readUShort(f)# How many weights each vertex has for this shape

        # Note: Constant values are set in "VertexAttribute" (Use constants instead of an array to save space, assuming all values are the same)
        #Bit Flags:
        # PositionUseConstant : 00000001
        # NormalsUseConstant  : 00000010
        # TangentsUseConstant : 00000100 (MM3D/LM3D/EO only)
        # ColorsUseConstant   : 00000100
        # UV0UseConstant      : 00001000
        # UV1UseConstant      : 00010000
        # UV2UseConstant      : 00100000
        # IndicesUseConstant  : 01000000
        # WeightsUseConstant  : 10000000
        self.constantFlags = readUShort(f)

        [readShort(f) for _ in range(self.primSetCount)]# PrimitiveSetOffset(s)
        align(f)# 4 byte alignment
        self.primitiveSets = [PrimitiveSet().read(f) for _ in range(self.primSetCount)]

        return self



class Material(object):
    def __init__(self):
        self.isFragmentLightingEnabled = False
        self.isVertexLightingEnabled = True
        self.isHemiSphereLightingEnabled = True
        self.isHemiSphereOcclusionEnabled = False
        self.faceCulling = CullMode.Front
        self.isPolygonOffsetEnabled = False
        self.polygonOffset = 0.0

        self.Unk0 = 0# Sometimes 1 (Haven't tested changes in-game yet)
        self.TextureMappersUsed = 0
        self.TextureCoordsUsed = 0
        self.TextureMappers = [TexMapper() for _ in range(3)]
        self.TextureCoords  = [TexCoords() for _ in range(3)]

        self.emissionColor  = [0, 0, 0, 0]
        self.ambientColor   = [102, 102, 102, 0]
        self.diffuseColor   = [127, 127, 127, 255]
        self.specular0Color = [255, 255, 255, 255]
        self.specular1Color = [0, 0, 0, 0]
        self.constantColors = [[0, 0, 0, 255] for _ in range(6)]
        self.bufferColor    = [0.0, 0.0, 0.0, 1.0]

        self.bumpTexture = BumpTexture.Texture0
        self.bumpMode = BumpMode.NotUsed
        self.isBumpRenormalize = False

        self.layerConfig = LayerConfig.LayerConfig0
        self.FresnelSelector = FresnelConfig.No
        self.isClampHighlight = False
        self.isDistribution0Enabled = False
        self.isDistribution1Enabled = False
        self.isGeometricFactor0Enabled = False
        self.isGeometricFactor1Enabled = False
        self.IsReflectionEnabled = False

        self.reflectanceRSampler = Sampler()
        self.reflectanceGSampler = Sampler()
        self.reflectanceBSampler = Sampler()
        self.distibution0Sampler = Sampler()
        self.distibution1Sampler = Sampler()
        self.fresnelSampler      = Sampler()
        self.texEnvStageCount = 0
        self.texEnvStagesIndices = [-1,-1,-1,-1,-1,-1]
        self.texEnvStages = [Combiner()]

        self.alphaTestEnabled = True
        self.alphaTestReferenceValue = 128
        self.alphaTestFunction = TestFunc.Greater
        self.depthTestEnabled = True
        self.depthWriteEnabled = True
        self.depthTestFunction = TestFunc.Less
        self.blendMode = BlendMode.BlendNone

        self.alphaSrcFunc = BlendFactor.SourceAlpha
        self.alphaDstFunc = BlendFactor.OneMinusSourceAlpha
        self.alphaEquation = BlendEquation.FuncAdd
        self.colorSrcFunc = BlendFactor.One
        self.colorDstFunc = BlendFactor.Zero
        self.colorEquation = BlendEquation.FuncAdd
        self.blendColor = [0.0, 0.0, 0.0, 1.0]

        self.stencilEnabled = False
        self.stencilReferenceValue = 0
        self.bufferMask = 255
        self.buffer = 0
        self.StencilFunc = TestFunc.Never
        self.failOP = StencilTestOp.Keep
        self.zFailOP = StencilTestOp.Keep
        self.zPassOP = StencilTestOp.Keep
        self.Unk1 = 0

    def read(self,f):
        self.isFragmentLightingEnabled = readUByte(f) != 0
        self.isVertexLightingEnabled = readUByte(f) != 0
        self.isHemiSphereLightingEnabled = readUByte(f) != 0
        self.isHemiSphereOcclusionEnabled = readUByte(f) != 0
        self.faceCulling = CullMode(readUByte(f))
        self.isPolygonOffsetEnabled = readUByte(f) != 0
        self.polygonOffset = float(readShort(f) / 65534)

        if(Version > 10):
            self.Unk0 = readUInt32(f)
            self.TextureMappersUsed = readShort(f)
            self.TextureCoordsUsed = readShort(f)
        else:
            self.TextureMappersUsed = readUInt32(f)
            self.TextureCoordsUsed = readUInt32(f)

        self.TextureMappers = [TexMapper().read(f) for _ in range(3)]
        self.TextureCoords = [TexCoords().read(f) for _ in range(3)]

        self.emissionColor = readArray(f, 4, DataTypes.UByte)
        self.ambientColor = readArray(f, 4, DataTypes.UByte)
        self.diffuseColor = readArray(f, 4, DataTypes.UByte)
        self.specular0Color = readArray(f, 4, DataTypes.UByte)
        self.specular1Color = readArray(f, 4, DataTypes.UByte)
        self.constantColors = [readArray(f, 4, DataTypes.UByte) for _ in range(6)]
        self.bufferColor = readArray(f, 4)

        self.bumpTexture = BumpTexture(readUShort(f))
        self.bumpMode = BumpMode(readUShort(f))
        self.isBumpRenormalize = readUInt32(f) != 0

        self.layerConfig = LayerConfig(readUInt32(f))
        self.FresnelSelector = FresnelConfig(readUShort(f))
        self.isClampHighlight = readUByte(f) != 0
        self.isDistribution0Enabled = readUByte(f) != 0
        self.isDistribution1Enabled = readUByte(f) != 0
        self.isGeometricFactor0Enabled = readUByte(f) != 0
        self.isGeometricFactor1Enabled = readUByte(f) != 0
        self.IsReflectionEnabled = readUByte(f) != 0

        self.reflectanceRSampler = Sampler().read(f)
        self.reflectanceGSampler = Sampler().read(f)
        self.reflectanceBSampler = Sampler().read(f)
        self.distibution0Sampler = Sampler().read(f)
        self.distibution1Sampler = Sampler().read(f)
        self.fresnelSampler      = Sampler().read(f)

        self.texEnvStageCount = readUInt32(f)
        self.texEnvStagesIndices = [readShort(f) for _ in range(6)]

        self.alphaTestEnabled = readUByte(f) != 0
        self.alphaTestReferenceValue = readUByte(f) / 255
        self.alphaTestFunction = TestFunc(readUShort(f))
        self.depthTestEnabled = readUByte(f) != 0
        self.depthWriteEnabled = readUByte(f) != 0
        self.depthTestFunction = TestFunc(readUShort(f))
        self.blendMode = BlendMode(readUByte(f))
        align(f)

        self.alphaSrcFunc = BlendFactor(readUShort(f))
        self.alphaDstFunc = BlendFactor(readUShort(f))
        self.alphaEquation = BlendEquation(readUInt32(f))
        self.colorSrcFunc = BlendFactor(readUShort(f))
        self.colorDstFunc = BlendFactor(readUShort(f))
        self.colorEquation = BlendEquation(readUInt32(f))
        self.blendColor = readArray(f, 4)

        if(Version > 6):
            self.stencilEnabled = readUByte(f) != 0
            self.stencilReferenceValue = readUByte(f)
            self.bufferMask = readUByte(f)
            self.buffer = readUByte(f)
            self.StencilFunc = TestFunc(readUShort(f))
            self.failOP = StencilTestOp(readUShort(f))
            self.zFailOP = StencilTestOp(readUShort(f))
            self.zPassOP = StencilTestOp(readUShort(f))
            self.Unk1 = readUInt32(f)# CRC32 of something?
        return self

class Mat(object):
    def __init__(self):
        self.magic = "mats"# MATerials
        self.chunkSize = 348
        self.matCount = 0
        self.materials = []

    def read(self,f):
        self.magic = readString(f, 4)
        self.chunkSize = readUInt32(f)
        self.matCount = readUInt32(f)
        self.materials = [Material().read(f) for _ in range(self.matCount)]
        combiners = []

        for m in self.materials:
            for _ in range(m.texEnvStageCount): combiners.append(Combiner().read(f))

        for m in self.materials:
            m.texEnvStages = []# Make sure combiners are empty
            for index in m.texEnvStagesIndices:
                if(index != -1):
                    m.texEnvStages.append(combiners[index])
        return self


class Shp(object):
    def __init__(self):
        self.magic = "shp\x20"#SHaPe
        self.chunkSize = 16
        self.shapeCount = 0
        #No idea... but it does something to materials and it's never used on ANY model but link's in OoT3D
        #Set to 0x58 on "link_v2.cmb"
        self.flags = 0
        self.shapes = []

    def read(self,f):
        self.magic = readString(f, 4)
        self.chunkSize = readUInt32(f)
        self.shapeCount = readUInt32(f)
        self.flags = readUInt32(f)

        [readShort(f) for _ in range(self.shapeCount)]# ShapeOffset(s)
        align(f)
        self.shapes = [Sepd().read(f) for _ in range(self.shapeCount)]
        return self

class Sklm(object):
    def __init__(self):
        self.magic = "sklm"# SKeLetal Model
        self.chunkSize = 0
        self.mshOffset = 0
        self.shpOffset = 0
        self.meshes = []
        self.shapes = []

    def read(self,f):
        self.magic = readString(f, 4)
        self.chunkSize = readUInt32(f)
        self.mshOffset = readUInt32(f)
        self.shpOffset = readUInt32(f)
        self.meshes = Mshs().read(f).meshes
        self.shapes = Shp().read(f).shapes
        return self

class AttributeSlice(object):
    def __init__(self):
        self.size = 0
        self.startOfs = 0
    def read(self,f):
        self.size = readUInt32(f)
        self.startOfs = readUInt32(f)
        return self

class Vatr(object):
    def __init__(self):
        self.magic = "vatr"
        self.chunkSize = 0
        self.maxIndex = 3

        self.position = AttributeSlice()
        self.normal = AttributeSlice()
        self.tangent = AttributeSlice()
        self.color = AttributeSlice()
        self.uv0 = AttributeSlice()
        self.uv1 = AttributeSlice()
        self.uv2 = AttributeSlice()
        self.bIndices = AttributeSlice()
        self.bWeights = AttributeSlice()

    def read(self,f):
        self.magic = readString(f, 4)
        self.chunkSize = readUInt32(f)
        self.maxIndex = readUInt32(f)# i.e., vertex count of model

        # Basically just used to get each attibute into it's own byte[] (We won't be doing that here)
        self.position = AttributeSlice().read(f)
        self.normal = AttributeSlice().read(f)
        self.tangent = AttributeSlice().read(f) if (Version > 6) else self.tangent
        self.color = AttributeSlice().read(f)
        self.uv0 = AttributeSlice().read(f)
        self.uv1 = AttributeSlice().read(f)
        self.uv2 = AttributeSlice().read(f)
        self.bIndices = AttributeSlice().read(f)
        self.bWeights = AttributeSlice().read(f)
        return self

def readCmb(fileio, startOff):
    return Cmb().read(fileio, startOff)
