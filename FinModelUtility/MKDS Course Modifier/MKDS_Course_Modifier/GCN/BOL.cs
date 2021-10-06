// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.GCN.BOL
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.UI.MKDS;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.GCN
{
  public class BOL
  {
    public const string Signature = "0015";
    public const float RotDivide = 65536f;
    public BOL.BOLHeader Header;
    public BOL.ItemEnemyPoint[] ItemEnemyPoints;
    public BOL.CheckpointGroup[] CheckpointGroups;
    public BOL.Checkpoint[] Checkpoints;
    public BOL.Route[] Routes;
    public BOL.RoutePoint[] RoutePoints;
    public BOL.Object[] Objects;
    public BOL.StartPosition[] StartPositions;
    public BOL.Area[] Areas;
    public BOL.Camera[] Cameras;
    public BOL.Respawn[] Respawns;

    public BOL(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.BigEndian);
      bool OK;
      this.Header = new BOL.BOLHeader(er, "0015", out OK);
      if (!OK)
      {
        int num = (int) MessageBox.Show("Error 1");
      }
      else
      {
        er.BaseStream.Position = (long) this.Header.ItemEnemyPointsOffset;
        this.ItemEnemyPoints = new BOL.ItemEnemyPoint[(int) this.Header.NrItemEnemyPoints];
        for (int index = 0; index < (int) this.Header.NrItemEnemyPoints; ++index)
          this.ItemEnemyPoints[index] = new BOL.ItemEnemyPoint(er);
        er.BaseStream.Position = (long) this.Header.CheckpointGroupsOffset;
        this.CheckpointGroups = new BOL.CheckpointGroup[(int) this.Header.NrCheckpointGroups];
        for (int index = 0; index < (int) this.Header.NrCheckpointGroups; ++index)
          this.CheckpointGroups[index] = new BOL.CheckpointGroup(er);
        int length = 0;
        for (int index = 0; index < (int) this.Header.NrCheckpointGroups; ++index)
          length += (int) this.CheckpointGroups[index].NrPoints;
        this.Checkpoints = new BOL.Checkpoint[length];
        for (int index = 0; index < length; ++index)
          this.Checkpoints[index] = new BOL.Checkpoint(er);
        er.BaseStream.Position = (long) this.Header.RoutesOffset;
        this.Routes = new BOL.Route[(int) this.Header.NrRoutes];
        for (int index = 0; index < (int) this.Header.NrRoutes; ++index)
          this.Routes[index] = new BOL.Route(er);
        er.BaseStream.Position = (long) this.Header.RoutePointsOffset;
        this.RoutePoints = new BOL.RoutePoint[(this.Header.ObjectsOffset - this.Header.RoutePointsOffset) / 32U];
        for (int index = 0; (long) index < (long) ((this.Header.ObjectsOffset - this.Header.RoutePointsOffset) / 32U); ++index)
          this.RoutePoints[index] = new BOL.RoutePoint(er);
        er.BaseStream.Position = (long) this.Header.ObjectsOffset;
        this.Objects = new BOL.Object[(int) this.Header.NrObjects];
        for (int index = 0; index < (int) this.Header.NrObjects; ++index)
          this.Objects[index] = new BOL.Object(er);
        er.BaseStream.Position = (long) this.Header.StartPositionsOffset;
        this.StartPositions = new BOL.StartPosition[(this.Header.AreasOffset - this.Header.StartPositionsOffset) / 40U];
        for (int index = 0; (long) index < (long) ((this.Header.AreasOffset - this.Header.StartPositionsOffset) / 40U); ++index)
          this.StartPositions[index] = new BOL.StartPosition(er);
        er.BaseStream.Position = (long) this.Header.AreasOffset;
        this.Areas = new BOL.Area[(int) this.Header.NrAreas];
        for (int index = 0; index < (int) this.Header.NrAreas; ++index)
          this.Areas[index] = new BOL.Area(er);
        er.BaseStream.Position = (long) this.Header.CameraOffset;
        this.Cameras = new BOL.Camera[(int) this.Header.NrCameras];
        for (int index = 0; index < (int) this.Header.NrCameras; ++index)
          this.Cameras[index] = new BOL.Camera(er);
        er.BaseStream.Position = (long) this.Header.RespawnPositionsOffset;
        this.Respawns = new BOL.Respawn[(this.Header.Section10Offset - this.Header.RespawnPositionsOffset) / 32U];
        for (int index = 0; (long) index < (long) ((this.Header.Section10Offset - this.Header.RespawnPositionsOffset) / 32U); ++index)
          this.Respawns[index] = new BOL.Respawn(er);
      }
      er.Close();
    }

    public MKDS_Course_Modifier.MKDS.NKM ToNKM()
    {
      MKDS_Course_Modifier.MKDS.NKM nkm = new MKDS_Course_Modifier.MKDS.NKM(false);
      foreach (BOL.Object @object in this.Objects)
      {
        MKDS_Course_Modifier.MKDS.NKM.OBJIEntry objiEntry = new MKDS_Course_Modifier.MKDS.NKM.OBJIEntry();
        objiEntry.Position = new Vector3(@object.Position.X / 4f, @object.Position.Y / 4f, @object.Position.Z / 4f);
        objiEntry.Scale = @object.Scale;
        objiEntry.TimeTrails = true;
        switch (@object.ObjectID)
        {
          case 1:
            objiEntry.ObjectID = @object.RouteID != (short) -1 ? (ushort) 201 : (ushort) 101;
            break;
          case 3401:
            objiEntry.ObjectID = (ushort) 1;
            break;
          case 3405:
            objiEntry.ObjectID = (ushort) 302;
            break;
          case 3503:
            objiEntry.ObjectID = (ushort) 5;
            break;
          case 4203:
            objiEntry.ObjectID = (ushort) 431;
            uint num = (@object.Unknown4 & (uint) ushort.MaxValue) / 10U;
            objiEntry.Setting1 = (uint) (((int) (num >> 8) & (int) byte.MaxValue) << 24 | ((int) num & (int) byte.MaxValue) << 16 | ((int) (@object.Unknown3 >> 24) & (int) byte.MaxValue) << 8 | (int) (@object.Unknown3 >> 16) & (int) byte.MaxValue);
            objiEntry.Setting2 = 55U;
            break;
          case 4222:
            objiEntry.ObjectID = (ushort) 14;
            break;
          case 4701:
            objiEntry.ObjectID = (ushort) 403;
            break;
          case 4702:
            objiEntry.ObjectID = (ushort) 408;
            break;
          default:
            objiEntry.ObjectID = @object.ObjectID;
            break;
        }
        if (@object.ObjectID == (ushort) 1)
          objiEntry.TimeTrails = false;
        objiEntry.RouteID = @object.RouteID;
        if (@object.RouteID != (short) -1)
          objiEntry.Setting1 = 32U;
        if ((int) objiEntry.ObjectID != (int) @object.ObjectID)
          nkm.OBJI.Add(objiEntry);
      }
      int num1 = 0;
      foreach (BOL.Route route in this.Routes)
      {
        MKDS_Course_Modifier.MKDS.NKM.PATHEntry pathEntry = new MKDS_Course_Modifier.MKDS.NKM.PATHEntry();
        pathEntry.Index = (byte) num1++;
        pathEntry.Loop = false;
        pathEntry.NrPoit = (short) route.NrPoints;
        for (int index = 0; index < (int) route.NrPoints; ++index)
        {
          BOL.RoutePoint routePoint = this.RoutePoints[(int) route.FirstPointIndex + index];
          nkm.POIT.Add(new MKDS_Course_Modifier.MKDS.NKM.POITEntry()
          {
            Position = new Vector3(routePoint.Position.X / 4f, routePoint.Position.Y / 4f, routePoint.Position.Z / 4f),
            Index = (short) (byte) index
          });
        }
        nkm.PATH.Add(pathEntry);
      }
      foreach (BOL.StartPosition startPosition in this.StartPositions)
      {
        nkm.KTPS.Add(new MKDS_Course_Modifier.MKDS.NKM.KTPSEntry()
        {
          Position = new Vector3(startPosition.Position.X / 4f, startPosition.Position.Y / 4f, startPosition.Position.Z / 4f),
          Index = (short) startPosition.PlayerID,
          Padding = ushort.MaxValue
        });
        nkm.KTP2.Add(new MKDS_Course_Modifier.MKDS.NKM.KTP2Entry()
        {
          Position = new Vector3(startPosition.Position.X / 4f, startPosition.Position.Y / 4f, startPosition.Position.Z / 4f),
          Index = (short) startPosition.PlayerID,
          Padding = ushort.MaxValue
        });
      }
      int num2 = 0;
      int num3 = 0;
      foreach (BOL.Checkpoint checkpoint in this.Checkpoints)
      {
        NKMProperties.CPOI cpoi = (NKMProperties.CPOI) new MKDS_Course_Modifier.MKDS.NKM.CPOIEntry()
        {
          Position1 = new Vector2(checkpoint.Position1.X / 4f, checkpoint.Position1.Z / 4f),
          Position2 = new Vector2(checkpoint.Position2.X / 4f, checkpoint.Position2.Z / 4f)
        };
        int num4 = num2 % 4 != 0 || num2 == 0 ? 1 : (num2 == this.Checkpoints.Length - 1 ? 1 : 0);
        cpoi.KeyPoint = num4 != 0 ? (short) -1 : (short) num3++;
        cpoi.RespawnID = (short) checkpoint.Unknown1[0];
        if (num2 == 0)
        {
          cpoi.SectionData = 4294901760U;
          cpoi.Distance = -1f;
        }
        else if (num2 == this.Checkpoints.Length - 1)
        {
          cpoi.SectionData = (uint) ushort.MaxValue;
          cpoi.Distance = -1f;
        }
        else
          cpoi.SectionData = uint.MaxValue;
        ++num2;
        nkm.CPOI.Add((MKDS_Course_Modifier.MKDS.NKM.CPOIEntry) cpoi);
      }
      int num5 = 0;
      foreach (BOL.CheckpointGroup checkpointGroup in this.CheckpointGroups)
      {
        MKDS_Course_Modifier.MKDS.NKM.CPATEntry cpatEntry = new MKDS_Course_Modifier.MKDS.NKM.CPATEntry();
        cpatEntry.ComesFrom1 = (sbyte) checkpointGroup.ComesFrom1;
        cpatEntry.ComesFrom2 = (sbyte) checkpointGroup.ComesFrom2;
        cpatEntry.ComesFrom3 = (sbyte) checkpointGroup.ComesFrom3;
        cpatEntry.GoesTo1 = (sbyte) checkpointGroup.GoesTo1;
        cpatEntry.GoesTo2 = (sbyte) checkpointGroup.GoesTo2;
        cpatEntry.GoesTo3 = (sbyte) checkpointGroup.GoesTo3;
        cpatEntry.Length = (short) checkpointGroup.NrPoints;
        cpatEntry.StartIdx = (short) num5;
        num5 += (int) checkpointGroup.NrPoints;
        cpatEntry.SectionOrder = (short) 0;
        nkm.CPAT.Add(cpatEntry);
      }
      MKDS_Course_Modifier.MKDS.NKM.IPATEntry ipatEntry = new MKDS_Course_Modifier.MKDS.NKM.IPATEntry();
      MKDS_Course_Modifier.MKDS.NKM.EPATEntry epatEntry = new MKDS_Course_Modifier.MKDS.NKM.EPATEntry();
      ipatEntry.ComesFrom1 = epatEntry.ComesFrom1 = (sbyte) 0;
      ipatEntry.ComesFrom2 = epatEntry.ComesFrom2 = (sbyte) -1;
      ipatEntry.ComesFrom3 = epatEntry.ComesFrom3 = (sbyte) -1;
      ipatEntry.GoesTo1 = epatEntry.GoesTo1 = (sbyte) 0;
      ipatEntry.GoesTo2 = epatEntry.GoesTo2 = (sbyte) -1;
      ipatEntry.GoesTo3 = epatEntry.GoesTo3 = (sbyte) -1;
      ipatEntry.StartIdx = epatEntry.StartIdx = (short) 0;
      ipatEntry.Length = epatEntry.Length = (short) this.ItemEnemyPoints.Length;
      nkm.IPAT.Add(ipatEntry);
      nkm.EPAT.Add(epatEntry);
      List<int>[] intListArray = new List<int>[this.ItemEnemyPoints.Length];
      for (int index = 0; index < this.ItemEnemyPoints.Length; ++index)
        intListArray[index] = new List<int>();
      int num6 = 0;
      foreach (BOL.ItemEnemyPoint itemEnemyPoint in this.ItemEnemyPoints)
      {
        if (itemEnemyPoint.GroupLink != ushort.MaxValue)
          intListArray[(int) itemEnemyPoint.GroupID].Add((int) itemEnemyPoint.GroupLink);
        nkm.IPOI.Add(new MKDS_Course_Modifier.MKDS.NKM.IPOIEntry()
        {
          Position = new Vector3(itemEnemyPoint.Position.X / 4f, itemEnemyPoint.Position.Y / 4f, itemEnemyPoint.Position.Z / 4f)
        });
        nkm.EPOI.Add(new MKDS_Course_Modifier.MKDS.NKM.EPOIEntry()
        {
          Position = new Vector3(itemEnemyPoint.Position.X / 4f, itemEnemyPoint.Position.Y / 4f, itemEnemyPoint.Position.Z / 4f)
        });
        ++num6;
      }
      int num7 = 0;
      foreach (BOL.Respawn respawn in this.Respawns)
        nkm.KTPJ.Add(new MKDS_Course_Modifier.MKDS.NKM.KTPJEntry()
        {
          Position = new Vector3(respawn.Position.X / 4f, respawn.Position.Y / 4f, respawn.Position.Z / 4f),
          Index = num7++
        });
      foreach (BOL.Camera camera in this.Cameras)
      {
        MKDS_Course_Modifier.MKDS.NKM.CAMEEntry cameEntry = new MKDS_Course_Modifier.MKDS.NKM.CAMEEntry();
        if (camera.CameraType == (byte) 5 || camera.CameraType == (byte) 6)
        {
          cameEntry.Position1 = new Vector3(camera.Position1.X / 4f, camera.Position1.Y / 4f, camera.Position1.Z / 4f);
          cameEntry.Position2 = new Vector3(camera.Position3.X / 4f, camera.Position3.Y / 4f, camera.Position3.Z / 4f);
          cameEntry.Position3 = new Vector3(camera.Position2.X / 4f, camera.Position2.Y / 4f, camera.Position2.Z / 4f);
          cameEntry.LinkedRoute = camera.LinkedRoute;
          cameEntry.NextCame = camera.NextCamera;
          cameEntry.TotalLength = camera.TotalTime;
          cameEntry.Unknown1 = 113442841U;
          cameEntry.Unknown2 = 986752U;
          cameEntry.PointSpeed = (short) ((double) camera.Unknown7 / 60.0 * 100.0);
          cameEntry.RouteSpeed = (short) ((double) camera.Unknown6 / 60.0 * 100.0);
          cameEntry.CameraType = (short) 3;
          nkm.CAME.Add(cameEntry);
        }
      }
      return nkm;
    }

    public class BOLHeader
    {
      public string Type;
      public byte[] Unknown1;
      public short NrItemEnemyPoints;
      public short NrCheckpointGroups;
      public short NrObjects;
      public short NrAreas;
      public short NrCameras;
      public short NrRoutes;
      public short NrSectionUnknownEntries;
      public byte[] Unknown2;
      public uint FileStartOffset;
      public uint ItemEnemyPointsOffset;
      public uint CheckpointGroupsOffset;
      public uint RoutesOffset;
      public uint RoutePointsOffset;
      public uint ObjectsOffset;
      public uint StartPositionsOffset;
      public uint AreasOffset;
      public uint CameraOffset;
      public uint RespawnPositionsOffset;
      public uint Section10Offset;
      public uint FileSize;
      public byte[] Unknown3;

      public BOLHeader(EndianBinaryReader er, string Signature, out bool OK)
      {
        this.Type = er.ReadString(Encoding.ASCII, 4);
        if (this.Type != Signature)
        {
          OK = false;
        }
        else
        {
          this.Unknown1 = er.ReadBytes(22);
          this.NrItemEnemyPoints = er.ReadInt16();
          this.NrCheckpointGroups = er.ReadInt16();
          this.NrObjects = er.ReadInt16();
          this.NrAreas = er.ReadInt16();
          this.NrCameras = er.ReadInt16();
          this.NrRoutes = er.ReadInt16();
          this.NrSectionUnknownEntries = er.ReadInt16();
          this.Unknown2 = er.ReadBytes(24);
          this.FileStartOffset = er.ReadUInt32();
          this.ItemEnemyPointsOffset = er.ReadUInt32();
          this.CheckpointGroupsOffset = er.ReadUInt32();
          this.RoutesOffset = er.ReadUInt32();
          this.RoutePointsOffset = er.ReadUInt32();
          this.ObjectsOffset = er.ReadUInt32();
          this.StartPositionsOffset = er.ReadUInt32();
          this.AreasOffset = er.ReadUInt32();
          this.CameraOffset = er.ReadUInt32();
          this.RespawnPositionsOffset = er.ReadUInt32();
          this.Section10Offset = er.ReadUInt32();
          this.FileSize = er.ReadUInt32();
          this.Unknown3 = er.ReadBytes(12);
          OK = true;
        }
      }
    }

    public class ItemEnemyPoint
    {
      public Vector3 Position;
      public ushort PointSetting1;
      public ushort GroupLink;
      public float Scale;
      public ushort GroupSetting;
      public byte GroupID;
      public byte PointSetting2;
      public byte[] Unknown1;

      public ItemEnemyPoint(EndianBinaryReader er)
      {
        this.Position = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
        this.PointSetting1 = er.ReadUInt16();
        this.GroupLink = er.ReadUInt16();
        this.Scale = er.ReadSingle();
        this.GroupSetting = er.ReadUInt16();
        this.GroupID = er.ReadByte();
        this.PointSetting2 = er.ReadByte();
        this.Unknown1 = er.ReadBytes(8);
      }
    }

    public class CheckpointGroup
    {
      public ushort NrPoints;
      public ushort GroupLink;
      public short ComesFrom1;
      public short ComesFrom2;
      public short ComesFrom3;
      public short ComesFrom4;
      public short GoesTo1;
      public short GoesTo2;
      public short GoesTo3;
      public short GoesTo4;

      public CheckpointGroup(EndianBinaryReader er)
      {
        this.NrPoints = er.ReadUInt16();
        this.GroupLink = er.ReadUInt16();
        this.ComesFrom1 = er.ReadInt16();
        this.ComesFrom2 = er.ReadInt16();
        this.ComesFrom3 = er.ReadInt16();
        this.ComesFrom4 = er.ReadInt16();
        this.GoesTo1 = er.ReadInt16();
        this.GoesTo2 = er.ReadInt16();
        this.GoesTo3 = er.ReadInt16();
        this.GoesTo4 = er.ReadInt16();
      }
    }

    public class Checkpoint
    {
      public Vector3 Position1;
      public Vector3 Position2;
      public byte[] Unknown1;

      public Checkpoint(EndianBinaryReader er)
      {
        this.Position1 = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
        this.Position2 = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
        this.Unknown1 = er.ReadBytes(4);
      }
    }

    public class Route
    {
      public ushort NrPoints;
      public ushort FirstPointIndex;
      public byte[] Unknown1;

      public Route(EndianBinaryReader er)
      {
        this.NrPoints = er.ReadUInt16();
        this.FirstPointIndex = er.ReadUInt16();
        this.Unknown1 = er.ReadBytes(12);
      }
    }

    public class RoutePoint
    {
      public Vector3 Position;
      public byte[] Unknown1;

      public RoutePoint(EndianBinaryReader er)
      {
        this.Position = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
        this.Unknown1 = er.ReadBytes(20);
      }
    }

    public class Object
    {
      public Vector3 Position;
      public Vector3 Scale;
      public Vector3 Rotation;
      public ushort ObjectID;
      public short RouteID;
      public uint Unknown1;
      public uint Unknown2;
      public uint Unknown3;
      public uint Unknown4;
      public uint Unknown5;
      public uint Unknown6;

      public Object(EndianBinaryReader er)
      {
        this.Position = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
        this.Scale = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
        this.Rotation = new Vector3((float) er.ReadInt32() / 65536f, (float) er.ReadInt32() / 65536f, (float) er.ReadInt32() / 65536f);
        this.ObjectID = er.ReadUInt16();
        this.RouteID = er.ReadInt16();
        this.Unknown1 = er.ReadUInt32();
        this.Unknown2 = er.ReadUInt32();
        this.Unknown3 = er.ReadUInt32();
        this.Unknown4 = er.ReadUInt32();
        this.Unknown5 = er.ReadUInt32();
        this.Unknown6 = er.ReadUInt32();
      }
    }

    public class StartPosition
    {
      public Vector3 Position;
      public Vector3 Scale;
      public Vector3 Rotation;
      public byte PolePosition;
      public sbyte PlayerID;
      public ushort Unknown1;

      public StartPosition(EndianBinaryReader er)
      {
        this.Position = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
        this.Scale = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
        this.Rotation = new Vector3((float) er.ReadInt32(), (float) er.ReadInt32(), (float) er.ReadInt32());
        this.PolePosition = er.ReadByte();
        this.PlayerID = er.ReadSByte();
        this.Unknown1 = er.ReadUInt16();
      }
    }

    public class Area
    {
      public Vector3 Position;
      public Vector3 Scale;
      public Vector3 Rotation;
      public ushort Unknown1;
      public ushort Unknown2;
      public byte[] Unknown3;

      public Area(EndianBinaryReader er)
      {
        this.Position = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
        this.Scale = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
        this.Rotation = new Vector3((float) er.ReadInt32() / 65536f, (float) er.ReadInt32() / 65536f, (float) er.ReadInt32() / 65536f);
        this.Unknown1 = er.ReadUInt16();
        this.Unknown2 = er.ReadUInt16();
        this.Unknown3 = er.ReadBytes(16);
      }
    }

    public class Camera
    {
      public Vector3 Position1;
      public Vector3 Rotation;
      public Vector3 Position2;
      public Vector3 Position3;
      public byte Unknown1;
      public byte CameraType;
      public ushort Unknown2;
      public TimeSpan TotalTime;
      public ushort Unknown3;
      public ushort Unknown4;
      public ushort Unknown5;
      public short LinkedRoute;
      public ushort Unknown6;
      public ushort Unknown7;
      public short NextCamera;
      public string Unknown8;

      public Camera(EndianBinaryReader er)
      {
        this.Position1 = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
        this.Rotation = new Vector3((float) er.ReadInt32() / 65536f, (float) er.ReadInt32() / 65536f, (float) er.ReadInt32() / 65536f);
        this.Position2 = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
        this.Position3 = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
        this.Unknown1 = er.ReadByte();
        this.CameraType = er.ReadByte();
        this.Unknown2 = er.ReadUInt16();
        this.TotalTime = TimeSpan.FromSeconds((double) er.ReadInt16() / 60.0);
        this.Unknown3 = er.ReadUInt16();
        this.Unknown4 = er.ReadUInt16();
        this.Unknown5 = er.ReadUInt16();
        this.LinkedRoute = er.ReadInt16();
        this.Unknown6 = er.ReadUInt16();
        this.Unknown7 = er.ReadUInt16();
        this.NextCamera = er.ReadInt16();
        this.Unknown8 = er.ReadString(Encoding.ASCII, 4);
      }
    }

    public class Respawn
    {
      public Vector3 Position;
      public Vector3 Rotation;
      public ushort RespawnID;
      public ushort Unknown1;
      public ushort Unknown2;
      public ushort Unknown3;

      public Respawn(EndianBinaryReader er)
      {
        this.Position = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
        this.Rotation = new Vector3((float) er.ReadInt32() / 65536f, (float) er.ReadInt32() / 65536f, (float) er.ReadInt32() / 65536f);
        this.RespawnID = er.ReadUInt16();
        this.Unknown1 = er.ReadUInt16();
        this.Unknown2 = er.ReadUInt16();
        this.Unknown3 = er.ReadUInt16();
      }
    }
  }
}
