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
        public bool IsIncomplete { get; private set; }
        public int section;
        public int[] noteWeights = new int[12];
        public Measure PrevMeasure { get; private set; }
        public Measure NextMeasure { get; private set; }
        public string NoteNames 
        { 
            get 
            { 
                string noteNames = null; 
                for(int i = 0; i < notes.Count; i++) 
                {
                    noteNames += notes[i].noteName;
                }
                return noteNames;
            } 
        }
        public Measure(int _index)
        {
            index = _index;
        }

        public void Link(Measure prev, Measure next)
        {
            PrevMeasure = prev;
            NextMeasure = next;
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
            SetNoteWeights();
            IsIncomplete = CheckIncompleteMeasure();
        }

        private void SetNoteWeights()
        {
            for(int i = 0; i < noteWeights.Length; i++)
            {
                noteWeights[i] = 0;
            }
            for (int i = 0; i < notes.Count; i++)
            {
                noteWeights[notes[i].noteNumber % 12] += notes[i].length;
            }
        }

        private bool CheckIncompleteMeasure()
        {
            if(notes.Count < 1)
            {
                return false;
            }
            if(NextMeasure == null)
            {
                return false;
            }
            if(notes[0].onTime == GetFirstTick())
            {
                return false;
            }
            else
            {
                // 반보다 많이 쉰 상태에서 노트가 시작하면
                if((int)(Song.MidiFile.TicksPerQuarterNote * Song.TimeSigTop * 0.5f) < notes[0].onTime - GetFirstTick())
                {
                    return true;
                }
                else
                {
                    int currentMeasureNoteLengthsSum = 0;
                    int nextMeasureNoteLengthSum = 0;
                    for(int i = 0; i < notes.Count; i++)
                    {
                        currentMeasureNoteLengthsSum += notes[i].length;
                    }
                    for(int i = 0; i < NextMeasure.notes.Count; i++)
                    {
                        nextMeasureNoteLengthSum += NextMeasure.notes[i].length;
                    }
                    return nextMeasureNoteLengthSum > currentMeasureNoteLengthsSum;
                }
            }
        }


        public bool NoteExists()
        {
            return notes.Count > 0;
        }

        public int GetFirstTick()
        {
            return Song.MidiFile.TicksPerQuarterNote * Song.TimeSigTop * index;
        }

        public static int CheckSimilarity(Measure a, Measure b)
        {
            int[,] notes = new int[a.notes.Count + 1, b.notes.Count + 1];
            for(int i = 0; i < notes.GetLength(0); i++)
            {
                notes[i, 0] = 0;
            }
            for(int i = 0; i < notes.GetLength(1); i++)
            {
                notes[0, i] = 0;
            }
            int cost = 0;
            int addNum, minusNum, modiNum;
            for(int i = 1; i < notes.GetLength(0); i++)
            {
                for(int j = 1; j < notes.GetLength(1); j++)
                {
                    if(a.notes[i - 1].noteNumber != b.notes[j - 1].noteNumber)
                    {
                        cost = 1;
                    }
                    else
                    {
                        cost = 0;
                    }
                    addNum = notes[i - 1, j] + 1;
                    minusNum = notes[i, j - 1] + 1;
                    modiNum = notes[i - 1, j - 1] + cost;
                    notes[i, j] = Min(addNum, minusNum, modiNum);
                }
            }
            return notes[notes.GetLength(0) - 1, notes.GetLength(1) - 1];
        }

        private static int Min(int a, int b, int c)
        {
            int min = int.MaxValue;
            if(a < min)
            {
                min = a;
            }
            if(b < min)
            {
                min = b;
            }
            if(c < min)
            {
                min = c;
            }
            return min;
        }

        private int GetLastTick()
        {
            return Song.MidiFile.TicksPerQuarterNote * Song.TimeSigTop * (index + 1);
        }
    }
}
