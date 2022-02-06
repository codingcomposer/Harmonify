using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
            Iterate();
        }

        private void Iterate()
        {
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
                            int biggestPoint = int.MinValue;
                            Chord biggestMatch = null;
                            Measure currentMeasure = sections[i].measures[j];
                            foreach (Chord candidate in currentMeasure.candidateChords)
                            {
                                int prevPoint = 0;
                                // 이전에 코드가 있을 경우
                                if (j > 0 && sections[i].measures[j - 1].chords.Count > 0)
                                {
                                    Measure prevMeasure = sections[i].measures[j - 1];
                                    if (prevMeasure.chords[0] == null)
                                    {
                                        int here = 0;
                                    }
                                    // 이전으로부터의 코드 진행 점수를 구함.
                                    prevPoint = ChordFunction.GetProgressionPoint(prevMeasure.chords[prevMeasure.chords.Count - 1].EChordFunction, candidate.isSecondaryDominant ? (EChordFunction)(KeySignature.GetKeyNotes(KeySignature).ToList().IndexOf(candidate.Root))  : candidate.EChordFunction);
                                }
                                int nextPoint = 0;
                                if (j < sections[i].measures.Count - 1)
                                {
                                    Measure nextMeasure = sections[i].measures[j + 1];
                                    Chord nextImaginaryChord = null;
                                    // 이후에 코드가 있을 경우
                                    if (nextMeasure.chords.Count > 0)
                                    {
                                        // 이후로부터의 코드 진행 점수르 구함.
                                        nextPoint = ChordFunction.GetProgressionPoint(candidate.EChordFunction, nextMeasure.chords[0].EChordFunction);
                                        nextImaginaryChord = nextMeasure.chords[0];
                                    }
                                    // 이후에 코드가 없으 경우
                                    else
                                    {
                                        int imaginaryPoint = 0;
                                        int maxImaginaryPoint = int.MinValue;
                                        // 세컨더리 도미넌트일 경우
                                        if (candidate.isSecondaryDominant)
                                        {
                                            int resolveRoot = (candidate.Root + 5) % 12;
                                            foreach(Chord nextCandidate in nextMeasure.candidateChords)
                                            {
                                                if(nextCandidate.Root % 12== resolveRoot)
                                                {
                                                    nextPoint = 5;
                                                    imaginaryPoint = ChordFunction.GetProgressionPoint(EChordFunction.Dominant, EChordFunction.Tonic);
                                                    maxImaginaryPoint = imaginaryPoint;
                                                    nextImaginaryChord = nextCandidate;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // 이후의 코드후보로부터 가장 높은 포인트를 얻은 코드로 구함.
                                            foreach (Chord nextCandidate in nextMeasure.candidateChords)
                                            {
                                                imaginaryPoint = ChordFunction.GetProgressionPoint(candidate.EChordFunction, nextCandidate.EChordFunction);
                                                if (imaginaryPoint > maxImaginaryPoint)
                                                {
                                                    maxImaginaryPoint = imaginaryPoint;
                                                    nextImaginaryChord = nextCandidate;
                                                }
                                                else if(imaginaryPoint == maxImaginaryPoint)
                                                {
                                                    if(nextImaginaryChord.match < nextCandidate.match)
                                                    {
                                                        maxImaginaryPoint = imaginaryPoint;
                                                        nextImaginaryChord = nextCandidate;
                                                    }
                                                }
                                            }
                                        }
                                        nextPoint = maxImaginaryPoint;
                                    }
                                }
                                // 이전코드로부터의 진행점수와 다음 코드로의 진행점수를 합산.
                                int currentPoint = prevPoint + nextPoint;
                                if (currentPoint > biggestPoint || biggestMatch == null)
                                {
                                    biggestMatch = candidate;
                                    biggestPoint = currentPoint;
                                }
                                else if(currentPoint == biggestPoint)
                                {
                                    if(biggestMatch.match < candidate.match)
                                    {
                                        biggestMatch = candidate;
                                        biggestPoint = currentPoint;
                                    }
                                }

                            }
                            // 여기서 올림.
                            currentMeasure.chords.Add(biggestMatch);
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
                    if (sections[sectionIndex].measures[measureIndex].NoteExists())
                    {
                        Measure measure = sections[sectionIndex].measures[measureIndex];
                        measure.candidateChords.AddRange(GetDiatonicCandidateChords(measure.noteWeights));
                        // 논 다이어토닉 코드를 포함하고 있으면
                        if (!KeySignature.IsDiatonic(sections[sectionIndex].measures[measureIndex].notes))
                        {
                            // 세컨더리 도미넌트 코드도 추가.
                            measure.candidateChords.AddRange(GetMatchingSecondaryDominants(sections[sectionIndex].measures[measureIndex].noteWeights));
                        }
                        for(int chordIndex = 0;chordIndex < measure.candidateChords.Count; chordIndex++)
                        {
                            if(measure.candidateChords[chordIndex].match < 700)
                            {
                                measure.candidateChords.RemoveAt(chordIndex);
                                chordIndex--;
                            }
                        }
                    }
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
                    if (noteWeights[k] > 0 && chords[chordIndex].chordNotes != null && chords[chordIndex].chordNotes.Length > 1)
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
            float currentMatch;
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

        private List<Chord> GetMatchingSecondaryDominants(int[] noteWeights)
        {
            List<KeySignature> nearKeys = KeySignature.GetNearKeys(1);
            List<Chord> candidates = new List<Chord>();
            float currentMatch;
            for (int keyIndex = 0; keyIndex < nearKeys.Count; keyIndex++)
            {
                List<Chord> dominantChords = ChordFunction.GetAvailable7Chords(nearKeys[keyIndex], EChordFunction.Dominant);
                for (int chordIndex = 0; chordIndex < dominantChords.Count; chordIndex++)
                {
                    dominantChords[chordIndex].isSecondaryDominant = true;
                    currentMatch = 0;
                    int[] chordNotes = new int[dominantChords[chordIndex].chordNotes.Length];
                    Array.Copy(dominantChords[chordIndex].chordNotes, chordNotes, chordNotes.Length);
                    for (int j = 0; j < noteWeights.Length; j++)
                    {
                        if(noteWeights[j] > 0)
                        {
                            currentMatch += Chord.Match(nearKeys[keyIndex], j, chordNotes) * noteWeights[j];
                        }
                    }
                    dominantChords[chordIndex].match = currentMatch;
                    candidates.Add(dominantChords[chordIndex]);
                }
            }
            return candidates;
        }

    }
}
