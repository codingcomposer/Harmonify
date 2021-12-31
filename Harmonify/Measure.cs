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
            for(int i = 0; i < notes.Count; i++)
            {
                if(Song.GetMeasureIndex(notes[i].onTime) < index)
                {
                    notes[i].onTime = GetFirstTick();
                    
                }
                if(Song.GetMeasureIndex(notes[i].offTime) > index)
                {
                    notes[i].offTime = GetLastTick();
                }
                notes[i].length = notes[i].offTime - notes[i].onTime;
            }
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
