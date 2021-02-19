// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.IO.ByteFileInfo
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Archive_Format;
using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.Misc;
using System;

namespace MKDS_Course_Modifier.IO
{
  public class ByteFileInfo : IDisposable
  {
    public byte[] Data;
    private NARC.FileEntry NARCFileEntry;
    private PAZ.PAZFileEntry PAZFileEntry;
    private bool FromFileEntry;

    public ByteFileInfo(NARC.FileEntry File)
    {
      this.FromFileEntry = true;
      this.NARCFileEntry = File;
      this.Data = File.Content;
      this.FileID = File.Id;
      this.FileName = File.Name;
      this.IsLocal = false;
      this.Path = (string) null;
      int num;
      if (this.Data.Length > 4)
        num = !(Convert.ToChar(this.Data[0]).ToString() + (object) Convert.ToChar(this.Data[1]) + (object) Convert.ToChar(this.Data[2]) + (object) Convert.ToChar(this.Data[3]) == "LZ77") ? 1 : 0;
      else
        num = 1;
      if (num == 0)
      {
        this.GotLZ77Header = true;
        this.IsLZ77Compressed = true;
        this.Data = Compression.LZ77DecompressHeader(this.Data);
      }
      else if (this.Data.Length > 4 && (this.Data[0] == (byte) 16 || this.Data[0] == (byte) 0))
      {
        try
        {
          byte[] numArray = Compression.LZ77Decompress(this.Data);
          if (numArray.Length != 0)
          {
            this.GotLZ77Header = false;
            this.IsLZ77Compressed = true;
            this.Data = numArray;
          }
          else
          {
            this.GotLZ77Header = false;
            this.IsLZ77Compressed = false;
          }
        }
        catch
        {
          this.GotLZ77Header = false;
          this.IsLZ77Compressed = false;
        }
      }
      if (this.Data.Length > 4 && this.Data[0] == (byte) 17 && !this.IsLZ77Compressed)
      {
        this.GotLZ77Header = false;
        try
        {
          byte[] numArray = Compression.LZ11Decompress(this.Data);
          if (numArray.Length != 0)
          {
            this.IsLZ11Compressed = true;
            this.Data = numArray;
          }
          else
            this.IsLZ11Compressed = false;
        }
        catch
        {
          this.IsLZ11Compressed = false;
        }
      }
      string type = FileHandler.GetType(this);
      if (!(type == "NDS") && !(type == "NARC") && !(type == "SDAT") && !(type == "PAZ"))
        return;
      this.IsArchive = true;
    }

    public ByteFileInfo(PAZ.PAZFileEntry File)
    {
      this.FromFileEntry = true;
      this.PAZFileEntry = File;
      this.Data = File.Data;
      this.FileID = -1;
      this.FileName = File.Name;
      this.IsLocal = false;
      this.Path = (string) null;
      int num;
      if (this.Data.Length > 4)
        num = !(Convert.ToChar(this.Data[0]).ToString() + (object) Convert.ToChar(this.Data[1]) + (object) Convert.ToChar(this.Data[2]) + (object) Convert.ToChar(this.Data[3]) == "LZ77") ? 1 : 0;
      else
        num = 1;
      if (num == 0)
      {
        this.GotLZ77Header = true;
        this.IsLZ77Compressed = true;
        this.Data = Compression.LZ77DecompressHeader(this.Data);
      }
      else if (this.Data.Length > 4 && (this.Data[0] == (byte) 16 || this.Data[0] == (byte) 0))
      {
        try
        {
          byte[] numArray = Compression.LZ77Decompress(this.Data);
          if (numArray.Length != 0)
          {
            this.GotLZ77Header = false;
            this.IsLZ77Compressed = true;
            this.Data = numArray;
          }
          else
          {
            this.GotLZ77Header = false;
            this.IsLZ77Compressed = false;
          }
        }
        catch
        {
          this.GotLZ77Header = false;
          this.IsLZ77Compressed = false;
        }
      }
      if (this.Data.Length > 4 && this.Data[0] == (byte) 17 && !this.IsLZ77Compressed)
      {
        this.GotLZ77Header = false;
        try
        {
          byte[] numArray = Compression.LZ11Decompress(this.Data);
          if (numArray.Length != 0)
          {
            this.IsLZ11Compressed = true;
            this.Data = numArray;
          }
          else
            this.IsLZ11Compressed = false;
        }
        catch
        {
          this.IsLZ11Compressed = false;
        }
      }
      string type = FileHandler.GetType(this);
      if (!(type == "NDS") && !(type == "NARC") && !(type == "SDAT") && !(type == "PAZ"))
        return;
      this.IsArchive = true;
    }

