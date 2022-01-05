using System;
using System.Collections.Generic;
using System.Text;

namespace Harmonify
{
    public class Chord
    {
        public int root;
        public bool major;
        public List<int> chordNotes;

        public string GetChordNotation()
        {
            List<int> notes = new List<int>();
            notes.AddRange(chordNotes);
            if (notes.Count < 3)
            {
                return null;
            }
            else if (notes.Count == 3)
            {
                List<int> possibleTriad = GetPossibleTriad(notes);
                return Note.GetNoteName(possibleTriad[0]) + (possibleTriad[1] - possibleTriad[2] == 3 ? "m" : "");
            }
            else
            {
                // TODO :
                return null;
            }
        }

        public static string GetChordNotation(List<int> _chordNotes)
        {
            List<int> notes = new List<int>();
            notes.AddRange(_chordNotes);
            if (notes.Count < 3)
            {
                return null;
            }
            else if (notes.Count == 3)
            {
                List<int> possibleTriad = GetPossibleTriad(notes);
                return Note.GetNoteName(possibleTriad[0]) + (possibleTriad[1] - possibleTriad[2] == 3 ? "m" : "");
            }
            else
            {
                // TODO :
                return null;
            }
        }

        public static List<int> GetPossibleTriad(List<int> notes)
        {
            if (notes.Count > 3)
            {
                return null;
            }
            else
            {
                // 그자체로 Triad
                if (IsThirdStacked(notes))
                {
                    return notes;
                }
                else
                {
                    // 두번 inverse 해봄
                    for (int i = 0; i < 2; i++)
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
                if(interval < 0)
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

        public static int Match(int note, List<int> chordNotes)
        {
            int root = chordNotes[0];
            if (note < root)
            {
                note += 12;
            }
            int difference = note - root;
            // 완전 1도
            if (difference == 0)
            {
                return 2;
            }
            // 감2도
            else if (difference == 1)
            {
                return 0;
            }
            // 단2도
            else if (difference == 2)
            {
                return 1;
            }
            // 단3도
            else if (difference == 3)
            {
                if (difference == (chordNotes[1] - root))
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
            // 장3도
            else if (difference == 4)
            {
                if (difference == (chordNotes[1] - root))
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
            else if (difference == 5)
            {
                return 0;
            }
            else if (difference == 6)
            {
                return 0;
            }
            else if (difference == 7)
            {
                return 2;
            }
            else if (difference == 8)
            {
                return 0;
            }
            else if (difference == 9)
            {
                return 1;
            }
            else if (difference == 10)
            {
                return 2;
            }
            else if (difference == 11)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
