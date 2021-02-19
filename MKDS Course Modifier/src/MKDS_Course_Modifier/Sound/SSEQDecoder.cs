// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQDecoder
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Sound.SSEQEvents;
using NAudio.Midi;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound
{
  public class SSEQDecoder
  {
    public int LoopStart = -1;
    public int LoopEnd = -1;
    private List<MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent> Events;
    private Dictionary<long, int> EventOffsets;
    private int StartOffset;

    public SSEQDecoder(byte[] data, int StartOffset = 0)
    {
      this.StartOffset = StartOffset;
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(data), Endianness.LittleEndian);
      this.Events = new List<MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent>();
      this.EventOffsets = new Dictionary<long, int>();
      while (er.BaseStream.Position != er.BaseStream.Length)
      {
        this.EventOffsets.Add(er.BaseStream.Position, this.Events.Count);
        byte EventID = er.ReadByte();
        if (EventID < (byte) 128)
        {
          this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQNoteEvent(EventID, er));
        }
        else
        {
          switch (EventID)
          {
            case 128:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQDelayEvent(EventID, er));
              break;
            case 129:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQBankChangeEvent(EventID, er));
              break;
            case 147:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQTrackEvent(EventID, er));
              break;
            case 148:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQJumpEvent(EventID, er));
              break;
            case 149:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQCallEvent(EventID, er));
              break;
            case 160:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQRandomEvent(EventID, er));
              break;
            case 161:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQVarEvent(EventID, er));
              break;
            case 162:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQIfEvent(EventID, er));
              break;
            case 176:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQSetVarEvent(EventID, er));
              break;
            case 177:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQAddVarEvent(EventID, er));
              break;
            case 178:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQSubVarEvent(EventID, er));
              break;
            case 179:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQMulVarEvent(EventID, er));
              break;
            case 180:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQDivVarEvent(EventID, er));
              break;
            case 181:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQShiftVarEvent(EventID, er));
              break;
            case 182:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQRandVarEvent(EventID, er));
              break;
            case 183:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQCmpEqEvent(EventID, er));
              break;
            case 184:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQCmpGeEvent(EventID, er));
              break;
            case 185:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQCmpGtEvent(EventID, er));
              break;
            case 186:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQCmpLeEvent(EventID, er));
              break;
            case 187:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQCmpLtEvent(EventID, er));
              break;
            case 188:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQCmpNeEvent(EventID, er));
              break;
            case 189:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQRandVarEvent(EventID, er));
              break;
            case 192:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQPanEvent(EventID, er));
              break;
            case 193:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQVolumeEvent(EventID, er));
              break;
            case 194:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQMasterVolumeEvent(EventID, er));
              break;
            case 195:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQTransposeEvent(EventID, er));
              break;
            case 196:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQPitchEvent(EventID, er));
              break;
            case 197:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQPitchRangeEvent(EventID, er));
              break;
            case 198:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQTrackPriorityEvent(EventID, er));
              break;
            case 199:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQNoteWaitModeEvent(EventID, er));
              break;
            case 200:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQTieEvent(EventID, er));
              break;
            case 201:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQPortamentoEvent(EventID, er));
              break;
            case 202:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQModulationDepthEvent(EventID, er));
              break;
            case 203:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQModulationSpeedEvent(EventID, er));
              break;
            case 204:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQModulationTypeEvent(EventID, er));
              break;
            case 205:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQModulationRangeEvent(EventID, er));
              break;
            case 206:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQPortamentoOnOffEvent(EventID, er));
              break;
            case 207:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQPortamentoTimeEvent(EventID, er));
              break;
            case 208:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQAttackRateEvent(EventID, er));
              break;
            case 209:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQDecayRateEvent(EventID, er));
              break;
            case 210:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQSustainRateEvent(EventID, er));
              break;
            case 211:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQReleaseRateEvent(EventID, er));
              break;
            case 212:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQLoopStartMarkerEvent(EventID, er));
              break;
            case 213:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQExpressionEvent(EventID, er));
              break;
            case 214:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQPrintVariableEvent(EventID, er));
              break;
            case 224:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQModulationDelayEvent(EventID, er));
              break;
            case 225:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQTempoEvent(EventID, er));
              break;
            case 227:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQSweepPitchEvent(EventID, er));
              break;
            case 252:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQLoopEndMarkerEvent(EventID, er));
              break;
            case 253:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQReturnEvent(EventID, er));
              break;
            case 254:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQAllocTrackEvent(EventID, er));
              break;
            case byte.MaxValue:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQEndOfTrackEvent(EventID, er));
              break;
          }
        }
      }
      er.Close();
    }

    public IList<MidiEvent>[] GetTracks()
    {
      Dictionary<int, int> dictionary = new Dictionary<int, int>();
      SSEQMidiResult[] sseqMidiResultArray = new SSEQMidiResult[16];
      short[] GlobalVariables = new short[16];
      sseqMidiResultArray[0] = new SSEQMidiResult(0, GlobalVariables);
      int index = 0;
      int TrackID = 0;
      Stack<int> intStack1 = new Stack<int>();
      Stack<int> intStack2 = new Stack<int>();
      intStack1.Push(0);
      intStack2.Push(this.Events.Count);
      bool flag = false;
      int num = -1;
      for (int key = this.EventOffsets[(long) this.StartOffset]; key < this.Events.Count; ++key)
      {
        if (!dictionary.ContainsKey(key) && !flag)
          dictionary.Add(key, sseqMidiResultArray[index].CurrentTime);
        if (sseqMidiResultArray[index].If && sseqMidiResultArray[index].ComparisonResult || !sseqMidiResultArray[index].If)
        {
          sseqMidiResultArray[index].If = false;
          if (this.Events[key] is SSEQTrackEvent)
          {
            intStack2.Push(key);
            intStack1.Push(index);
            index = (int) ((SSEQTrackEvent) this.Events[key]).TrackNr;
            ++TrackID;
            if (TrackID == 9)
              ++TrackID;
            key = this.EventOffsets[(long) ((SSEQTrackEvent) this.Events[key]).Offset] - 1;
            sseqMidiResultArray[index] = new SSEQMidiResult(TrackID, GlobalVariables);
            sseqMidiResultArray[index].CurrentTime = sseqMidiResultArray[intStack1.Peek()].CurrentTime;
          }
          else
          {
            this.Events[key].AddMidiEvents(ref sseqMidiResultArray[index]);
            if (sseqMidiResultArray[index].Goto)
            {
              sseqMidiResultArray[index].ReturnOffset = key;
              key = this.EventOffsets[(long) sseqMidiResultArray[index].GotoOffset] - 1;
              sseqMidiResultArray[index].Goto = false;
              sseqMidiResultArray[index].GotoOffset = 0U;
              flag = true;
            }
            else
            {
              if (sseqMidiResultArray[index].Return)
              {
                key = sseqMidiResultArray[index].ReturnOffset;
                sseqMidiResultArray[index].Return = false;
                sseqMidiResultArray[index].ReturnOffset = -1;
                flag = false;
              }
              if (this.Events[key] is SSEQJumpEvent && (long) num != (long) ((SSEQJumpEvent) this.Events[key]).Offset)
              {
                num = (int) ((SSEQJumpEvent) this.Events[key]).Offset;
                key = this.EventOffsets[(long) ((SSEQJumpEvent) this.Events[key]).Offset] - 1;
                flag = true;
              }
              else
              {
                if (this.Events[key] is SSEQEndOfTrackEvent || this.Events[key] is SSEQJumpEvent && (long) num == (long) ((SSEQJumpEvent) this.Events[key]).Offset)
                {
                  GlobalVariables = sseqMidiResultArray[index].GlobalVariables;
                  sseqMidiResultArray[intStack1.Peek()].GlobalVariables = GlobalVariables;
                  if (sseqMidiResultArray[index].TrackID == 9 && TrackID != 10)
                    --TrackID;
                  key = intStack2.Pop();
                  index = intStack1.Pop();
                }
                if (key == this.Events.Count)
                  break;
              }
            }
          }
        }
        else if (sseqMidiResultArray[index].If && !sseqMidiResultArray[index].ComparisonResult)
          sseqMidiResultArray[index].If = false;
      }
      List<IList<MidiEvent>> midiEventListList = new List<IList<MidiEvent>>();
      foreach (SSEQMidiResult sseqMidiResult in sseqMidiResultArray)
      {
        if (sseqMidiResult != null)
          midiEventListList.Add(sseqMidiResult.MidiTrack);
      }
      return midiEventListList.ToArray();
    }

    public void GetTreeNodes(TreeNodeCollection Nodes)
    {
      TreeNode node1 = new TreeNode("Track 0", 4, 4);
      Nodes.Add(node1);
      Stack<int> intStack1 = new Stack<int>();
      Stack<int> intStack2 = new Stack<int>();
      intStack1.Push(0);
      intStack2.Push(this.Events.Count);
      int num1 = 0;
      bool flag = false;
      int num2 = -1;
      int num3 = -1;
      for (int index = this.EventOffsets[(long) this.StartOffset]; index < this.Events.Count; ++index)
      {
        if (this.Events[index] is SSEQTrackEvent)
        {
          intStack2.Push(index);
          intStack1.Push(num1);
          TreeNode node2 = new TreeNode("Track " + (object) ((SSEQTrackEvent) this.Events[index]).TrackNr, 4, 4);
          node2.Tag = !flag ? (object) index : (object) -1;
          node1.Nodes.Add(node2);
          node1 = node2;
          num1 = (int) ((SSEQTrackEvent) this.Events[index]).TrackNr;
          index = this.EventOffsets[(long) ((SSEQTrackEvent) this.Events[index]).Offset] - 1;
        }
        else
        {
          node1.Nodes.Add(this.Events[index].GetTreeNode());
          if (this.Events[index] is SSEQCallEvent)
          {
            num2 = index;
            index = this.EventOffsets[(long) ((SSEQCallEvent) this.Events[index]).Offset] - 1;
            node1 = node1.Nodes[node1.Nodes.Count - 1];
          }
          else if (this.Events[index] is SSEQReturnEvent)
          {
            index = num2;
            num2 = -1;
            node1 = node1.Parent;
          }
          else if (this.Events[index] is SSEQJumpEvent && (long) num3 != (long) ((SSEQJumpEvent) this.Events[index]).Offset)
          {
            num3 = (int) ((SSEQJumpEvent) this.Events[index]).Offset;
            index = this.EventOffsets[(long) ((SSEQJumpEvent) this.Events[index]).Offset] - 1;
            node1 = node1.Nodes[node1.Nodes.Count - 1];
            flag = true;
          }
          else
          {
            if (this.Events[index] is SSEQEndOfTrackEvent || this.Events[index] is SSEQJumpEvent && (long) num3 == (long) ((SSEQJumpEvent) this.Events[index]).Offset)
            {
              if (this.Events[index] is SSEQJumpEvent && (long) num3 == (long) ((SSEQJumpEvent) this.Events[index]).Offset || flag)
                node1 = node1.Parent;
              node1 = node1.Parent;
              num3 = -1;
              flag = false;
              index = intStack2.Pop();
              num1 = intStack1.Pop();
            }
            if (index == this.Events.Count)
              break;
          }
        }
      }
    }
  }
}
