// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.MKDS.ObjectDb
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;

namespace MKDS_Course_Modifier.MKDS
{
  public class ObjectDb
  {
    public List<ObjectDb.Category> Categories = new List<ObjectDb.Category>();

    public ObjectDb(byte[] Database, byte[] Schema)
    {
      XmlSchema schema = XmlSchema.Read((Stream) new MemoryStream(Schema), (ValidationEventHandler) null);
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Schemas.Add(schema);
      xmlDocument.Load((Stream) new MemoryStream(Database));
      foreach (XmlNode childNode in xmlDocument.ChildNodes[1].ChildNodes)
      {
        if (childNode.Name == "Category")
          this.Categories.Add(new ObjectDb.Category(childNode));
      }
    }

    public byte[] Write()
    {
      MemoryStream memoryStream = new MemoryStream();
      XmlTextWriter w = new XmlTextWriter((Stream) memoryStream, Encoding.GetEncoding("UTF-8"));
      w.Formatting = Formatting.Indented;
      w.Namespaces = false;
      w.WriteStartDocument();
      w.WriteStartElement("Root");
      w.WriteAttributeString("xmlns", "http://tempuri.org/XMLSchema.xsd");
      foreach (ObjectDb.Category category in this.Categories)
        category.Write(w);
      w.WriteEndElement();
      w.Flush();
      byte[] array = memoryStream.ToArray();
      w.Close();
      return array;
    }

    public void GetTreeNodes(TreeNodeCollection t, string Selectname = null)
    {
      TreeNode treeNode1 = t.Add("Root");
      treeNode1.Tag = (object) false;
      TreeNode treeNode2 = (TreeNode) null;
      foreach (ObjectDb.Category category in this.Categories)
      {
        TreeNode n = treeNode1.Nodes.Add(category.Name);
        n.Tag = (object) false;
        if (category.Name == Selectname)
          treeNode2 = n;
        category.GetTreeNodes(n, Selectname);
      }
      treeNode1.Expand();
      if (treeNode2 == null)
        return;
      treeNode2.EnsureVisible();
      treeNode2.Expand();
      treeNode2.TreeView.SelectedNode = treeNode2;
    }

    public bool AddCategory(string Path, string Name)
    {
      if (Path == "Root")
      {
        this.Categories.Add(new ObjectDb.Category(Name));
        return true;
      }
      if (!Path.StartsWith("Root"))
        return false;
      Path = Path.Remove(0, 4);
      string[] strArray = Path.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < this.Categories.Count; ++index)
      {
        if (this.Categories[index].AddCategory(string.Join("\\", strArray), Name))
          return true;
      }
      return false;
    }

