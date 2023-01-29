'''Noesis import plugin. Written by HimeWorks'''

from inc_noesis import *
import noesis
import rapi
import math
import struct

def registerNoesisTypes():
    '''Register the plugin. Just change the Game name and extension.'''
    
    handle = noesis.register("Midnight Club 2", ".xmod")
    noesis.logPopup()
    noesis.setHandlerTypeCheck(handle, noepyCheckType)
    noesis.setHandlerLoadModel(handle, noepyLoadModel)
    return 1

def noepyCheckType(data):
    '''Verify that the format is supported by this plugin. Default yes'''
    
    if len(data) < 4:
        return 0
    try:
        bs = NoeBitStream(data)
        idstring = bs.readline()
        return idstring.strip() == "version: 1.10"
    except:
        return 0
    
def noepyLoadModel(data, mdlList):
    '''Build the model, set materials, bones, and animations. You do not
    need all of them as long as they are empty lists (they are by default)'''
    
    parser = SanaeParser(data, mdlList)
    parser.parse_file()
    ctx = rapi.rpgCreateContext()
    return 1

class SanaeParser(object):
    
    def __init__(self, data, mdlList):    
        '''Initialize some data. Refer to Sanae.py to see what is already
        initialized'''
        
        self.inFile = NoeBitStream(data)
        self.vertBuffs = []
        self.idxBuffs = []
        self.animList = []
        self.texList = []
        self.matList = []
        self.boneList = []        
        self.numBones = 0
        self.mdlList = mdlList
        
    def read_name(self):
        
        string = self.inFile.readBytes(self.inFile.readUShort())
        try:
            return noeStrFromBytes(string)
        except:
            return "mesh"
        
    def parse_texture(self):
        texName = self.read_name()
        texSize = self.inFile.readUInt()
        texData = self.inFile.readBytes(texSize)
        self.inFile.seek(26, 1)
        
        tex = rapi.loadTexByHandler(texData, ".dds")
        if tex is not None:
            tex.name = texName
            
            self.texList.append(tex)
        return texName
        
    def parse_materials(self, numMat):
        
        for i in range(numMat):
            diffuse = self.inFile.read('4f')
            ambient = self.inFile.read('4f')
            specular = self.inFile.read('4f')
            emissive = self.inFile.read('4f')
            power = self.inFile.readFloat()
            
            matName = "material[%d]" %len(self.matList)
            texName = self.parse_texture()
            
            material = NoeMaterial(matName, texName)
            material.setDefaultBlend(0)
            self.matList.append(material)
    
    def parse_animations(self, numAnims):
        
        for i in range(numAnims):
            name = self.read_name()
            numBones = self.inFile.readUShort()
            for j in range(numBones):
                boneName = self.read_name()
                numTranslate = self.inFile.readUInt()
                for k in range(numTranslate):
                    keyFrameSec = self.inFile.readFloat()
                    x, y, z = self.inFile.read('3f')
                    keyFrameIndex = int(math.floor(keyFrameSec*100))
                    
                numRotate = self.inFile.readUInt()
                for k in range(numRotate):
                    keyFrameSec = self.inFile.readFloat()
                    x, y, z, w = self.inFile.read('4f')
                    keyFrameIndex = int(math.floor(keyFrameSec*100))                    
    def parse_bones(self, numBones):
        
        for i in range(numBones):
            boneName = self.read_name()
            parentIndex = self.inFile.readUInt()
            boneMatrix = self.inFile.read('16f')
            parentMatrix = self.inFile.read('16f')
    
    def parse_vertices(self, numVerts):
        
        verts = []
        for i in range(numVerts):
          tokens = self.inFile.readline().strip().split("\t")
          verts.extend(map(float, tokens[1:]))
        return struct.pack("%df" %len(verts), *verts)
          
        
    def parse_faces(self, numIdx):
        
        return self.inFile.readBytes(numIdx*2)
    
    def parse_weights(self):
        
        numWeights = self.inFile.readUShort()
        for i in range(numWeights):
            for j in range(self.numBones):
                count = self.inFile.readUInt()
                boneIDs = self.inFile.read('%dL' %count)
                weights = self.inFile.read('%df' %count)
    
    def parse_meshes(self, numMesh):
        
        print(numMesh, "meshes")
        for i in range(numMesh):
            meshName = self.read_name()
            worldMatrix = self.inFile.read('16f')
            localMatrix = self.inFile.read('16f')
            self.inFile.readInt()
            matNum = self.inFile.readByte()
            numVerts = self.inFile.readUShort()
            numIdx = self.inFile.readUShort() * 3
            vertBuff = self.parse_vertices(numVerts)
            idxBuff = self.parse_faces(numIdx)
            chunk = self.inFile.readUInt()
            if chunk == 0x41470205:
                self.parse_weights()
            else:
                self.inFile.seek(-4, 1)
            
            self.vertBuffs.append([vertBuff, numVerts])
            self.idxBuffs.append([idxBuff, numIdx, matNum])
            
    def get_value(self):
        line = self.inFile.readline()
        tokens = line.split(":")
        return int(tokens[1])
        
    def parse_file(self):
        '''Main parser method'''
        
        version = self.inFile.readline()
        numVerts = self.get_value()
        norms = self.get_value()
        colors = self.get_value()
        tex1s = self.get_value()
        tex2s = self.get_value()
        tangents = self.get_value()
        numMat = self.get_value()
        adjuncts = self.get_value()
        primitives = self.get_value()
        matrices = self.get_value()
        reskins = self.get_value()
        self.inFile.readline()
        vertBuff = self.parse_vertices(numVerts)
        
        rapi.rpgReset()
        rapi.rpgBindPositionBufferOfs(vertBuff, noesis.RPGEODATA_FLOAT, 12, 0)
        rapi.rpgCommitTriangles(None, noesis.RPGEODATA_USHORT, 0, noesis.RPGEO_POINTS, 1)    
        mdl = rapi.rpgConstructModel()
        self.mdlList.append(mdl)