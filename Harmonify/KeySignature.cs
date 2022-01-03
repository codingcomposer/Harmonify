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
                result.Add((notes[i] + keyRoot) % 12);
            }
            return result;
        }

        public static void AssumeKey(Section section)
        {
            int[] weightedNotes = new int[12];
            for (int i = 0; i < section.measures.Count; i++)
            {
                for (int j = 0; j < section.measures[i].notes.Count; j++)
                {

                    weightedNotes[section.measures[i].notes[j].noteNumber % 12] += section.measures[i].notes[j].length;
                }
            }
            List<Tuple<int, int>> tuples = new List<Tuple<int, int>>();
            for(int i = 0; i < weightedNotes.Length; i++)
            {
                if(weightedNotes[i] != 0)
                {
                    tuples.Add(new Tuple<int, int>(i, weightedNotes[i]));
                }
            }
            tuples.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            for(int i = 0; i < tuples.Count; i++)
            {
                if(i > 6)
                {
                    tuples.RemoveAt(i);
                    i--;
                }
            }
            List<int> notes = new List<int>();
            for(int i = 0; i < tuples.Count; i++)
            {
                notes.Add(tuples[i].Item1);
            }
            notes.Sort();
            string notesString = null;
            for (int i = 0; i < notes.Count; i++)
            {
                notesString += Song.GetNoteName(notes[i]) + ",";
            }
            List<int> assumedKeys = AssumeKeysFromNotes(notes);
            System.Windows.Forms.MessageBox.Show(notesString);
            int lastNote = section.measures[section.measures.Count - 1].notes[section.measures[section.measures.Count - 1].notes.Count - 1].noteNumber;
            System.Windows.Forms.MessageBox.Show("마지막 : " + Song.GetNoteName(lastNote));
            for (int i = 0; i < assumedKeys.Count; i++)
            {
                if (lastNote.Equals(assumedKeys[i]))
                {

                    System.Windows.Forms.MessageBox.Show(assumedKeys[i] + "키로 보임");
                }
            }
        }

        private static int GetSimilarity(List<int> keyNotes, List<int> notes)
        {
            keyNotes.Sort();
            notes.Sort();
            int count = notes.Count < keyNotes.Count ? notes.Count : keyNotes.Count;
            int similarity = count;
            for(int i = 0; i < count; i++)
            {
                if(keyNotes[i] != notes[i])
                {
                    similarity--;
                }
            }
            return similarity;
        }

        private static List<int> AssumeKeysFromNotes(List<int> notes)
        {
            List<int> keys = new List<int>();
            for(int i = 0; i < notes.Count; i++)
            {
                List<int> keyNotes = GetKeyNotes(notes[i], true);
                int similarity = GetSimilarity(keyNotes, notes);
                if (similarity > 5)
                {
                    
                    keys.Add(notes[i]);
                }
            }
            string assumedKeys = "예상키 :";
            for(int i = 0; i < keys.Count; i++)
            {
                assumedKeys += Song.GetNoteName(keys[i]) + ",";
            }
            System.Windows.Forms.MessageBox.Show(assumedKeys);
            return keys;
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
