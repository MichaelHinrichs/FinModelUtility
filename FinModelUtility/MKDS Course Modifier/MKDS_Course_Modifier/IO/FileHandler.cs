// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.IO.FileHandler
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier._3D_Formats;
using MKDS_Course_Modifier.Archive_Format;
using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.G2D_Binary_File_Format;
using MKDS_Course_Modifier.G3D_Binary_File_Format;
using MKDS_Course_Modifier.GCN;
using MKDS_Course_Modifier.Misc;
using MKDS_Course_Modifier.MKDS;
using MKDS_Course_Modifier.MPDS;
using MKDS_Course_Modifier.NITRO_CHARACTER_Binary_File_Format;
using MKDS_Course_Modifier.Properties;
using MKDS_Course_Modifier.Sound;
using MKDS_Course_Modifier.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.IO
{
  public class FileHandler
  {
    private static Form OpenDialog = (Form) null;
    private static List<ByteFileInfo> OpenedFiles = new List<ByteFileInfo>();
    private static List<ByteFileInfo> OpenedArchives = new List<ByteFileInfo>();
    private static MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR LastPal = (MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR) null;
    private static NCGR LastGraphic = (NCGR) null;
    private static byte[] LastRawPal = (byte[]) null;
    private static byte[] LastRawGraphic = (byte[]) null;
    private static MusicPlayer m;

    public static void SetMusicPlayer(MusicPlayer mm)
    {
      FileHandler.m = mm;
    }

    public static event EventHandler OnSave;

    public static bool Open(string file, Form1 Owner)
    {
      return FileHandler.Open(new ByteFileInfo(file), Owner, (object) true);
    }

    public static bool Open(ByteFileInfo file, Form1 Owner, object Parameter = null)
    {
      if (!FileHandler.Open(file, Owner, Parameter, false))
        return false;
      if (!file.IsArchive)
        FileHandler.OpenedFiles.Add(file);
      else
        FileHandler.OpenedArchives.Add(file);
      return true;
    }

    private static bool Open(ByteFileInfo file, Form1 Owner, object Parameter = null, bool filedialog = false)
    {
      switch (FileHandler.GetType(file))
      {
        case "NARC":
          NARC.DirectoryEntry Root = NARC.Unpack(file.Data);
          if (FileHandler.OpenedArchives.Count != 0 && !FileHandler.OpenedArchives[0].FileName.EndsWith(".nds"))
            FileHandler.OpenedArchives.RemoveAt(FileHandler.OpenedArchives.Count - 1);
          Owner.OpenNarc(Root);
          return true;
        case "NCGR":
          NCGR Graphic = new NCGR(file.Data);
          switch (FileHandler.OpenDialog)
          {
            case MKDS_Course_Modifier.UI.BNCL _:
              ((MKDS_Course_Modifier.UI.BNCL) FileHandler.OpenDialog).SetNCGR(Graphic);
              break;
            case MKDS_Course_Modifier.UI.NCER _:
              ((MKDS_Course_Modifier.UI.NCER) FileHandler.OpenDialog).SetNCGR(Graphic, FileHandler.OpenedFiles.Count);
              return true;
          }
          return false;
        case "NCLR":
          MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR nclr = new MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR(file.Data);
          switch (FileHandler.OpenDialog)
          {
            case null:
              FileHandler.OpenDialog = (Form) new MKDS_Course_Modifier.UI.NCLR(nclr);
              FileHandler.OpenDialog.Show((IWin32Window) Owner);
              FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
              return true;
            case MKDS_Course_Modifier.UI.BNCL _:
              ((MKDS_Course_Modifier.UI.BNCL) FileHandler.OpenDialog).SetNCLR(nclr);
              break;
            case MKDS_Course_Modifier.UI.NCER _:
              ((MKDS_Course_Modifier.UI.NCER) FileHandler.OpenDialog).SetNCLR(nclr);
              break;
          }
          return false;
        case "NSCR":
          MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR nscr = new MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR(file.Data);
          return false;
        case "NSBMD":
          MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD nsbmd = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD(file.Data);
          if (FileHandler.OpenDialog is MKDS_Course_Modifier.UI.NSBMD)
          {
            FileHandler.OpenedFiles.RemoveAt(FileHandler.OpenedFiles.Count - 1);
            ((MKDS_Course_Modifier.UI.NSBMD) FileHandler.OpenDialog).SetNSBMD(nsbmd);
            return true;
          }
          if (FileHandler.OpenDialog != null)
            return false;
          FileHandler.OpenDialog = (Form) new MKDS_Course_Modifier.UI.NSBMD(nsbmd);
          FileHandler.OpenDialog.Show((IWin32Window) Owner);
          FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
          return true;
        case "NSBTX":
          MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX Btx = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX(file.Data);
          if (FileHandler.OpenDialog is MKDS_Course_Modifier.UI.NSBMD)
          {
            ((MKDS_Course_Modifier.UI.NSBMD) FileHandler.OpenDialog).SetNSBTX(Btx);
            return false;
          }
          FileHandler.OpenDialog = (Form) new MKDS_Course_Modifier.UI.NSBTX(Btx);
          FileHandler.OpenDialog.Show((IWin32Window) Owner);
          FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
          return true;
        case "NSBCA":
          NSBCA Bca1 = new NSBCA(file.Data);
          if (!(FileHandler.OpenDialog is MKDS_Course_Modifier.UI.NSBMD))
            return false;
          ((MKDS_Course_Modifier.UI.NSBMD) FileHandler.OpenDialog).SetNSBCA(Bca1);
          return false;
        case "NSBTA":
          MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTA nsbta = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTA(file.Data);
          int num = (int) MessageBox.Show("Due to problems with texture matrices, is nsbta temporary disabled.");
          return false;
        case "NSBMA":
          NSBMA Bma = new NSBMA(file.Data);
          if (!(FileHandler.OpenDialog is MKDS_Course_Modifier.UI.NSBMD))
            return false;
          ((MKDS_Course_Modifier.UI.NSBMD) FileHandler.OpenDialog).SetNSBMA(Bma);
          return false;
        case "NSBVA":
          NSBVA Bva = new NSBVA(file.Data);
          if (!(FileHandler.OpenDialog is MKDS_Course_Modifier.UI.NSBMD))
            return false;
          ((MKDS_Course_Modifier.UI.NSBMD) FileHandler.OpenDialog).SetNSBVA(Bva);
          return false;
        case "NSBTP":
          NSBTP Btp = new NSBTP(file.Data);
          if (!(FileHandler.OpenDialog is MKDS_Course_Modifier.UI.NSBMD))
            return false;
          ((MKDS_Course_Modifier.UI.NSBMD) FileHandler.OpenDialog).SetNSBTP(Btp);
          return false;
        case "KCL":
          KCL KCL = new KCL(file.Data);
          if (FileHandler.OpenDialog is MKDS_Course_Modifier.UI.MKDS.NKM)
            ((MKDS_Course_Modifier.UI.MKDS.NKM) FileHandler.OpenDialog).SetKCL(KCL);
          return false;
        case "MR":
          MKDS_Course_Modifier.MKDS.MR Mission = new MKDS_Course_Modifier.MKDS.MR(file.Data);
          if (FileHandler.OpenDialog != null)
            return false;
          FileHandler.OpenDialog = (Form) new MKDS_Course_Modifier.UI.MKDS.MR(Mission);
          FileHandler.OpenDialog.Show((IWin32Window) Owner);
          FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
          return true;
        case "NKM":
          MKDS_Course_Modifier.MKDS.NKM File1 = new MKDS_Course_Modifier.MKDS.NKM(file.Data);
          if (FileHandler.OpenDialog != null)
            return false;
          FileHandler.OpenDialog = (Form) new MKDS_Course_Modifier.UI.MKDS.NKM(File1);
          FileHandler.OpenDialog.Show((IWin32Window) Owner);
          FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
          return true;
        case "NCG.BIN":
          return false;
        case "NCL.BIN":
          return false;
        case "NSC.BIN":
          return false;
        case "SPA":
          MKDS_Course_Modifier.Particles.SPA Spa = new MKDS_Course_Modifier.Particles.SPA(file.Data);
          if (FileHandler.OpenDialog != null)
            return false;
          FileHandler.OpenDialog = (Form) new MKDS_Course_Modifier.UI.SPA(Spa);
          FileHandler.OpenDialog.Show((IWin32Window) Owner);
          FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
          return true;
        case "SSEQ":
          MKDS_Course_Modifier.Sound.SSEQ file1 = new MKDS_Course_Modifier.Sound.SSEQ(file.Data);
          if (FileHandler.OpenDialog != null)
            return false;
          FileHandler.OpenDialog = (Form) new MKDS_Course_Modifier.UI.SSEQ(file1, FileHandler.m);
          FileHandler.OpenDialog.Show((IWin32Window) Owner);
          FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
          return true;
        case "SBNK":
          SBNK k = new SBNK(file.Data);
          if (FileHandler.OpenDialog != null && FileHandler.OpenDialog is MKDS_Course_Modifier.UI.SSEQ)
          {
            SBNK s = SBNK.InitDLS(k, (SWAR[]) Parameter);
            ((MKDS_Course_Modifier.UI.SSEQ) FileHandler.OpenDialog).SetDLS(SBNK.ToDLS(s));
          }
          return false;
        case "SDAT":
          SDAT SDAT = new SDAT(file.Data);
          Owner.OpenSDAT(SDAT);
          return true;
        case "SSAR":
          MKDS_Course_Modifier.Sound.SSEQ file2 = new MKDS_Course_Modifier.Sound.SSEQ(file.Data, (int) Parameter);
          if (FileHandler.OpenDialog != null)
            return false;
          FileHandler.OpenDialog = (Form) new MKDS_Course_Modifier.UI.SSEQ(file2, FileHandler.m);
          FileHandler.OpenDialog.Show((IWin32Window) Owner);
          FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
          return true;
        case "NDS":
          NDS Rom = new NDS(file.Data);
          FileHandler.OpenedArchives.Clear();
          Owner.OpenNDS(Rom);
          return true;
        case "NCER":
          MKDS_Course_Modifier.G2D_Binary_File_Format.NCER Cell = new MKDS_Course_Modifier.G2D_Binary_File_Format.NCER(file.Data);
          if (FileHandler.OpenDialog == null)
          {
            FileHandler.OpenDialog = (Form) new MKDS_Course_Modifier.UI.NCER(Cell);
            FileHandler.OpenDialog.Show((IWin32Window) Owner);
            FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
            return true;
          }
          if (FileHandler.OpenDialog is MKDS_Course_Modifier.UI.BNCL)
            ((MKDS_Course_Modifier.UI.BNCL) FileHandler.OpenDialog).SetNCER(Cell);
          return false;
        case "BMG":
          MKDS_Course_Modifier.Misc.BMG File2 = new MKDS_Course_Modifier.Misc.BMG(file.Data);
          if (FileHandler.OpenDialog != null)
            return false;
          FileHandler.OpenDialog = (Form) new MKDS_Course_Modifier.UI.BMG(File2);
          FileHandler.OpenDialog.Show((IWin32Window) Owner);
          FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
          return true;
        case "SM64BMD":
          MKDS_Course_Modifier.SM64DS.BMD bmd = new MKDS_Course_Modifier.SM64DS.BMD(file.Data);
          if (FileHandler.OpenDialog is MKDS_Course_Modifier.UI.BMD)
          {
            ((MKDS_Course_Modifier.UI.BMD) FileHandler.OpenDialog).SetBMD(bmd);
            return false;
          }
          FileHandler.OpenDialog = (Form) new MKDS_Course_Modifier.UI.BMD(bmd);
          FileHandler.OpenDialog.Show((IWin32Window) Owner);
          FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
          return true;
        case "SM64BCA":
          MKDS_Course_Modifier.SM64DS.BCA Bca2 = new MKDS_Course_Modifier.SM64DS.BCA(file.Data);
          if (!(FileHandler.OpenDialog is MKDS_Course_Modifier.UI.BMD))
            return false;
          ((MKDS_Course_Modifier.UI.BMD) FileHandler.OpenDialog).SetBCA(Bca2);
          return true;
        case "BNCL":
          MKDS_Course_Modifier.Misc.BNCL Bncl = new MKDS_Course_Modifier.Misc.BNCL(file.Data);
          if (FileHandler.OpenDialog != null)
            return false;
          FileHandler.OpenDialog = (Form) new MKDS_Course_Modifier.UI.BNCL(Bncl);
          FileHandler.OpenDialog.Show((IWin32Window) Owner);
          FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
          return true;
        case "GCNBMD":
          MKDS_Course_Modifier.GCN.BMD file3 = new MKDS_Course_Modifier.GCN.BMD(file.Data);
          if (FileHandler.OpenDialog != null)
            return false;
          FileHandler.OpenDialog = (Form) new J3D1(file3);
          FileHandler.OpenDialog.Show((IWin32Window) Owner);
          FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
          return true;
        case "HBDF":
          HBDF hbdf = new HBDF(file.Data);
          if (hbdf.MDLFBlocks.Length > 0)
          {
            if (FileHandler.OpenDialog is MDLF)
            {
              ((MDLF) FileHandler.OpenDialog).SetHBDF(hbdf);
            }
            else
            {
              FileHandler.OpenDialog = (Form) new MDLF(hbdf);
              FileHandler.OpenDialog.Show((IWin32Window) Owner);
              FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
              return true;
            }
          }
          return false;
        case "GCNBOL":
          BOL bol = new BOL(file.Data);
          return false;
        case "PAZ":
          PAZ Arc = new PAZ(file.Data);
          Owner.OpenPAZ(Arc);
          return true;
        case "TEX":
          TEX tex = new TEX(file.Data);
          return false;
        case "GRPCONF":
          Grpconf grpconf = new Grpconf(file.Data);
          return false;
        case "OBJ":
          MKDS_Course_Modifier._3D_Formats.OBJ obj = new MKDS_Course_Modifier._3D_Formats.OBJ(file.Path);
          if (FileHandler.OpenDialog != null)
            return false;
          FileHandler.OpenDialog = (Form) new MKDS_Course_Modifier.UI.OBJ(obj, obj.MLTName == null ? (MLT) null : new MLT(obj.MLTName));
          FileHandler.OpenDialog.Show((IWin32Window) Owner);
          FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
          return true;
        case "GCNBLO":
          MKDS_Course_Modifier.GCN.BLO Layout = new MKDS_Course_Modifier.GCN.BLO(file.Data, file.Path);
          if (FileHandler.OpenDialog != null)
            return false;
          FileHandler.OpenDialog = (Form) new MKDS_Course_Modifier.UI.BLO(Layout);
          FileHandler.OpenDialog.Show((IWin32Window) Owner);
          FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
          return true;
        case "3DSCGFX":
          MKDS_Course_Modifier._3DS.CGFX cgfx = new MKDS_Course_Modifier._3DS.CGFX(file.Data);
          if (FileHandler.OpenDialog != null)
            return false;
          FileHandler.OpenDialog = (Form) new MKDS_Course_Modifier.UI.CGFX(cgfx);
          FileHandler.OpenDialog.Show((IWin32Window) Owner);
          FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
          return true;
        case "FMVVideo":
          MKDS_Course_Modifier.Misc.FMV Video = new MKDS_Course_Modifier.Misc.FMV(file.Data);
          if (FileHandler.OpenDialog != null)
            return false;
          FileHandler.OpenDialog = (Form) new MKDS_Course_Modifier.UI.FMV(Video);
          FileHandler.OpenDialog.Show((IWin32Window) Owner);
          FileHandler.OpenDialog.FormClosed += new FormClosedEventHandler(FileHandler.OpenDialog_FormClosed);
          return true;
        default:
          return false;
      }
    }

    public static void CloseFile(int ID)
    {
      FileHandler.OpenedFiles.RemoveAt(ID);
    }

    private static void OpenDialog_FormClosed(object sender, FormClosedEventArgs e)
    {
      FileHandler.OpenedFiles.Clear();
      FileHandler.OpenDialog = (Form) null;
    }

    public static void SetBigImageList(ref ImageList imageList2)
    {
      imageList2.Images.Add((Image) Resources.folder);
      imageList2.Images.Add((Image) Resources.color_swatch);
      imageList2.Images.Add((Image) Resources.image);
      imageList2.Images.Add((Image) Resources.image_sunset);
      imageList2.Images.Add((Image) Resources.zone);
      imageList2.Images.Add((Image) Resources.leaf);
      imageList2.Images.Add((Image) Resources.stack);
      imageList2.Images.Add((Image) Resources.lollypopanim);
      imageList2.Images.Add((Image) Resources.map);
      imageList2.Images.Add((Image) Resources.Cone);
      imageList2.Images.Add((Image) Resources.sunset_vertical);
      imageList2.Images.Add((Image) Resources.water);
      imageList2.Images.Add((Image) Resources.leaf_brown_pencil);
      imageList2.Images.Add((Image) Resources.traffic_light_anim);
      imageList2.Images.Add((Image) Resources.note);
      imageList2.Images.Add((Image) Resources.guitar);
      imageList2.Images.Add((Image) Resources.speaker);
      imageList2.Images.Add((Image) Resources.note_box);
      imageList2.Images.Add((Image) Resources.speaker2);
      imageList2.Images.Add((Image) Resources.speaker2_box);
      imageList2.Images.Add((Image) Resources.script_text);
      imageList2.Images.Add((Image) Resources.disc_music);
      imageList2.Images.Add((Image) Resources.truck_anim);
    }

    public static string GetType(ByteFileInfo file)
    {
      if (file.Data.Length >= 4)
      {
        string str = Convert.ToChar(file.Data[0]).ToString() + (object) Convert.ToChar(file.Data[1]) + (object) Convert.ToChar(file.Data[2]) + (object) Convert.ToChar(file.Data[3]);
        if (str.StartsWith("BM"))
        {
          try
          {
            Image.FromStream((Stream) new MemoryStream(file.Data));
            str = "BM";
          }
          catch
          {
          }
        }
        switch (str)
        {
          case " APS":
            return "SPA";
          case "0015":
            return "GCNBOL";
          case "add\0":
            return "PAZ";
          case "BCA0":
            return "NSBCA";
          case "BLCM":
            return "ZCB";
          case "BM":
            return "BMP";
          case "BMA0":
            return "NSBMA";
          case "BMD0":
            return "NSBMD";
          case "BTA0":
            return "NSBTA";
          case "BTP0":
            return "NSBTP";
          case "BTX0":
            return "NSBTX";
          case "BVA0":
            return "NSBVA";
          case "HBDF":
            return "HBDF";
          case "J3D1":
          case "J3D2":
            return "GCNBMD";
          case "JNCL":
            return "BNCL";
          case "MESG":
            return "BMG";
          case "NARC":
            return "NARC";
          case "NCCL":
            return "NCL";
          case "NKMD":
            return "NKM";
          case "NKMR":
            return "MR";
          case "RCSN":
            return "NSCR";
          case "RECN":
            return "NCER";
          case "RGCN":
            return "NCGR";
          case "RLCN":
          case "RPCN":
            return "NCLR";
          case "SBNK":
            return "SBNK";
          case "SDAT":
            return "SDAT";
          case "SSAR":
            return "SSAR";
          case "SSEQ":
            return "SSEQ";
          case "tex\0":
            return "TEX";
          case "SCRN":
            return "GCNBLO";
          case "CGFX":
            return "3DSCGFX";
          case "FMV!":
            return "FMVVideo";
        }
      }
      switch (Path.GetExtension(file.FileName).ToLower())
      {
        case ".enpg":
          return "ENPG";
        case ".kcl":
          return "KCL";
        case ".bin":
          if (file.FileName.Length > 7)
          {
            switch (file.FileName.Substring(file.FileName.Length - 7, 3).ToLower())
            {
              case "ncg":
                return "NCG.BIN";
              case "icg":
                return "ICG.BIN";
              case "icl":
                return "ICL.BIN";
              case "isc":
                return "ISC.BIN";
              case "ncl":
                return "NCL.BIN";
              case "nsc":
                return "NSC.BIN";
            }
          }
          else
            break;
          break;
        case ".nds":
          return "NDS";
        case ".bmd":
          return "SM64BMD";
        case ".bca":
          return "SM64BCA";
        case ".ssar":
          return "SSAR";
        case ".tbl":
          switch (file.FileName)
          {
            case "grpconf.tbl":
              return "GRPCONF";
          }
          break;
        case ".obj":
          return "OBJ";
      }
      return "";
    }

    public static Bitmap GetPreview(ByteFileInfo file)
    {
      switch (FileHandler.GetType(file))
      {
        case "NCGR":
          NCGR ncgr = new NCGR(file.Data);
          FileHandler.LastGraphic = ncgr;
          if (FileHandler.LastPal == null)
            return (Bitmap) null;
          try
          {
            return ncgr.CharacterData.ToBitmap(FileHandler.LastPal, 0);
          }
          catch
          {
            return (Bitmap) null;
          }
        case "NCLR":
          MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR nclr = new MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR(file.Data);
          System.Drawing.Color[] colorArray1 = nclr.PaletteData.ToColorArray();
          Bitmap bitmap1 = new Bitmap(128, (int) Math.Round((double) colorArray1.Length / 16.0) * 8);
          using (Graphics graphics = Graphics.FromImage((Image) bitmap1))
          {
            graphics.Clear(System.Drawing.Color.Transparent);
            int num = 0;
            for (int y = 0; y < (int) Math.Round((double) colorArray1.Length / 16.0) * 8; y += 8)
            {
              for (int x = 0; x < 128; x += 8)
              {
                if (num < colorArray1.Length)
                  graphics.FillRectangle((Brush) new SolidBrush(colorArray1[num++]), x, y, 8, 8);
              }
            }
          }
          FileHandler.LastPal = nclr;
          return bitmap1;
        case "NCL":
          System.Drawing.Color[] colorArray2 = new NCL(file.Data).PaletteData.ToColorArray();
          Bitmap bitmap2 = new Bitmap(128, (int) Math.Round((double) colorArray2.Length / 16.0) * 8);
          using (Graphics graphics = Graphics.FromImage((Image) bitmap2))
          {
            graphics.Clear(System.Drawing.Color.Transparent);
            int num = 0;
            for (int y = 0; y < (int) Math.Round((double) colorArray2.Length / 16.0) * 8; y += 8)
            {
              for (int x = 0; x < 128; x += 8)
              {
                if (num < colorArray2.Length)
                  graphics.FillRectangle((Brush) new SolidBrush(colorArray2[num++]), x, y, 8, 8);
              }
            }
          }
          return bitmap2;
        case "NSCR":
          MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR nscr = new MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR(file.Data);
          if (FileHandler.LastPal == null || FileHandler.LastGraphic == null)
            return (Bitmap) null;
          try
          {
            return nscr.ScreenData.ToBitmap(FileHandler.LastGraphic, FileHandler.LastPal);
          }
          catch
          {
            return (Bitmap) null;
          }
        case "NCER":
          MKDS_Course_Modifier.G2D_Binary_File_Format.NCER ncer = new MKDS_Course_Modifier.G2D_Binary_File_Format.NCER(file.Data);
          if (FileHandler.LastPal == null || FileHandler.LastGraphic == null)
            return (Bitmap) null;
          try
          {
            return ncer.CellBankBlock.CellDataBank.GetBitmap(0, FileHandler.LastGraphic, FileHandler.LastPal);
          }
          catch
          {
            return (Bitmap) null;
          }
        case "ICG.BIN":
        case "NCG.BIN":
          FileHandler.LastRawGraphic = file.Data;
          return FileHandler.LastRawPal != null ? Graphic.ConvertData(FileHandler.LastRawGraphic, FileHandler.LastRawPal, 0, (int) Math.Sqrt(FileHandler.LastRawPal.Length / 2 >= 256 ? (double) FileHandler.LastRawGraphic.Length : (double) (FileHandler.LastRawGraphic.Length * 2)), (int) Math.Sqrt(FileHandler.LastRawPal.Length / 2 >= 256 ? (double) FileHandler.LastRawGraphic.Length : (double) (FileHandler.LastRawGraphic.Length * 2)), FileHandler.LastRawPal.Length / 2 >= 256 ? Graphic.GXTexFmt.GX_TEXFMT_PLTT256 : Graphic.GXTexFmt.GX_TEXFMT_PLTT16, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR, true, false) : (Bitmap) null;
        case "ICL.BIN":
        case "NCL.BIN":
          System.Drawing.Color[] colorArray3 = Graphic.ConvertABGR1555(file.Data);
          Bitmap bitmap3 = new Bitmap(128, (int) Math.Round((double) colorArray3.Length / 16.0) * 8);
          using (Graphics graphics = Graphics.FromImage((Image) bitmap3))
          {
            int num = 0;
            for (int y = 0; y < (int) Math.Round((double) colorArray3.Length / 16.0) * 8; y += 8)
            {
              for (int x = 0; x < 128; x += 8)
                graphics.FillRectangle((Brush) new SolidBrush(colorArray3[num++]), x, y, 8, 8);
            }
          }
          FileHandler.LastRawPal = file.Data;
          return bitmap3;
        case "ISC.BIN":
        case "NSC.BIN":
          if (FileHandler.LastRawPal != null && FileHandler.LastRawGraphic != null)
          {
            try
            {
              return Graphic.ConvertData(FileHandler.LastRawGraphic, (int) Math.Sqrt(FileHandler.LastRawPal.Length / 2 >= 256 ? (double) FileHandler.LastRawGraphic.Length : (double) (FileHandler.LastRawGraphic.Length * 2)), (int) Math.Sqrt(FileHandler.LastRawPal.Length / 2 >= 256 ? (double) FileHandler.LastRawGraphic.Length : (double) (FileHandler.LastRawGraphic.Length * 2)), FileHandler.LastRawPal, file.Data, (int) Math.Sqrt((double) (file.Data.Length / 2)) * 8, (int) Math.Sqrt((double) (file.Data.Length / 2)) * 8, FileHandler.LastRawPal.Length / 2 >= 256 ? Graphic.GXTexFmt.GX_TEXFMT_PLTT256 : Graphic.GXTexFmt.GX_TEXFMT_PLTT16, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR);
            }
            catch
            {
            }
          }
          return (Bitmap) null;
        case "ENPG":
          try
          {
            return Graphic.ConvertData(((IEnumerable<byte>) file.Data).ToList<byte>().GetRange(0, file.Data.Length - 512).ToArray(), ((IEnumerable<byte>) file.Data).ToList<byte>().GetRange(file.Data.Length - 512, 512).ToArray(), 0, (int) Math.Sqrt((double) (file.Data.Length - 512)), (int) Math.Sqrt((double) (file.Data.Length - 512)), Graphic.GXTexFmt.GX_TEXFMT_PLTT256, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_BMP, true, false);
          }
          catch
          {
            return (Bitmap) null;
          }
        case "BMP":
          return (Bitmap) Image.FromStream((Stream) new MemoryStream(file.Data));
        case "TEX":
          return new TEX(file.Data).GetBitmap(0);
        default:
          return (Bitmap) null;
      }
    }

    public static bool ShowFileDialog(Form1 Owner)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      if (openFileDialog.ShowDialog() != DialogResult.OK || openFileDialog.FileName.Length <= 0 || !FileHandler.Open(openFileDialog.FileName, Owner))
        ;
      return false;
    }

    public static void Save(byte[] Data, int idx = 0, bool IsArchive = false)
    {
      if (IsArchive)
      {
        FileHandler.OpenedArchives.Last<ByteFileInfo>().Save(Data);
      }
      else
      {
        FileHandler.OpenedFiles[idx].Save(Data);
        if (FileHandler.OnSave != null && FileHandler.OpenedArchives.Count != 0)
          FileHandler.OnSave((object) null, (EventArgs) null);
      }
    }

    public static void SetListViewItemInfo(ListViewItem l1, string type, ListView l)
    {
      switch (type)
      {
        case "TEX":
        case "ENPG":
        case "BMP":
        case "ICG.BIN":
        case "NCG.BIN":
        case "NCGR":
          l1.ImageIndex = 2;
          l1.Group = l.Groups[4];
          break;
        case "ICL.BIN":
        case "NCL.BIN":
        case "NCLR":
          l1.ImageIndex = 1;
          l1.Group = l.Groups[3];
          break;
        case "ISC.BIN":
        case "NSC.BIN":
        case "NSCR":
          l1.ImageIndex = 8;
          l1.Group = l.Groups[6];
          break;
        case "SM64BMD":
        case "NSBMD":
          l1.ImageIndex = 5;
          l1.Group = l.Groups[1];
          break;
        case "NSBTX":
          l1.ImageIndex = 3;
          l1.Group = l.Groups[2];
          break;
        case "SM64BCA":
        case "NSBCA":
          l1.Group = l.Groups[5];
          l1.ImageIndex = 7;
          break;
        case "NSBTA":
          l1.Group = l.Groups[5];
          l1.ImageIndex = 10;
          break;
        case "NSBMA":
          l1.Group = l.Groups[5];
          l1.ImageIndex = 13;
          break;
        case "NSBTP":
          l1.Group = l.Groups[5];
          l1.ImageIndex = 6;
          break;
        case "NSBVA":
          l1.Group = l.Groups[5];
          l1.ImageIndex = 22;
          break;
        case "NCER":
          l1.Group = l.Groups[10];
          break;
        case "ZCB":
          l1.ImageIndex = 12;
          break;
        case "KCL":
          l1.Group = l.Groups[7];
          l1.ImageIndex = 12;
          break;
        case "NKM":
          l1.ImageIndex = 9;
          l1.Group = l.Groups[7];
          break;
        case "SPA":
          l1.ImageIndex = 11;
          l1.Group = l.Groups[8];
          break;
        case "BMG":
          l1.ImageIndex = 20;
          l1.Group = l.Groups[9];
          break;
        case "SDAT":
          l1.ImageIndex = 21;
          l1.Group = l.Groups[11];
          break;
      }
    }
  }
}
