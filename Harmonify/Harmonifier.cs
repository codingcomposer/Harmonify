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
            for (int sectionIndex = 0; sectionIndex < sections.Count; sectionIndex++)
            {
                if (sections[sectionIndex].measures[0].notes.Count > 0)
                {
                    for (int measureIndex = 0; measureIndex < sections[sectionIndex].measures.Count - 1; measureIndex++)
                    {
                        for(int slotIndex = 0; slotIndex < sections[sectionIndex].measures[measureIndex].chordSlots.Count; slotIndex++)
                        {
                            // 이미 코드가 있으면 넘김.
                            if (sections[sectionIndex].measures[measureIndex].chordSlots[slotIndex].chord != null)
                            {
                                continue;
                            }
                            // 아니면
                            else
                            {
                                int biggestPoint = int.MinValue;
                                Chord biggestMatch = null;
                                Measure currentMeasure = sections[sectionIndex].measures[measureIndex];
                                ChordSlot currentSlot = sections[sectionIndex].measures[measureIndex].chordSlots[slotIndex];
                                ChordSlot prevChordSlot = null;
                                ChordSlot nextChordSlot = null;
                                // 각 후보코드당
                                foreach (Chord candidate in currentSlot.candidateChords)
                                {
                                    // 과거로부터의 점수 계산
                                    int prevPoint = 0;
                                    // 현재 슬롯이 마디의 첫번째 슬롯이면 이전 슬롯은 이전 마디의 마지막 슬롯
                                    if(slotIndex == 0)
                                    {
                                        if(currentMeasure.PrevMeasure != null)
                                        {
                                            prevChordSlot = currentMeasure.PrevMeasure.chordSlots[^1];
                                        }
                                    }
                                    // 현재 슬롯이 마디의 첫번째 슬롯이 아니라면 이전 슬롯은 현재 마디의 이전 슬롯
                                    else
                                    {
                                        prevChordSlot = currentMeasure.chordSlots[slotIndex - 1];
                                    }
                                    // 이전에 코드가 있을 경우
                                    // 이전으로부터의 코드 진행 점수를 구함.
                                    if (prevChordSlot != null && prevChordSlot.chord != null)
                                    {
                                        prevPoint = ChordFunction.GetProgressionPoint(prevChordSlot.chord.EChordFunction, candidate.isSecondaryDominant ? (EChordFunction)(KeySignature.GetKeyNotes(KeySignature).ToList().IndexOf(candidate.Root)) : candidate.EChordFunction);
                                    }
                                    // 미래로의 점수 계산
                                    int nextPoint = 0;
                                    // 현재 슬롯이 현재 마디의 마지막 슬롯이라면
                                    if(slotIndex + 1 >= currentMeasure.chordSlots.Count)
                                    {
                                        // 다음 슬롯은 다음 마디의 첫번째 슬롯
                                        if(currentMeasure.NextMeasure != null)
                                        {
                                            nextChordSlot = currentMeasure.NextMeasure.chordSlots[0];
                                        }
                                    }
                                    // 현재 슬롯이 현재 마디의 마지막 슬롯이 아니라면
                                    else
                                    {
                                        // 다음 슬롯은 현재 마디의 다음 슬롯
                                        nextChordSlot = currentMeasure.chordSlots[slotIndex + 1];
                                    }
                                    if (nextChordSlot != null)
                                    {
                                        Chord nextImaginaryChord = null;
                                        // 이후에 코드가 있을 경우
                                        if (nextChordSlot.chord != null)
                                        {
                                            // 이후로부터의 코드 진행 점수르 구함.
                                            nextPoint = ChordFunction.GetProgressionPoint(candidate.EChordFunction, nextChordSlot.chord.EChordFunction);
                                            nextImaginaryChord = nextChordSlot.chord;
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
                                                foreach (Chord nextCandidate in nextChordSlot.candidateChords)
                                                {
                                                    if (nextCandidate.Root % 12 == resolveRoot)
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
                                                foreach (Chord nextCandidate in nextChordSlot.candidateChords)
                                                {
                                                    imaginaryPoint = ChordFunction.GetProgressionPoint(candidate.EChordFunction, nextCandidate.EChordFunction);
                                                    if (imaginaryPoint > maxImaginaryPoint)
                                                    {
                                                        maxImaginaryPoint = imaginaryPoint;
                                                        nextImaginaryChord = nextCandidate;
                                                    }
                                                    else if (imaginaryPoint == maxImaginaryPoint)
                                                    {
                                                        if (nextImaginaryChord.match < nextCandidate.match)
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
                                    else if (currentPoint == biggestPoint)
                                    {
                                        if (biggestMatch.match < candidate.match)
                                        {
                                            biggestMatch = candidate;
                                            biggestPoint = currentPoint;
                                        }
                                    }

                                }
                                currentSlot.chord = biggestMatch;
                            }
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
            SetSecondaryDominantsFirst();
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
                        sections[i].measures[0].chordSlots[0].chord = GetMatchingChord(oneTwoFour, sections[i].measures[0].noteWeights, sections[i], 0, spice);
                    }
                    // 마지막 코드 작성 : 전체 마지막 슬롯은 무조건 1도
                    Chord lastChord;
                    if (i == sections.Count - 1)
                    {
                        List<EChordFunction> one = new List<EChordFunction>();
                        one.Add(EChordFunction.Tonic);
                        lastChord = GetMatchingChord(one, sections[i].measures[lastMeasureIndex].noteWeights, sections[i], lastMeasureIndex, spice);
                        sections[i].measures[lastMeasureIndex].chordSlots[^1].chord = lastChord;
                    }
                    // 전체 마지막이 아니면 1도 혹은 5도.
                    else
                    {
                        List<EChordFunction> oneFive = new List<EChordFunction>();
                        oneFive.Add(EChordFunction.Tonic);
                        oneFive.Add(EChordFunction.Dominant);
                        lastChord = GetMatchingChord(oneFive, sections[i].measures[lastMeasureIndex].noteWeights, sections[i], lastMeasureIndex, spice);
                        sections[i].measures[lastMeasureIndex].chordSlots[^1].chord = lastChord;
                    }
                    // 섹션에 마디가 둘 이상 있고, 마지막 코드가 1도면
                    if (lastMeasureIndex > 0 && lastChord.Root == keyNotes[0])
                    {
                        // 마지막에서 두번째 코드 : 4, 5도
                        List<EChordFunction> fourFive = new List<EChordFunction>();
                        fourFive.Add(EChordFunction.SubDominant);
                        fourFive.Add(EChordFunction.Dominant);
                        Measure lastMeasure = sections[i].measures[lastMeasureIndex];
                        ChordSlot secondLastSlot = lastMeasure.chordSlots.Count > 1 ? lastMeasure.chordSlots[lastMeasure.chordSlots.Count - 2] : lastMeasure.PrevMeasure.chordSlots[^1];
                        secondLastSlot.chord = GetMatchingChord(fourFive, sections[i].measures[lastMeasureIndex - 1].noteWeights, sections[i], lastMeasureIndex - 1, spice);
                    }

                }
            }
        }

        private void SetSecondaryDominantsFirst()
        {
            for(int sectionIndex = 0;sectionIndex <sections.Count;sectionIndex++)
            {
                for(int measureIndex = 0;measureIndex < sections[sectionIndex].measures.Count; measureIndex++)
                {
                    for(int slotIndex = 0;slotIndex < sections[sectionIndex].measures[measureIndex].chordSlots.Count; slotIndex++)
                    {
                        ChordSlot slot = sections[sectionIndex].measures[measureIndex].chordSlots[slotIndex];
                        // 이미 코드가 있으면 넘김.
                        if (slot.chord != null)
                        {
                            continue;
                        }
                        // 아니면
                        else
                        {
                            int biggestPoint = int.MinValue;
                            Chord biggestMatch = null;
                            bool hasSecondaryDominant = false;
                            foreach(Chord candidate in slot.candidateChords)
                            {
                                if(candidate.isSecondaryDominant)
                                {
                                    hasSecondaryDominant = true;
                                    break;
                                }
                            }
                            if (hasSecondaryDominant)
                            {
                                // 각 후보코드당
                                foreach (Chord candidate in slot.candidateChords)
                                {

                                    // 미래로의 점수 계산
                                    int nextPoint = 0;
                                    if (slot.nextSlot != null)
                                    {
                                        Chord nextImaginaryChord = null;
                                        // 이후에 코드가 있을 경우
                                        if (slot.nextSlot.chord != null)
                                        {
                                            // 이후로부터의 코드 진행 점수르 구함.
                                            nextPoint = ChordFunction.GetProgressionPoint(candidate.EChordFunction, slot.nextSlot.chord.EChordFunction);
                                            nextImaginaryChord = slot.nextSlot.chord;
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
                                                foreach (Chord nextCandidate in slot.nextSlot.candidateChords)
                                                {
                                                    if (nextCandidate.Root % 12 == resolveRoot)
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
                                                foreach (Chord nextCandidate in slot.nextSlot.candidateChords)
                                                {
                                                    imaginaryPoint = ChordFunction.GetProgressionPoint(candidate.EChordFunction, nextCandidate.EChordFunction);
                                                    if (imaginaryPoint > maxImaginaryPoint)
                                                    {
                                                        maxImaginaryPoint = imaginaryPoint;
                                                        nextImaginaryChord = nextCandidate;
                                                    }
                                                    else if (imaginaryPoint == maxImaginaryPoint)
                                                    {
                                                        if (nextImaginaryChord.match < nextCandidate.match)
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
                                    int currentPoint = nextPoint;
                                    if (currentPoint > biggestPoint || biggestMatch == null)
                                    {
                                        biggestMatch = candidate;
                                        biggestPoint = currentPoint;
                                    }
                                    else if (currentPoint == biggestPoint)
                                    {
                                        if (biggestMatch.match < candidate.match)
                                        {
                                            biggestMatch = candidate;
                                            biggestPoint = currentPoint;
                                        }
                                    }

                                }
                                slot.chord = biggestMatch;
                                if (slot.chord.isSecondaryDominant)
                                {
                                    foreach(Chord nextCandidate in slot.nextSlot.candidateChords)
                                    {
                                        if(nextCandidate.Root % 12 == (slot.chord.Root + 5) % 12)
                                        {
                                            slot.nextSlot.chord = nextCandidate;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
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
                        for (int slotIndex = 0; slotIndex < measure.chordSlots.Count; slotIndex++)
                        {
                            measure.chordSlots[slotIndex].candidateChords.AddRange(GetDiatonicCandidateChords(measure.noteWeights));
                            // 논 다이어토닉 코드를 포함하고 있으면
                            if (!KeySignature.IsDiatonic(sections[sectionIndex].measures[measureIndex].notes))
                            {
                                // 세컨더리 도미넌트 코드도 추가.
                                measure.chordSlots[slotIndex].candidateChords.AddRange(GetMatchingSecondaryDominants(sections[sectionIndex].measures[measureIndex].noteWeights));
                            }
                            for (int chordIndex = 0; chordIndex < measure.chordSlots[slotIndex].candidateChords.Count; chordIndex++)
                            {
                                if (!ChordWorks(measure, measure.chordSlots[slotIndex].candidateChords[chordIndex]))
                                {
                                    measure.chordSlots[slotIndex].candidateChords.RemoveAt(chordIndex);
                                    chordIndex--;
                                }
                            }
                            // 하나도 맞는 코드가 없으면
                            if (measure.chordSlots[slotIndex].candidateChords.Count < 1)
                            {

                            }
                        }
                    }
                }
            }
        }

        private bool ChordWorks(Measure measure, Chord chord)
        {
            if(measure.NoteWeightsSum > 700)
            {
                return chord.match > 700;
            }
            else
            {
                return chord.match / measure.NoteWeightsSum > 0.7f;
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
