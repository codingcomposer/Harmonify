using System;
using System.Collections.Generic;
using System.Text;

namespace Harmonify
{
    public class ChordSlot
    {
        public ChordSlot prevSlot;
        public ChordSlot nextSlot;
        public Chord chord;
        public List<Chord> candidateChords = new List<Chord>();
        public int[] noteWeights;

        public ChordSlot(int[] _noteWeights)
        {
            noteWeights = _noteWeights;
        }
    }
}
