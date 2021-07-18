Imports System.IO
Public Class PresetReader
    Public Function ReadHumanActorDB(ByVal fn As String) As ActorDB()
        Dim Reader As StreamReader = New StreamReader(fn, System.Text.Encoding.UTF8)
        Dim tDB As ActorDB() = {}
        Dim actorCnt As Integer = 0
        Dim varCnt As Integer = 0
        Dim tLine As String = ""
        Dim tNextLine As String = ""
        Dim tTest As String = ""
        Dim Tokens() As String = {""}
        Dim nextTokens() As String = {""}
        Dim intParse As Integer = 0
        While Reader.Peek <> -1
            tLine = Reader.ReadLine
            tTest = Mid(tLine, 1, 1)
            Tokens = tLine.Split(" ")
            If Int16.TryParse(tTest, intParse) Then
                tNextLine = Reader.ReadLine
                ReDim Preserve tDB(actorCnt)
                With tDB(actorCnt)
                    .no = Int32.Parse(Tokens(0), Globalization.NumberStyles.HexNumber)
                    .grp = Int32.Parse(Tokens(1), Globalization.NumberStyles.HexNumber)
                    .desc = "?"
                    If Tokens.Length > 2 Then
                        For I As Integer = 2 To Tokens.Length - 1
                            .desc += Tokens(2 + (I - 2)) & " "
                        Next
                    End If
                End With
                nextTokens = tNextLine.Split(" ")
                If nextTokens.Length > 2 Then
                    While nextTokens(0) = "" And nextTokens(1) = "-" And Int32.TryParse(nextTokens(2), intParse)

                        ReDim Preserve tDB(actorCnt).var(varCnt)
                        With tDB(actorCnt).var(varCnt)
                            .var = Int32.Parse(nextTokens(2), Globalization.NumberStyles.HexNumber)
                            If nextTokens.Length > 3 Then
                                For I As Integer = 4 To nextTokens.Length - 1
                                    .desc += nextTokens(I) & " "
                                Next
                            Else
                                .desc = "?"
                            End If
                        End With
                        tNextLine = Reader.ReadLine
                        nextTokens = tNextLine.Split(" ")
                        If nextTokens.Length < 2 Then
                            Exit While
                        End If
                        varCnt += 1
                    End While
                End If
                varCnt = 0
                actorCnt += 1
            End If
        End While
        Reader.Dispose()
        Return tDB
    End Function
End Class
