// Decompiled with JetBrains decompiler
// Type: Flobbster.Windows.Forms.PropertyBag
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;

namespace Flobbster.Windows.Forms
{
  public class PropertyBag : ICustomTypeDescriptor
  {
    private string defaultProperty;
    private PropertyBag.PropertySpecCollection properties;

    public PropertyBag()
    {
      this.defaultProperty = (string) null;
      this.properties = new PropertyBag.PropertySpecCollection();
    }

    [Browsable(false)]
    public string DefaultProperty
    {
      get
      {
        return this.defaultProperty;
      }
      set
      {
        this.defaultProperty = value;
      }
    }

    [Browsable(false)]
    public PropertyBag.PropertySpecCollection Properties
    {
      get
      {
        return this.properties;
      }
    }

    public event PropertySpecEventHandler GetValue;

    public event PropertySpecEventHandler SetValue;

    protected virtual void OnGetValue(PropertySpecEventArgs e)
    {
      if (this.GetValue == null)
        return;
      this.GetValue((object) this, e);
    }

    protected virtual void OnSetValue(PropertySpecEventArgs e)
    {
      if (this.SetValue == null)
        return;
      this.SetValue((object) this, e);
    }

    AttributeCollection ICustomTypeDescriptor.GetAttributes()
    {
      return TypeDescriptor.GetAttributes((object) this, true);
    }

    string ICustomTypeDescriptor.GetClassName()
    {
      return TypeDescriptor.GetClassName((object) this, true);
    }

    string ICustomTypeDescriptor.GetComponentName()
    {
      return TypeDescriptor.GetComponentName((object) this, true);
    }

    TypeConverter ICustomTypeDescriptor.GetConverter()
    {
      return TypeDescriptor.GetConverter((object) this, true);
    }

    EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
    {
      return TypeDescriptor.GetDefaultEvent((object) this, true);
    }

    PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
    {
      PropertySpec propertySpec = (PropertySpec) null;
      if (this.defaultProperty != null)
        propertySpec = this.properties[this.properties.IndexOf(this.defaultProperty)];
      return propertySpec != null ? (PropertyDescriptor) new PropertyBag.PropertySpecDescriptor(propertySpec, this, propertySpec.Name, (Attribute[]) null) : (PropertyDescriptor) null;
    }

