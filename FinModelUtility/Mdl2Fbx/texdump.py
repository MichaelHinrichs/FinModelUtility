from PyQt4 import QtGui
import btypes as bt

class Color:

  def __init__(self,r=0,g=0,b=0,a=0xFF):
    self.r = r
    self.g = g
    self.b = b
    self.a = a

  @staticmethod
  def from_i4(color):
    c = color << 4 | color
    return Color(c,c,c,c)

  @staticmethod
  def from_i8(color):
    return Color(color,color,color,color)

  @staticmethod
  def from_ia4(color):
    c = ((color & 0xF) << 4) | color & 0xF
    a = (color & 0xF0) | (color >> 4)
    return Color(c,c,c,a)

  @staticmethod
  def from_ia8(color):
    i = color & 0xFF
    a = color >> 8
    return Color(i,i,i,a)

  @staticmethod
  def from_rgb565(color):
    r = ((color >> 8) & 0xF8) | ((color >> 11) & 0x7)
    g = ((color >> 3) & 0xFC) | ((color >> 5) & 0x3)
    b = ((color << 3) & 0xF8) | (color & 0x7)
    return Color(r,g,b)

  @staticmethod
  def from_rgb5a3(color):
    if color & 0x8000:
      r = ((color >> 7) & 0xF8) | ((color >> 12) & 0x7)
      g = ((color >> 2) & 0xF8) | ((color >> 5) & 0x7)
      b = ((color << 3) & 0xF8) | (color & 0x7)
      return Color(r,g,b)
    else:
      r = ((color >> 4) & 0xF0) | ((color >> 8) & 0xF)
      g = (color & 0xF0) | ((color >> 4) & 0xF)
      b = ((color << 4) & 0xF0) | (color & 0xF)
      a = ((color >> 7) & 0xE0) | ((color >> 10) & 0x1C) | ((color >> 13) & 0x3)
      return Color(r,g,b,a)

  def to_argb8(self):
    return (self.a << 24) | (self.r << 16) | (self.g << 8) | self.b

  def __add__(self,c):
    return Color(self.r + c.r,self.g + c.g,self.b + c.b,self.a + c.a)

  def __mul__(self,s):
    return Color(self.r*s,self.g*s,self.b*s,self.a*s)

  def __rmul__(self,s):
    return Color(s*self.r,s*self.g,s*self.b,s*self.a)

  def __truediv__(self,s):
    return Color(self.r//s,self.g//s,self.b//s,self.a//s)


def unpack_i4(stream,width,height):
  image = QtGui.QImage(width,height,QtGui.QImage.Format_RGB32)

  for tile_row in range(height//8):
    for tile_col in range(width//8):
      for i in range(8):
        for j in range(4):
          colors = stream >> bt.uint8
          color0 = Color.from_i4(colors & 0xF)
          color1 = Color.from_i4(colors >> 4)
          image.setPixel(8*tile_col + 2*j,8*tile_row + i,color0.to_argb8())
          image.setPixel(8*tile_col + 2*j + 1,8*tile_row + i,color1.to_argb8())
  
  return image


def unpack_i8(stream,width,height):
  image = QtGui.QImage(width,height,QtGui.QImage.Format_RGB32)

  for tile_row in range(height//4):
    for tile_col in range(width//8):
      for i in range(4):
        for j in range(8):
          color = Color.from_ia8(stream >> bt.uint8)
          image.setPixel(8*tile_col + j,4*tile_row + i,color.to_argb8())

  return image


def unpack_ia4(stream,width,height):
  image = QtGui.QImage(width,height,QtGui.QImage.Format_ARGB32)

  for tile_row in range(height//4):
    for tile_col in range(width//8):
      for i in range(4):
        for j in range(8):
          color = Color.from_ia4(stream >> bt.uint8)
          image.setPixel(8*tile_col + j,4*tile_row + i,color.to_argb8())

  return image


def unpack_ia8(stream,width,height):
  image = QtGui.QImage(width,height,QtGui.QImage.Format_ARGB32)

  for tile_row in range(height//4):
    for tile_col in range(width//4):
      for i in range(4):
        for j in range(4):
          color = Color.from_ia8(stream >> bt.uint16)
          image.setPixel(4*tile_col + j,4*tile_row + i,color.to_argb8())

  return image


def unpack_rgb565(stream,width,height):
  image = QtGui.QImage(width,height,QtGui.QImage.Format_RGB32)

  for tile_row in range(height//4):
    for tile_col in range(width//4):
      for i in range(4):
        for j in range(4):
          color = Color.from_rgb565(stream >> bt.uint16)
          image.setPixel(4*tile_col + j,4*tile_row + i,color.to_argb8())

  return image


def unpack_rgb5a3(stream,width,height):
  image = QtGui.QImage(width,height,QtGui.QImage.Format_RGB32)

  for tile_row in range(height//4):
    for tile_col in range(width//4):
      for i in range(4):
        for j in range(4):
          color = Color.from_rgb5a3(stream >> bt.uint16)
          image.setPixel(4*tile_col + j,4*tile_row + i,color.to_argb8())

  return image


def unpack_rgba8(stream,width,height):
  image = QtGui.QImage(width,height,QtGui.QImage.Format_ARGB32)

  for tile_row in range(height//4):
    for tile_col in range(width//4):
      tile = stream.read(0x40)
      for i in range(4):
        for j in range(4):
          color = Color(tile[8*i + 2*j + 1],tile[0x20 + 8*i + 2*j],tile[0x20 + 8*i + 2*j + 1],tile[8*i + 2*j])
          image.setPixel(4*tile_col + j,4*tile_row + i,color.to_argb8())

  return image


def unpack_cmpr(stream,width,height):
  image = QtGui.QImage(width,height,QtGui.QImage.Format_ARGB32)

  color = [None]*4
  for tile_row in range(height//8):
    for tile_col in range(width//8):
      for sub_tile_row in range(2):
        for sub_tile_col in range(2):
          color0 = stream >> bt.uint16
          color1 = stream >> bt.uint16
          color[0] = Color.from_rgb565(color0)
          color[1] = Color.from_rgb565(color1)
          if color0 > color1:
            color[2] = (2*color[0] + color[1])/3
            color[3] = (2*color[1] + color[0])/3
          else:
            color[2] = (color[0] + color[1])/2
            color[3] = (2*color[1] + color[0])/3
            color[3].a = 0
          index = stream >> bt.uint32
          argb8_color = [c.to_argb8() for c in color]
          for i in range(4):
            for j in range(4):
              image.setPixel(8*tile_col + 4*sub_tile_col + j,8*tile_row + 4*sub_tile_row + i,argb8_color[(index >> (30 - 2*(4*i + j))) & 0x3])

  return image

