Imports System.IO
Imports System.Math
Module ObjToF3DEX2
    Private tempDL(0) As N64DisplayList
    Private tempVerts() As N64Vertex
    Private rand As New Random()
    Public Function SplitVertices(ByRef OBJ As OBJModel) As N64Vertex()
        Dim verts() As N64Vertex
        ReDim verts(0)
        Dim CurGroup As Integer = 0
        Dim tracker As Integer = 31
        With verts(0)
            ReDim .x(31)
            ReDim .y(31)
            ReDim .z(31)
            ReDim .u(31)
            ReDim .v(31)
            ReDim .r(31)
            ReDim .g(31)
            ReDim .b(31)
            ReDim .a(31)
        End With
        For i As Integer = 0 To OBJ.Parts.Length - 1
            For i1 As Integer = 0 To OBJ.Parts(i).Faces.Length
                With OBJ.Parts(i).Faces(i1)
                    For i2 As Integer = 0 To 2
                        Select Case i2
                            Case 0
                                verts(CurGroup).x(tracker) = OBJ.Vertices(.V1).x
                                verts(CurGroup).y(tracker) = OBJ.Vertices(.V1).y
                                verts(CurGroup).z(tracker) = OBJ.Vertices(.V1).z
                                .V1 = tracker * 2
                            Case 1
                                verts(CurGroup).x(tracker) = OBJ.Vertices(.V2).x
                                verts(CurGroup).y(tracker) = OBJ.Vertices(.V2).y
                                verts(CurGroup).z(tracker) = OBJ.Vertices(.V2).z
                                .V2 = tracker * 2
                            Case 2
                                verts(CurGroup).x(tracker) = OBJ.Vertices(.V3).x
                                verts(CurGroup).y(tracker) = OBJ.Vertices(.V3).y
                                verts(CurGroup).z(tracker) = OBJ.Vertices(.V3).z
                                .V3 = tracker * 2
                        End Select

                        verts(CurGroup).u(tracker) = &HFFFF
                        verts(CurGroup).v(tracker) = &HFFFF
                        verts(CurGroup).r(tracker) = rand.Next(0, 255)
                        verts(CurGroup).g(tracker) = rand.Next(0, 255)
                        verts(CurGroup).b(tracker) = rand.Next(0, 255)
                        verts(CurGroup).a(tracker) = rand.Next(0, 255)
                        tracker += 1
                    Next
                End With
            Next
            If tracker = 31 Then
                tracker = 0
                CurGroup += 1
                ReDim Preserve verts(CurGroup)
                With verts(CurGroup)
                    ReDim .x(31)
                    ReDim .y(31)
                    ReDim .z(31)
                    ReDim .u(31)
                    ReDim .v(31)
                    ReDim .r(31)
                    ReDim .g(31)
                    ReDim .b(31)
                    ReDim .a(31)
                End With
            Else
                tracker += 1
            End If
        Next
        Return verts
    End Function
    Public Function ConvertToF3DEX2(ByVal OBJ As OBJModel) As N64DisplayList
        tempVerts = SplitVertices(OBJ)
        For i As Integer = 0 To OBJ.Parts.Length

        Next
    End Function
End Module
