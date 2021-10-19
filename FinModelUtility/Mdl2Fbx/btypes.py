"""Library for reading and writing data structures.

"""

from struct import pack as data_pack,unpack as data_unpack
import io

#------------------------------------------------------------------------------

class Packer:

  def __init__(self,btype,value):
    self.btype = btype
    self.value = value

  def pack(self,stream):
    self.btype.pack(self.value,stream)

#------------------------------------------------------------------------------

class BasicType:

  def __init__(self,basic,size):
    self.basic = basic
    self.size = size

  def pack(self,value,stream):
    stream.write(data_pack(stream.endian + self.basic,value))

  def unpack(self,stream):
    return data_unpack(stream.endian + self.basic,stream.read(self.size))[0]

  def sizeof(self):
    return self.size

  def __call__(self,value):
    return Packer(self,value)

bool8 = BasicType('?',1)
sint8 = BasicType('b',1)
uint8 = BasicType('B',1)
sint16 = BasicType('h',2)
uint16 = BasicType('H',2)
sint32 = BasicType('l',4)
uint32 = BasicType('L',4)
sint64 = BasicType('q',8)
uint64 = BasicType('Q',8)
float32 = BasicType('f',4)
float64 = BasicType('d',8)

#------------------------------------------------------------------------------

class FixedPoint: #<-?

  def __init__(self,integer_type,scale):
    self.integer_type = integer_type
    self.scale = scale

  def pack(self,value,stream):
    self.integer_type.pack(int(value/self.scale),stream)

  def unpack(self,stream):
    return self.integer_type.unpack(stream)*self.scale

  def sizeof(self):
    return self.integer_type.sizeof()

  def __call__(self,value):
    return Packer(self,value)


class ByteString:

  def __init__(self,length):
    self.length = length

  def pack(self,string,stream):
    stream.write(string)

  def unpack(self,stream):
    return stream.read(self.length)

  def sizeof(self):
    return self.length


class Array:

  def __init__(self,element_type,length):
    self.element_type = element_type
    self.length = length

  def pack(self,array,stream):
    for value in array:
      self.element_type.pack(value,stream)

  def unpack(self,stream):
    return [self.element_type.unpack(stream) for i in range(self.length)]

  def sizeof(self):
    return self.element_type.sizeof()*self.length


class CString:

  def __init__(self,encoding):
    self.encoding = encoding

  def pack(self,string,stream):
    stream.write((string + '\0').encode(self.encoding))

  # NOTICE: This might not work for all encodings.
  # But it works for ascii, UTF-8, UTF-16 and Shift JIS.
  def unpack(self,stream):
    null = '\0'.encode(self.encoding)
    string = b''
    while True:
      c = stream.read(len(null))
      if c == null: break
      string += c
    return string.decode(self.encoding)

  def __call__(self,string):
    return Packer(self,string)

cstring = CString('ascii')


class Align:

  default_padding = b'This is padding data to alignment.' # sic

  def __init__(self,length,padding=default_padding):
    self.length = length
    self.padding = padding

  def pack(self,stream):
    if stream.tell() % self.length == 0: return
    n,r = divmod(self.length - (stream.tell() % self.length),len(self.padding))
    stream.write(n*self.padding + self.padding[0:r])

  def unpack(self,stream):
    if stream.tell() % self.length == 0: return
    stream.read(self.length - (stream.tell() % self.length))

#------------------------------------------------------------------------------

def Struct(*fields):
  size = sum(field.size for field in fields)

  class StructType:

    #__slots__ = tuple() #TODO

    def pack(self,stream):
      for field in fields:
        field.pack_part(self,stream)

    @classmethod
    def unpack(cls,stream):
      struct = cls.__new__(cls) #TODO: what if __init__ does something important?
      for field in fields:
        field.unpack_part(struct,stream)
      return struct

    @staticmethod
    def sizeof():
      return size

  return StructType


class Field:

  def __init__(self,field_type,name):
    self.type = field_type #<-?
    self.size = field_type.sizeof()
    self.name = name

  def pack_part(self,struct,stream):
    self.type.pack(getattr(struct,self.name),stream)

  def unpack_part(self,struct,stream):
    setattr(struct,self.name,self.type.unpack(stream))

class Padding:

  def __init__(self,size,padding=b'\xFF'):
    self.size = size
    self.padding = padding

  def pack_part(self,struct,stream):
    stream.write(self.padding*self.size)

  def unpack_part(self,struct,stream):
    stream.read(self.size)

#------------------------------------------------------------------------------

NE = '@' # native endian
LE = '<' # little endian
BE = '>' # big endian

class StreamBase:

  def __init__(self,endian):
    self.endian = endian

  def __lshift__(self,packer):
    packer.pack(self)
    return self

  def __rshift__(self,btype):
    return btype.unpack(self)


class FileStream(StreamBase,io.FileIO):
  def __init__(self,filename,mode='r',endian=NE):
    StreamBase.__init__(self,endian)
    io.FileIO.__init__(self,filename,mode)

#------------------------------------------------------------------------------

class Block:
  class Stream(StreamBase):
    def __init__(self,block,position=0):
      super().__init__(block.endian)
      self.block = block
      self.position = position

    def read(self,length):
      buf = self.block[self.position:self.position + length]
      self.position += length
      return buf

    def seek(self,pos,w):
      self.position += pos

  class Pointer:
    def __init__(self,block,value_type,address):
      self.block = block
      self.value_type = value_type
      self.address = address

    def __getitem__(self,key):
      return self.block.stream(self.address) >> self.value_type

  def __init__(self,block,endian=NE):
    self._wrapped_block = block
    self.endian = endian

  def __getitem__(self,key):
    return self._wrapped_block[key]

  def stream(self,position=0):
    return Block.Stream(self,position)

  def pointer(self,value_type,address):
    return Block.Pointer(self,value_type,address)

#______________________________________________________________________________