    public bool RemoveCategory(string Path)
    {
      if (Path == "Root" || !Path.StartsWith("Root"))
        return false;
      Path = Path.Remove(0, 4);
      string[] strArray = Path.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < this.Categories.Count; ++index)
      {
        if (strArray.Length == 1)
        {
          if (this.Categories[index].Name == strArray[0])
          {
            this.Categories.RemoveAt(index);
            return true;
          }
        }
        else if (this.Categories[index].RemoveCategory(string.Join("\\", strArray)))
          return true;
      }
      return false;
    }

    public bool AddObject(string Path, string Name, ushort ObjectId)
    {
      if (Path == "Root" || !Path.StartsWith("Root"))
        return false;
      Path = Path.Remove(0, 4);
      string[] strArray = Path.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < this.Categories.Count; ++index)
      {
        if (this.Categories[index].AddObject(string.Join("\\", strArray), Name, ObjectId))
          return true;
      }
      return false;
    }

    public bool RemoveObject(string Path)
    {
      if (Path == "Root" || !Path.StartsWith("Root"))
        return false;
      Path = Path.Remove(0, 4);
      string[] strArray = Path.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < this.Categories.Count; ++index)
      {
        if (this.Categories[index].RemoveObject(string.Join("\\", strArray)))
          return true;
      }
      return false;
    }

    public ObjectDb.Object GetObject(string Path)
    {
      if (Path == "Root" || !Path.StartsWith("Root"))
        return (ObjectDb.Object) null;
      Path = Path.Remove(0, 4);
      string[] strArray = Path.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < this.Categories.Count; ++index)
      {
        ObjectDb.Object @object = this.Categories[index].GetObject(string.Join("\\", strArray));
        if (@object != null)
          return @object;
      }
      return (ObjectDb.Object) null;
    }

    public ObjectDb.Object GetObject(ushort ObjectId)
    {
      for (int index = 0; index < this.Categories.Count; ++index)
      {
        ObjectDb.Object @object = this.Categories[index].GetObject(ObjectId);
        if (@object != null)
          return @object;
      }
      return (ObjectDb.Object) null;
    }

    public class Category
    {
      public List<ObjectDb.Object> Objects = new List<ObjectDb.Object>();
      public List<ObjectDb.Category> Categories = new List<ObjectDb.Category>();
      public string Name;

      public Category(XmlNode Node)
      {
        this.Name = Node.Attributes[0].InnerText;
        foreach (XmlNode childNode in Node.ChildNodes)
        {
          if (childNode.Name == "Object")
            this.Objects.Add(new ObjectDb.Object(childNode));
          else
            this.Categories.Add(new ObjectDb.Category(childNode));
        }
      }

      public Category(string Name)
      {
        this.Name = Name;
      }

      public void Write(XmlTextWriter w)
      {
        w.WriteStartElement(nameof (Category));
        w.WriteAttributeString("Name", this.Name);
        foreach (ObjectDb.Object @object in this.Objects)
          @object.Write(w);
        foreach (ObjectDb.Category category in this.Categories)
          category.Write(w);
        w.WriteEndElement();
      }

      public void GetTreeNodes(TreeNode n, string Selectname = null)
      {
        TreeNode treeNode = (TreeNode) null;
        foreach (ObjectDb.Object @object in this.Objects)
          n.Nodes.Add(@object.ToString()).Tag = (object) true;
        foreach (ObjectDb.Category category in this.Categories)
        {
          TreeNode n1 = n.Nodes.Add(category.Name);
          n1.Tag = (object) false;
          if (category.Name == Selectname)
            treeNode = n1;
          category.GetTreeNodes(n1, Selectname);
        }
        if (treeNode == null)
          return;
        treeNode.EnsureVisible();
        treeNode.Expand();
        treeNode.TreeView.SelectedNode = treeNode;
      }

      public bool AddCategory(string Path, string Name)
      {
        if (Path == this.Name)
        {
          this.Categories.Add(new ObjectDb.Category(Name));
          return true;
        }
        if (!Path.StartsWith(this.Name))
          return false;
        Path = Path.Remove(0, this.Name.Length);
        string[] strArray = Path.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
        for (int index = 0; index < this.Categories.Count; ++index)
        {
          if (this.Categories[index].AddCategory(string.Join("\\", strArray), Name))
            return true;
        }
        return false;
      }

      public bool RemoveCategory(string Path)
      {
        if (Path == this.Name || !Path.StartsWith(this.Name))
          return false;
        Path = Path.Remove(0, this.Name.Length);
        string[] strArray = Path.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
        for (int index = 0; index < this.Categories.Count; ++index)
        {
          if (strArray.Length == 1)
          {
            if (this.Categories[index].Name == strArray[0])
            {
              this.Categories.RemoveAt(index);
              return true;
            }
          }
          else if (this.Categories[index].RemoveCategory(string.Join("\\", strArray)))
            return true;
        }
        return false;
      }

      public bool AddObject(string Path, string Name, ushort ObjectId)
      {
        if (Path == this.Name)
        {
          this.Objects.Add(new ObjectDb.Object(Name, ObjectId));
          return true;
        }
        if (!Path.StartsWith(this.Name))
          return false;
        Path = Path.Remove(0, this.Name.Length);
        string[] strArray = Path.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
        for (int index = 0; index < this.Categories.Count; ++index)
        {
          if (this.Categories[index].AddObject(string.Join("\\", strArray), Name, ObjectId))
            return true;
        }
        return false;
      }

      public bool RemoveObject(string Path)
      {
        if (Path == this.Name || !Path.StartsWith(this.Name))
          return false;
        Path = Path.Remove(0, this.Name.Length);
        string[] strArray = Path.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length == 1)
        {
          for (int index = 0; index < this.Objects.Count; ++index)
          {
            if (this.Objects[index].ToString() == strArray[0])
            {
              this.Objects.RemoveAt(index);
              return true;
            }
          }
        }
        else
        {
          for (int index = 0; index < this.Categories.Count; ++index)
          {
            if (this.Categories[index].RemoveObject(string.Join("\\", strArray)))
              return true;
          }
        }
        return false;
      }

      public ObjectDb.Object GetObject(string Path)
      {
        if (Path == this.Name || !Path.StartsWith(this.Name))
          return (ObjectDb.Object) null;
        Path = Path.Remove(0, this.Name.Length);
        string[] strArray = Path.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length == 1)
        {
          for (int index = 0; index < this.Objects.Count; ++index)
          {
            if (this.Objects[index].ToString() == strArray[0])
              return this.Objects[index];
          }
        }
        for (int index = 0; index < this.Categories.Count; ++index)
        {
          ObjectDb.Object @object = this.Categories[index].GetObject(string.Join("\\", strArray));
          if (@object != null)
            return @object;
        }
        return (ObjectDb.Object) null;
      }

      public ObjectDb.Object GetObject(ushort ObjectId)
      {
        for (int index = 0; index < this.Objects.Count; ++index)
        {
          if ((int) this.Objects[index].ObjectId == (int) ObjectId)
            return this.Objects[index];
        }
        for (int index = 0; index < this.Categories.Count; ++index)
        {
          ObjectDb.Object @object = this.Categories[index].GetObject(ObjectId);
          if (@object != null)
            return @object;
        }
        return (ObjectDb.Object) null;
      }

      public override string ToString()
      {
        return this.Name;
      }
    }

    public class Object
    {
      public ObjectDb.Object.Setting[] Settings = new ObjectDb.Object.Setting[4];
      public ushort ObjectId;

      public Object(XmlNode Node)
      {
        string innerText = Node.Attributes[0].InnerText;
        this.ObjectId = ushort.Parse(innerText[2].ToString() + (object) innerText[3] + (object) innerText[0] + (object) innerText[1], NumberStyles.HexNumber);
        this.Name = Node.Attributes[1].InnerText;
        this.RouteRequired = false;
        this.RequiredFiles = new List<ObjectDb.Object.File>();
        foreach (XmlNode childNode in Node.ChildNodes)
        {
          switch (childNode.Name)
          {
            case nameof (Description):
              this.Description = childNode.InnerText;
              break;
            case nameof (Picture):
              this.Picture = new Uri(childNode.InnerText);
              break;
            case "Route":
              this.RouteRequired = childNode.Attributes[0].InnerText == "true";
              break;
            case "Files":
              IEnumerator enumerator = childNode.ChildNodes.GetEnumerator();
              try
              {
                while (enumerator.MoveNext())
                  this.RequiredFiles.Add(new ObjectDb.Object.File((XmlNode) enumerator.Current));
                break;
              }
              finally
              {
                if (enumerator is IDisposable disposable)
                  disposable.Dispose();
              }
            case "Setting":
              this.Settings[int.Parse(childNode.Attributes[0].InnerText)] = new ObjectDb.Object.Setting(childNode);
              break;
          }
        }
      }

      public Object(string Name, ushort ObjectId)
      {
        this.Name = Name;
        this.ObjectId = ObjectId;
        this.RouteRequired = false;
        this.RequiredFiles = new List<ObjectDb.Object.File>();
        this.Settings[0] = new ObjectDb.Object.Setting();
        this.Setting1[0].Name = "Unknown1";
        this.Settings[1] = new ObjectDb.Object.Setting();
        this.Setting2[0].Name = "Unknown2";
        this.Settings[2] = new ObjectDb.Object.Setting();
        this.Setting3[0].Name = "Unknown3";
        this.Settings[3] = new ObjectDb.Object.Setting();
        this.Setting4[0].Name = "Unknown4";
      }

      public void Write(XmlTextWriter w)
      {
        w.WriteStartElement(nameof (Object));
        w.WriteAttributeString("Id", BitConverter.ToString(BitConverter.GetBytes(this.ObjectId)).Replace("-", ""));
        w.WriteAttributeString("Name", this.Name);
        if (this.Description != null && this.Description != "")
        {
          w.WriteStartElement("Description");
          w.WriteString(this.Description);
          w.WriteEndElement();
        }
        if (this.Picture != (Uri) null && this.Picture.ToString() != "")
        {
          w.WriteStartElement("Picture");
          w.WriteString(this.Picture.ToString());
          w.WriteEndElement();
        }
        if (this.RouteRequired)
        {
          w.WriteStartElement("Route");
          w.WriteAttributeString("Required", "true");
          w.WriteEndElement();
        }
        w.WriteStartElement("Files");
        foreach (ObjectDb.Object.File requiredFile in this.RequiredFiles)
          requiredFile.Write(w);
        w.WriteEndElement();
        this.Settings[0].Write(w, 0);
        this.Settings[1].Write(w, 1);
        this.Settings[2].Write(w, 2);
        this.Settings[3].Write(w, 3);
        w.WriteEndElement();
      }

      public string Name { get; set; }

      [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof (UITypeEditor))]
      public string Description { get; set; }

      [Browsable(false)]
      public Uri Picture { get; set; }

      public bool RouteRequired { get; set; }

      public List<ObjectDb.Object.File> RequiredFiles { get; set; }

      [Editor(typeof (ObjectSettingsEditor), typeof (UITypeEditor))]
      public List<ObjectDb.Object.Setting.SettingData> Setting1
      {
        get
        {
          return this.Settings[0].Data;
        }
        set
        {
          this.Settings[0].Data = value;
        }
      }

      [Editor(typeof (ObjectSettingsEditor), typeof (UITypeEditor))]
      public List<ObjectDb.Object.Setting.SettingData> Setting2
      {
        get
        {
          return this.Settings[1].Data;
        }
        set
        {
          this.Settings[1].Data = value;
        }
      }

      [Editor(typeof (ObjectSettingsEditor), typeof (UITypeEditor))]
      public List<ObjectDb.Object.Setting.SettingData> Setting3
      {
        get
        {
          return this.Settings[2].Data;
        }
        set
        {
          this.Settings[2].Data = value;
        }
      }

      [Editor(typeof (ObjectSettingsEditor), typeof (UITypeEditor))]
      public List<ObjectDb.Object.Setting.SettingData> Setting4
      {
        get
        {
          return this.Settings[3].Data;
        }
        set
        {
          this.Settings[3].Data = value;
        }
      }

      public override string ToString()
      {
        return this.Name + " (" + BitConverter.ToString(BitConverter.GetBytes(this.ObjectId)).Replace("-", "") + ")";
      }

      public Bitmap GetPicture()
      {
        try
        {
          WebClient webClient = new WebClient();
          string s = BitConverter.ToString(BitConverter.GetBytes(this.ObjectId)).Replace("-", "") + ".png";
          string lower = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(s))).Replace("-", "").ToLower();
          return new Bitmap((Stream) new MemoryStream(webClient.DownloadData("http://florian.nouwt.com/wiki/images/" + (object) lower[0] + "/" + (object) lower[0] + (object) lower[1] + "/" + s)));
        }
        catch
        {
          return (Bitmap) null;
        }
      }

      public bool GotPicture()
      {
        string s = BitConverter.ToString(BitConverter.GetBytes(this.ObjectId)).Replace("-", "") + ".png";
        string lower = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(s))).Replace("-", "").ToLower();
        try
        {
          HttpWebRequest httpWebRequest = WebRequest.Create("http://florian.nouwt.com/wiki/images/" + (object) lower[0] + "/" + (object) lower[0] + (object) lower[1] + "/" + s) as HttpWebRequest;
          httpWebRequest.Method = "HEAD";
          return (httpWebRequest.GetResponse() as HttpWebResponse).StatusCode == HttpStatusCode.OK;
        }
        catch
        {
          return false;
        }
      }

      public void GetPictureAsyc(PictureBox ResultOutput)
      {
        string s = BitConverter.ToString(BitConverter.GetBytes(this.ObjectId)).Replace("-", "") + ".png";
        string lower = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(s))).Replace("-", "").ToLower();
        ResultOutput.LoadAsync("http://florian.nouwt.com/wiki/images/" + (object) lower[0] + "/" + (object) lower[0] + (object) lower[1] + "/" + s);
      }

      public class File
      {
        public File(XmlNode Node)
        {
          this.FileName = Node.Attributes[0].InnerText;
        }

        public File()
        {
        }

        public void Write(XmlTextWriter w)
        {
          w.WriteStartElement(nameof (File));
          w.WriteAttributeString("FileName", this.FileName);
          w.WriteEndElement();
        }

        public string FileName { get; set; }

        public override string ToString()
        {
          return this.FileName;
        }
      }

      public class Setting
      {
        public List<ObjectDb.Object.Setting.SettingData> Data = new List<ObjectDb.Object.Setting.SettingData>();

        public Setting(XmlNode Node)
        {
          foreach (XmlNode childNode in Node.ChildNodes)
          {
            switch (childNode.Name)
            {
              case "U32":
                this.Data.Add((ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U32(childNode));
                break;
              case "S32":
                this.Data.Add((ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S32(childNode));
                break;
              case "U16":
                this.Data.Add((ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(childNode));
                break;
              case "S16":
                this.Data.Add((ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(childNode));
                break;
            }
          }
        }

        public Setting()
        {
          this.Data.Add((ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U32(true));
        }

        public void Write(XmlTextWriter w, int Index)
        {
          w.WriteStartElement(nameof (Setting));
          w.WriteAttributeString("Id", Index.ToString());
          foreach (ObjectDb.Object.Setting.SettingData settingData in this.Data)
          {
            w.WriteStartElement(settingData.GetType().Name);
            if (settingData.Name != "Unknown")
              w.WriteAttributeString("Name", settingData.Name);
            if (settingData.Hex)
              w.WriteAttributeString("Hex", "true");
            w.WriteEndElement();
          }
          w.WriteEndElement();
        }

        public class SettingData
        {
          public string Name = "Unknown";
          public bool Hex = false;

          public SettingData(XmlNode Node)
          {
            foreach (XmlAttribute attribute in (XmlNamedNodeMap) Node.Attributes)
            {
              switch (attribute.Name)
              {
                case nameof (Name):
                  this.Name = attribute.InnerText;
                  break;
                case nameof (Hex):
                  this.Hex = attribute.InnerText == "true";
                  break;
              }
            }
          }

          public SettingData(bool Hex)
          {
            this.Hex = Hex;
          }
        }

        public class U32 : ObjectDb.Object.Setting.SettingData
        {
          public U32(XmlNode Node)
            : base(Node)
          {
          }

          public U32(bool Hex)
            : base(Hex)
          {
          }
        }

        public class S32 : ObjectDb.Object.Setting.SettingData
        {
          public S32(XmlNode Node)
            : base(Node)
          {
          }

          public S32(bool Hex)
            : base(Hex)
          {
          }
        }

        public class U16 : ObjectDb.Object.Setting.SettingData
        {
          public U16(XmlNode Node)
            : base(Node)
          {
          }

          public U16(bool Hex)
            : base(Hex)
          {
          }
        }

        public class S16 : ObjectDb.Object.Setting.SettingData
        {
          public S16(XmlNode Node)
            : base(Node)
          {
          }

          public S16(bool Hex)
            : base(Hex)
          {
          }
        }
      }
    }
  }
}
