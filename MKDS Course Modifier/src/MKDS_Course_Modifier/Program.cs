// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Program
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.Converters.Colission;
using MKDS_Course_Modifier.G2D_Binary_File_Format;
using MKDS_Course_Modifier.IO;
using MKDS_Course_Modifier.Language;
using MKDS_Course_Modifier.MKDS;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MKDS_Course_Modifier
{
  internal static class Program
  {
    [STAThread]
    private static void Main(string[] args)
    {
      if (args != null && args.Length > 0)
      {
        if (!Program.AttachConsole(-1))
          Program.AllocConsole();
        switch (args[0].ToLower())
        {
          case "asm":
            if (args.Length > 2)
            {
              switch (args[1].ToLower())
              {
                case "patch":
                  ARM9 arM9 = new ARM9(System.IO.File.ReadAllBytes(args[2]));
                  arM9.AddCustomCode(Path.GetDirectoryName(args[2]));
                  System.IO.File.WriteAllBytes(Path.GetDirectoryName(args[2]) + "\\" + Path.GetFileNameWithoutExtension(args[2]) + "_new.bin", arM9.Write());
                  break;
                default:
                  Program.PrintUsage();
                  break;
              }
            }
            else
            {
              Program.PrintUsage();
              break;
            }
            break;
          case "g2d":
            if (args.Length > 4)
            {
              switch (args[1].ToLower())
              {
                case "pal":
                  switch (args[2].ToLower())
                  {
                    case "4bpp":
                      Bitmap b1 = (Bitmap) Image.FromFile(args[3]);
                      NCLR nclr1 = new NCLR(Graphic.ToABGR1555(Graphic.GeneratePalette(b1, 16, false, false)), Graphic.GXTexFmt.GX_TEXFMT_PLTT16);
                      System.IO.File.Create(args[4]).Close();
                      System.IO.File.WriteAllBytes(args[4], nclr1.Write());
                      b1.Dispose();
                      break;
                    case "8bpp":
                      Bitmap b2 = (Bitmap) Image.FromFile(args[4]);
                      NCLR nclr2 = new NCLR(Graphic.ToABGR1555(Graphic.GeneratePalette(b2, 256, false, false)), Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
                      System.IO.File.Create(args[5]).Close();
                      System.IO.File.WriteAllBytes(args[5], nclr2.Write());
                      b2.Dispose();
                      break;
                    default:
                      Program.PrintUsage();
                      break;
                  }
                  break;
                case "grp":
                  bool firstTransparent;
                  if (args.Length > 5)
                  {
                    switch (args[2].ToLower())
                    {
                      case "4bpp":
                        Bitmap b3 = (Bitmap) Image.FromFile(args[3]);
                        byte[] Data1;
                        byte[] Palette1;
                        Graphic.ConvertBitmap(b3, out Data1, out Palette1, Graphic.GXTexFmt.GX_TEXFMT_PLTT16, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR, out firstTransparent, false);
                        NCLR nclr3 = new NCLR(Palette1, Graphic.GXTexFmt.GX_TEXFMT_PLTT16);
                        NCGR ncgr1 = new NCGR(Data1, b3.Width / 8, b3.Height / 8, Graphic.GXTexFmt.GX_TEXFMT_PLTT16);
                        System.IO.File.Create(args[4]).Close();
                        System.IO.File.WriteAllBytes(args[4], nclr3.Write());
                        System.IO.File.Create(args[5]).Close();
                        System.IO.File.WriteAllBytes(args[5], ncgr1.Write());
                        b3.Dispose();
                        break;
                      case "8bpp":
                        Bitmap b4 = (Bitmap) Image.FromFile(args[3]);
                        byte[] Data2;
                        byte[] Palette2;
                        Graphic.ConvertBitmap(b4, out Data2, out Palette2, Graphic.GXTexFmt.GX_TEXFMT_PLTT256, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR, out firstTransparent, false);
                        NCLR nclr4 = new NCLR(Palette2, Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
                        NCGR ncgr2 = new NCGR(Data2, b4.Width / 8, b4.Height / 8, Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
                        System.IO.File.Create(args[4]).Close();
                        System.IO.File.WriteAllBytes(args[4], nclr4.Write());
                        System.IO.File.Create(args[5]).Close();
                        System.IO.File.WriteAllBytes(args[5], ncgr2.Write());
                        b4.Dispose();
                        break;
                      default:
                        Program.PrintUsage();
                        break;
                    }
                  }
                  else
                  {
                    Program.PrintUsage();
                    break;
                  }
                  break;
                case "map":
                  if (args.Length > 7)
                  {
                    switch (args[2].ToLower())
                    {
                      case "8bpp":
                        switch (args[3].ToLower())
                        {
                          case "single":
                            Bitmap b5 = (Bitmap) Image.FromFile(args[4]);
                            byte[] Palette3;
                            byte[] Tilemap1;
                            byte[] Screendata;
                            Graphic.ConvertBitmap(b5, out Palette3, out Tilemap1, out Screendata, Graphic.GXTexFmt.GX_TEXFMT_PLTT256, true);
                            NCLR nclr5 = new NCLR(Palette3, Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
                            NCGR ncgr3 = new NCGR(Tilemap1, Tilemap1.Length / 64 * 8, 8, Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
                            NSCR nscr1 = new NSCR(Screendata, b5.Width, b5.Height, Graphic.NNSG2dColorMode.NNS_G2D_SCREENCOLORMODE_16x16);
                            System.IO.File.Create(args[5]).Close();
                            System.IO.File.WriteAllBytes(args[5], nclr5.Write());
                            System.IO.File.Create(args[6]).Close();
                            System.IO.File.WriteAllBytes(args[6], ncgr3.Write());
                            System.IO.File.Create(args[7]).Close();
                            System.IO.File.WriteAllBytes(args[7], nscr1.Write());
                            b5.Dispose();
                            break;
                          case "duo":
                            if (args.Length > 9)
                            {
                              Bitmap a = (Bitmap) Image.FromFile(args[4]);
                              Bitmap b6 = (Bitmap) Image.FromFile(args[5]);
                              byte[] Palette4;
                              byte[] Tilemap2;
                              byte[] ScreendataA;
                              byte[] ScreendataB;
                              Graphic.ConvertBitmap(a, b6, out Palette4, out Tilemap2, out ScreendataA, out ScreendataB, Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
                              NCLR nclr6 = new NCLR(Palette4, Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
                              NCGR ncgr4 = new NCGR(Tilemap2, Tilemap2.Length / 64 * 8, 8, Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
                              NSCR nscr2 = new NSCR(ScreendataA, b6.Width, b6.Height, Graphic.NNSG2dColorMode.NNS_G2D_SCREENCOLORMODE_16x16);
                              NSCR nscr3 = new NSCR(ScreendataB, b6.Width, b6.Height, Graphic.NNSG2dColorMode.NNS_G2D_SCREENCOLORMODE_16x16);
                              System.IO.File.Create(args[6]).Close();
                              System.IO.File.WriteAllBytes(args[6], nclr6.Write());
                              System.IO.File.Create(args[7]).Close();
                              System.IO.File.WriteAllBytes(args[7], ncgr4.Write());
                              System.IO.File.Create(args[8]).Close();
                              System.IO.File.WriteAllBytes(args[8], nscr2.Write());
                              System.IO.File.Create(args[9]).Close();
                              System.IO.File.WriteAllBytes(args[9], nscr3.Write());
                              a.Dispose();
                              b6.Dispose();
                              break;
                            }
                            Program.PrintUsage();
                            break;
                          default:
                            Program.PrintUsage();
                            break;
                        }
                        break;
                      default:
                        Program.PrintUsage();
                        break;
                    }
                  }
                  else
                  {
                    Program.PrintUsage();
                    break;
                  }
                  break;
                default:
                  Program.PrintUsage();
                  break;
              }
            }
            else
            {
              Program.PrintUsage();
              break;
            }
            break;
          case "help":
            Program.PrintUsage();
            break;
          case "mkds":
            if (args.Length > 1)
            {
              switch (args[1])
              {
                case "kcl":
                  if (args.Length > 3)
                  {
                    Obj2Kcl.ConvertToKcl(args[2], args[3]);
                    break;
                  }
                  Program.PrintUsage();
                  break;
              }
            }
            else
            {
              Program.PrintUsage();
              break;
            }
            break;
          default:
            Program.PrintUsage();
            break;
        }
        Program.FreeConsole();
        SendKeys.SendWait("{ENTER}");
      }
      else
        Program.Run((string) null);
    }

    private static void Run(string Path = null)
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      LanguageHandler.Initialize();
      MKDS_Const.ObjectDatabase = new ObjectDb(System.IO.File.ReadAllBytes("ObjectDb.xml"), System.IO.File.ReadAllBytes("ObjectDb.xsd"));
      if (Path == null)
        Application.Run((Form) new Form1());
      else
        Application.Run((Form) new Form1(Path));
    }

    private static void PrintUsage()
    {
      Console.WriteLine("Usage:");
      Console.WriteLine("asm - ASM Hacking");
      Console.WriteLine("    patch [arm9.bin Path]");
      Console.WriteLine("g2d - Nitro 2D");
      Console.WriteLine("    grp - NCLR + NCGR");
      Console.WriteLine("        4bpp [Bitmap Path] [NCLR Path] [NCGR Path]");
      Console.WriteLine("        8bpp [Bitmap Path] [NCLR Path] [NCGR Path]");
      Console.WriteLine("    map - NCLR + NCGR + NSCR");
      Console.WriteLine("        8bpp");
      Console.WriteLine("             single [Bitmap Path] [NCLR Path] [NCGR Path] [NSCR Path]");
      Console.WriteLine("             duo [Bitmap 1 Path] [Bitmap 2 Path] [NCLR Path] [NCGR Path] [NSCR 1 Path] [NSCR 2 Path]");
      Console.WriteLine("    pal - NCLR");
      Console.WriteLine("        4bpp [Bitmap Path] [NCLR Path]");
      Console.WriteLine("        8bpp [Bitmap Path] [NCLR Path]");
      Console.WriteLine("help - Show this information");
      Console.WriteLine("mkds - MKDS related");
      Console.WriteLine("     kcl [OBJ Path] [KCL Path]");
    }

    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern bool AttachConsole(int pid);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeConsole();
  }
}
