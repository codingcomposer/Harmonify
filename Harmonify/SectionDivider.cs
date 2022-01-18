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
                        addNew = i + 1 < measures.Count && !measures[i].IsIncompleteMeasure(measures[i + 1]);
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
                            addNew = !measures[i - 2].NoteExists() && measures[i - 1].IsIncompleteMeasure(measures[i]);
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
        private void SplitDuplicateMeasures()
        {
            int lastmaxSectionIndex = GetBiggestSectionIndex();
            int originalStartIndex = 0;
            int originalIndex = 0;

            while (originalIndex < measures.Count)
            {
                int nextIndex = GetNextSectionIndex(originalIndex);
                if (nextIndex - originalStartIndex > 4 && measures[originalStartIndex].NoteExists())
                {
                    bool continuing = false;
                    int originalSectionIndex = GetBiggestSectionIndex() + 1;
                    while (originalIndex < nextIndex - 4)
                    {
                        int comparisonSectionIndex = originalSectionIndex;
                        for (int comparisonIndex = originalIndex + 4; comparisonIndex < nextIndex; comparisonIndex++)
                        {
                            int checkSimilarity = Measure.CheckSimilarity(measures[originalIndex], measures[comparisonIndex]);
                            if (checkSimilarity == 0)
                            {
                                // 반복부분이 없다가 만난경우 비교마디 섹션 인덱스 새로 설정.
                                if (!continuing)
                                {
                                    comparisonSectionIndex++;
                                    continuing = true;
                                }
                                // 원본을 설정
                                if (measures[originalIndex].section <= lastmaxSectionIndex)
                                {
                                    measures[originalIndex].section = originalSectionIndex;
                                }
                                // 비교마디 섹션 인덱스 설정
                                if (measures[comparisonIndex].section <= lastmaxSectionIndex)
                                {
                                    measures[comparisonIndex].section = comparisonSectionIndex;
                                }
                                originalIndex++;
                            }
                            // 겹치는 부분이 없을 경우
                            else
                            {
                                continuing = false;
                                // 원본 인덱스 재설정
                                originalIndex = originalStartIndex;
                            }
                        }
                        originalIndex = nextIndex;
                        originalStartIndex = originalIndex;
                    }
                }
                // 길이가 5마디 미만이거나 빈 마디면 넘김.
                else
                {
                    originalIndex = nextIndex;
                    originalStartIndex = originalIndex;
                }
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

    }
}
