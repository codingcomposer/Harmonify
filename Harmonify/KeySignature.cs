using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Harmonify
{
    public class KeySignature
    {
        public int TonicNote { get { return tonicNote; } }
        private int tonicNote;
        public enum Majority { major, naturalMinor, harmonicMinor}
        public Majority majority;
        private static readonly int[] majorNotes = new int[7] { 0, 2, 4, 5, 7, 9, 11 };
        private static readonly int[] naturalMinorNotes = new int[7] { 0, 2, 3, 5, 7, 8, 10 };
        private static readonly int[] harmonicMinorNotes = new int[7] { 0, 2, 3, 5, 7, 8, 11 };

        public KeySignature(int _tonicNote, Majority _majority)
        {
            tonicNote = _tonicNote;
            majority = _majority;
        }
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

        public int GetKeyDistance(int to)
        {
            int sharpDistance = 0;
            int flatDistance = 0;
            int sharpRoot = tonicNote;
            int flatRoot = tonicNote;
            if(tonicNote == to)
            {
                return 0;
            }
            // C G D A E B F#
            while(sharpDistance < 7 && to != sharpRoot)
            {
                sharpRoot = ((sharpRoot + 7) % 12);
                sharpDistance++;
            }
            // C F Bb Eb Ab Db Gb
            while(flatDistance < 7 && to != flatRoot)
            {
                flatRoot = (flatRoot + 5) % 12;
                flatDistance++;
            }
            return sharpDistance < flatDistance ? sharpDistance : flatDistance;
        }

        public List<KeySignature> GetNearKeys(int distance)
        {
            List<KeySignature> nearKeys = new List<KeySignature>();
            int sharpDistance = 0;
            int flatDistance = 0;
            int sharpRoot = tonicNote;
            int flatRoot = tonicNote;
            if (majority == Majority.major)
            {
                nearKeys.Add(new KeySignature((sharpRoot + 8) % 12, Majority.major));
            }
            else
            {
                nearKeys.Add(new KeySignature((sharpRoot + 3) % 12, Majority.harmonicMinor));
            }
            // C G D A E B F#
            while (sharpDistance < distance)
            {
                sharpRoot = ((sharpRoot + 7) % 12);
                nearKeys.Add(new KeySignature(sharpRoot, majority));
                if (majority == Majority.major)
                {
                    nearKeys.Add(new KeySignature((sharpRoot + 8) % 12, Majority.harmonicMinor));
                }
                else
                {
                    nearKeys.Add(new KeySignature((sharpRoot + 3) % 12, Majority.major));
                }
                sharpDistance++;
            }
            // C F Bb Eb Ab Db Gb
            while (flatDistance < distance)
            {
                flatRoot = (flatRoot + 5) % 12;
                nearKeys.Add(new KeySignature(flatRoot, Majority.major));
                if (majority == Majority.major)
                {
                    nearKeys.Add(new KeySignature((flatRoot + 4) % 12, Majority.harmonicMinor));
                }
                else
                {
                    nearKeys.Add(new KeySignature((flatRoot + 3) % 12, Majority.major));
                }
                flatDistance++;
            }
            for (int i = 0; i < nearKeys.Count; i++)
            {
                System.Windows.Forms.MessageBox.Show(Note.GetNoteName(nearKeys[i].TonicNote) + "," + nearKeys[i].majority);
            }
            return nearKeys;
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
            List<KeySignature> assumedKeys = AssumeKeysFromNotes(presentedNotes);
            // 마지막 노트는 섹션의 마지막 마디의 노트 중 마지막것이다.
            int lastNote = section.measures[section.measures.Count - 1].notes[section.measures[section.measures.Count - 1].notes.Count - 1].noteNumber;

            // 등장한 음으로 예상한 키 중
            for (int i = 0; i < assumedKeys.Count; i++)
            {
                //i번째 키 기준으로 마지막 노트에서 뽑아낼 수 있는 다이어토닉코드들을 찾아서
                List<int> diatonicChordRoots = GetPossibleDiatonicChordRoots(assumedKeys[i], lastNote, 3);
                if (diatonicChordRoots.Count > 0)
                {
                    for (int j = 0; j < diatonicChordRoots.Count; j++)
                    {
                        // 해당 키의 토닉 코드인경우 그 키이다.
                        if (assumedKeys[i].tonicNote == diatonicChordRoots[j])
                        {
                            result.Add(assumedKeys[i]);
                            return result;
                        }
                    }
                }
            }
            for(int i = 0; i < assumedKeys.Count; i++)
            {
                result.Add(assumedKeys[i]);

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

        private static List<KeySignature> AssumeKeysFromNotes(List<int> notes)
        {
            List<KeySignature> keys = new List<KeySignature>();
            for (int i = 0; i < notes.Count; i++)
            {
                int[] keyNotes = GetKeyNotes(new KeySignature(notes[i], Majority.major));
                float similarity = GetSimilarity(keyNotes, notes);
                if (similarity > 0.8f)
                {

                    keys.Add(new KeySignature(notes[i], Majority.major));
                }
            }
            return keys;
        }

        // 해당 키에서 note를 포함한 diatonic chord들의 root를 반환한다.
        private static List<int> GetPossibleDiatonicChordRoots(KeySignature keySignature, int note, int noteCount)
        {
            List<int> result = new List<int>();
            note %= 12;
            int[] keyNotes = GetKeyNotes(keySignature);
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
                    if(GetDiatonicChordNotesFromRoot(keySignature, keyNotes[i], noteCount).Contains(note))
                    {
                        // 그 음을 추가.
                        result.Add(keyNotes[i]);
                    }
                }
                return result;
            }
        }
        // 키에 포함된 노트들을 가지고 온다.
        public static int[] GetKeyNotes(KeySignature keySignature)
        {
            int[] notes = GetMajorityNotes(keySignature.majority);
            int[] keyNotes = new int[notes.Length];
            for (int i = 0; i < notes.Length; i++)
            {
                keyNotes[i] = (notes[i] + keySignature.tonicNote) % 12;
            }
            return keyNotes;
        }

        public bool IsDiatonic(List<Note> notes)
        {
            List<int> keyNotes = GetKeyNotes(this).ToList();
            for (int i = 0; i < notes.Count; i++)
            {
                if (!keyNotes.Contains(notes[i].noteNumber % 12))
                {
                    return false;
                }
            }
            return true;
        }


        // 키에서 해당 노트를 루트로 하는 코드의 구성음을 가져온다.
        public static List<int> GetDiatonicChordNotesFromRoot(KeySignature keySignature, int chordRoot, int noteCount)
        {
            List<int> result = new List<int>();
            int[] keyNotes = GetKeyNotes(keySignature);
            int chordRootIndex = Array.IndexOf(keyNotes, chordRoot);
            // 코드 루트가 다이어토닉이 아니다.
            if (chordRootIndex < 0)
            {
                return result;
            }
            else
            {
                for (int i = 0; i < noteCount; i++)
                {
                    result.Add(keyNotes[(chordRootIndex + 2 * i) % 7]);
                }
            }
            return result;
        }

        private static int[] GetMajorityNotes(Majority majority)
        {
            return majority switch
            {
                Majority.major => majorNotes,
                Majority.naturalMinor => naturalMinorNotes,
                Majority.harmonicMinor => harmonicMinorNotes,
                _ => majorNotes,
            };
        }
    }
}
