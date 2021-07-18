Public Class MIPS
    Enum R4300
        RType = 0
        OP_REGIMM = 1
        j = 2
        jal = 3
        beq = 4
        bne = 5
        blez = 6
        bgtz = 7
        addi = 8
        addiu = 9
        slti = 10
        sltiu = 11
        andi = 12
        ori = 13
        xori = 14
        lui = 15
        OP_COPRO0 = 16
        OP_COPRO1 = 17

        OP_BEQL = 20
        OP_BNEL = 21
        OP_BLEZL = 22
        OP_BGTZL = 23
        OP_DADDI = 24
        OP_DADDIU = 25
        OP_LDL = 26
        OP_LDR = 27
        OP_PATCH = 28
        OP_SRHACK_UNOPT = 29
        OP_SRHACK_OPT = 30
        OP_SRHACK_NOOPT = 31

        lb = 32
        lh = 33
        OP_LWL = 34
        lw = 35
        lbu = 36
        lhu = 37
        OP_LWR = 38
        OP_LWU = 39
        sb = 40
        sh = 41
        OP_SWL = 42
        sw = 43
        OP_SDL = 44
        OP_SDR = 45
        OP_SWR = 46
        OP_CACHE = 47
        OP_LL = 48
        lwc1 = 49
        OP_UNK7 = 50
        OP_UNK8 = 51
        OP_LLD = 52
        OP_LDC1 = 53
        OP_LDC2 = 54
        OP_LD = 55
        OP_SC = 56
        swc1 = 57
        OP_DBG_BKPT = 58
        OP_UNK10 = 59
        OP_SCD = 60
        OP_SDC1 = 61
        OP_SDC2 = 62
        OP_SD = 63
    End Enum
    Structure MainRegisters
        Dim r0 As Integer
        Dim at As Integer
        Dim v0 As Integer
        Dim v1 As Integer
        Dim a0 As Integer
        Dim a1 As Integer
        Dim a2 As Integer
        Dim a3 As Integer
        Dim t0 As Integer
        Dim t1 As Integer
        Dim t2 As Integer
        Dim t3 As Integer
        Dim t4 As Integer
        Dim t5 As Integer
        Dim t6 As Integer
        Dim t7 As Integer

        Dim s0 As Integer
        Dim s1 As Integer
        Dim s2 As Integer
        Dim s3 As Integer
        Dim s4 As Integer
        Dim s5 As Integer
        Dim s6 As Integer
        Dim s7 As Integer

        Dim t8 As Integer
        Dim t9 As Integer
        Dim k0 As Integer
        Dim k1 As Integer
        Dim gp As Integer
        Dim sp As Integer
        Dim s8 As Integer
        Dim ra As Integer
    End Structure
    Public Sub Parse(ByVal code() As Byte)
        Dim instruction As Byte = 0
        Dim rs As Byte = 0
        Dim rt As Byte = 0
        Dim rd As Byte = 0
        Dim sa As Byte = 0
        Dim fs As Byte = 0
        Dim ft As Byte = 0
        Dim fd As Byte = 0
        Dim base As Byte = 0
        Dim imm As UShort = 0
        Dim offset As UShort = 0
        Dim target As UInteger = 0
        Dim pc As Integer = 0
        Dim Regs As New MainRegisters
        For i As Integer = 0 To code.Length - 1 Step 8
            instruction = code(i)

            Select Case instruction
                Case R4300.j

                Case R4300.jal

                Case R4300.addiu

                Case R4300.lui

            End Select

            pc += 8
        Next
    End Sub
End Class
