// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.GCN.MAP
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Globalization;
using System.IO;

namespace MKDS_Course_Modifier.GCN
{
  public class MAP
  {
    public MAP(string Path)
    {
      TextReader textReader = (TextReader) new StreamReader((Stream) File.OpenRead(Path));
      bool flag = false;
      string str1;
      while ((str1 = textReader.ReadLine()) != null)
      {
        if (str1.Length >= 4)
        {
          string str2 = !str1.Trim().Contains(" ") ? str1.Trim() : str1.Trim().Remove(str1.Trim().IndexOf(" "));
          if (!(str2 == "UNUSED"))
          {
            if (str2 == ".text")
              flag = true;
            else if (str2 == ".init")
              flag = true;
            else if (!(str2 == "Starting") && !(str2 == "extab"))
            {
              if (!(str2 == ".ctors") && !(str2 == ".dtors"))
              {
                if (!(str2 == ".rodata") && !(str2 == ".data") && (!(str2 == ".sbss") && !(str2 == ".sdata")) && (!(str2 == ".sdata2") && !(str2 == "address") && !(str2 == "-----------------------")))
                {
                  if (!(str2 == ".sbss2"))
                  {
                    if (str2[1] != ']' && flag)
                    {
                      string[] strArray1 = str1.Split(new char[1]
                      {
                        ' '
                      }, StringSplitOptions.RemoveEmptyEntries);
                      string[] strArray2 = str1.Split(new char[1]
                      {
                        '\t'
                      }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[1]
                      {
                        ' '
                      }, StringSplitOptions.RemoveEmptyEntries);
                      uint.Parse(strArray1[0], NumberStyles.HexNumber);
                      uint.Parse(strArray1[1], NumberStyles.HexNumber);
                      uint num = uint.Parse(strArray1[2], NumberStyles.HexNumber);
                      string str3 = !uint.TryParse(strArray1[3], NumberStyles.HexNumber, (IFormatProvider) null, out uint _) ? str1.Substring(str1.IndexOf(strArray1[3]), str1.IndexOf('\t') - str1.IndexOf(strArray1[3])).Trim() : strArray1[4];
                      strArray2[0].Trim();
                      string str4 = strArray2[1];
                      Console.WriteLine("{0:x} - {1} from {2}", (object) num, (object) str3, (object) str4);
                    }
                  }
                  else
                    break;
                }
              }
              else
                break;
            }
          }
        }
      }
      textReader.Close();
    }
  }
}
