using System;
using System.Collections.Generic;
using System.Text;

namespace Harmonify
{
    public class Section
    {
        public string sectionName;
        public List<Measure> measures = new List<Measure>();

        public Section Copy(int startIndex, int endIndex)
        {
            Section section = new Section();
            section.sectionName = sectionName;
            List<Measure> measures = new List<Measure>();
            for(int i = startIndex;i<endIndex; i++)
            {
                section.measures.Add(measures[i]);
            }
            return section;
        }
    }
}
