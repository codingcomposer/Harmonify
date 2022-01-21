using System;
using System.Collections.Generic;
using System.Text;

namespace Harmonify
{
    class SectionDivider
    {
        private List<Measure> measures;

        public void Divide(List<Measure> _measures)
        {
            measures = _measures;
            SplitMultimeasureRests();
            SplitDuplicateMeasures();
            MergeShortSections();
            DivideEightMeasures();
        }
        private void SplitMultimeasureRests()
        {
            int sectionIndex = 0;
            measures[0].section = 0;
            // 큼직하게 분할 (절과 절)
            for (int i = 1; i < measures.Count; i++)
            {
                bool addNew = false;
                // 없으면
                if (!measures[i].NoteExists())
                {
                    // 없은지 첫번째면
                    if (measures[i - 1].NoteExists())
                    {
                        // 마지막 마디이거나 다음 마디가 찼으면 새섹션 아님(기존 섹션에 추가)
                        addNew = !(i + 1 >= measures.Count || measures[i + 1].NoteExists());
                    }
                    // 없은지 여러번이면 그냥 기존 섹션에 추가.
                    else
                    {
                        addNew = false;
                    }
                }
                // 있으면
                else
                {
                    // 있은지 첫번째면
                    if (!measures[i - 1].NoteExists())
                    {
                        // 못갖춘마디가 아니면 새 섹션
                        addNew = i + 1 < measures.Count && !measures[i].IsIncomplete;
                    }
                    //있은지 여러번이면
                    else
                    {
                        // 2번째마디면 새 섹션
                        if (i == 1)
                        {
                            addNew = true;
                        }
                        // 3번째마디부터는
                        else if (i > 1)
                        {
                            // 전전마디가 비어있으면서 이전 마디가 못갖춘마디면 새섹션
                            addNew = !measures[i - 2].NoteExists() && measures[i - 1].IsIncomplete;
                        }
                    }
                }
                if (addNew)
                {
                    sectionIndex++;
                    measures[i].section = sectionIndex;
                }
                else
                {
                    measures[i].section = sectionIndex;
                }
            }
        }

        // 다른 방법 다 했을 때, 8마디씩 대해서 자름.
        private void DivideEightMeasures()
        {
            int measureIndex = 0;
            while (measureIndex < measures.Count)
            {
                int sectionLength = GetLengthOfSection(measureIndex);
                if (sectionLength >= 16 && sectionLength % 8 == 0)
                {
                    int biggestSectionIndex = GetBiggestSectionIndex();
                    int newLastIndex = GetFirstSectionIndex(measureIndex) + (GetNextSectionIndex(measureIndex) - GetFirstSectionIndex(measureIndex)) / 2;
                    biggestSectionIndex++;
                    for (int i = measureIndex; i < newLastIndex; i++)
                    {
                        measures[i].section = biggestSectionIndex;
                    }
                }
                measureIndex = GetNextSectionIndex(measureIndex);
            }
        }

