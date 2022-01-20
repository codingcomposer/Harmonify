using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Harmonify
{
    public class Chord
    {
        public int root;
        public ChordStack chordStack;
        public EChordFunction eChordFunction;
        //public bool major;
        public List<int> chordNotes = new List<int>();
        public int match;
        public bool isSecondaryDominant;

        public string GetChordNotation()
        {
            return Note.GetNoteName(root) + ChordPrototype.GetStackNotation(chordStack.EStackType);
        }
        

        private static List<int> GetPossibleTriad(List<int> notes)
        {
            // 그자체로 Triad
            if (IsThirdStacked(notes))
            {
                return notes;
            }
            else
            {
                // 노트 갯수만큼 inverse 해봄
                for (int i = 0; i < notes.Count; i++)
                {
                    Inverse(notes);
                    if (IsThirdStacked(notes))
                    {
                        return notes;
                    }
                }
                // 그래도 안되면 Null.
                return null;

            }

        }

        public static void Inverse(List<int> notes)
        {
            int firstNote = notes[0];
            notes.RemoveAt(0);
            notes.Add(firstNote);
        }

        public static bool IsThirdStacked(List<int> notes)
        {
            for (int i = 0; i < notes.Count - 1; i++)
            {
                int interval = notes[i + 1] - notes[i];
                if (interval < 0)
                {
                    interval += 12;
                }
                if (interval != 3 && interval != 4)
                {
                    return false;
                }
            }
            return true;
        }

        public static int Match(KeySignature keySignature, int note, List<int> chordNotes)
        {
            note %= 12;
            int mode = note - keySignature.TonicNote;
            if(KeySignature.IsMinor(keySignature.majority))
            {
                mode += 5;
            }
            int match = 0;
            if (mode < 0)
            {
                mode += 7;
            }
            List<int> avoidNotes = GetAvoidNotes(keySignature.TonicNote, mode);
            List<int> availableTensions = GetAvailableTensions(keySignature.TonicNote, mode);
            if (chordNotes.Contains(note))
            {
                match += 5;
            }
            else if (avoidNotes.Contains(note))
            {
                match -= 2;
            }
            else if (availableTensions.Contains(note))
            {
                match += 2;
            }
            else
            {
                int[] keyNotes = KeySignature.GetKeyNotes(keySignature);
                if (keyNotes.ToList().Contains(note))
                {
                    match += 1;
                }
                else
                {
                    match -= 3;
                }
            }
            return match;
        }

        private static List<int> GetAvoidNotes(int keyRoot, int mode)
        {
            List<int> avoidNotes = new List<int>();

            switch (mode)
            {
                // 아이오니안 : F
                case 0:
                    avoidNotes.Add((keyRoot + 5) % 12);
                    break;
                // 도리안 : B
                case 1:
                    avoidNotes.Add((keyRoot + 11) % 12);
                    break;
                // 프리지안 : F, C
                case 2:
                    avoidNotes.Add((keyRoot + 5) % 12);
                    avoidNotes.Add(keyRoot);
                    break;
                // 리디안 : 없음.
                // 믹솔리디안 : C
                case 4:
                    avoidNotes.Add(keyRoot);
                    break;
                // 에올리안
                case 5:
                    avoidNotes.Add((keyRoot + 5) % 12);
                    break;
                // 로크리안
                case 6:
                    avoidNotes.Add(keyRoot);
                    break;
            }
            return avoidNotes;
        }

        private static List<int> GetAvailableTensions(int keyRoot, int mode)
        {
            List<int> availableTensions = new List<int>();
            switch (mode)
            {
                // 아이오니안 : D, A
                case 0:
                    availableTensions.Add((keyRoot + (int)Note.eNoteName.D) % 12);
                    availableTensions.Add((keyRoot + (int)Note.eNoteName.A) % 12);
                    break;
                // 도리안 : E, G
                case 1:
                    availableTensions.Add((keyRoot + (int)Note.eNoteName.E) % 12);
                    availableTensions.Add((keyRoot + (int)Note.eNoteName.G) % 12);
                    break;
                // 프리지안 : A
                case 2:
                    availableTensions.Add((keyRoot + (int)Note.eNoteName.A) % 12);
                    break;
                // 리디안 : G, B, D
                case 3:
                    availableTensions.Add((keyRoot + (int)Note.eNoteName.G) % 12);
                    availableTensions.Add((keyRoot + (int)Note.eNoteName.B) % 12);
                    availableTensions.Add((keyRoot + (int)Note.eNoteName.D) % 12);
                    break;
                // 믹솔리디안 : A, E
                case 4:
                    availableTensions.Add((keyRoot + (int)Note.eNoteName.A) % 12);
                    availableTensions.Add((keyRoot + (int)Note.eNoteName.E) % 12);
                    break;
                // 애올리안 : B, D
                case 5:
                    availableTensions.Add((keyRoot + (int)Note.eNoteName.B) % 12);
                    availableTensions.Add((keyRoot + (int)Note.eNoteName.D) % 12);
                    break;
                // 로크리안 : E, G
                case 6:
                    availableTensions.Add((keyRoot + (int)Note.eNoteName.E) % 12);
                    availableTensions.Add((keyRoot + (int)Note.eNoteName.G) % 12);
                    break;
            }
            return availableTensions;
        }
    }
}
