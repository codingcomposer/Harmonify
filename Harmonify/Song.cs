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
        private int targetTrackIndex = 0;
        private IuiHandler iuiHandler;
        public List<Section> sections = new List<Section>();

        public Song(string path, IuiHandler _iuiHandler)
        {
            if (string.IsNullOrEmpty(path))
            {
                MidiFile = null;
                return;
            }
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
        }
        public void Analyze()
        {
            targetTrackIndex = GetTargetTrackIndex();
            if(targetTrackIndex != -1)
            {
                MakeNotes();
                MakeMeasures();
                MakeSection();
                Chordify();
                // Analyze Song forms
                // incomplete bar check
            }

        }

        private void AssumeKey()
        {
            
        }
        private void Chordify()
        {
            for(int i = 0; i < sections.Count; i++)
            {
                if (sections[i].measures[0].notes.Count > 0)
                {
                    for(int j = 0; j < sections[i].measures.Count; j++)
                    {
                        Chord chord = new Chord();
                        List<int> mostweightedNotes = GetMostWeightedNotes(sections[i].measures[j]);
                        int matchestRoot = 0;
                        int matchestMatch = 0;
                        int currentMatch = 0;
                        for(int k = 0; k < 12; k++)
                        {
                            currentMatch = 0;
                            for(int m = 0; m < mostweightedNotes.Count; m++)
                            {
                                List<int> chordNotes = KeySignature.GetDiatonicChordNotes(0, true, k);
                                if(chordNotes != null && chordNotes.Count > 1)
                                {
                                    currentMatch += Chord.Match(mostweightedNotes[m], chordNotes);
                                }
                            }
                            // 현재 거가 더 잘맞으면
                            if(currentMatch > matchestMatch)
                            {
                                matchestRoot = k;
                                matchestMatch = currentMatch;
                            }
                        }
                        chord.root = matchestRoot;
                        chord.major = true;
                        sections[i].measures[j].chords.Add(chord);
                    }
                }
            }
        }

        private List<int> GetMostWeightedNotes(Measure measure)
        {
            List<Tuple<int, int>> weightedNotes = new List<Tuple<int, int>>();
            int[] weights = new int[12];
            for(int i= 0; i < measure.notes.Count; i++)
            {
                weights[measure.notes[i].noteNumber % 12] += measure.notes[i].length;
            }
            for(int i = 0; i < weights.Length; i++)
            {
                if(weights[i] > 0)
                {
                    weightedNotes.Add(new Tuple<int, int>(i, weights[i]));
                }
            }
            weightedNotes.Sort((a, b) => b.Item2.CompareTo(a.Item2));
            if(weightedNotes.Count > 3)
            {
                while(weightedNotes.Count < 4)
                {
                    weightedNotes.RemoveAt(weightedNotes.Count - 1);
                }
            }
            List<int> frequentNotes = new List<int>();
            for(int i = 0; i < weightedNotes.Count; i++)
            {
                frequentNotes.Add(weightedNotes[i].Item1);
            }
            return frequentNotes;
        }

        private void MakeSection()
        {
            int sectionIndex = 0;
            Section currentSection = new Section();
            currentSection.sectionName = "0";
            currentSection.measures.Add(measures[0]);
            sections.Add(currentSection);
            for(int i = 1; i < measures.Count; i++)
            {
                // 한쪽은 음표가 없는데 한쪽은 음표가 없다.
                if(NoteExistenceDifferent(measures[i], measures[i - 1]))
                {
                    sectionIndex++;
                    currentSection = new Section();
                    currentSection.sectionName = sectionIndex.ToString();
                    sections.Add(currentSection);
                }
                currentSection.measures.Add(measures[i]);
            }
        }

        private bool NoteExistenceDifferent(Measure a, Measure b)
        {
            return (a.notes.Count > 0 && b.notes.Count == 0) || (a.notes.Count == 0 && b.notes.Count > 0);
        }

        private bool AllTimeSet()
        {
            if (Bpm != 0 && TimeSigTop != 0 && TimeSigBottom != 0)
            {
                return true;
            }
            return false;
        }


        private int GetTargetTrackIndex()
        {
            List<Tuple<int, string>> trackTuples = new List<Tuple<int, string>>();
            // 트랙 중
            for (int i = 0; i < MidiFile.Tracks.Length; i++)
            {
                // 그 트랙의 미디이벤트 중
                for(int j = 0; j < MidiFile.Tracks[i].MidiEvents.Count; j++)
                {
                    // 노트 온이 있으면
                    if (MidiFile.Tracks[i].MidiEvents[j].MidiEventType.Equals(MidiParser.MidiEventType.NoteOn))
                    {
                        trackTuples.Add(new Tuple<int, string>(i, FindTrackName(MidiFile.Tracks[i])));
                        break;
                    }
                }
            }
            if(trackTuples.Count == 1)
            {
                return trackTuples[0].Item1;
            }
            else if(trackTuples.Count == 0)
            {
                MessageBox.Show("적절한 트랙이 없음");
                return -1;
            }
            else
            {
                List<string> trackNames = new List<string>();
                for(int i = 0; i < trackTuples.Count; i++)
                {
                    trackNames.Add(trackTuples[i].Item2);
                }
                return iuiHandler.GetTrackIndex(trackNames);
            }
        }

        private string FindTrackName(MidiParser.MidiTrack midiTrack)
        {
            for(int i = 0; i < midiTrack.TextEvents.Count; i++)
            {
                if (midiTrack.TextEvents[i].TextEventType.Equals(MidiParser.TextEventType.TrackName))
                {
                    return midiTrack.TextEvents[i].Value;
                }
                else if (midiTrack.TextEvents[i].TextEventType.Equals(MidiParser.TextEventType.Text))
                {
                    return midiTrack.TextEvents[i].Value;
                }
            }
            return midiTrack.ToString();
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
            MidiParser.MidiTrack midiTrack = MidiFile.Tracks[targetTrackIndex];
            for (int j = 0; j < midiTrack.MidiEvents.Count; j++)
            {
                // NoteOn일 경우
                if (midiTrack.MidiEvents[j].MidiEventType == MidiParser.MidiEventType.NoteOn)
                {
                    for (int k = j; k < midiTrack.MidiEvents.Count; k++)
                    {
                        if (midiTrack.MidiEvents[k].MidiEventType == MidiParser.MidiEventType.NoteOff && midiTrack.MidiEvents[k].Note == midiTrack.MidiEvents[j].Note)
                        {
                            notes.Add(new Note(midiTrack.MidiEvents[j].Note, midiTrack.MidiEvents[j].Time, midiTrack.MidiEvents[k].Time));
                            break;
                        }
                    }
                }
            }

        }

        private void MakeMeasures()
        {
            if(notes.Count < 1)
            {
                MessageBox.Show("There is no note in the track.");
                return;
            }
            int lastOfftime = notes[^1].offTime;
            int numberOfMeasures = GetMeasureIndex(lastOfftime) + (IsFirstBeat(lastOfftime) ? 0 : 1);
            for (int i = 0; i < numberOfMeasures; i++)
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
            for (int i = 0; i < measures.Count; i++)
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
