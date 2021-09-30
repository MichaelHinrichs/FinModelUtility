// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G3D_Binary_File_Format.Dictionary`1
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MKDS_Course_Modifier.G3D_Binary_File_Format
{
  public class Dictionary<T> where T : DictionaryData, new()
  {
    public byte revision = 0;
    public byte numEntry;
    public ushort sizeDictBlk;
    public ushort ofsEntry;
    public Dictionary<T>.PtreeNode[] node;
    public Dictionary<T>.DictEntry entry;
    public List<string> names;

    public Dictionary(EndianBinaryReader er)
    {
      this.revision = er.ReadByte();
      this.numEntry = er.ReadByte();
      this.sizeDictBlk = er.ReadUInt16();
      er.ReadBytes(2);
      this.ofsEntry = er.ReadUInt16();
      this.node = new Dictionary<T>.PtreeNode[(int) this.numEntry + 1];
      for (int index = 0; index < (int) this.numEntry + 1; ++index)
      {
        this.node[index] = new Dictionary<T>.PtreeNode();
        this.node[index].refBit = er.ReadByte();
        this.node[index].idxLeft = er.ReadByte();
        this.node[index].idxRight = er.ReadByte();
        this.node[index].idxEntry = er.ReadByte();
      }
      this.entry = new Dictionary<T>.DictEntry();
      this.entry.sizeUnit = er.ReadUInt16();
      this.entry.ofsName = er.ReadUInt16();
      this.entry.data = new List<T>();
      for (int index = 0; index < (int) this.numEntry; ++index)
      {
        this.entry.data.Add(new T());
        this.entry.data[index].Read(er);
      }
      this.names = new List<string>();
      for (int index = 0; index < (int) this.numEntry; ++index)
        this.names.Add(er.ReadString(Encoding.ASCII, 16).Replace("\0", ""));
    }

    public Dictionary()
    {
      this.entry = new Dictionary<T>.DictEntry();
      this.names = new List<string>();
      this.RegenerateTree();
    }

    public void Write(EndianBinaryWriter er)
    {
      long position1 = er.BaseStream.Position;
      er.Write(this.revision);
      er.Write(this.numEntry);
      er.Write((ushort) 0);
      er.Write((ushort) 8);
      er.Write((ushort) (((int) this.numEntry + 1) * 4 + 8));
      foreach (Dictionary<T>.PtreeNode ptreeNode in this.node)
        ptreeNode.Write(er);
      this.entry.Write(er);
      foreach (string name in this.names)
        er.Write(name.PadRight(16, char.MinValue), Encoding.ASCII, false);
      long position2 = er.BaseStream.Position;
      er.BaseStream.Position = position1 + 2L;
      er.Write((ushort) (position2 - position1));
      er.BaseStream.Position = position2;
    }

    public bool Contains(string s)
    {
      return this.names.Contains(s);
    }

    public bool Contains(T d)
    {
      return this.entry.data.Contains(d);
    }

    public int IndexOf(string s)
    {
      if (this.numEntry < (byte) 16)
      {
        int num = 0;
        foreach (string name in this.names)
        {
          if (name == s)
            return num;
          ++num;
        }
      }
      else
      {
        s.PadRight(16, char.MinValue);
        int num1 = 0;
        Dictionary<T>.PtreeNode ptreeNode1 = this.node[0];
        if (ptreeNode1.idxLeft != (byte) 0)
        {
          Dictionary<T>.PtreeNode ptreeNode2;
          for (ptreeNode2 = this.node[num1 + (int) ptreeNode1.idxLeft]; (int) ptreeNode1.refBit > (int) ptreeNode2.refBit; ptreeNode2 = this.node[num1 + (((int) s[(int) ptreeNode2.refBit / 8] >> (int) ptreeNode2.refBit - (int) ptreeNode2.refBit / 8 * 8 & 1) == 0 ? (int) ptreeNode2.idxLeft : (int) ptreeNode2.idxRight)])
          {
            ptreeNode1 = ptreeNode2;
            uint num2 = 0;
            int index = ((int) ptreeNode2.refBit >> 5) * 4;
            int num3 = 3;
            while (index < s.Length)
            {
              num2 |= (uint) s[index] << num3 * 8;
              ++index;
              --num3;
            }
          }
          string name = this.names[(int) ptreeNode2.idxEntry];
          name.PadRight(16, char.MinValue);
          if (name == s)
            return (int) ptreeNode2.idxEntry;
        }
      }
      return -1;
    }

    public void Add(string Name, T Data)
    {
      this.entry.data.Add(Data);
      this.names.Add(Name);
      ++this.numEntry;
      this.RegenerateTree();
    }

    public void Insert(int Index, string Name, T Data)
    {
      this.entry.data.Insert(Index, Data);
      this.names.Insert(Index, Name);
      ++this.numEntry;
      this.RegenerateTree();
    }

    public void RemoveAt(int Index)
    {
      this.names.RemoveAt(Index);
      this.entry.data.RemoveAt(Index);
      --this.numEntry;
      this.RegenerateTree();
    }

    private void RegenerateTree()
    {
      this.node = Dictionary<T>.PatriciaTreeGenerator.Generate(this.names.ToArray());
    }

    public KeyValuePair<string, T> this[int i]
    {
      get
      {
        return new KeyValuePair<string, T>(this.names[i], this.entry.data[i]);
      }
      set
      {
        this.names[i] = value.Key;
        this.entry.data[i] = value.Value;
      }
    }

    public T this[string i]
    {
      get
      {
        return this.entry.data[this.IndexOf(i)];
      }
      set
      {
        this.entry.data[this.IndexOf(i)] = value;
      }
    }

    private class PatriciaTreeGenerator
    {
      private List<Dictionary<T>.PatriciaTreeGenerator.PatTreeNode> Nodes = new List<Dictionary<T>.PatriciaTreeGenerator.PatTreeNode>();

      public PatriciaTreeGenerator()
      {
        this.AddRootPatTreeNode();
      }

      private Dictionary<T>.PatriciaTreeGenerator.PatTreeNode AddRootPatTreeNode()
      {
        Dictionary<T>.PatriciaTreeGenerator.PatTreeNode patTreeNode = new Dictionary<T>.PatriciaTreeGenerator.PatTreeNode()
        {
          refbit = (int) sbyte.MaxValue
        };
        patTreeNode.left = patTreeNode;
        patTreeNode.right = patTreeNode;
        patTreeNode.idxEntry = 0;
        patTreeNode.name = "\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0";
        this.Nodes.Add(patTreeNode);
        return patTreeNode;
      }

      public Dictionary<T>.PatriciaTreeGenerator.PatTreeNode AddPatTreeNode(
        string Name,
        int Index)
      {
        foreach (Dictionary<T>.PatriciaTreeGenerator.PatTreeNode node in this.Nodes)
        {
          if (node.name == Name)
            return (Dictionary<T>.PatriciaTreeGenerator.PatTreeNode) null;
        }
        Dictionary<T>.PatriciaTreeGenerator.PatTreeNode patTreeNode1 = new Dictionary<T>.PatriciaTreeGenerator.PatTreeNode();
        Name = Name.PadRight(16, char.MinValue);
        int num1 = 0;
        Dictionary<T>.PatriciaTreeGenerator.PatTreeNode patTreeNode2 = (Dictionary<T>.PatriciaTreeGenerator.PatTreeNode) null;
        Dictionary<T>.PatriciaTreeGenerator.PatTreeNode patTreeNode3 = this.Nodes[0].left;
        if (this.Nodes[0].refbit > this.Nodes[0].left.refbit)
        {
          int refbit = this.Nodes[0].left.refbit;
          Dictionary<T>.PatriciaTreeGenerator.PatTreeNode patTreeNode4;
          do
          {
            patTreeNode4 = patTreeNode3;
            patTreeNode3 = (this.GetStringPart(Name, (refbit >> 5 & 3) * 4) >> refbit & 1) == 0 ? patTreeNode3.left : patTreeNode3.right;
            refbit = patTreeNode3.refbit;
          }
          while (patTreeNode4.refbit > refbit);
        }
        num1 = 0;
        patTreeNode2 = (Dictionary<T>.PatriciaTreeGenerator.PatTreeNode) null;
        int maxValue = (int) sbyte.MaxValue;
        if ((patTreeNode3.idxEntry ^ this.GetStringPart(Name, 12)) >= 0)
        {
          int num2;
          do
          {
            --maxValue;
            num2 = maxValue >> 5 & 3;
          }
          while (((this.GetStringPart(patTreeNode3.name, num2 * 4) ^ this.GetStringPart(Name, num2 * 4)) >> (maxValue & 31 & (int) byte.MaxValue) & 1) == 0);
        }
        num1 = 0;
        patTreeNode2 = (Dictionary<T>.PatriciaTreeGenerator.PatTreeNode) null;
        Dictionary<T>.PatriciaTreeGenerator.PatTreeNode patTreeNode5 = this.Nodes[0].left;
        Dictionary<T>.PatriciaTreeGenerator.PatTreeNode patTreeNode6 = this.Nodes[0];
        int refbit1 = this.Nodes[0].left.refbit;
        if (this.Nodes[0].refbit > this.Nodes[0].left.refbit)
        {
          while (refbit1 > maxValue)
          {
            patTreeNode6 = patTreeNode5;
            patTreeNode5 = (this.GetStringPart(Name, (refbit1 >> 5 & 3) * 4) >> refbit1 & 1) == 0 ? patTreeNode5.left : patTreeNode5.right;
            refbit1 = patTreeNode5.refbit;
            if (patTreeNode6.refbit <= refbit1)
              break;
          }
        }
        patTreeNode1.refbit = maxValue;
        patTreeNode1.left = (Dictionary<T>.PatriciaTreeGenerator.PatTreeNode) null;
        patTreeNode1.right = (Dictionary<T>.PatriciaTreeGenerator.PatTreeNode) null;
        patTreeNode1.idxEntry = Index;
        patTreeNode1.name = Name;
        patTreeNode1.left = (this.GetStringPart(Name, (maxValue >> 5 & 3) * 4) >> maxValue & 1) != 0 ? patTreeNode5 : patTreeNode1;
        patTreeNode1.right = (this.GetStringPart(Name, (maxValue >> 5 & 3) * 4) >> maxValue & 1) != 0 ? patTreeNode1 : patTreeNode5;
        if ((this.GetStringPart(Name, (patTreeNode6.refbit >> 5 & 3) * 4) >> patTreeNode6.refbit & 1) != 0)
        {
          patTreeNode6.right = patTreeNode1;
          this.Nodes.Add(patTreeNode1);
          return patTreeNode1;
        }
        patTreeNode6.left = patTreeNode1;
        this.Nodes.Add(patTreeNode1);
        return patTreeNode1;
      }

      private int GetStringPart(string s, int Offset)
      {
        int num = 0;
        for (int index = 0; index < 4; ++index)
          num |= (int) s[Offset + index] << index * 8;
        return num;
      }

      public void Sort()
      {
        SortedDictionary<string, Dictionary<T>.PatriciaTreeGenerator.PatTreeNode> sortedDictionary = new SortedDictionary<string, Dictionary<T>.PatriciaTreeGenerator.PatTreeNode>();
        foreach (Dictionary<T>.PatriciaTreeGenerator.PatTreeNode node in this.Nodes)
        {
          if (!(node.name.TrimEnd(new char[1]) == ""))
            sortedDictionary.Add(node.name.TrimEnd(new char[1]), node);
        }
        List<Dictionary<T>.PatriciaTreeGenerator.PatTreeNode> patTreeNodeList1 = new List<Dictionary<T>.PatriciaTreeGenerator.PatTreeNode>();
        foreach (Dictionary<T>.PatriciaTreeGenerator.PatTreeNode patTreeNode in sortedDictionary.Values)
          patTreeNodeList1.Add(patTreeNode);
        List<Dictionary<T>.PatriciaTreeGenerator.PatTreeNode> patTreeNodeList2 = new List<Dictionary<T>.PatriciaTreeGenerator.PatTreeNode>();
        for (int index1 = 0; index1 < this.Nodes.Count - 1; ++index1)
        {
          int index2 = -1;
          int num = -1;
          for (int index3 = 0; index3 < patTreeNodeList1.Count; ++index3)
          {
            if (patTreeNodeList1[index3].name.TrimEnd(new char[1]).Length > num)
            {
              index2 = index3;
              num = patTreeNodeList1[index3].name.TrimEnd(new char[1]).Length;
            }
          }
          patTreeNodeList2.Add(patTreeNodeList1[index2]);
          patTreeNodeList1.RemoveAt(index2);
        }
        patTreeNodeList2.Insert(0, this.Nodes[0]);
        this.Nodes = patTreeNodeList2;
      }

      public Dictionary<T>.PtreeNode[] GetRawNodes()
      {
        List<Dictionary<T>.PtreeNode> ptreeNodeList = new List<Dictionary<T>.PtreeNode>();
        foreach (Dictionary<T>.PatriciaTreeGenerator.PatTreeNode node in this.Nodes)
          ptreeNodeList.Add(new Dictionary<T>.PtreeNode()
          {
            refBit = (byte) node.refbit,
            idxLeft = (byte) this.Nodes.IndexOf(node.left),
            idxRight = (byte) this.Nodes.IndexOf(node.right),
            idxEntry = (byte) node.idxEntry
          });
        return ptreeNodeList.ToArray();
      }

      public static Dictionary<T>.PtreeNode[] Generate(string[] Names)
      {
        Dictionary<T>.PatriciaTreeGenerator patriciaTreeGenerator = new Dictionary<T>.PatriciaTreeGenerator();
        for (int Index = 0; Index < Names.Length; ++Index)
          patriciaTreeGenerator.AddPatTreeNode(Names[Index], Index);
        patriciaTreeGenerator.Sort();
        return patriciaTreeGenerator.GetRawNodes();
      }

      public class PatTreeNode
      {
        public int refbit;
        public Dictionary<T>.PatriciaTreeGenerator.PatTreeNode left;
        public Dictionary<T>.PatriciaTreeGenerator.PatTreeNode right;
        public int idxEntry;
        public string name;

        public override string ToString()
        {
          return this.name;
        }
      }
    }

    public class PtreeNode
    {
      public byte refBit;
      public byte idxLeft;
      public byte idxRight;
      public byte idxEntry;

      public void Write(EndianBinaryWriter er)
      {
        er.Write(this.refBit);
        er.Write(this.idxLeft);
        er.Write(this.idxRight);
        er.Write(this.idxEntry);
      }
    }

    public class DictEntry
    {
      public ushort sizeUnit;
      public ushort ofsName;
      public List<T> data;

      public DictEntry()
      {
        this.data = new List<T>();
      }

      public void Write(EndianBinaryWriter er)
      {
        int dataSize = (int) new T().GetDataSize();
        er.Write((ushort) dataSize);
        er.Write((ushort) (4 + dataSize * this.data.Count));
        foreach (T obj in this.data)
          obj.Write(er);
      }
    }
  }
}
