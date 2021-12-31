using System;
using System.Collections.Generic;
using System.Text;

namespace Harmonify
{
    public class KeySignature
    {
        private static int[] majorNotes = new int[7] { 0, 2, 4, 5, 7, 9, 11 };
        private static int[] minorNotes = new int[7] { 0, 2, 3, 5, 7, 8, 10 };


        public static List<int> GetKeyNotes(int keyRoot, bool major)
        {
            List<int> result = new List<int>();
            int[] notes = major ? majorNotes : minorNotes;
            for(int i = 0; i < notes.Length; i++)
            {
                result.Add((notes[i] + 12) % 12);
            }
            return result;
        }

        public static List<int> GetDiatonicChordNotes(int keyRoot, bool major, int chordRoot)
        {
            List<int> result = new List<int>();
            int[] notes = major ? majorNotes : minorNotes;
            int chordRootIndex = Array.IndexOf(notes, chordRoot);
            // 코드 루트가 다이어토닉이 아니다.
            if (chordRootIndex < 0)
            {
                return null;
            }
            else
            {
                
                for (int i = 0; i < 3; i++)
                {
                    result.Add(notes[(chordRootIndex + 2 * i) % 7]);
                }
            }
            for(int i = 0; i < result.Count; i++)
            {
                result[i] = (result[i] + keyRoot + 12) % 12;
            }
            return result;
            
        }

    }
}
