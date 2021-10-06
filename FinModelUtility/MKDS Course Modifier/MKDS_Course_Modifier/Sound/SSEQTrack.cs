// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQTrack
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Sound.SSEQEvents;
using System.Collections.Generic;
using System.IO;

namespace MKDS_Course_Modifier.Sound
{
  public class SSEQTrack
  {
    private List<MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent> Events = new List<MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent>();
    private Dictionary<long, int> EventOffsets = new Dictionary<long, int>();

    public SSEQTrack(EndianBinaryReader er, int TrackID)
    {
      this.TrackID = TrackID;
      byte EventID;
      while (true)
      {
        this.EventOffsets.Add(er.BaseStream.Position, this.Events.Count);
        EventID = er.ReadByte();
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
            case 148:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQJumpEvent(EventID, er));
              break;
            case 149:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQCallEvent(EventID, er));
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
            case 252:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQLoopEndMarkerEvent(EventID, er));
              break;
            case 253:
              this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQReturnEvent(EventID, er));
              break;
            case byte.MaxValue:
              goto label_34;
          }
        }
      }
label_34:
      this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQEndOfTrackEvent(EventID, er));
    }

    public int TrackID { get; private set; }
  }
}
