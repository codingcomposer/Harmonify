using System;
using System.Collections.Generic;
using System.Text;

namespace Harmonify
{
    public class Chord
    {
        public int root;
        public bool major;
        public List<int> chordNotes = new List<int>();

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

        public static string GetChordNotation(List<int> chordNotes)
        {
            string result = null;
            if (chordNotes.Count < 3)
            {
                return null;
            }
            else
            {
                
                List<int> possibleTriad = GetPossibleTriad(chordNotes);
                result = Note.GetNoteName(possibleTriad[0]);
                int mediantInterval = (possibleTriad[1] - possibleTriad[0]);
                if(mediantInterval < 0)
                {
                    mediantInterval += 12;
                }
                if(mediantInterval == 3)
                {
                    result += "m";
                }
                if(chordNotes.Count == 4)
                {
                    int sevenInterval = possibleTriad[3] - possibleTriad[0];
                    if(sevenInterval == 10)
                    {
                        result += "7";
                    }
                    else if(sevenInterval == 11) 
                    {
                        result += "M7";
                    }
                }
                return result;
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
            int match = 0;
            int thirdInterval = chordNotes[1] - note;
            int fifthInterval = chordNotes[2] - note;
            if (chordNotes.Contains(note))
            {
                return 10;
            }
            else if(thirdInterval == -1 || thirdInterval == 1 || fifthInterval == -1 || fifthInterval == 1)
            {
                return -10;
            }
            else
            {
                return 0;
            }
            /*
                int chordNote = chordNotes[0];
            
                if (note < chordNote)
                {
                    note += 12;
                }
                int difference = note - chordNote;
                // 완전 1도
                if (difference == 0)
                {
                    match += 2;
                }
                // 감2도
                else if (difference == 1)
                {
                }
                // 단2도
                else if (difference == 2)
                {
                    match += 1;
                }
                // 단3도
                else if (difference == 3)
                {
                    if (difference == (chordNotes[1] - chordNote))
                    {
                        match += 2;
                    }
                }
                // 장3도
                else if (difference == 4)
                {
                    if (difference == (chordNotes[1] - chordNote))
                    {
                        match += 2;
                    }
                }
                // 완전4도
                else if (difference == 5)
                {
                }
                // 감5도. 트라이톤
                else if (difference == 6)
                {
                }
                // 완전5도
                else if (difference == 7)
                {
                    match += 2;
                }
                // 증5도. 단6도
                else if (difference == 8)
                {
                }
                // 장6도
                else if (difference == 9)
                {
                    match += 1;
                }
                // 단7도
                else if (difference == 10)
                {
                    match += 2;
                }
                // 장7도
                else if (difference == 11)
                {
                    match += 2;
                }
                else
                {
                }
            */
            return match;
        }
    }
}
