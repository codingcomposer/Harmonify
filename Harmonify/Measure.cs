using System;
using System.Collections.Generic;
using System.Text;

namespace Harmonify
{
    public class Measure
    {
        public int index;
        public List<Note> notes = new List<Note>();
        public List<Chord> chords = new List<Chord>();

        public Measure(int _index)
        {
            index = _index;
        }

        public void TrimNotes()
        {
            for (int i = 0; i < notes.Count; i++)
            {
                if (Song.GetMeasureIndex(notes[i].onTime) < index)
                {
                    notes[i].onTime = GetFirstTick();

                }
                if (Song.GetMeasureIndex(notes[i].offTime) > index)
                {
                    notes[i].offTime = GetLastTick();
                }
                notes[i].length = notes[i].offTime - notes[i].onTime;
            }
        }

        // 마디에서 가장 주요한 음을 찾는다.
        public List<Tuple<int, int>> GetWeightedNotes()
        {
            int[] weightedNotes = new int[12];
            for (int j = 0; j < notes.Count; j++)
            {

                weightedNotes[notes[j].noteNumber % 12] += notes[j].length;
            }
            List<Tuple<int, int>> tuples = new List<Tuple<int, int>>();
            for (int i = 0; i < weightedNotes.Length; i++)
            {
                if (weightedNotes[i] != 0)
                {
                    tuples.Add(new Tuple<int, int>(i, weightedNotes[i]));
                }
            }
            tuples.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            for(int i = 0; i < tuples.Count; i++)
            {
                // 쓰이지 않은 음은 제거
                if(tuples[i].Item2 <= 0)
                {
                    tuples.RemoveAt(i);
                    i--;
                }
            }
            return tuples;
        }

        private int GetFirstTick()
        {
            return Song.MidiFile.TicksPerQuarterNote * Song.TimeSigTop * index;
        }

        private int GetLastTick()
        {
            return Song.MidiFile.TicksPerQuarterNote * Song.TimeSigTop * (index + 1);
        }
    }
}