        // 반복 부분 잘라낸 후, 조금씩 남은 마디에 대해서 이전 섹션과 합침.
        private void MergeShortSections()
        {
            int measureIndex = 0;
            while (measureIndex < measures.Count)
            {
                // 마지막 꼬다리는
                if (measureIndex == measures.Count - 1)
                {
                    // 이전에 붙임.
                    int nextIndex = GetNextSectionIndex(measureIndex);
                    int sectionNumber = measures[measureIndex - 1].section;
                    for (int i = measureIndex; i < nextIndex; i++)
                    {
                        measures[i].section = sectionNumber;
                    }
                }
                // 노트가 있고, 못갖춘 마디가 아닌 경우에 대해서
                else if (measures[measureIndex].NoteExists() && !measures[measureIndex].IsIncomplete)
                {
                    // 짧은 섹션이라면
                    if (GetLengthOfSection(measureIndex) < 4)
                    {
                        // 이전 마디가 있고, 이전 마디가 못갖춘마디가 아닌 경우
                        if (measureIndex > 0 && measures[measureIndex - 1].NoteExists() && !measures[measureIndex - 1].IsIncomplete)
                        {
                            // 이전에 붙임.
                            int nextIndex = GetNextSectionIndex(measureIndex);
                            int sectionNumber = measures[measureIndex - 1].section;
                            for (int i = measureIndex; i < nextIndex; i++)
                            {
                                measures[i].section = sectionNumber;
                            }
                        }
                        // 이후에 붙임.
                        else
                        {
                            int nextIndex = GetNextSectionIndex(measureIndex);
                            int sectionNumber = measures[nextIndex].section;
                            for (int i = measureIndex; i < nextIndex; i++)
                            {
                                measures[i].section = sectionNumber;
                            }
                        }
                    }
                }
                measureIndex = GetNextSectionIndex(measureIndex);
            }
        }
        private void SplitDuplicateMeasures()
        {
            int originalStartIndex = 0;
            int originalIndex = 0;
            while (originalIndex < measures.Count - 4)
            {
                int undeterminedSectionIndex = GetBiggestSectionIndex();
                int nextIndex = GetNextSectionIndex(originalIndex);
                bool continuing = false;
                int originalSectionIndex = GetBiggestSectionIndex() + 1;
                // 현재 원본 섹션의 첫마디가 정해지지 않았으면서 마디 수가 10마디 이상이면서 첫 마디에 노트가 존재할 경우
                if (measures[originalStartIndex].section <= undeterminedSectionIndex && nextIndex - originalStartIndex > 9 && measures[originalStartIndex].NoteExists())
                {
                    int comparisonSectionIndex = originalSectionIndex;
                    for (int comparisonIndex = originalIndex + 4; comparisonIndex < nextIndex; comparisonIndex++)
                    {
                        int checkSimilarity = Measure.CheckSimilarity(measures[originalIndex], measures[comparisonIndex]);
                        // 비슷하면
                        if (checkSimilarity == 0)
                        {
                            // 반복부분이 없다가 만난경우 비교마디 섹션 인덱스 새로 설정.
                            if (!continuing)
                            {
                                comparisonSectionIndex++;
                            }
                            continuing = true;
                            // 원본을 설정
                            if (measures[originalIndex].section <= undeterminedSectionIndex)
                            {
                                measures[originalIndex].section = originalSectionIndex;
                            }
                            // 비교마디 섹션 인덱스 설정
                            if (measures[comparisonIndex].section <= undeterminedSectionIndex)
                            {
                                measures[comparisonIndex].section = comparisonSectionIndex;
                            }
                            originalIndex++;
                        }
                        // 겹치는 부분이 없을 경우
                        else
                        {
                            // 반복되고 있었을 경우
                            if (continuing)
                            {
                                // 나머지 체크
                                int originalRemainderLength = GetFirstSectionIndex(comparisonIndex - 1) - GetNextSectionIndex(originalIndex - 1);
                                if (originalRemainderLength < 4)
                                {
                                    int originalSection = measures[originalIndex - 1].section;
                                    for (int remainderIndex = 0; remainderIndex < originalRemainderLength; remainderIndex++)
                                    {
                                        measures[originalIndex + remainderIndex].section = originalSection;
                                    }
                                }
                                int originalLength = GetLengthOfSection(originalIndex);
                                int comparisonSection = measures[comparisonIndex - 1].section;
                                int comparisonLastIndex = GetFirstSectionIndex(comparisonIndex - 1) + originalLength;
                                if(comparisonLastIndex > measures.Count)
                                {
                                    comparisonLastIndex = measures.Count;
                                }
                                for (int remainderIndex = comparisonIndex; remainderIndex < comparisonLastIndex; remainderIndex++)
                                {
                                    measures[remainderIndex].section = comparisonSection;
                                }
                            }
                            continuing = false;
                            // 원본 인덱스 재설정
                            originalIndex = originalStartIndex;
                        }
                    }
                }
                originalIndex = GetNextSectionIndex(originalIndex);
                originalStartIndex = originalIndex;
            }
        }

        private int GetBiggestSectionIndex()
        {
            int biggest = 0;
            for (int i = 0; i < measures.Count; i++)
            {
                if (biggest < measures[i].section)
                {
                    biggest = measures[i].section;
                }
            }
            return biggest;
        }

        // 현재 마디가 속해있는 섹션의 다음 섹션의 첫부분 마디 인덱스를 반환한다.
        private int GetNextSectionIndex(int fromIndex)
        {
            int sectionIndex = measures[fromIndex].section;
            int newSectionIndex = measures.Count;
            for (int i = fromIndex; i < measures.Count; i++)
            {
                if (measures[i].section != sectionIndex)
                {
                    newSectionIndex = i;
                    break;
                }
            }
            return newSectionIndex;
        }

        // 현재 마디가 속해있는 섹션의 첫부분 마디 인덱스를 반환한다.
        private int GetFirstSectionIndex(int fromIndex)
        {
            int sectionIndex = measures[fromIndex].section;
            for (int i = fromIndex; i >= 0; i--)
            {
                if (measures[i].section != sectionIndex)
                {
                    return i + 1;
                }
            }
            return 0;
        }


        private int GetLengthOfSection(int measureIndex)
        {
            return GetNextSectionIndex(measureIndex) - GetFirstSectionIndex(measureIndex);
        }

    }
}
