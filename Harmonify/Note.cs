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
        public enum eNoteName { C, Csharp, D, Dsharp, E, F, Fsharp, G, Gsharp, A, Asharp, B };

        public const int C = 0;
        public const int CsDb = 1;
        public const int D = 2;
        public const int DsEb = 3;
        public const int E = 4;
        public const int F = 5;
        public const int FsGb = 6;
        public const int G = 7;
        public const int GsAb = 8;
        public const int A = 9;
        public const int AsBb = 10;
        public const int B = 11;
        public const int OCTAVE_CHROMATIC = 12;

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
            length = note.length;
            noteName = note.noteName;
        }
        
        public static string GetNoteName(int noteNumber)
        {
            return ((eNoteName)((noteNumber) % 12)).ToString().Replace("sharp", "#");
        }



        public static long GetTimingEuclideanDistance(Note a, Note b)
        {
            return (long)MathF.Sqrt(MathF.Pow(a.onTime - b.onTime, 2) + MathF.Pow(a.offTime - b.offTime, 2));
        }


    }
}
