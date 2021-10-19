import os
import argparse
import numpy as np
import btypes as bt
import texdump

#------------------------------------------------------------------------------

class Header(bt.Struct(
  bt.Field(bt.uint32,'magic'), # magic == 0x04B40000
  bt.Field(bt.uint16,'num_faces'),
  bt.Padding(2,b'\x00'),
  bt.Field(bt.uint16,'num_nodes'), # num_nodes == num_joints
  bt.Field(bt.uint16,'num_shape_packets'),
  bt.Field(bt.uint16,'num_weighted_matrices'),
  bt.Field(bt.uint16,'num_joints'),
  bt.Field(bt.uint16,'num_vertices'),
  bt.Field(bt.uint16,'num_normals'),
  bt.Field(bt.uint16,'num_colors'),
  bt.Field(bt.uint16,'num_texcoords'),
  bt.Padding(8,b'\x00'),
  bt.Field(bt.uint16,'num_textures'),
  bt.Padding(2,b'\x00'),
  bt.Field(bt.uint16,'num_texobjs'),
  bt.Field(bt.uint16,'num_draw_elements'), # num_draw_elements == num_shapes
  bt.Field(bt.uint16,'num_materials'),
  bt.Field(bt.uint16,'num_shapes'),
  bt.Padding(4,b'\x00'),
  bt.Field(bt.uint32,'node_offset'),
  bt.Field(bt.uint32,'shape_packet_offset'),
  bt.Field(bt.uint32,'matrix_offset'),
  bt.Field(bt.uint32,'weight_offset'),
  bt.Field(bt.uint32,'joint_index_offset'),
  bt.Field(bt.uint32,'num_weights_offset'),
  bt.Field(bt.uint32,'vertex_offset'),
  bt.Field(bt.uint32,'normal_offset'),
  bt.Field(bt.uint32,'color_offset'),
  bt.Field(bt.uint32,'texcoord_offset'),
  bt.Padding(8,b'\x00'),
  bt.Field(bt.uint32,'texture_location_offset'),
  bt.Padding(4,b'\x00'),
  bt.Field(bt.uint32,'material_offset'),
  bt.Field(bt.uint32,'texobj_offset'),
  bt.Field(bt.uint32,'shape_offset'),
  bt.Field(bt.uint32,'draw_element_offset'),
  bt.Padding(8,b'\x00'))): pass


class Vector(bt.Struct(
  bt.Field(bt.float32,'x'),
  bt.Field(bt.float32,'y'),
  bt.Field(bt.float32,'z'))): pass


class TextureCoordinate(bt.Struct(
  bt.Field(bt.float32,'s'),
  bt.Field(bt.float32,'t'))): pass


class Color(bt.Struct(
  bt.Field(bt.uint8,'r'),
  bt.Field(bt.uint8,'g'),
  bt.Field(bt.uint8,'b'),
  bt.Field(bt.uint8,'a'))): pass


class Matrix:

  IDENTITY = np.matrix([[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]])

  @staticmethod
  def unpack(stream):
    row0 = [stream >> bt.float32 for i in range(4)]
    row1 = [stream >> bt.float32 for i in range(4)]
    row2 = [stream >> bt.float32 for i in range(4)]
    return np.matrix([row0,row1,row2,[0,0,0,1]])


class Node(bt.Struct(
  bt.Field(bt.uint16,'unknown0'),
  bt.Field(bt.uint16,'unknown1'), # unknown1 in (0,1)
  bt.Field(bt.uint16,'unknown2'),
  bt.Field(bt.uint16,'unknown3'), # unknown3 in (0,1)
  bt.Field(bt.uint16,'unknown4'),
  bt.Field(bt.uint16,'unknown5'),
  bt.Padding(4,b'\x00'))): pass


class TextureHeader(bt.Struct(
  bt.Field(bt.uint8,'format'),
  bt.Padding(1,b'\x00'),
  bt.Field(bt.uint16,'width'),
  bt.Field(bt.uint16,'height'),
  bt.Padding(26,b'\x00'))): pass