    public ByteFileInfo(string File)
    {
      this.Data = System.IO.File.ReadAllBytes(File);
      this.FileID = -1;
      this.FileName = System.IO.Path.GetFileName(File);
      this.IsLocal = true;
      this.Path = File;
      int num;
      if (this.Data.Length > 4)
        num = !(Convert.ToChar(this.Data[0]).ToString() + (object) Convert.ToChar(this.Data[1]) + (object) Convert.ToChar(this.Data[2]) + (object) Convert.ToChar(this.Data[3]) == "LZ77") ? 1 : 0;
      else
        num = 1;
      if (num == 0)
      {
        this.GotLZ77Header = true;
        this.IsLZ77Compressed = true;
        this.Data = Compression.LZ77DecompressHeader(this.Data);
      }
      else if (this.Data.Length > 4 && (this.Data[0] == (byte) 16 || this.Data[0] == (byte) 0))
      {
        try
        {
          byte[] numArray = Compression.LZ77Decompress(this.Data);
          if (numArray.Length != 0)
          {
            this.GotLZ77Header = false;
            this.IsLZ77Compressed = true;
            this.Data = numArray;
          }
          else
          {
            this.GotLZ77Header = false;
            this.IsLZ77Compressed = false;
          }
        }
        catch
        {
          this.GotLZ77Header = false;
          this.IsLZ77Compressed = false;
        }
      }
      if (this.Data.Length > 4 && this.Data[0] == (byte) 17 && !this.IsLZ77Compressed)
      {
        this.GotLZ77Header = false;
        try
        {
          byte[] numArray = Compression.LZ11Decompress(this.Data);
          if (numArray.Length != 0)
          {
            this.IsLZ11Compressed = true;
            this.Data = numArray;
          }
          else
            this.IsLZ11Compressed = false;
        }
        catch
        {
          this.IsLZ11Compressed = false;
        }
      }
      string type = FileHandler.GetType(this);
      if (!(type == "NDS") && !(type == "NARC") && !(type == "SDAT") && !(type == "PAZ"))
        return;
      this.IsArchive = true;
    }

    public ByteFileInfo(byte[] File, string FileName)
    {
      this.Data = File;
      this.FileID = -1;
      this.FileName = FileName;
      this.IsLocal = false;
      this.Path = (string) null;
      int num;
      if (this.Data.Length > 4)
        num = !(Convert.ToChar(this.Data[0]).ToString() + (object) Convert.ToChar(this.Data[1]) + (object) Convert.ToChar(this.Data[2]) + (object) Convert.ToChar(this.Data[3]) == "LZ77") ? 1 : 0;
      else
        num = 1;
      if (num == 0)
      {
        this.GotLZ77Header = true;
        this.IsLZ77Compressed = true;
        this.Data = Compression.LZ77DecompressHeader(this.Data);
      }
      else if (this.Data.Length > 4 && (this.Data[0] == (byte) 16 || this.Data[0] == (byte) 0))
      {
        try
        {
          byte[] numArray = Compression.LZ77Decompress(this.Data);
          if (numArray.Length != 0)
          {
            this.GotLZ77Header = false;
            this.IsLZ77Compressed = true;
            this.Data = numArray;
          }
          else
          {
            this.GotLZ77Header = false;
            this.IsLZ77Compressed = false;
          }
        }
        catch
        {
          this.GotLZ77Header = false;
          this.IsLZ77Compressed = false;
        }
      }
      if (this.Data.Length <= 4 || this.Data[0] != (byte) 17 || this.IsLZ77Compressed)
        return;
      this.GotLZ77Header = false;
      try
      {
        byte[] numArray = Compression.LZ11Decompress(this.Data);
        if (numArray.Length != 0)
        {
          this.IsLZ11Compressed = true;
          this.Data = numArray;
        }
        else
          this.IsLZ11Compressed = false;
      }
      catch
      {
        this.IsLZ11Compressed = false;
      }
    }

    public bool IsLocal { get; set; }

    public string Path { get; set; }

    public string FileName { get; set; }

    public int FileID { get; set; }

    public bool IsLZ77Compressed { get; set; }

    public bool IsLZ11Compressed { get; set; }

    public bool GotLZ77Header { get; set; }

    public bool IsArchive { get; set; }

    public void Dispose()
    {
      this.Data = (byte[]) null;
      this.Path = (string) null;
      this.FileName = (string) null;
      this.IsLocal = false;
      this.FileID = -1;
      this.IsLZ77Compressed = false;
      this.GotLZ77Header = false;
    }

    public void Save(byte[] Data)
    {
      byte[] bytes = Data;
      if (this.IsLZ11Compressed)
        bytes = Compression.LZ11Compress(Data);
      else if (this.IsLZ77Compressed && this.GotLZ77Header)
        bytes = Compression.MI_Compress.FastCompress(Data, true);
      else if (this.IsLZ77Compressed && !this.GotLZ77Header)
        bytes = Compression.MI_Compress.FastCompress(Data, false);
      if (this.IsLocal)
      {
        System.IO.File.Create(this.Path).Close();
        System.IO.File.WriteAllBytes(this.Path, bytes);
      }
      else
      {
        if (!this.FromFileEntry)
          return;
        if (this.NARCFileEntry != null)
        {
          this.NARCFileEntry.Content = bytes;
        }
        else
        {
          if (this.PAZFileEntry == null)
            return;
          this.PAZFileEntry.Data = bytes;
        }
      }
    }
  }
}
