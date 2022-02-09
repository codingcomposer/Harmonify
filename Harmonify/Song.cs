using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace Harmonify
{
    public class Song
    {
        public static MidiParser.MidiFile MidiFile { get; set; }
        public static int Bpm { get; private set; }
        public static int TimeSigTop { get; private set; }
        public static int TimeSigBottom { get; private set; }
        public List<Measure> measures = new List<Measure>();
        private List<Note> notes = new List<Note>();
        private int targetTrackIndex = -1;
        private IuiHandler iuiHandler;
        private List<Section> sections = new List<Section>();

        public KeySignature KeySignature { get; private set; }
        private int spice;

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
                if (AllTimeSet())
                {
                    break;
                }
            }
            targetTrackIndex = GetTargetTrackIndex();
            if (targetTrackIndex != -1)
            {
                try
                {
                    MakeNotes();
                    QuantizeNotes();
                    MakeMeasures();
                    MakeSection();
                }
                catch
                {
                    MessageBox.Show("오류 발생");
                }
            }

        }

        public string PrintChords()
        {
            string result = null;
            for (int i = 0; i < sections.Count; i++)
            {
                result += "\r\n" + i.ToString() + ":";
                for (int j = 0; j < sections[i].measures.Count; j++)
                {
                    for(int slotIndex = 0;slotIndex < sections[i].measures[j].chordSlots.Count; slotIndex++)
                    {
                            for (int k = 0; k < sections[i].measures[j].chordSlots.Count; k++)
                            {
                                if (sections[i].measures[j].chordSlots[k].chord != null)
                                {
                                    result += sections[i].measures[j].chordSlots[k].chord.ToString();
                                }
                                else
                                {
                                    result += "-";
                                }
                            }
                    }
                    result += "|";
                }
            }
            return result;
        }

        public KeySignature AssumeKey()
        {
            return KeySignature.AssumeKey(measures, notes);
        }

        public void Analyze(KeySignature keySignature, int _spice)
        {
            spice = _spice;
            ClearChords();
            KeySignature = keySignature;
            if (targetTrackIndex != -1)
            {
                if (KeySignature == null)
                {
                    KeySignature candidate = AssumeKey();

                    if (candidate == null)
                    {
                        MessageBox.Show("적합한 키를 찾지 못했습니다.");
                        return;
                    }
                    else
                    {
                        KeySignature = candidate;
                    }
                }
                new Harmonifier().Chordify(sections, KeySignature, spice);
            }

        }

        private void ClearChords()
        {
            for (int i = 0; i < sections.Count; i++)
            {
                for (int j = 0; j < sections[i].measures.Count; j++)
                {
                    for (int k = 0; k < sections[i].measures[j].chordSlots.Count; k++)
                    {
                        sections[i].measures[j].chordSlots[k].chord = null;
                    }
                }
            }
        }
        private void MakeSection()
        {
            new SectionDivider().Divide(measures);
            InstantiateSections();
        }

        private void InstantiateSections()
        {
            int sectionNumber = -1;
            Section currentSection = null;
            for (int i = 0; i < measures.Count; i++)
            {
                if (sectionNumber != measures[i].section)
                {
                    sectionNumber = measures[i].section;
                    currentSection = new Section();
                    currentSection.sectionName = measures[i].section.ToString();
                    sections.Add(currentSection);
                }
                currentSection.measures.Add(measures[i]);
            }
        }

        private void PrintNotes()
        {
            string noteString = null;
            for (int i = 0; i < sections.Count; i++)
            {
                noteString += sections[i].sectionName + ":";
                for (int j = 0; j < sections[i].measures.Count; j++)
                {
                    noteString += sections[i].measures[j].NoteNames + "|";
                }
                noteString += "\r\n";
            }
            MessageBox.Show(noteString);
        }

        private void PrintSection()
        {
            int currentSection = -1;
            string str = null;
            for (int i = 0; i < measures.Count; i++)
            {
                if (currentSection != measures[i].section)
                {
                    str += "|";
                    currentSection = measures[i].section;
                }
                str += measures[i].index + " ";
            }
            MessageBox.Show(str);
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
                for (int j = 0; j < MidiFile.Tracks[i].MidiEvents.Count; j++)
                {
                    // 노트 온이 있으면
                    if (MidiFile.Tracks[i].MidiEvents[j].MidiEventType.Equals(MidiParser.MidiEventType.NoteOn))
                    {
                        trackTuples.Add(new Tuple<int, string>(i, FindTrackName(MidiFile.Tracks[i])));
                        break;
                    }
                }
            }
            if (trackTuples.Count == 1)
            {
                return trackTuples[0].Item1;
            }
            else if (trackTuples.Count == 0)
            {
                MessageBox.Show("적절한 트랙이 없음");
                return -1;
            }
            else
            {
                List<string> trackNames = new List<string>();
                for (int i = 0; i < trackTuples.Count; i++)
                {
                    trackNames.Add(trackTuples[i].Item2);
                }
                return iuiHandler.GetTrackIndex(trackNames);
            }
        }

        private string FindTrackName(MidiParser.MidiTrack midiTrack)
        {
            for (int i = 0; i < midiTrack.TextEvents.Count; i++)
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

        public void SetTrackIndex(int index)
        {
            targetTrackIndex = index;
            MessageBox.Show(targetTrackIndex + "번 트랙 선택됨");
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

        private void QuantizeNotes()
        {
            int sixteenthNoteTick = (int)(MidiFile.TicksPerQuarterNote * 0.25f);
            for (int i = 0; i < notes.Count; i++)
            {
                int differenceFromFront = notes[i].onTime % sixteenthNoteTick;
                // 노트 시작이 뒤에 붙었으면 뒤로 보냄.
                if (differenceFromFront > sixteenthNoteTick / 2)
                {
                    notes[i].onTime = notes[i].onTime + (sixteenthNoteTick - differenceFromFront);
                }
                // 아니면 앞으로 보냄.
                else
                {
                    notes[i].onTime = notes[i].onTime - differenceFromFront;
                }
                int offDifferenceFromFront = notes[i].offTime % sixteenthNoteTick;
                // 노트 끝이 뒤에 붙었으면 뒤로 보냄.
                if (offDifferenceFromFront > sixteenthNoteTick / 2)
                {
                    notes[i].offTime = notes[i].offTime + (sixteenthNoteTick - offDifferenceFromFront);
                }
                else
                {
                    notes[i].offTime = notes[i].offTime - offDifferenceFromFront;
                }
            }
        }

        private void MakeMeasures()
        {
            if (notes.Count < 1)
            {
                MessageBox.Show("There is no note in the track.");
                return;
            }
            // 마지막 노트의 끝나는 틱
            int lastOfftime = notes[^1].offTime;
            // 마디 개수 : 마지막 노트의 마디 인덱스 + 마지막 노트의 끝나는 틱이 마디의 끝이 아니면 +1 아니면(다음마디의 시작과 같으면) 0.
            int numberOfMeasures = GetMeasureIndex(lastOfftime) + (IsFirstBeat(lastOfftime) ? 0 : 1);
            for (int i = 0; i < numberOfMeasures; i++)
            {
                measures.Add(new Measure(i));
            }
            // 마디를 연결한다.
            for (int i = 0; i < numberOfMeasures; i++)
            {
                Measure prev = null, next = null;
                if (i > 0)
                {
                    prev = measures[i - 1];
                }
                if (i < numberOfMeasures - 1)
                {
                    next = measures[i + 1];
                }
                measures[i].Link(prev, next);
            }
            for (int i = 0; i < notes.Count; i++)
            {
                int onMeasureIndex = GetMeasureIndex(notes[i].onTime);
                int offMeasureIndex = GetMeasureIndex(notes[i].offTime);
                // 노트가 여러마디에 걸쳐있는 경우
                if (offMeasureIndex > onMeasureIndex && !IsFirstBeat(notes[i].offTime))
                {
                    // 그 노트가 걸치는 모든 마디에 해당 노트를 일단 추가해둔다. 
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
            // 다른 마디에 걸쳐있는 노트의 시작이나 끝 부분을 잘라낸다.
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
