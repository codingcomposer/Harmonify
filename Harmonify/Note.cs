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

        public Note(int _noteNumber, int _onTime, int _offTime)
        {
            noteNumber = _noteNumber;
            onTime = _onTime;
            offTime = _offTime;
            length = offTime - onTime;
            noteName = Song.GetNoteName(noteNumber);
        }

        public Note(Note note)
        {
            noteNumber = note.noteNumber;
            onTime = note.onTime;
            offTime = note.offTime;
            length = offTime - onTime;
            noteName = Song.GetNoteName(noteNumber);
        }

    }
}
