using System;
using System.Collections.Generic;
using System.Text;

namespace Harmonify
{
    public class Harmonifier
    {
        List<Section> sections;
        private KeySignature KeySignature;
        private int spice;
        public void Chordify(List<Section> _sections, KeySignature _keySignature, int _spice)
        {
            sections = _sections;
            KeySignature = _keySignature;
            spice = _spice;
            int[] keyNotes = KeySignature.GetKeyNotes(KeySignature);
            
            List<EChordFunction> diatonics = new List<EChordFunction>();
            foreach (EChordFunction eChordFunction in Enum.GetValues(typeof(EChordFunction)))
            {
                diatonics.Add(eChordFunction);
            }
            AddCandidateChords();
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

        private void AddCandidateChords()
        {
            for(int sectionIndex = 0; sectionIndex < sections.Count; sectionIndex++)
            {
                for(int measureIndex = 0;measureIndex < sections[sectionIndex].measures.Count; measureIndex++)
                {
                    Measure measure = sections[sectionIndex].measures[measureIndex];
                    measure.candidateChords.AddRange(GetDiatonicCandidateChords(measure.noteWeights));
                    // 논 다이어토닉 코드를 포함하고 있으면
                    if (!KeySignature.IsDiatonic(sections[sectionIndex].measures[measureIndex].notes))
                    {
                        measure.candidateChords.Add(GetMatchingSecondaryDominant(sections[sectionIndex].measures[measureIndex].noteWeights));
                    }
                    string msg = sectionIndex.ToString() + " " + measureIndex.ToString();
                    for (int chordIndex = 0;chordIndex < measure.candidateChords.Count; chordIndex++)
                    {
                        msg += measure.candidateChords[chordIndex].ToString() + " " + (measure.NoteWeightsSum > 0 ? MathF.Round((float)measure.candidateChords[chordIndex].match / measure.NoteWeightsSum, 1) : 0);

                    }
                    System.Windows.Forms.MessageBox.Show(msg);
                }
            }
        }

        private List<Chord> GetDiatonicCandidateChords(int[] noteWeights)
        {
            List<Chord> chords = new List<Chord>();
            foreach(EChordFunction eChordFunction in Enum.GetValues(typeof(EChordFunction)))
            {
                chords.AddRange(ChordFunction.GetAvailable7Chords(KeySignature, eChordFunction));
            }
            for (int chordIndex = 0; chordIndex < chords.Count; chordIndex++)
            {
                for (int k = 0; k < noteWeights.Length; k++)
                {
                    if (chords[chordIndex].chordNotes != null && chords[chordIndex].chordNotes.Length > 1)
                    {
                        chords[chordIndex].match += Chord.Match(KeySignature, k, chords[chordIndex].chordNotes) * noteWeights[k];
                    }
                }
            }
            return chords;
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
                List<Chord> dominantChords = ChordFunction.GetAvailable7Chords(nearKeys[i], EChordFunction.Dominant);
                currentMatch = 0;
                for (int chordIndex = 0; chordIndex < dominantChords.Count; chordIndex++)
                {
                    for (int j = 0; j < noteWeights.Length; j++)
                    {
                        if(noteWeights[j] > 0)
                        {
                            currentMatch += Chord.Match(nearKeys[i], j, dominantChords[chordIndex].chordNotes) * noteWeights[j];
                        }
                    }
                    dominantChords[chordIndex].match = currentMatch;
                    // 나란한조의 경우 인센티브 줌.
                    //currentMatch = (int)(currentMatch * (KeySignature.AreRelativeKeys(KeySignature, nearKeys[i]) ? 1.5f : 1f));
                    if (matchestChord == null || currentMatch >= matchestMatch)
                    {
                        matchestMatch = currentMatch;
                        matchestChord = dominantChords[chordIndex];
                    }
                }
            }
            return matchestChord;
        }

    }
}
