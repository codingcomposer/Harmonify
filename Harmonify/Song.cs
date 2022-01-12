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
        public List<Note> notes = new List<Note>();
        private int targetTrackIndex = -1;
        private IuiHandler iuiHandler;
        public List<Section> sections = new List<Section>();

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
            }
            targetTrackIndex = GetTargetTrackIndex();
            if (targetTrackIndex != -1)
            {
                try
                {
                    MakeNotes();
                    MakeMeasures();
                    MakeSection();
                }
                catch
                {
                    MessageBox.Show("오류 발생");
                }
            }

        }

        public List<KeySignature> AssumeKeys()
        {
            Section firstNonEmptySection = GetFirstNonEmptySection();
            if (firstNonEmptySection != null)
            {
                return KeySignature.AssumeKeys(firstNonEmptySection);
            }
            else
            {
                return null;
            }
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
                    List<KeySignature> candidates = KeySignature.AssumeKeys(sections[sections.Count - 1]);

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
            for(int i = 0; i < sections.Count; i++)
            {
                for(int j = 0; j < sections[i].measures.Count; j++)
                {
                    for(int k = 0; k < sections[i].measures[j].chords.Count; k++)
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
            int chordNoteCount = spice > 0 ? 4 : 3;
            int[] keyNotes = KeySignature.GetKeyNotes(KeySignature);
            List<int> primaryTriads = new List<int>();
            primaryTriads.Add(keyNotes[0]);
            primaryTriads.Add(keyNotes[1]);
            primaryTriads.Add(keyNotes[2]);
            primaryTriads.Add(keyNotes[3]);
            primaryTriads.Add(keyNotes[4]);
            primaryTriads.Add(keyNotes[5]);
            PreliminaryPass(keyNotes, chordNoteCount);
            for (int i = 0; i < sections.Count; i++)
            {
                if (sections[i].measures[0].notes.Count > 0)
                {
                    for (int j = 1; j < sections[i].measures.Count - 1; j++)
                    {
                        // 이미 코드가 있으면 넘김.
                        if (sections[i].measures[j].chords.Count > 0)
                        {
                            continue;
                        }
                        // 아니면
                        else
                        {
                            List<int> candidateChords = new List<int>();
                            for (int k = 0; k < primaryTriads.Count; k++)
                            {
                                if (sections[i].measures[j - 1].chords[0].root != primaryTriads[k])
                                {
                                    candidateChords.Add(primaryTriads[k]);
                                }
                            }
                            sections[i].measures[j].chords.Add(GetMatchingChord(candidateChords, sections[i].measures[j].GetWeightedNotes(), sections[i], j, chordNoteCount));
                        }
                    }
                }
            }

        }

        // 첫 패스 전에 해야 할 것 : 첫코드, 마지막 코드 작성.
        private void PreliminaryPass(int[] keyNotes, int chordNoteCount)
        {
            for (int i = 0; i < sections.Count; i++)
            {
                // 비지 않은 섹션에 대해서.
                if (sections[i].measures.Count > 0 && sections[i].measures[0].notes.Count > 0)
                {
                    // 첫코드 작성

                    // 1, 4도만 처음에 올수 있음. 체크
                    List<int> oneFour = new List<int>();
                    oneFour.Add(keyNotes[0]);
                    oneFour.Add(keyNotes[3]);
                    sections[i].measures[0].chords.Add(GetMatchingChord(oneFour, sections[i].measures[0].GetWeightedNotes(), sections[i], 0, chordNoteCount));

                    // 마지막 코드 작성 : 무조건 1도
                    List<int> one = new List<int>();
                    one.Add(keyNotes[0]);
                    int lastMeasureIndex = sections[i].measures.Count - 1;
                    sections[i].measures[lastMeasureIndex].chords.Add(GetMatchingChord(one, sections[i].measures[lastMeasureIndex].GetWeightedNotes(), sections[i], lastMeasureIndex, chordNoteCount));

                    // 마지막에서 두번째 코드 : 4, 5도
                    List<int> fourFive = new List<int>();
                    fourFive.Add(keyNotes[3]);
                    fourFive.Add(keyNotes[4]);
                    sections[i].measures[lastMeasureIndex - 1].chords.Add(GetMatchingChord(fourFive, sections[i].measures[lastMeasureIndex - 1].GetWeightedNotes(), sections[i], lastMeasureIndex - 1, chordNoteCount));
                    
                    // secondary dominanant 필수 : D, E (한단계), A, B (두단계)
                    List<int> diatonics = new List<int>();
                    diatonics.Add(keyNotes[0]); diatonics.Add(keyNotes[1]); diatonics.Add(keyNotes[2]); diatonics.Add(keyNotes[3]); diatonics.Add(keyNotes[4]); diatonics.Add(keyNotes[5]);

                    for (int j = 0; j < sections[i].measures.Count; j++)
                    {
                        if (!KeySignature.IsDiatonic(sections[i].measures[j].notes))
                        {
                            List<Tuple<int, int>> weightedNotes = sections[i].measures[j].GetWeightedNotes();
                            Chord matchestDiatonic = GetMatchingChord(diatonics, weightedNotes, sections[i], j, chordNoteCount);
                            Chord matchestSecondaryDominant = GetMatchingSecondaryDominant(weightedNotes);
                            sections[i].measures[j].chords.Add(matchestSecondaryDominant);
                            /*
                            if(matchestDiatonic.match < matchestSecondaryDominant.match)
                            {
                                sections[i].measures[j].chords.Add(matchestSecondaryDominant);
                            }
                            */
                        }
                    }

                }
            }
        }

        private float AvoidRepetition(Section section, int measureIndex, int chordRoot)
        {
            int currentIndex = measureIndex - 1;
            float coefficient = 1f;
            // (4마디 이후부턴 중복되어도 상관 없다.
            while (currentIndex > 0 && measureIndex - currentIndex < 4)
            {
                if (section.measures[currentIndex].chords.Count > 0 && section.measures[currentIndex].chords[0].root == chordRoot)
                {
                    coefficient -= 1f / (measureIndex - currentIndex) * (measureIndex - currentIndex);
                }
                currentIndex--;
            }
            return coefficient;
        }

        private Chord GetMatchingChord(List<int> candidateChordRoots, List<Tuple<int, int>> weightedNotes, Section section, int measureIndex, int chordNoteCount)
        {
            int matchestRoot = 0;
            int matchestMatch = 0;
            int currentMatch;
            for (int j = 0; j < candidateChordRoots.Count; j++)
            {
                currentMatch = 0;
                for (int k = 0; k < weightedNotes.Count; k++)
                {
                    List<int> chordNotes = KeySignature.GetDiatonicChordNotesFromRoot(KeySignature, candidateChordRoots[j], chordNoteCount);
                    if (chordNotes != null && chordNotes.Count > 1)
                    {
                        currentMatch += Chord.Match(KeySignature, weightedNotes[k].Item1, chordNotes) * weightedNotes[k].Item2;
                    }
                }
                currentMatch = (int)(currentMatch * AvoidRepetition(section, measureIndex, candidateChordRoots[j]));
                // 현재 거가 더 잘맞으면
                if (currentMatch >= matchestMatch)
                {
                    matchestRoot = candidateChordRoots[j];
                    matchestMatch = currentMatch;
                }
            }
            return new Chord
            {
                root = matchestRoot,
                chordNotes = KeySignature.GetDiatonicChordNotesFromRoot(KeySignature, matchestRoot, chordNoteCount),
                major = true,
                match = matchestMatch
            };
        }

        private Chord GetMatchingSecondaryDominant(List<Tuple<int, int>> weightedNotes)
        {
            List<KeySignature> nearKeys = KeySignature.GetNearKeys(2);
            List<int> chordNotes = new List<int>();
            int currentMatch;
            int matchestMatch = 0;
            int matchestRoot = 0;
            for (int i = 0; i < nearKeys.Count; i++)
            {
                chordNotes.Clear();
                chordNotes.Add((nearKeys[i].TonicNote + 7) % 12); chordNotes.Add((nearKeys[i].TonicNote + 11) % 12); chordNotes.Add((nearKeys[i].TonicNote + 2) % 12);
                if(spice > 0)
                {
                    chordNotes.Add((nearKeys[i].TonicNote + 5) % 12);
                }
                MessageBox.Show(Chord.GetChordNotation(chordNotes));
                currentMatch = 0;
                for (int j = 0; j < weightedNotes.Count; j++)
                {
                    currentMatch += Chord.Match(nearKeys[i], weightedNotes[j].Item1, chordNotes) * weightedNotes[j].Item2;
                }
                if (currentMatch >= matchestMatch)
                {
                    matchestMatch = currentMatch;
                    matchestRoot = (nearKeys[i].TonicNote + 7) % 12;
                }
            }
            chordNotes.Clear();
            chordNotes.Add(matchestRoot); chordNotes.Add((matchestRoot + 4) % 12); chordNotes.Add((matchestRoot + 7) % 12);
            return new Chord()
            {
                chordNotes = chordNotes,
                major = true,
                root = matchestRoot,
                match = matchestMatch
            };
        }



        private void MakeSection()
        {
            int sectionIndex = 0;
            Section currentSection = new Section();
            currentSection.sectionName = "0";
            currentSection.measures.Add(measures[0]);
            sections.Add(currentSection);
            for (int i = 1; i < measures.Count; i++)
            {
                // 한쪽은 음표가 없는데 한쪽은 음표가 없다.
                if (NoteExistenceDifferent(measures[i], measures[i - 1]))
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

        private void MakeMeasures()
        {
            if (notes.Count < 1)
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
