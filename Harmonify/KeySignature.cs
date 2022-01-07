using System;
using System.Collections.Generic;
using System.Text;

namespace Harmonify
{
    public class KeySignature
    {
        public int tonicNote;
        private static readonly int[] majorNotes = new int[7] { 0, 2, 4, 5, 7, 9, 11 };
        private static readonly int[] minorNotes = new int[7] { 0, 2, 3, 5, 7, 8, 10 };

        private static void Log(string str)
        {
            System.Windows.Forms.MessageBox.Show(str);
        }

        private static void Log(List<int> ints)
        {
            string str = null;
            for(int i = 0; i < ints.Count; i++)
            {
                str += ints[i];
            }
            System.Windows.Forms.MessageBox.Show(str);
        }


        public static List<KeySignature> AssumeKeys(Section section)
        {
            List<KeySignature> result = new List<KeySignature>();
            // 전체 마디에서 음들의 경중을 따짐.
            int[] weightedNotes = new int[12];
            for (int i = 0; i < section.measures.Count; i++)
            {
                for (int j = 0; j < section.measures[i].notes.Count; j++)
                {

                    weightedNotes[section.measures[i].notes[j].noteNumber % 12] += section.measures[i].notes[j].length;
                }
            }
            List<Tuple<int, int>> tuples = new List<Tuple<int, int>>();
            for (int i = 0; i < weightedNotes.Length; i++)
            {
                if (weightedNotes[i] != 0)
                {
                    tuples.Add(new Tuple<int, int>(i, weightedNotes[i]));
                }
            }
            // 가장 중한 음 순으로 정렬
            tuples.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            // 7개 음까지만 추림.
            for (int i = 0; i < tuples.Count; i++)
            {

                if (i > 6)
                {
                    tuples.RemoveAt(i);
                    i--;
                }
            }
            List<int> presentedNotes = new List<int>();
            for (int i = 0; i < tuples.Count; i++)
            {
                presentedNotes.Add(tuples[i].Item1);
            }
            presentedNotes.Sort();
            List<int> assumedKeys = AssumeKeysFromNotes(presentedNotes);
            // 마지막 노트는 섹션의 마지막 마디의 노트 중 마지막것이다.
            int lastNote = section.measures[section.measures.Count - 1].notes[section.measures[section.measures.Count - 1].notes.Count - 1].noteNumber;

            // 등장한 음으로 예상한 키 중
            for (int i = 0; i < assumedKeys.Count; i++)
            {
                //i번째 키 기준으로 마지막 노트에서 뽑아낼 수 있는 다이어토닉코드들을 찾아서
                List<int> diatonicChordRoots = GetPossibleDiatonicChordRoots(assumedKeys[i], true, lastNote);
                if (diatonicChordRoots.Count > 0)
                {
                    for (int j = 0; j < diatonicChordRoots.Count; j++)
                    {
                        // 해당 키의 토닉 코드인경우 그 키이다.
                        if (assumedKeys[i] == diatonicChordRoots[j])
                        {
                            KeySignature keySignature = new KeySignature();
                            keySignature.tonicNote = assumedKeys[i];
                            result.Add(keySignature);
                            return result;
                        }
                    }
                }
            }
            for(int i = 0; i < assumedKeys.Count; i++)
            {
                result.Add(new KeySignature() { tonicNote = assumedKeys[i] });

            }
            return result;
        }

        private static float GetSimilarity(int[] keyNotes, List<int> notes)
        {
            Array.Sort(keyNotes);
            notes.Sort();
            int count = notes.Count < keyNotes.Length ? notes.Count : keyNotes.Length;
            int sameNotes = count;
            for (int i = 0; i < count; i++)
            {
                if (keyNotes[i] != notes[i])
                {
                    sameNotes--;
                }
            }
            return (float)sameNotes / count;
        }

        private static List<int> AssumeKeysFromNotes(List<int> notes)
        {
            List<int> keys = new List<int>();
            for (int i = 0; i < notes.Count; i++)
            {
                int[] keyNotes = GetKeyNotes(notes[i], true);
                float similarity = GetSimilarity(keyNotes, notes);
                if (similarity > 0.8f)
                {

                    keys.Add(notes[i]);
                }
            }
            return keys;
        }

        // 해당 키에서 note를 포함한 diatonic chord들의 root를 반환한다.
        private static List<int> GetPossibleDiatonicChordRoots(int keyRoot, bool major, int note)
        {
            List<int> result = new List<int>();
            note %= 12;
            int[] keyNotes = GetKeyNotes(keyRoot, major);
            int noteIndex = Array.IndexOf(keyNotes, note);
            // 노트가 다이어토닉이 아니다.
            if (noteIndex < 0)
            {
                return result;
            }
            // 노트가 다이어토닉이면
            else
            {
                // 키에 있는 음 중
                for(int i = 0; i < keyNotes.Length; i++)
                {
                    
                    // 그 음에서 쌓아올린 다이어토닉 코드에 노트가 포함되어 있다면
                    if(GetDiatonicChordNotesFromRoot(keyRoot, major, keyNotes[i]).Contains(note))
                    {
                        // 그 음을 추가.
                        result.Add(keyNotes[i]);
                    }
                }
                return result;
            }
        }
        // 키에 포함된 노트들을 가지고 온다.
        public static int[] GetKeyNotes(int keyRoot, bool major)
        {
            int[] notes = major ? majorNotes : minorNotes;
            int[] keyNotes = new int[notes.Length];
            for (int i = 0; i < notes.Length; i++)
            {
                keyNotes[i] = (notes[i] + keyRoot) % 12;
            }
            return keyNotes;
        }


        // 키에서 해당 노트를 루트로 하는 코드의 구성음을 가져온다.
        public static List<int> GetDiatonicChordNotesFromRoot(int keyRoot, bool major, int chordRoot)
        {
            List<int> result = new List<int>();
            int[] notes = major ? majorNotes : minorNotes;
            int[] keyNotes = GetKeyNotes(keyRoot, major);
            int chordRootIndex = Array.IndexOf(keyNotes, chordRoot);
            // 코드 루트가 다이어토닉이 아니다.
            if (chordRootIndex < 0)
            {
                return result;
            }
            else
            {

                for (int i = 0; i < 3; i++)
                {
                    result.Add(keyNotes[(chordRootIndex + 2 * i) % 7]);
                }
            }
            return result;

        }

    }
}
