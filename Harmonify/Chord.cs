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
        public int match;

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

        public static int Match(int keyRoot, int note, List<int> chordNotes)
        {
            note %= 12;
            int mode = note - keyRoot;
            int match = 0;
            if(mode < 0)
            {
                mode += 12;
            }
            List<int> avoidNotes = GetAvoidNotes(keyRoot, mode);
            if (chordNotes.Contains(note))
            {
                match += 2;
            }
            else if (avoidNotes.Contains(note))
            {
                match -= 2;
            }
            else
            {
                match += 1;
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
                case 2:
                    avoidNotes.Add((keyRoot + 11) % 12);
                    break;
                // 프리지안 : F, C
                case 4:
                    avoidNotes.Add((keyRoot + 5) % 12);
                    avoidNotes.Add(keyRoot);
                    break;
                // 믹솔리디안
                case 7:
                    avoidNotes.Add(keyRoot);
                    break;
                // 에올리안
                case 9:
                    avoidNotes.Add((keyRoot + 5) % 12);
                    break;
                // 로크리안
                case 11:
                    avoidNotes.Add(keyRoot);
                    break;
            }
            return avoidNotes;
        }
    }
}
