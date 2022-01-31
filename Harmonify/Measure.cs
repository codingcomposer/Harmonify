﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Harmonify
{
    public class Measure
    {
        public int index;
        public readonly List<Note> notes = new List<Note>();
        public List<Chord> chords = new List<Chord>();
        public List<Chord> candidateChords = new List<Chord>();
        public bool IsIncomplete { get; private set; }
        public int section;
        public int[] noteWeights = new int[12];
        public List<int> presentedNotes = new List<int>();
        public int[] firstHalfNoteWeights = new int[12];
        public int[] secondHalfNoteWeights = new int[12];
        public int NoteWeightsSum { get; private set; }
        public Measure PrevMeasure { get; private set; }
        public Measure NextMeasure { get; private set; }
        public int chordCount;
        public int FirstBeatTick { get; private set; }
        public int SecondBeatTick { get; private set; }
        public int ThirdBeatTick { get; private set; }
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
            FirstBeatTick = GetFirstTick();
            ThirdBeatTick = FirstBeatTick + ((GetLastTick() - FirstBeatTick) / 2);
            SecondBeatTick = FirstBeatTick + ((ThirdBeatTick - FirstBeatTick) / 2);
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
                    notes[i].onTime = FirstBeatTick;

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
            int halfTick = FirstBeatTick + (GetLastTick() - FirstBeatTick / 2);
            for (int i = 0; i < notes.Count; i++)
            {
                int weight = notes[i].length;
                if (notes[i].onTime == FirstBeatTick)
                {
                    weight = (int)(weight * 2f);
                }
                else if (notes[i].onTime <= SecondBeatTick && notes[i].offTime > SecondBeatTick)
                {
                    weight = (int)(weight * 1.2f);
                }
                else if (notes[i].onTime <= ThirdBeatTick && notes[i].offTime > ThirdBeatTick)
                {
                    weight = (int)(weight * 1.5f);
                }
                noteWeights[notes[i].noteNumber % 12] += weight;
                if(notes[i].offTime <= halfTick)
                {
                    firstHalfNoteWeights[notes[i].noteNumber % 12] += weight;
                }
                else if(notes[i].onTime >= halfTick)
                {
                    secondHalfNoteWeights[notes[i].noteNumber % 12] += weight;
                }
                else
                {
                    firstHalfNoteWeights[notes[i].noteNumber % 12] += (halfTick - notes[i].onTime);
                    secondHalfNoteWeights[notes[i].noteNumber % 12] += (notes[i].offTime - halfTick);
                }
                NoteWeightsSum += notes[i].length;
            }
            for(int i = 0; i < noteWeights.Length; i++)
            {
                if(noteWeights[i] > 0)
                {
                    presentedNotes.Add(i);
                }
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
            if(notes[0].onTime == FirstBeatTick)
            {
                return false;
            }
            else
            {
                // 반보다 많이 쉰 상태에서 노트가 시작하면
                if((int)(Song.MidiFile.TicksPerQuarterNote * Song.TimeSigTop * 0.5f) < notes[0].onTime - FirstBeatTick)
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
