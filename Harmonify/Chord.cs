using System;
using System.Collections.Generic;
using System.Text;

namespace Harmonify
{
    public class Chord
    {
        public int root;
        public bool major;
        
        public static int Match(int note, List<int> chordNotes)
        {
            int root = chordNotes[0];
            if(note < root)
            {
                note += 12;
            }
            int difference = note - root;
            // 완전 1도
            if(difference == 0)
            {
                return 2;
            }
            // 감2도
            else if(difference == 1)
            {
                return 0;
            }
            // 단2도
            else if(difference == 2)
            {
                return 1;
            }
            // 단3도
            else if(difference == 3)
            {
                if(difference == (chordNotes[1] - root))
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
            // 장3도
            else if(difference == 4)
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
            else if(difference == 5)
            {
                return 0;
            }
            else if(difference == 6)
            {
                return 0;
            }
            else if(difference == 7)
            {
                return 2;
            }
            else if(difference == 8)
            {
                return 0;
            }
            else if(difference == 9)
            {
                return 1;
            }
            else if(difference == 10)
            {
                return 2;
            }
            else if(difference == 11)
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
