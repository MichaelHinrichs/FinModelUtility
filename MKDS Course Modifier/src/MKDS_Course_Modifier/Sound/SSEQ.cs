// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQ
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.G3D_Binary_File_Format;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound
{
  public class SSEQ
  {
    public const string Signature = "SSEQ";
    public FileHeader.HeaderInfo Header;
    public SSEQ.DataSection Data;

    public SSEQ(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader.HeaderInfo(er, nameof (SSEQ), out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 1");
      }
      else
      {
        this.Data = new SSEQ.DataSection(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 2");
        }
      }
      er.Close();
    }

    public SSEQ(byte[] Notes, int Offset)
    {
      this.Data = new SSEQ.DataSection(Notes, Offset);
    }

    public class DataSection
    {
      private CheckedListBox checkedListBox1 = new CheckedListBox();
      private TreeView treeView1 = new TreeView();
      private int loopstart = -1;
      private int loopend = -1;
      private short[] vars = new short[(int) byte.MaxValue];
      private int nrloop = -1;
      public const string Signature = "DATA";
      public DataBlockHeader Header;
      private byte[] Data;
      public uint SequenceOffset;
      public MidiEventCollection Midi;

      public DataSection(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "DATA", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.SequenceOffset = er.ReadUInt32();
          int count = (int) (er.BaseStream.Length - er.BaseStream.Position);
          this.Data = er.ReadBytes(count);
          SSEQDecoder sseqDecoder = new SSEQDecoder(this.Data, 0);
          IList<MidiEvent>[] tracks = sseqDecoder.GetTracks();
          this.loopend = sseqDecoder.LoopEnd;
          this.loopstart = sseqDecoder.LoopStart;
          MidiEventCollection midiEventCollection = new MidiEventCollection(1, 48);
          foreach (IList<MidiEvent> initialEvents in tracks)
            midiEventCollection.AddTrack(initialEvents);
          this.Midi = midiEventCollection;
          sseqDecoder.GetTreeNodes(this.treeView1.Nodes);
          OK = true;
        }
      }

      public DataSection(byte[] Notes, int Offset)
      {
        SSEQDecoder sseqDecoder = new SSEQDecoder(Notes, Offset);
        IList<MidiEvent>[] tracks = sseqDecoder.GetTracks();
        this.loopend = sseqDecoder.LoopEnd;
        this.loopstart = sseqDecoder.LoopStart;
        MidiEventCollection midiEventCollection = new MidiEventCollection(1, 48);
        foreach (IList<MidiEvent> initialEvents in tracks)
          midiEventCollection.AddTrack(initialEvents);
        this.Midi = midiEventCollection;
        sseqDecoder.GetTreeNodes(this.treeView1.Nodes);
      }

      public TreeNodeCollection GetTreeNodes()
      {
        return this.treeView1.Nodes;
      }

      public CheckedListBox.ObjectCollection GetCheckedListboxItems()
      {
        return this.checkedListBox1.Items;
      }

      public int GetLoopStart()
      {
        return this.loopstart;
      }

      public int GetLoopEnd()
      {
        return this.loopend;
      }

      public int GetNrLoop()
      {
        return this.nrloop;
      }

      private int smfReadVarLength(EndianBinaryReader er)
      {
        byte num1 = er.ReadByte();
        int num2 = (int) num1 & (int) sbyte.MaxValue;
        while (er.BaseStream.Position < er.BaseStream.Length && ((int) num1 & 128) != 0)
        {
          int num3 = num2 << 7;
          num1 = er.ReadByte();
          num2 = num3 | (int) num1 & (int) sbyte.MaxValue;
        }
        return num2;
      }

      private int smfGetVarLengthSize(int value)
      {
        int num = 1;
        for (int index = value; index > (int) sbyte.MaxValue && num < 4; index >>= 7)
          ++num;
        return num;
      }

      private MidiEventCollection ToMidi(EndianBinaryReader er)
      {
        MidiEventCollection midiEventCollection = new MidiEventCollection(1, 48);
        List<SSEQ.DataSection.SSEQEvent> sseqEventList = new List<SSEQ.DataSection.SSEQEvent>();
        List<List<SSEQ.DataSection.SSEQEvent>> sseqEventListList = new List<List<SSEQ.DataSection.SSEQEvent>>();
        List<int[]> numArrayList = new List<int[]>();
        int num1 = 0;
        int channel = 1;
        int num2 = -1;
        List<int> intList1 = new List<int>();
        int num3 = -1;
        int num4 = 0;
        bool flag1 = false;
        bool flag2 = false;
        int num5 = 0;
        bool flag3 = false;
        bool flag4 = false;
        int num6 = -1;
        List<int> intList2 = new List<int>();
        Dictionary<int, int> dictionary = new Dictionary<int, int>();
        midiEventCollection.AddTrack();
        int index1 = num2 + 1;
        int num7 = 0;
        this.checkedListBox1.Items.Add((object) ("Track " + (index1 + 1).ToString()), true);
        this.treeView1.Nodes.Insert(index1, "Track " + (index1 + 1).ToString());
        this.treeView1.Nodes[index1].SelectedImageIndex = 1;
        this.treeView1.Nodes[index1].ImageIndex = 1;
        bool flag5 = true;
        bool flag6 = false;
        int num8;
        if (er.ReadByte() == (byte) 254)
        {
          int num9 = (int) er.ReadInt16();
          er.BaseStream.Position -= 2L;
          sseqEventList.Add(new SSEQ.DataSection.SSEQEvent(254, er.ReadBytes(2)));
          this.treeView1.Nodes.Add("Multitrack");
          while (er.ReadByte() == (byte) 128)
          {
            int num10 = this.smfReadVarLength(er);
            this.treeView1.Nodes.Add("Rest (" + (object) num10 + ")");
            this.treeView1.Nodes[this.treeView1.Nodes.Count - 1].ImageIndex = 4;
            this.treeView1.Nodes[this.treeView1.Nodes.Count - 1].SelectedImageIndex = 4;
            er.BaseStream.Position -= (long) this.smfGetVarLengthSize(num10);
            sseqEventList.Add(new SSEQ.DataSection.SSEQEvent(128, er.ReadBytes(this.smfGetVarLengthSize(num10))));
          }
          --er.BaseStream.Position;
          while (er.ReadByte() == (byte) 147)
          {
            num8 = (int) er.ReadByte();
            byte[] numArray = er.ReadBytes(3);
            Array.Reverse((Array) numArray);
            int num10 = int.Parse(BitConverter.ToString(numArray).Replace("-", ""), NumberStyles.HexNumber);
            intList1.Add(num10);
            ++num1;
            er.BaseStream.Position -= 4L;
            sseqEventList.Add(new SSEQ.DataSection.SSEQEvent(147, er.ReadBytes(4)));
            this.treeView1.Nodes.Add("Open Track");
          }
          --er.BaseStream.Position;
        }
        --er.BaseStream.Position;
        byte num11 = 0;
        while (er.BaseStream.Position != er.BaseStream.Length)
        {
          int num9;
          if (intList1.Contains((int) er.BaseStream.Position - 28) && !flag2 && num11 != byte.MaxValue)
          {
            midiEventCollection[index1].Add((MidiEvent) new MetaEvent(MetaEventType.EndTrack, 0, (long) num5));
            SSEQ.DataSection.SSEQEvent[] array = new SSEQ.DataSection.SSEQEvent[sseqEventList.Count];
            sseqEventList.CopyTo(array);
            sseqEventListList.Add(((IEnumerable<SSEQ.DataSection.SSEQEvent>) array).ToList<SSEQ.DataSection.SSEQEvent>());
            sseqEventList.Clear();
            if (num1 != index1)
            {
              if (num1 != index1)
              {
                midiEventCollection.AddTrack();
                ++index1;
                ++channel;
                if (channel == 11)
                  channel = num6;
                num5 = 0;
                if (channel == 10)
                  ++channel;
                this.vars = new short[(int) byte.MaxValue];
                num7 = 0;
                CheckedListBox.ObjectCollection items = this.checkedListBox1.Items;
                num9 = index1 + 1;
                string str = "Track " + num9.ToString();
                items.Add((object) str, true);
                TreeNodeCollection nodes = this.treeView1.Nodes;
                int index2 = index1;
                num9 = index1 + 1;
                string text = "Track " + num9.ToString();
                nodes.Insert(index2, text);
                this.treeView1.Nodes[index1].SelectedImageIndex = 1;
                this.treeView1.Nodes[index1].ImageIndex = 1;
              }
            }
            else
              break;
          }
          if (num5 < num7)
            num5 = num7;
          intList2.Add(num7);
          if (index1 == 0 && !dictionary.ContainsKey((int) er.BaseStream.Position - 28))
            dictionary.Add((int) er.BaseStream.Position - 28, num7);
          byte num10 = er.ReadByte();
          if (num10 < (byte) 128)
          {
            int velocity = Convert.ToInt32(er.ReadByte());
            int num12 = this.smfReadVarLength(er);
            if (!flag2)
            {
              er.BaseStream.Position -= (long) (this.smfGetVarLengthSize(num12) + 1);
              sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(this.smfGetVarLengthSize(num12) + 1)));
              this.treeView1.Nodes[index1].Nodes.Add("Note (" + (object) num10 + ")");
              this.treeView1.Nodes[index1].Nodes[this.treeView1.Nodes[index1].Nodes.Count - 1].ImageIndex = 2;
              this.treeView1.Nodes[index1].Nodes[this.treeView1.Nodes[index1].Nodes.Count - 1].SelectedImageIndex = 2;
            }
            if (num3 == -1)
              ;
            if (velocity > (int) sbyte.MaxValue)
              velocity = (int) sbyte.MaxValue;
            midiEventCollection[index1].Add((MidiEvent) new NoteEvent((long) num7, channel, MidiCommandCode.NoteOn, (int) num10, velocity));
            if (num12 != 0)
              midiEventCollection[index1].Add((MidiEvent) new NoteEvent((long) (num7 + num12), channel, MidiCommandCode.NoteOff, (int) num10, 64));
            if (flag1)
              num7 += num12;
            if (num5 < num7 + num12)
              num5 = num7 + num12;
          }
          else
          {
            int controllerValue1;
            switch (num10)
            {
              case 128:
                int num12 = this.smfReadVarLength(er);
                num7 += num12;
                if (!flag2)
                {
                  this.treeView1.Nodes[index1].Nodes.Add("Rest (" + (object) num12 + ")");
                  this.treeView1.Nodes[index1].Nodes[this.treeView1.Nodes[index1].Nodes.Count - 1].ImageIndex = 4;
                  this.treeView1.Nodes[index1].Nodes[this.treeView1.Nodes[index1].Nodes.Count - 1].SelectedImageIndex = 4;
                  er.BaseStream.Position -= (long) this.smfGetVarLengthSize(num12);
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(this.smfGetVarLengthSize(num12))));
                  break;
                }
                break;
              case 129:
                int num13 = this.smfReadVarLength(er);
                int patchNumber = num13 % 128;
                int num14 = num13 / 128 & 15;
                int num15 = num13 / 128 / 128 & 15;
                midiEventCollection[index1].Insert(0, MidiEvent.FromRawMessage(MidiMessage.ChangeControl(0, num15, channel).RawData));
                midiEventCollection[index1].Insert(0, MidiEvent.FromRawMessage(MidiMessage.ChangeControl(32, num14, channel).RawData));
                midiEventCollection[index1].Insert(0, (MidiEvent) new PatchChangeEvent((long) num7, channel, patchNumber));
                if (patchNumber == (int) sbyte.MaxValue)
                {
                  flag3 = true;
                  flag4 = true;
                  num6 = channel;
                  channel = 10;
                  foreach (MidiEvent midiEvent in (IEnumerable<MidiEvent>) midiEventCollection[index1])
                    midiEvent.Channel = channel;
                }
                if (!flag2)
                {
                  this.treeView1.Nodes[index1].Nodes.Add("Bank Change (" + (object) patchNumber + ")");
                  er.BaseStream.Position -= (long) this.smfGetVarLengthSize(num13);
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(this.smfGetVarLengthSize(num13))));
                  break;
                }
                break;
              case 147:
                num8 = (int) er.ReadByte();
                byte[] numArray1 = er.ReadBytes(3);
                Array.Reverse((Array) numArray1);
                int num16 = int.Parse(BitConverter.ToString(numArray1).Replace("-", ""), NumberStyles.HexNumber);
                intList1.Add(num16);
                ++num1;
                if (!flag2)
                {
                  er.BaseStream.Position -= 4L;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(4)));
                  this.treeView1.Nodes.Add("Open Track");
                  break;
                }
                break;
              case 148:
                byte[] numArray2 = er.ReadBytes(3);
                Array.Reverse((Array) numArray2);
                int index2 = int.Parse(BitConverter.ToString(numArray2).Replace("-", ""), NumberStyles.HexNumber);
                if (!flag2)
                {
                  this.treeView1.Nodes[index1].Nodes.Add("Jump");
                  er.BaseStream.Position -= 3L;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(3)));
                }
                if (flag6 && flag5 || !flag6)
                {
                  try
                  {
                    if (this.loopend == -1)
                    {
                      this.loopend = num7;
                      this.loopstart = dictionary[index2];
                      midiEventCollection[index1].Add((MidiEvent) new TextEvent("loopEnd", MetaEventType.Marker, (long) num7));
                      midiEventCollection[index1].Add((MidiEvent) new TextEvent("loopStart", MetaEventType.Marker, (long) dictionary[index2]));
                    }
                  }
                  catch
                  {
                    this.loopend = -1;
                    this.loopstart = -1;
                  }
                  break;
                }
                break;
              case 149:
                byte[] numArray3 = er.ReadBytes(3);
                Array.Reverse((Array) numArray3);
                int num17 = int.Parse(BitConverter.ToString(numArray3).Replace("-", ""), NumberStyles.HexNumber);
                num4 = (int) er.BaseStream.Position;
                if (!flag2)
                {
                  this.treeView1.Nodes[index1].Nodes.Add("Call");
                  er.BaseStream.Position -= 3L;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(3)));
                  numArrayList.Add(new int[4]
                  {
                    index1,
                    this.treeView1.Nodes[index1].Nodes.Count,
                    sseqEventList.Count,
                    num17
                  });
                }
                er.BaseStream.Seek((long) (num17 + 28), SeekOrigin.Begin);
                flag2 = true;
                break;
              case 160:
                byte num18 = er.ReadByte();
                int num19 = (int) er.ReadInt16();
                int num20 = (int) er.ReadInt16();
                Random random = new Random();
                switch (num18)
                {
                  case 128:
                    num7 += random.Next(num19, num20);
                    break;
                  case 192:
                    int controllerValue2 = (int) (byte) random.Next(num19, num20);
                    midiEventCollection[index1].Add((MidiEvent) new ControlChangeEvent((long) num7, channel, MidiController.Pan, controllerValue2));
                    break;
                  case 193:
                    controllerValue1 = (int) (byte) random.Next(Math.Min(num19, num20), Math.Max(num19, num20));
                    midiEventCollection[index1].Add((MidiEvent) new ControlChangeEvent((long) num7, channel, MidiController.MainVolume, controllerValue1));
                    break;
                  case 196:
                    short num21 = (short) (((int) (short) random.Next(num19, num20) + 128) * 64);
                    if (num21 >= (short) 0 && num21 <= (short) 16384)
                    {
                      midiEventCollection[index1].Add((MidiEvent) new PitchWheelChangeEvent((long) num7, channel, (int) num21));
                      break;
                    }
                    break;
                }
                if (!flag2)
                {
                  er.BaseStream.Position -= 5L;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(5)));
                  this.treeView1.Nodes[index1].Nodes.Add("Random");
                  break;
                }
                break;
              case 161:
                byte num22 = er.ReadByte();
                if (num22 >= (byte) 176 && num22 <= (byte) 189)
                {
                  int num23 = (int) er.ReadByte();
                  int num24 = (int) er.ReadByte();
                  if (!flag2)
                  {
                    er.BaseStream.Position -= 3L;
                    sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(3)));
                    this.treeView1.Nodes[index1].Nodes.Add("Var");
                    break;
                  }
                  break;
                }
                int index3 = (int) er.ReadByte();
                switch (num22)
                {
                  case 128:
                    num7 += (int) this.vars[index3];
                    break;
                  case 192:
                    int var = (int) this.vars[index3];
                    midiEventCollection[index1].Add((MidiEvent) new ControlChangeEvent((long) num7, channel, MidiController.Pan, var));
                    break;
                  case 193:
                    controllerValue1 = (int) this.vars[index3];
                    midiEventCollection[index1].Add((MidiEvent) new ControlChangeEvent((long) num7, channel, MidiController.MainVolume, Math.Abs(controllerValue1)));
                    break;
                  case 196:
                    short num25 = (short) (((int) this.vars[index3] + 128) * 64);
                    if (num25 >= (short) 0 && num25 <= (short) 16384)
                    {
                      midiEventCollection[index1].Add((MidiEvent) new PitchWheelChangeEvent((long) num7, channel, (int) num25));
                      break;
                    }
                    break;
                }
                if (!flag2)
                {
                  er.BaseStream.Position -= 2L;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(2)));
                  this.treeView1.Nodes[index1].Nodes.Add("Var");
                }
                break;
              case 162:
                flag6 = true;
                if (!flag2)
                {
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, (byte[]) null));
                  this.treeView1.Nodes[index1].Nodes.Add("If");
                  break;
                }
                break;
              case 176:
              case 177:
              case 178:
              case 179:
              case 180:
              case 181:
              case 182:
              case 184:
              case 185:
              case 186:
              case 187:
              case 188:
              case 189:
                string[] strArray1 = new string[14]
                {
                  "Set Variable",
                  "Add Variable",
                  "Sub Variable",
                  "Mul Variable",
                  "Div Variable",
                  "Shift Vabiable",
                  "Rand Variable",
                  "",
                  "If Variable ==",
                  "If Variable >=",
                  "If Variable >",
                  "If Variable <=",
                  "If Variable <",
                  "If Variable !="
                };
                int index4 = (int) er.ReadByte();
                short num26 = er.ReadInt16();
                switch (num10)
                {
                  case 176:
                    this.vars[index4] = num26;
                    break;
                  case 177:
                    this.vars[index4] += num26;
                    break;
                  case 178:
                    this.vars[index4] -= num26;
                    break;
                  case 179:
                    this.vars[index4] *= num26;
                    break;
                  case 180:
                    this.vars[index4] /= num26;
                    break;
                  case 181:
                    this.vars[index4] >>= (int) num26;
                    break;
                  case 182:
                    this.vars[index4] = (short) new Random().Next((int) short.MaxValue);
                    break;
                  case 184:
                    flag5 = (int) this.vars[index4] == (int) num26;
                    break;
                  case 185:
                    flag5 = (int) this.vars[index4] >= (int) num26;
                    break;
                  case 186:
                    flag5 = (int) this.vars[index4] > (int) num26;
                    break;
                  case 187:
                    flag5 = (int) this.vars[index4] <= (int) num26;
                    break;
                  case 188:
                    flag5 = (int) this.vars[index4] < (int) num26;
                    break;
                  case 189:
                    flag5 = (int) this.vars[index4] != (int) num26;
                    break;
                }
                if (!flag2)
                {
                  this.treeView1.Nodes[index1].Nodes.Add(strArray1[(int) num10 - 176]);
                  er.BaseStream.Position -= 3L;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(3)));
                  break;
                }
                break;
              case 192:
                int controllerValue3 = (int) er.ReadByte();
                midiEventCollection[index1].Add((MidiEvent) new ControlChangeEvent((long) num7, channel, MidiController.Pan, controllerValue3));
                if (!flag2)
                {
                  TreeNodeCollection nodes = this.treeView1.Nodes[index1].Nodes;
                  num9 = controllerValue3 - 64;
                  string text = "Pan (" + num9.ToString() + ")";
                  nodes.Add(text);
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  break;
                }
                break;
              case 193:
                controllerValue1 = (int) er.ReadByte();
                midiEventCollection[index1].Add((MidiEvent) new ControlChangeEvent((long) num7, channel, MidiController.MainVolume, controllerValue1));
                if (!flag2)
                {
                  this.treeView1.Nodes[index1].Nodes.Add("Volume (" + controllerValue1.ToString() + ")");
                  this.treeView1.Nodes[index1].Nodes[this.treeView1.Nodes[index1].Nodes.Count - 1].SelectedImageIndex = 5;
                  this.treeView1.Nodes[index1].Nodes[this.treeView1.Nodes[index1].Nodes.Count - 1].ImageIndex = 5;
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  break;
                }
                break;
              case 194:
                controllerValue1 = (int) er.ReadByte();
                midiEventCollection[index1].Add((MidiEvent) new SequencerSpecificEvent(new byte[8]
                {
                  (byte) 240,
                  (byte) 127,
                  (byte) 127,
                  (byte) 4,
                  (byte) 1,
                  (byte) 0,
                  (byte) controllerValue1,
                  (byte) 247
                }, (long) num7));
                if (!flag2)
                {
                  this.treeView1.Nodes[index1].Nodes.Add("Master Volume (" + controllerValue1.ToString() + ")");
                  this.treeView1.Nodes[index1].Nodes[this.treeView1.Nodes[index1].Nodes.Count - 1].SelectedImageIndex = 5;
                  this.treeView1.Nodes[index1].Nodes[this.treeView1.Nodes[index1].Nodes.Count - 1].ImageIndex = 5;
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  break;
                }
                break;
              case 195:
                midiEventCollection[index1].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(101, 0, channel).RawData));
                midiEventCollection[index1].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(100, 2, channel).RawData));
                midiEventCollection[index1].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(6, (int) er.ReadByte() + 64, channel).RawData));
                if (!flag2)
                {
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  this.treeView1.Nodes[index1].Nodes.Add("Transpose");
                  break;
                }
                break;
              case 196:
                short num27 = (short) (((int) (short) er.ReadSByte() + 128) * 64);
                if (num27 >= (short) 0 && num27 <= (short) 16384)
                  midiEventCollection[index1].Add((MidiEvent) new PitchWheelChangeEvent((long) num7, channel, (int) num27));
                if (!flag2)
                {
                  this.treeView1.Nodes[index1].Nodes.Add("Pitch Change");
                  this.treeView1.Nodes[index1].Nodes[this.treeView1.Nodes[index1].Nodes.Count - 1].SelectedImageIndex = 6;
                  this.treeView1.Nodes[index1].Nodes[this.treeView1.Nodes[index1].Nodes.Count - 1].ImageIndex = 6;
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  break;
                }
                break;
              case 197:
                int num28 = (int) er.ReadByte();
                midiEventCollection[index1].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(101, 0, channel).RawData));
                midiEventCollection[index1].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(100, 0, channel).RawData));
                midiEventCollection[index1].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(6, num28, channel).RawData));
                if (!flag2)
                {
                  this.treeView1.Nodes[index1].Nodes.Add("Pitch Range");
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  break;
                }
                break;
              case 198:
                int num29 = (int) er.ReadByte();
                if (!flag2)
                {
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  this.treeView1.Nodes[index1].Nodes.Add("Priority");
                  break;
                }
                break;
              case 199:
                flag1 = Convert.ToBoolean((int) er.ReadByte());
                --er.BaseStream.Position;
                sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                break;
              case 200:
                int num30 = (int) er.ReadByte();
                if (!flag2)
                {
                  this.treeView1.Nodes[index1].Nodes.Add("Tie");
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  break;
                }
                break;
              case 201:
                int num31 = (int) er.ReadByte();
                midiEventCollection[0].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(84, num31, channel).RawData));
                if (!flag2)
                {
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  this.treeView1.Nodes[index1].Nodes.Add("Portamento Control");
                  break;
                }
                break;
              case 202:
                midiEventCollection[index1].Add((MidiEvent) new ControlChangeEvent((long) num7, channel, MidiController.Modulation, (int) er.ReadByte()));
                if (!flag2)
                {
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  this.treeView1.Nodes[index1].Nodes.Add("Modulation Depth");
                  break;
                }
                break;
              case 203:
                int num32 = (int) er.ReadByte();
                midiEventCollection[index1].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(76, 64 + num32 / 2, channel).RawData));
                if (!flag2)
                {
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  this.treeView1.Nodes[index1].Nodes.Add("Modulation Speed");
                  break;
                }
                break;
              case 204:
                string[] strArray2 = new string[3]
                {
                  "Pitch",
                  "Volume",
                  "Pan"
                };
                int num33 = (int) er.ReadByte();
                if (!flag2)
                {
                  this.treeView1.Nodes[index1].Nodes.Add("Modulation Type: " + strArray2[num33 & 3]);
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  break;
                }
                break;
              case 205:
                midiEventCollection[index1].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(77, 64 + (int) er.ReadByte() / 2, channel).RawData));
                if (!flag2)
                {
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  this.treeView1.Nodes[index1].Nodes.Add("Modulation Range");
                  break;
                }
                break;
              case 206:
                int num34 = (int) er.ReadByte();
                midiEventCollection[0].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(65, num34, channel).RawData));
                if (!flag2)
                {
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  this.treeView1.Nodes[index1].Nodes.Add("Portamento");
                  break;
                }
                break;
              case 207:
                int num35 = (int) er.ReadByte();
                midiEventCollection[0].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(5, num35, channel).RawData));
                if (!flag2)
                {
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  this.treeView1.Nodes[index1].Nodes.Add("Portamento Time");
                  break;
                }
                break;
              case 208:
                midiEventCollection[index1].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(73, 64 + (int) er.ReadByte() / 2, channel).RawData));
                if (!flag2)
                {
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  this.treeView1.Nodes[index1].Nodes.Add("Attack Rate");
                  break;
                }
                break;
              case 209:
                midiEventCollection[index1].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(75, 64 + (int) er.ReadByte() / 2, channel).RawData));
                if (!flag2)
                {
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  this.treeView1.Nodes[index1].Nodes.Add("Decay Rate");
                  break;
                }
                break;
              case 210:
                int num36 = (int) er.ReadByte();
                if (!flag2)
                {
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  this.treeView1.Nodes[index1].Nodes.Add("Sustain Rate");
                  break;
                }
                break;
              case 211:
                midiEventCollection[index1].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(72, 64 + (int) er.ReadByte() / 2, channel).RawData));
                if (!flag2)
                {
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  this.treeView1.Nodes[index1].Nodes.Add("Release Rate");
                  break;
                }
                break;
              case 212:
                this.nrloop = (int) er.ReadByte();
                midiEventCollection[index1].Add((MidiEvent) new TextEvent("loopStart", MetaEventType.Marker, (long) num7));
                if (!flag2)
                {
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  this.treeView1.Nodes[index1].Nodes.Add("Loop Start Marker");
                  break;
                }
                break;
              case 213:
                midiEventCollection[index1].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(11, (int) er.ReadByte(), channel).RawData));
                if (!flag2)
                {
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  this.treeView1.Nodes[index1].Nodes.Add("Expression");
                  break;
                }
                break;
              case 214:
                int num37 = (int) er.ReadByte();
                if (!flag2)
                {
                  --er.BaseStream.Position;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(1)));
                  this.treeView1.Nodes[index1].Nodes.Add("Print Variable");
                  break;
                }
                break;
              case 224:
                midiEventCollection[index1].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(78, 64 + (int) er.ReadInt16() / 2, channel).RawData));
                if (!flag2)
                {
                  this.treeView1.Nodes[index1].Nodes.Add("Modulation Delay");
                  er.BaseStream.Position -= 2L;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(2)));
                  break;
                }
                break;
              case 225:
                byte[] numArray4 = er.ReadBytes(2);
                Array.Reverse((Array) numArray4);
                double num38 = (double) int.Parse(BitConverter.ToString(numArray4).Replace("-", ""), NumberStyles.HexNumber);
                midiEventCollection[index1].Add((MidiEvent) new TempoEvent((int) (60000000.0 / num38), (long) num7));
                if (!flag2)
                {
                  this.treeView1.Nodes[index1].Nodes.Add("Tempo (" + (object) num38 + " bpm)");
                  this.treeView1.Nodes[index1].Nodes[this.treeView1.Nodes[index1].Nodes.Count - 1].ImageIndex = 3;
                  this.treeView1.Nodes[index1].Nodes[this.treeView1.Nodes[index1].Nodes.Count - 1].SelectedImageIndex = 3;
                  er.BaseStream.Position -= 2L;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(2)));
                  break;
                }
                break;
              case 227:
                midiEventCollection[index1].Add(MidiEvent.FromRawMessage(MidiMessage.ChangeControl(78, 64 + (int) er.ReadInt16() / 2, channel).RawData));
                if (!flag2)
                {
                  this.treeView1.Nodes[index1].Nodes.Add("Sweep Pitch");
                  er.BaseStream.Position -= 2L;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(2)));
                  break;
                }
                break;
              case 252:
                midiEventCollection[index1].Add((MidiEvent) new TextEvent("loopEnd", MetaEventType.Marker, (long) num7));
                if (!flag2)
                {
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, (byte[]) null));
                  this.treeView1.Nodes[index1].Nodes.Add("Loop End Marker");
                  break;
                }
                break;
              case 253:
                if (!flag2)
                {
                  this.treeView1.Nodes[index1].Nodes.Add("Return");
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, (byte[]) null));
                }
                if (num4 != -1)
                {
                  er.BaseStream.Seek((long) num4, SeekOrigin.Begin);
                  flag2 = false;
                  num4 = -1;
                  break;
                }
                break;
              case 254:
                int num39 = (int) er.ReadInt16();
                if (!flag2)
                {
                  er.BaseStream.Position -= 2L;
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, er.ReadBytes(2)));
                  this.treeView1.Nodes.Add("Multitrack");
                  break;
                }
                break;
              case byte.MaxValue:
                midiEventCollection[index1].Add((MidiEvent) new MetaEvent(MetaEventType.EndTrack, 0, (long) num5));
                this.treeView1.Nodes[index1].Nodes.Add("End of Track");
                sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, (byte[]) null));
                SSEQ.DataSection.SSEQEvent[] array = new SSEQ.DataSection.SSEQEvent[sseqEventList.Count];
                sseqEventList.CopyTo(array);
                sseqEventListList.Add(((IEnumerable<SSEQ.DataSection.SSEQEvent>) array).ToList<SSEQ.DataSection.SSEQEvent>());
                sseqEventList.Clear();
                if (num1 != index1)
                {
                  if (num1 != index1)
                  {
                    er.BaseStream.Position = (long) (intList1[index1] + 28);
                    midiEventCollection.AddTrack();
                    ++index1;
                    ++channel;
                    if (channel == 11)
                      channel = num6;
                    num5 = 0;
                    if (channel == 10)
                      ++channel;
                    this.vars = new short[(int) byte.MaxValue];
                    num7 = 0;
                    CheckedListBox.ObjectCollection items = this.checkedListBox1.Items;
                    num9 = index1 + 1;
                    string str = "Track " + num9.ToString();
                    items.Add((object) str, true);
                    TreeNodeCollection nodes = this.treeView1.Nodes;
                    int index5 = index1;
                    num9 = index1 + 1;
                    string text = "Track " + num9.ToString();
                    nodes.Insert(index5, text);
                    this.treeView1.Nodes[index1].SelectedImageIndex = 1;
                    this.treeView1.Nodes[index1].ImageIndex = 1;
                    break;
                  }
                  break;
                }
                goto label_172;
              default:
                int num40 = (int) er.ReadByte();
                if (!flag2)
                {
                  --er.BaseStream.Position;
                  byte[] Params = er.ReadBytes(1);
                  sseqEventList.Add(new SSEQ.DataSection.SSEQEvent((int) num10, Params));
                  this.treeView1.Nodes[index1].Nodes.Add("Undefined");
                  int num23 = (int) MessageBox.Show("Undefined\nID: " + BitConverter.ToString(new byte[1]
                  {
                    num10
                  }).Replace("-", ""));
                  break;
                }
                break;
            }
            num11 = num10;
            if (flag6 && num11 != (byte) 162)
              flag6 = false;
          }
        }
label_172:
        return midiEventCollection;
      }

      public class SSEQEvent
      {
        public int Type;
        public byte[] Params;

        public SSEQEvent(int Type, byte[] Params)
        {
          this.Type = Type;
          this.Params = Params;
        }
      }
    }
  }
}
