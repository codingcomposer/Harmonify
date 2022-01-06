using System;
using System.Collections.Generic;
using System.Text;

namespace Harmonify
{
    public class Note
    {
        public int noteNumber;
        public int onTime;
        public int offTime;
        public int length;
        public string noteName;
        private enum eNoteName { C, Csharp, D, Dsharp, E, F, Fsharp, G, Gsharp, A, Asharp, B };

        public Note(int _noteNumber, int _onTime, int _offTime)
        {
            noteNumber = _noteNumber;
            onTime = _onTime;
            offTime = _offTime;
            length = offTime - onTime;
            noteName = GetNoteName(noteNumber);
        }

        public Note(Note note)
        {
            noteNumber = note.noteNumber;
            onTime = note.onTime;
            offTime = note.offTime;
            length = offTime - onTime;
            noteName = GetNoteName(noteNumber);
        }
        public static string GetNoteName(int noteNumber)
        {
            return ((eNoteName)((noteNumber) % 12)).ToString().Replace("sharp", "#");
        }

    }
}
