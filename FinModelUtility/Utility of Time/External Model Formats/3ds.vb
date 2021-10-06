Module _3ds
    Enum MaxModel
        MAIN3DS = &H4D4D

        ' Main Chunks

        EDIT3DS = &H3D3D  'start of the editor config
        KEYF3DS = &HB000  'start of the keyframer config

        ' sub defines of EDIT3DS

        EDIT_MATERIAL = &HAFFF
        EDIT_CONFIG1 = &H100
        EDIT_CONFIG2 = &H3E3D
        EDIT_VIEW_P1 = &H7012
        EDIT_VIEW_P2 = &H7011
        EDIT_VIEW_P3 = &H7020
        EDIT_VIEW1 = &H7001
        EDIT_BACKGR = &H1200
        EDIT_AMBIENT = &H2100
        EDIT_OBJECT = &H4000

        EDIT_UNKNW01 = &H1100
        EDIT_UNKNW02 = &H1201
        EDIT_UNKNW03 = &H1300
        EDIT_UNKNW04 = &H1400
        EDIT_UNKNW05 = &H1420
        EDIT_UNKNW06 = &H1450
        EDIT_UNKNW07 = &H1500
        EDIT_UNKNW08 = &H2200
        EDIT_UNKNW09 = &H2201
        EDIT_UNKNW10 = &H2210
        EDIT_UNKNW11 = &H2300
        EDIT_UNKNW12 = &H2302
        EDIT_UNKNW13 = &H3000
        EDIT_UNKNW14 = &HAFFF

        ' sub defines of EDIT_OBJECT
        OBJ_TRIMESH = &H4100
        OBJ_LIGHT = &H4600
        OBJ_CAMERA = &H4700

        OBJ_UNKNWN01 = &H4010
        OBJ_UNKNWN02 = &H4012 'Could be shadow

        ' sub defines of OBJ_CAMERA
        CAM_UNKNWN01 = &H4710
        CAM_UNKNWN02 = &H4720

        ' sub defines of OBJ_LIGHT
        LIT_OFF = &H4620
        LIT_SPOT = &H4610
        LIT_UNKNWN01 = &H465A

        'sub defines of OBJ_TRIMESH
        TRI_VERTEXL = &H4110
        TRI_FACEL2 = &H4111
        TRI_FACEL1 = &H4120
        TRI_SMOOTH = &H4150
        TRI_LOCAL = &H4160
        TRI_VISIBLE = &H4165

        'sub defs of KEYF3DS

        KEYF_UNKNWN01 = &HB009
        KEYF_UNKNWN02 = &HB00A
        KEYF_FRAMES = &HB008
        KEYF_OBJDES = &HB002

        'these define the different color chunk types
        COL_RGB = &H10
        COL_TRU = &H11
        COL_UNK = &H13

        'defines for viewport chunks

        TOP = &H1
        BOTTOM = &H2
        LEFT = &H3
        RIGHT = &H4
        FRONT = &H5
        BACK = &H6
        USER = &H7
        CAMERA = &H8 ' &hFFFF is the actual code read from file
        LIGHT = &H9
        DISABLED = &H10
        BOGUS = &H11
    End Enum

End Module