class TexObj(bt.Struct(
  bt.Field(bt.uint16,'texture_index'),
  bt.Padding(2),
  bt.Field(bt.uint8,'unknown1'), # unknown1 in (0,1,2)
  bt.Field(bt.uint8,'unknown2'), # unknown2 in (0,1,2)
  bt.Field(bt.uint8,'unknown3'), # unknown3 in (0,1,4)
  bt.Padding(1,b'\x00'))): pass


class TevStage(bt.Struct(
  bt.Field(bt.uint16,'unknown0'), # unknown0 in (0,0x200)
  bt.Field(bt.uint16,'texobj_index'),
  bt.Field(bt.Array(bt.float32,7),'unknown1'))): pass


class Material(bt.Struct(
  bt.Field(Color,'color'),
  bt.Field(bt.uint16,'unknown1'), # unknown1 in (0,1), number of colors?
  bt.Field(bt.uint8,'unknown2'), # unknown2 in (0,1,2,4)
  bt.Field(bt.uint8,'num_tev_stages'), # unknown3 in (0,1,2)
  bt.Field(bt.uint8,'unknown4'), # unknown4 in (0,1)
  bt.Padding(23,b'\x00'),
  bt.Field(bt.Array(TevStage,8),'tev_stages'))): pass


class Shape(bt.Struct(
  bt.Field(bt.uint8,'unknown0'), # unknown0 in (0,1,3)
  bt.Field(bt.uint8,'unknown1'), # unknown1 == 0
  bt.Field(bt.uint8,'unknown2'),
  bt.Field(bt.uint8,'unknown3'), # unknown3 in (0,1)
  bt.Field(bt.uint16,'num_packets'),
  bt.Field(bt.uint16,'first_packet'))): pass


class ShapePacket(bt.Struct(
  bt.Field(bt.uint32,'data_offset'),
  bt.Field(bt.uint32,'data_size'),
  bt.Field(bt.uint16,'unknown0'), # unknown0 in (0,2)
  bt.Field(bt.uint16,'num_matrix_indices'),
  bt.Field(bt.Array(bt.uint16,10),'matrix_indices'))): pass


class DrawElement(bt.Struct(
  bt.Field(bt.uint16,'material_index'),
  bt.Field(bt.uint16,'shape_index'))): pass

#------------------------------------------------------------------------------

parser = argparse.ArgumentParser(description='Convert Luigi\'s Mansion MDL files to Wavefront OBJ files')
parser.add_argument('ifile',metavar='input.mdl')
parser.add_argument('ofile',nargs='?',metavar='output.obj')
arguments = parser.parse_args()

if arguments.ofile is None:
  arguments.ofile = os.path.splitext(arguments.ifile)[0] + '.obj'

rootname = os.path.splitext(arguments.ofile)[0]

stream = bt.FileStream(arguments.ifile,'rb',bt.BE)
header = stream >> Header

stream.seek(header.material_offset)
materials = [stream >> Material for i in range(header.num_materials)]

stream.seek(header.texobj_offset)
texobjs = [stream >> TexObj for i in range(header.num_texobjs)]

stream.seek(header.shape_offset)
shapes = [stream >> Shape for i in range(header.num_shapes)]

stream.seek(header.draw_element_offset)
draw_elements = [stream >> DrawElement for i in range(header.num_draw_elements)]

stream.seek(header.shape_packet_offset)
shape_packets = [stream >> ShapePacket for i in range(header.num_shape_packets)]

stream.seek(header.texture_location_offset)
texture_offsets = [stream >> bt.uint32 for i in range(header.num_textures)]

stream.seek(header.vertex_offset)
vertices = [stream >> Vector for i in range(header.num_vertices)]

stream.seek(header.normal_offset)
normals = [stream >> Vector for i in range(header.num_normals)]

stream.seek(header.texcoord_offset)
texcoords = [stream >> TextureCoordinate for i in range(header.num_texcoords)]

stream.seek(header.matrix_offset)
global_matrix_table = [(stream >> Matrix).I for i in range(header.num_joints)] + [Matrix.IDENTITY]*header.num_weighted_matrices

# Export shapes

out = open(arguments.ofile,'w')
out.write('mtllib {}.mtl\n'.format(rootname))

if header.num_normals == 0 and header.num_texcoords == 0:
  face_format = 'f {0} {1} {2}\n'
elif header.num_normals == 0:
  face_format = 'f {0}/{0} {1}/{1} {2}/{2}\n'
