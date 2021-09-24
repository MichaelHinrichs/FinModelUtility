// Decompiled with JetBrains decompiler
// Type: Flobbster.Windows.Forms.PropertySpec
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;

namespace Flobbster.Windows.Forms
{
  public class PropertySpec
  {
    private Attribute[] attributes;
    private string category;
    private object defaultValue;
    private string description;
    private string editor;
    private string name;
    private string type;
    private string typeConverter;

    public PropertySpec(string name, string type)
      : this(name, type, (string) null, (string) null, (object) null)
    {
    }

    public PropertySpec(string name, Type type)
      : this(name, type.AssemblyQualifiedName, (string) null, (string) null, (object) null)
    {
    }

    public PropertySpec(string name, string type, string category)
      : this(name, type, category, (string) null, (object) null)
    {
    }

    public PropertySpec(string name, Type type, string category)
      : this(name, type.AssemblyQualifiedName, category, (string) null, (object) null)
    {
    }

    public PropertySpec(string name, string type, string category, string description)
      : this(name, type, category, description, (object) null)
    {
    }

    public PropertySpec(string name, Type type, string category, string description)
      : this(name, type.AssemblyQualifiedName, category, description, (object) null)
    {
    }

    public PropertySpec(
      string name,
      string type,
      string category,
      string description,
      object defaultValue)
    {
      this.name = name;
      this.type = type;
      this.category = category;
      this.description = description;
      this.defaultValue = defaultValue;
      this.attributes = (Attribute[]) null;
    }

    public PropertySpec(
      string name,
      Type type,
      string category,
      string description,
      object defaultValue)
      : this(name, type.AssemblyQualifiedName, category, description, defaultValue)
    {
    }

    public PropertySpec(
      string name,
      string type,
      string category,
      string description,
      object defaultValue,
      string editor,
      string typeConverter)
      : this(name, type, category, description, defaultValue)
    {
      this.editor = editor;
      this.typeConverter = typeConverter;
    }

    public PropertySpec(
      string name,
      Type type,
      string category,
      string description,
      object defaultValue,
      string editor,
      string typeConverter)
      : this(name, type.AssemblyQualifiedName, category, description, defaultValue, editor, typeConverter)
    {
    }

    public PropertySpec(
      string name,
      string type,
      string category,
      string description,
      object defaultValue,
      Type editor,
      string typeConverter)
      : this(name, type, category, description, defaultValue, editor.AssemblyQualifiedName, typeConverter)
    {
    }

    public PropertySpec(
      string name,
      Type type,
      string category,
      string description,
      object defaultValue,
      Type editor,
      string typeConverter)
      : this(name, type.AssemblyQualifiedName, category, description, defaultValue, editor.AssemblyQualifiedName, typeConverter)
    {
    }

    public PropertySpec(
      string name,
      string type,
      string category,
      string description,
      object defaultValue,
      string editor,
      Type typeConverter)
      : this(name, type, category, description, defaultValue, editor, typeConverter.AssemblyQualifiedName)
    {
    }

    public PropertySpec(
      string name,
      Type type,
      string category,
      string description,
      object defaultValue,
      string editor,
      Type typeConverter)
      : this(name, type.AssemblyQualifiedName, category, description, defaultValue, editor, typeConverter.AssemblyQualifiedName)
    {
    }

    public PropertySpec(
      string name,
      string type,
      string category,
      string description,
      object defaultValue,
      Type editor,
      Type typeConverter)
      : this(name, type, category, description, defaultValue, editor.AssemblyQualifiedName, typeConverter.AssemblyQualifiedName)
    {
    }

    public PropertySpec(
      string name,
      Type type,
      string category,
      string description,
      object defaultValue,
      Type editor,
      Type typeConverter)
      : this(name, type.AssemblyQualifiedName, category, description, defaultValue, editor.AssemblyQualifiedName, typeConverter.AssemblyQualifiedName)
    {
    }

    public Attribute[] Attributes
    {
      get
      {
        return this.attributes;
      }
      set
      {
        this.attributes = value;
      }
    }

    public string Category
    {
      get
      {
        return this.category;
      }
      set
      {
        this.category = value;
      }
    }

    public string ConverterTypeName
    {
      get
      {
        return this.typeConverter;
      }
      set
      {
        this.typeConverter = value;
      }
    }

    public object DefaultValue
    {
      get
      {
        return this.defaultValue;
      }
      set
      {
        this.defaultValue = value;
      }
    }

    public string Description
    {
      get
      {
        return this.description;
      }
      set
      {
        this.description = value;
      }
    }

    public string EditorTypeName
    {
      get
      {
        return this.editor;
      }
      set
      {
        this.editor = value;
      }
    }

    public string Name
    {
      get
      {
        return this.name;
      }
      set
      {
        this.name = value;
      }
    }

    public string TypeName
    {
      get
      {
        return this.type;
      }
      set
      {
        this.type = value;
      }
    }
  }
}
