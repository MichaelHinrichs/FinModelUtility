Imports System.IO
Public Module OBJData
    Structure OBJVert
        Dim x As Short
        Dim y As Short
        Dim z As Short
        Dim r As Byte
        Dim g As Byte
        Dim b As Byte
        Dim a As Byte
    End Structure
    Structure OBJUV
        Dim s As Short
        Dim t As Short
    End Structure
    Structure OBJNormal
        Dim x As Short
        Dim y As Short
        Dim z As Short
    End Structure
    Structure OBJFace
        Dim V1 As Integer
        Dim V2 As Integer
        Dim V3 As Integer

        Dim uV1 As Integer
        Dim uV2 As Integer
        Dim uV3 As Integer

        Dim nV1 As Integer
        Dim nV2 As Integer
        Dim nV3 As Integer
    End Structure
    Structure SettileParams
        Dim CMS As Integer
        Dim CMT As Integer
        Dim ULS As Integer
        Dim ULT As Integer
        Dim LRS As Integer
        Dim LRT As Integer
    End Structure
    Structure MTLTexture
        Dim Data() As Byte
        Dim bpp As Integer
        Dim width As Integer
        Dim height As Integer
    End Structure
    Structure OBJMTL
        Dim mtlId As String
        Dim texFile As String
        Dim Textures() As MTLTexture
    End Structure
    Structure OBJGroup
        Dim grpId As String
        Dim Materials() As OBJMTL
        Dim Faces() As OBJFace
    End Structure
    Structure OBJModel
        Dim mtlFile As StreamReader
        Dim Vertices() As OBJVert
        Dim TexCoords() As OBJUV
        Dim Normals() As OBJNormal
        Dim Parts() As OBJGroup
    End Structure
End Module
