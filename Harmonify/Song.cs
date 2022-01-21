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
                    if (sections[i].measures[j].chords.Count > 0)
                    {
                        for (int k = 0; k < sections[i].measures[j].chords.Count; k++)
                        {
                            if (sections[i].measures[j].chords[k].chordNotes.Length > 0)
                            {
                                result += sections[i].measures[j].chords[k].GetChordNotation();
                            }
                            else
                            {
                                result += "-";
                            }
                        }
                    }
                    else
                    {
                        result += "-";
                    }
                    result += "|";
                }
            }
            return result;
        }

        public List<KeySignature> AssumeKeys()
        {
            return KeySignature.AssumeKeys(measures, notes);
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
                    List<KeySignature> candidates = AssumeKeys();

                    if (candidates.Count < 1)
                    {
                        MessageBox.Show("적합한 키를 찾지 못했습니다.");
                        return;
                    }
                    else
                    {
                        KeySignature = candidates[0];
                    }
                }
                Chordify(spice);
                // Analyze Song forms
                // incomplete bar check
            }

        }

        private void ClearChords()
        {
            for (int i = 0; i < sections.Count; i++)
            {
                for (int j = 0; j < sections[i].measures.Count; j++)
                {
                    for (int k = 0; k < sections[i].measures[j].chords.Count; k++)
                    {
                        sections[i].measures[j].chords.Clear();
                    }
                }
            }
        }

        private Section GetFirstNonEmptySection()
        {
            for (int i = 0; i < sections.Count; i++)
            {
                if (sections[i].measures.Count > 0 && sections[i].measures[0].notes.Count > 0)
                {
                    return sections[i];
                }
            }
            return null;
        }

        private void Chordify(int spice)
        {
            int[] keyNotes = KeySignature.GetKeyNotes(KeySignature);
            List<EChordFunction> diatonics = new List<EChordFunction>();
            foreach (EChordFunction eChordFunction in Enum.GetValues(typeof(EChordFunction)))
            {
                diatonics.Add(eChordFunction);
            }
            PreliminaryPass(keyNotes);
            for (int i = 0; i < sections.Count; i++)
            {
                if (sections[i].measures[0].notes.Count > 0)
                {
                    for (int j = 0; j < sections[i].measures.Count - 1; j++)
                    {
                        // 이미 코드가 있으면 넘김.
                        if (sections[i].measures[j].chords.Count > 0)
                        {
                            continue;
                        }
                        // 아니면
                        else
                        {
                            sections[i].measures[j].chords.Add(GetMatchingChord(diatonics, sections[i].measures[j].noteWeights, sections[i], j, spice));

                        }
                    }
                }
            }
        }

        // 첫 패스 전에 해야 할 것 : 첫코드, 마지막 코드 작성.
        private void PreliminaryPass(int[] keyNotes)
        {
            int firstNonemptySectionIndex = sections.IndexOf(GetFirstNonEmptySection());
            for (int i = 0; i < sections.Count; i++)
            {
                // 비지 않은 섹션에 대해서.
                if (sections[i].measures.Count > 0 && sections[i].measures[0].notes.Count > 0)
                {

                    int lastMeasureIndex = sections[i].measures.Count - 1;
                    // 첫코드 작성
                    if (i == firstNonemptySectionIndex)
                    {

                        // 1, 2, 4도만 처음에 올수 있음.
                        List<EChordFunction> oneTwoFour = new List<EChordFunction>();
                        oneTwoFour.Add(EChordFunction.Tonic);
                        oneTwoFour.Add(EChordFunction.SuperTonic);
                        oneTwoFour.Add(EChordFunction.SubDominant);
                        sections[i].measures[0].chords.Add(GetMatchingChord(oneTwoFour, sections[i].measures[0].noteWeights, sections[i], 0, spice));
                    }
                    // 마지막 코드 작성 : 전체 마지막 마디는 무조건 1도
                    Chord lastChord;
                    if (i == sections.Count - 1)
                    {
                        List<EChordFunction> one = new List<EChordFunction>();
                        one.Add(EChordFunction.Tonic);
                        lastChord = GetMatchingChord(one, sections[i].measures[lastMeasureIndex].noteWeights, sections[i], lastMeasureIndex, spice);
                        sections[i].measures[lastMeasureIndex].chords.Add(lastChord);
                    }
                    // 전체 마지막이 아니면
                    else
                    {
                        List<EChordFunction> oneFive = new List<EChordFunction>();
                        oneFive.Add(EChordFunction.Tonic);
                        oneFive.Add(EChordFunction.Dominant);
                        lastChord = GetMatchingChord(oneFive, sections[i].measures[lastMeasureIndex].noteWeights, sections[i], lastMeasureIndex, spice);
                        sections[i].measures[lastMeasureIndex].chords.Add(lastChord);
                    }
                    // 섹션에 마디가 둘 이상 있고, 마지막 코드가 1도면
                    if (lastMeasureIndex > 0 && lastChord.Root == keyNotes[0])
                    {
                        // 마지막에서 두번째 코드 : 4, 5도
                        List<EChordFunction> fourFive = new List<EChordFunction>();
                        fourFive.Add(EChordFunction.SubDominant);
                        fourFive.Add(EChordFunction.Dominant);
                        sections[i].measures[lastMeasureIndex - 1].chords.Add(GetMatchingChord(fourFive, sections[i].measures[lastMeasureIndex - 1].noteWeights, sections[i], lastMeasureIndex - 1, spice));
                    }
                    // NonDiatonic 노트 있으면 따져보기.
                    List<EChordFunction> diatonics = new List<EChordFunction>();
                    foreach (EChordFunction eChordFunction in Enum.GetValues(typeof(EChordFunction)))
                    {
                        diatonics.Add(eChordFunction);
                    }
                    for (int j = 0; j < sections[i].measures.Count; j++)
                    {
                        if (!KeySignature.IsDiatonic(sections[i].measures[j].notes))
                        {
                            //List<Tuple<int, int>> weightedNotes = sections[i].measures[j].GetWeightedNotes();

                            Chord matchestDiatonic = GetMatchingChord(diatonics, sections[i].measures[j].noteWeights, sections[i], j, spice);
                            Chord matchestSecondaryDominant = GetMatchingSecondaryDominant(sections[i].measures[j].noteWeights);

                            sections[i].measures[j].chords.Add(matchestDiatonic.match > matchestSecondaryDominant.match ? matchestDiatonic : matchestSecondaryDominant);
                        }
                    }

                }
            }
        }

        private float AvoidRepetition(Section section, int measureIndex, EChordFunction eChordFunction)
        {
            int currentIndex = measureIndex - 1;
            float coefficient = 1f;
            // (4마디 이후부턴 중복되어도 상관 없다.
            while (currentIndex > 0 && measureIndex - currentIndex < 4)
            {
                if (section.measures[currentIndex].chords.Count > 0 && section.measures[currentIndex].chords[0].EChordFunction == eChordFunction)
                {
                    coefficient -= 1f / (measureIndex - currentIndex);
                }
                currentIndex--;
            }
            currentIndex = measureIndex + 1;
            while (currentIndex < section.measures.Count - 1 && currentIndex - measureIndex < 4)
            {
                if (section.measures[currentIndex].chords.Count > 0 && section.measures[currentIndex].chords[0].EChordFunction == eChordFunction)
                {
                    coefficient -= 1f / (currentIndex - measureIndex);
                }
                currentIndex++;
            }
            if (coefficient < 0f)
            {
                coefficient = 0f;
            }
            return coefficient;
        }

        private float IncentivizePrimaryChords(KeySignature key, int chordRoot)
        {
            if (key.TonicNote == chordRoot || key.TonicNote + (int)Note.eNoteName.F == chordRoot || key.TonicNote + (int)Note.eNoteName.G == chordRoot)
            {
                return 1.05f;
            }
            else
            {
                return 1f;
            }
        }

        private Chord GetMatchingChord(List<EChordFunction> eChordFunctions, int[] noteWeights, Section section, int measureIndex, int spice)
        {
            Chord matchestChord = null;
            int currentMatch;
            for (int functionIndex = 0; functionIndex < eChordFunctions.Count; functionIndex++)
            {
                currentMatch = 0;
                List<Chord> chords = ChordFunction.GetAvailableChords(KeySignature, eChordFunctions[functionIndex], spice);
                for (int chordIndex = 0; chordIndex < chords.Count; chordIndex++)
                {
                    for (int k = 0; k < noteWeights.Length; k++)
                    {
                        if (chords[chordIndex].chordNotes != null && chords[chordIndex].chordNotes.Length > 1)
                        {
                            currentMatch += Chord.Match(KeySignature, k, chords[chordIndex].chordNotes) * noteWeights[k];
                        }
                    }
                    currentMatch = (int)(currentMatch * AvoidRepetition(section, measureIndex, eChordFunctions[functionIndex]));
                    //currentMatch = (int)(currentMatch * IncentivizePrimaryChords(KeySignature, eChordFunctions[functionIndex]));
                    // 현재 거가 더 잘맞으면
                    if (matchestChord == null || currentMatch >= matchestChord.match)
                    {
                        matchestChord = chords[chordIndex];
                        matchestChord.match = currentMatch;
                    }
                }
            }
            return matchestChord;
        }

        private Chord GetMatchingSecondaryDominant(int[] noteWeights)
        {
            List<KeySignature> nearKeys = KeySignature.GetNearKeys(1);
            Chord matchestChord = null;
            int currentMatch;
            int matchestMatch = int.MinValue;
            for (int i = 0; i < nearKeys.Count; i++)
            {
                List<Chord> dominantChords = ChordFunction.GetAvailableChords(nearKeys[i], EChordFunction.Dominant, spice);
                currentMatch = 0;
                for (int chordIndex = 0; chordIndex < dominantChords.Count; chordIndex++)
                {
                    for (int j = 0; j < noteWeights.Length; j++)
                    {
                        currentMatch += Chord.Match(nearKeys[i], j, dominantChords[chordIndex].chordNotes) * noteWeights[j];
                    }
                    // 나란한조의 경우 인센티브 줌.
                    currentMatch = (int)(currentMatch * (KeySignature.AreRelativeKeys(KeySignature, nearKeys[i]) ? 1.5f : 1f));
                    if (matchestChord == null || currentMatch >= matchestMatch)
                    {
                        matchestMatch = currentMatch;
                        matchestChord = dominantChords[chordIndex];
                        matchestChord.match = matchestMatch;
                    }
                }
            }
            return matchestChord;
        }

        private void MakeSection()
        {
            new SectionDivider().Divide(measures);
            InstantiateSections();
            PrintNotes();
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
                    for (int k = 0; k < sections[i].measures[j].notes.Count; k++)
                    {
                        noteString += Note.GetNoteName(sections[i].measures[j].notes[k].noteNumber);
                    }
                    noteString += "|";
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