    object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
    {
      return TypeDescriptor.GetEditor((object) this, editorBaseType, true);
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
    {
      return TypeDescriptor.GetEvents((object) this, true);
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents(
      Attribute[] attributes)
    {
      return TypeDescriptor.GetEvents((object) this, attributes, true);
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
    {
      return ((ICustomTypeDescriptor) this).GetProperties(new Attribute[0]);
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(
      Attribute[] attributes)
    {
      ArrayList arrayList1 = new ArrayList();
      foreach (PropertySpec property in this.properties)
      {
        ArrayList arrayList2 = new ArrayList();
        if (property.Category != null)
          arrayList2.Add((object) new CategoryAttribute(property.Category));
        if (property.Description != null)
          arrayList2.Add((object) new DescriptionAttribute(property.Description));
        if (property.EditorTypeName != null)
          arrayList2.Add((object) new EditorAttribute(property.EditorTypeName, typeof (UITypeEditor)));
        if (property.ConverterTypeName != null)
          arrayList2.Add((object) new TypeConverterAttribute(property.ConverterTypeName));
        if (property.Attributes != null)
          arrayList2.AddRange((ICollection) property.Attributes);
        Attribute[] array = (Attribute[]) arrayList2.ToArray(typeof (Attribute));
        PropertyBag.PropertySpecDescriptor propertySpecDescriptor = new PropertyBag.PropertySpecDescriptor(property, this, property.Name, array);
        arrayList1.Add((object) propertySpecDescriptor);
      }
      arrayList1.AddRange((ICollection) TypeDescriptor.GetProperties((object) this, attributes, true));
      return new PropertyDescriptorCollection((PropertyDescriptor[]) arrayList1.ToArray(typeof (PropertyDescriptor)));
    }

    object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
    {
      return (object) this;
    }

    [Serializable]
    public class PropertySpecCollection : IList, ICollection, IEnumerable
    {
      private ArrayList innerArray;

      public PropertySpecCollection()
      {
        this.innerArray = new ArrayList();
      }

      public int Count
      {
        get
        {
          return this.innerArray.Count;
        }
      }

      public bool IsFixedSize
      {
        get
        {
          return false;
        }
      }

      public bool IsReadOnly
      {
        get
        {
          return false;
        }
      }

      public bool IsSynchronized
      {
        get
        {
          return false;
        }
      }

      object ICollection.SyncRoot
      {
        get
        {
          return (object) null;
        }
      }

      public PropertySpec this[int index]
      {
        get
        {
          return (PropertySpec) this.innerArray[index];
        }
        set
        {
          this.innerArray[index] = (object) value;
        }
      }

      public int Add(PropertySpec value)
      {
        return this.innerArray.Add((object) value);
      }

      public void AddRange(PropertySpec[] array)
      {
        this.innerArray.AddRange((ICollection) array);
      }

      public void Clear()
      {
        this.innerArray.Clear();
      }

      public bool Contains(PropertySpec item)
      {
        return this.innerArray.Contains((object) item);
      }

      public bool Contains(string name)
      {
        foreach (PropertySpec inner in this.innerArray)
        {
          if (inner.Name == name)
            return true;
        }
        return false;
      }

      public void CopyTo(PropertySpec[] array)
      {
        this.innerArray.CopyTo((Array) array);
      }

      public void CopyTo(PropertySpec[] array, int index)
      {
        this.innerArray.CopyTo((Array) array, index);
      }

      public IEnumerator GetEnumerator()
      {
        return this.innerArray.GetEnumerator();
      }

      public int IndexOf(PropertySpec value)
      {
        return this.innerArray.IndexOf((object) value);
      }

      public int IndexOf(string name)
      {
        int num = 0;
        foreach (PropertySpec inner in this.innerArray)
        {
          if (inner.Name == name)
            return num;
          ++num;
        }
        return -1;
      }

      public void Insert(int index, PropertySpec value)
      {
        this.innerArray.Insert(index, (object) value);
      }

      public void Remove(PropertySpec obj)
      {
        this.innerArray.Remove((object) obj);
      }

      public void Remove(string name)
      {
        this.RemoveAt(this.IndexOf(name));
      }

      public void RemoveAt(int index)
      {
        this.innerArray.RemoveAt(index);
      }

      public PropertySpec[] ToArray()
      {
        return (PropertySpec[]) this.innerArray.ToArray(typeof (PropertySpec));
      }

      void ICollection.CopyTo(Array array, int index)
      {
        this.CopyTo((PropertySpec[]) array, index);
      }

      int IList.Add(object value)
      {
        return this.Add((PropertySpec) value);
      }

      bool IList.Contains(object obj)
      {
        return this.Contains((PropertySpec) obj);
      }

      object IList.this[int index]
      {
        get
        {
          return (object) this[index];
        }
        set
        {
          this[index] = (PropertySpec) value;
        }
      }

      int IList.IndexOf(object obj)
      {
        return this.IndexOf((PropertySpec) obj);
      }

      void IList.Insert(int index, object value)
      {
        this.Insert(index, (PropertySpec) value);
      }

      void IList.Remove(object value)
      {
        this.Remove((PropertySpec) value);
      }
    }

    private class PropertySpecDescriptor : PropertyDescriptor
    {
      private PropertyBag bag;
      private PropertySpec item;

      public PropertySpecDescriptor(
        PropertySpec item,
        PropertyBag bag,
        string name,
        Attribute[] attrs)
        : base(name, attrs)
      {
        this.bag = bag;
        this.item = item;
      }

      public override Type ComponentType
      {
        get
        {
          return this.item.GetType();
        }
      }

      public override bool IsReadOnly
      {
        get
        {
          return this.Attributes.Matches((Attribute) ReadOnlyAttribute.Yes);
        }
      }

      public override Type PropertyType
      {
        get
        {
          return Type.GetType(this.item.TypeName);
        }
      }

      public override bool CanResetValue(object component)
      {
        return this.item.DefaultValue != null && !this.GetValue(component).Equals(this.item.DefaultValue);
      }

      public override object GetValue(object component)
      {
        PropertySpecEventArgs e = new PropertySpecEventArgs(this.item, (object) null);
        this.bag.OnGetValue(e);
        return e.Value;
      }

      public override void ResetValue(object component)
      {
        this.SetValue(component, this.item.DefaultValue);
      }

      public override void SetValue(object component, object value)
      {
        this.bag.OnSetValue(new PropertySpecEventArgs(this.item, value));
      }

      public override bool ShouldSerializeValue(object component)
      {
        object obj = this.GetValue(component);
        return (this.item.DefaultValue != null || obj != null) && !obj.Equals(this.item.DefaultValue);
      }
    }
  }
}