elif header.num_texcoords == 0:
  face_format = 'f {0}//{0} {1}//{1} {2}//{2}\n'
else:
  face_format = 'f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n'

index = 1

for draw_element in draw_elements:
  out.write('g shape{}\n'.format(draw_element.shape_index))
  out.write('usemtl material{}\n'.format(draw_element.material_index))

  shape = shapes[draw_element.shape_index]
  matrix_table = [None]*10

  for shape_packet_index in range(shape.first_packet,shape.first_packet + shape.num_packets):
    shape_packet = shape_packets[shape_packet_index]
    matrix = None

    for i,matrix_index in enumerate(shape_packet.matrix_indices[0:shape_packet.num_matrix_indices]):
      if matrix_index == 0xFFFF: continue
      matrix_table[i] = global_matrix_table[matrix_index]

    stream.seek(shape_packet.data_offset)
    while stream.tell() != shape_packet.data_offset + shape_packet.data_size:
      opcode = stream >> bt.uint8
      if opcode == 0x00: continue
      num_vertices = stream >> bt.uint16

      for i in range(num_vertices):
        matrix_index = stream >> bt.uint8
        if matrix_index != 0xFF:
          matrix = matrix_table[matrix_index//3]

        stream >> bt.uint8 # tex0 matrix index?
        stream >> bt.uint8 # tex1 matrix index?

        v = vertices[stream >> bt.uint16]
        v = matrix*np.matrix([[v.x],[v.y],[v.z],[1]])
        out.write('v {} {} {}\n'.format(v[0,0],v[1,0],v[2,0]))

        if header.num_normals > 0:
          vn = normals[stream >> bt.uint16]
          vn = matrix*np.matrix([[vn.x],[vn.y],[vn.z],[0]])
          vn /= np.linalg.norm(vn)
          out.write('vn {} {} {}\n'.format(vn[0,0],vn[1,0],vn[2,0]))

        if header.num_colors > 0:
          stream >> bt.uint16

        if header.num_texcoords > 0:
          vt = texcoords[stream >> bt.uint16]
          out.write('vt {} {}\n'.format(vt.s,-vt.t))

      if opcode == 0x98:
        for i in range(num_vertices - 2):
          out.write(face_format.format(index + i + 1 - (i % 2),index + i + (i % 2),index + i + 2))
      elif opcode == 0xA0:
        for i in range(num_vertices - 2):
          out.write(face_format.format(index,index + i + 1,index + i + 2))
      else:
        raise Exception('unsuported opcode: 0x{:02X}'.format(opcode))

      index += num_vertices

# Export materials

out = open('{}.mtl'.format(rootname),'w')

for material_index,material in enumerate(materials):
  out.write('newmtl material{}\n'.format(material_index))
  out.write('Kd {} {} {}\n'.format(material.color.r/255,material.color.g/255,material.color.b/255))
  out.write('d {}\n'.format(material.color.a/255))

  if material.num_tev_stages > 0:
    texobj = texobjs[material.tev_stages[0].texobj_index]
    out.write('map_Kd {}_{}.png\n'.format(rootname,texobj.texture_index))

  out.write('\n')

# Export textures

for texture_index,texture_offset in enumerate(texture_offsets):
  stream.seek(texture_offset)
  texture_header = stream >> TextureHeader
  
  if texture_header.format == 0x03:
    image = texdump.unpack_i4(stream,texture_header.width,texture_header.height)
  elif texture_header.format == 0x04:
    image = texdump.unpack_i8(stream,texture_header.width,texture_header.height)
  elif texture_header.format == 0x06:
    image = texdump.unpack_ia8(stream,texture_header.width,texture_header.height)
  elif texture_header.format == 0x07:
    image = texdump.unpack_rgb565(stream,texture_header.width,texture_header.height)
  elif texture_header.format == 0x08:
    image = texdump.unpack_rgb5a3(stream,texture_header.width,texture_header.height)
  elif texture_header.format == 0x0A:
    image = texdump.unpack_cmpr(stream,texture_header.width,texture_header.height)
  else:
    raise Exception('unsuported texture format: 0x{:02X}'.format(texture_header.format))

  image.save('{}_{}.png'.format(rootname,texture_index))

#________________________________________________________________________________
