// Decompiled with JetBrains decompiler
// Type: System.ObjectIDTypeConverter
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.MKDS;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace System
{
  public class ObjectIDTypeConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof (string) || base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      if (!(destinationType == typeof (string)) || !(value.GetType() == typeof (ushort)))
        return base.ConvertTo(context, culture, value, destinationType);
      string str = string.Format("{0:X4}", value);
      for (int index = 0; index < 4 - str.Length; ++index)
        str += "0";
      ObjectDb.Object @object = MKDS_Const.ObjectDatabase.GetObject(BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes((ushort) value)).Reverse<byte>().ToArray<byte>(), 0));
      return @object != null ? (object) (@object.Name + " (" + str + ")") : (object) str;
    }

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      if (!(value.GetType() == typeof (string)))
        return base.ConvertFrom(context, culture, value);
      string s = (string) value;
      if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        s = s.Substring(2);
      return (object) ushort.Parse(s, NumberStyles.HexNumber, (IFormatProvider) culture);
    }
  }
}
