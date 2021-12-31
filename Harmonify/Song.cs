using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Harmonify
{
    public class Song
    {
        public static MidiParser.MidiFile MidiFile { get; set; }
        public static int Bpm { get; private set; }
        public static int TimeSigTop { get; private set; }
        public static int TimeSigBottom { get; private set; }
        public List<Measure> measures = new List<Measure>();
        public List<Note> notes = new List<Note>();
        private enum eNoteName { C, Csharp, D, Dsharp, E, F, Fsharp, G, Gsharp, A, Asharp, B };
        private int keyAsMajor = 0;
        private int targetTrackIndex = 0;
        private IuiHandler iuiHandler;

        public Song(string path, IuiHandler _iuiHandler)
        {
            iuiHandler = _iuiHandler;
            MidiFile = new MidiParser.MidiFile(path);
            Bpm = 0;
            TimeSigTop = 0;
            TimeSigBottom = 0;
            for (int i = 0; i < MidiFile.TracksCount; i++)
            {
                for (int j = 0; j < MidiFile.Tracks[i].MidiEvents.Count; j++)
                {
                    if (MidiFile.Tracks[i].MidiEvents[j].MetaEventType == MidiParser.MetaEventType.Tempo)
                    {
                        Bpm = MidiFile.Tracks[i].MidiEvents[j].Note;
                        if (AllTimeSet())
                        {
                            break;
                        }
                    }
                    else if (MidiFile.Tracks[i].MidiEvents[j].MetaEventType == MidiParser.MetaEventType.TimeSignature)
                    {
                        TimeSigTop = MidiFile.Tracks[i].MidiEvents[j].Arg2;
                        TimeSigBottom = MidiFile.Tracks[i].MidiEvents[j].Arg3;

                        if (AllTimeSet())
                        {
                            break;
                        }
                    }
                }
            }
            Analyze();
        }

        private bool AllTimeSet()
        {
            if (Bpm != 0 && TimeSigTop != 0 && TimeSigBottom != 0)
            {
                return true;
            }
            return false;
        }

        public void Analyze()
        {
            GetTargetTrackIndex();
            //MakeNotes();
           // MakeMeasures(); 
            // Analyze Song forms
            // incomplete bar check

        }

        private void GetTargetTrackIndex()
        {
            string[] trackNames = new string[MidiFile.Tracks.Length];
            for(int i = 0; i < MidiFile.Tracks.Length; i++)
            {
                for(int j = 0;j < MidiFile.Tracks[i].TextEvents.Count; j++)
                {
                    if (MidiFile.Tracks[i].TextEvents[j].TextEventType.Equals(MidiParser.TextEventType.TrackName))
                    {
                        trackNames[i] = MidiFile.Tracks[i].TextEvents[j].Value + " | ";
                        break;
                    }
                }
            }
            iuiHandler.GetTrackIndex(trackNames);
        }

        public static string GetNoteName(int noteNumber)
        {
            return ((eNoteName)((noteNumber) % 12)).ToString();
        }

        public void SetTrackIndex(int index)
        {
            targetTrackIndex = index;
            MessageBox.Show(targetTrackIndex + "선택됨");
        }

        private void MakeNotes()
        {
            notes.Clear();
            if (MidiFile == null)
            {
                MessageBox.Show("미디파일이 없습니다.");
            }
            for (int i = 0; i < MidiFile.TracksCount; i++)
            {
                if (MidiFile.Tracks[i].MidiEvents.Count > 0)
                {
                    for (int j = 0; j < MidiFile.Tracks[i].MidiEvents.Count; j++)
                    {
                        // NoteOn일 경우
                        if (MidiFile.Tracks[i].MidiEvents[j].MidiEventType == MidiParser.MidiEventType.NoteOn)
                        {
                            for (int k = j; k < MidiFile.Tracks[i].MidiEvents.Count; k++)
                            {
                                if (MidiFile.Tracks[i].MidiEvents[k].MidiEventType == MidiParser.MidiEventType.NoteOff && MidiFile.Tracks[i].MidiEvents[k].Note == MidiFile.Tracks[i].MidiEvents[j].Note)
                                {
                                    notes.Add(new Note(MidiFile.Tracks[i].MidiEvents[j].Note, MidiFile.Tracks[i].MidiEvents[j].Time, MidiFile.Tracks[i].MidiEvents[k].Time));
                                    break;
                                }
                            }
                        }
                    }
                    //break;
                }
            }

        }

        private void MakeMeasures()
        {
            int lastOfftime = notes[notes.Count - 1].offTime;
            int numberOfMeasures = GetMeasureIndex(lastOfftime) + (IsFirstBeat(lastOfftime) ? 0 : 1);
            for(int i = 0; i < numberOfMeasures; i++)
            {
                measures.Add(new Measure(i));
            }
            for (int i = 0; i < notes.Count; i++)
            {
                int onMeasureIndex = GetMeasureIndex(notes[i].onTime);
                int offMeasureIndex = GetMeasureIndex(notes[i].offTime);
                // Tie로 인접 마디와 연결된 경우
                if (offMeasureIndex > onMeasureIndex && !IsFirstBeat(notes[i].offTime))
                {
                    for (int j = onMeasureIndex; j <= offMeasureIndex; j++)
                    {
                        measures[j].notes.Add(new Note(notes[i]));
                    }
                }
                // 아닌 경우
                else
                {
                    measures[onMeasureIndex].notes.Add(notes[i]);
                }
            }
            for(int i = 0; i < measures.Count; i++)
            {
                measures[i].TrimNotes();
            }
        }

        public static int GetMeasureIndex(int time)
        {
            return time / MidiFile.TicksPerQuarterNote / TimeSigTop;
        }

        public static bool IsFirstBeat(int time)
        {
            return time % (MidiFile.TicksPerQuarterNote * TimeSigTop) == 0;
        }


    }
}
