// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Language.LanguageHandler
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Language.Languages;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace MKDS_Course_Modifier.Language
{
  public class LanguageHandler
  {
    public static Dictionary<CultureInfo, ResourceManager> LanguageResources = new Dictionary<CultureInfo, ResourceManager>();

    public static void Initialize()
    {
      LanguageHandler.LanguageResources.Add(CultureInfo.CreateSpecificCulture("en"), en.ResourceManager);
      LanguageHandler.LanguageResources.Add(CultureInfo.CreateSpecificCulture("nl"), nl.ResourceManager);
      LanguageHandler.LanguageResources.Add(CultureInfo.CreateSpecificCulture("it"), it.ResourceManager);
      LanguageHandler.LanguageResources.Add(CultureInfo.CreateSpecificCulture("pl-PL"), pl_PL.ResourceManager);
      LanguageHandler.LanguageResources.Add(CultureInfo.CreateSpecificCulture("es-419"), es_419.ResourceManager);
      LanguageHandler.LanguageResources.Add(CultureInfo.CreateSpecificCulture("mk"), mk.ResourceManager);
      LanguageHandler.LanguageResources.Add(CultureInfo.CreateSpecificCulture("af-ZA"), af_ZA.ResourceManager);
      LanguageHandler.LanguageResources.Add(CultureInfo.CreateSpecificCulture("fr"), fr.ResourceManager);
      LanguageHandler.LanguageResources.Add(CultureInfo.CreateSpecificCulture("it-CH"), it_CH.ResourceManager);
      LanguageHandler.LanguageResources.Add(CultureInfo.CreateSpecificCulture("tr-TR"), tr_TR.ResourceManager);
    }

    public static string GetString(string name)
    {
      return LanguageHandler.LanguageResources.ContainsKey(CultureInfo.CurrentUICulture) && LanguageHandler.LanguageResources[CultureInfo.CurrentUICulture].GetString(name) != null ? LanguageHandler.LanguageResources[CultureInfo.CurrentUICulture].GetString(name) : en.ResourceManager.GetString(name);
    }
  }
}
