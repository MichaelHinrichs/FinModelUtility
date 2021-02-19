// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Language.Languages.nl
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace MKDS_Course_Modifier.Language.Languages
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [CompilerGenerated]
  [DebuggerNonUserCode]
  internal class nl
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal nl()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) nl.resourceMan, (object) null))
          nl.resourceMan = new ResourceManager("MKDS_Course_Modifier.Language.Languages.nl", typeof (nl).Assembly);
        return nl.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return nl.resourceCulture;
      }
      set
      {
        nl.resourceCulture = value;
      }
    }

    internal static string base_close
    {
      get
      {
        return nl.ResourceManager.GetString("base.close", nl.resourceCulture);
      }
    }

    internal static string base_edit
    {
      get
      {
        return nl.ResourceManager.GetString("base.edit", nl.resourceCulture);
      }
    }

    internal static string base_export
    {
      get
      {
        return nl.ResourceManager.GetString("base.export", nl.resourceCulture);
      }
    }

    internal static string base_import
    {
      get
      {
        return nl.ResourceManager.GetString("base.import", nl.resourceCulture);
      }
    }

    internal static string base_new
    {
      get
      {
        return nl.ResourceManager.GetString("base.new", nl.resourceCulture);
      }
    }

    internal static string base_open
    {
      get
      {
        return nl.ResourceManager.GetString("base.open", nl.resourceCulture);
      }
    }

    internal static string base_save
    {
      get
      {
        return nl.ResourceManager.GetString("base.save", nl.resourceCulture);
      }
    }
  }
}
