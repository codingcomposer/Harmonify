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
        public enum Majority { major, naturalMinor, harmonicMinor, melodicMinor }
        public Majority majority;
        private static readonly int[] majorNotes = new int[7] { 0, 2, 4, 5, 7, 9, 11 };
        private static readonly int[] naturalMinorNotes = new int[7] { 0, 2, 3, 5, 7, 8, 10 };
        private static readonly int[] harmonicMinorNotes = new int[7] { 0, 2, 3, 5, 7, 8, 11 };
        private static readonly int[] melodicMinorNotes = new int[7] { 0, 2, 3, 5, 7, 9, 1 };

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
            for (int i = 0; i < ints.Count; i++)
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
            if (tonicNote == to)
            {
                return 0;
            }
            // C G D A E B F#
            while (sharpDistance < 7 && to != sharpRoot)
            {
                sharpRoot = ((sharpRoot + 7) % 12);
                sharpDistance++;
            }
            // C F Bb Eb Ab Db Gb
            while (flatDistance < 7 && to != flatRoot)
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
            nearKeys.AddRange(GetRelativeKeys(this));
            // C G D A E B F#
            while (sharpDistance < distance)
            {
                sharpRoot = ((sharpRoot + 7) % 12);
                KeySignature nearKey = new KeySignature(sharpRoot, majority);
                nearKeys.Add(nearKey);

                if (sharpDistance < distance - 1)
                {
                    nearKeys.AddRange(GetRelativeKeys(nearKey));
                }
                sharpDistance++;
            }
            // C F Bb Eb Ab Db Gb
            while (flatDistance < distance)
            {
                flatRoot = (flatRoot + 5) % 12;
                KeySignature nearKey = new KeySignature(flatRoot, majority);
                nearKeys.Add(nearKey);
                if (sharpDistance < distance - 1)
                {
                    nearKeys.AddRange(GetRelativeKeys(nearKey));
                }
                flatDistance++;
            }
            return nearKeys;
        }

        public static bool IsMinor(Majority majority)
        {
            return majority == Majority.naturalMinor || majority == Majority.harmonicMinor || majority == Majority.melodicMinor;
        }


        private List<KeySignature> GetRelativeKeys(KeySignature originalKey)
        {
            List<KeySignature> relativeKeys = new List<KeySignature>();
            if (majority == Majority.major)
            {
                relativeKeys.Add(new KeySignature((originalKey.tonicNote + 9) % 12, Majority.harmonicMinor));
                relativeKeys.Add(new KeySignature((originalKey.tonicNote + 9) % 12, Majority.melodicMinor));
            }
            else
            {
                relativeKeys.Add(new KeySignature((originalKey.tonicNote + 3) % 12, Majority.major));
                if(originalKey.majority != Majority.harmonicMinor)
                {
                    relativeKeys.Add(new KeySignature(originalKey.tonicNote, Majority.harmonicMinor));
                }
                if(originalKey.majority != Majority.melodicMinor)
                {
                    relativeKeys.Add(new KeySignature(originalKey.tonicNote, Majority.melodicMinor));
                }
            }
            return relativeKeys;
        }

        public static bool AreRelativeKeys(KeySignature keyA, KeySignature keyB)
        {
            // 한쪽만 마이너면서  : 이부분은 나중에 모드 추가되면 수정할 것.
            if(IsMinor(keyA.majority) != IsMinor(keyB.majority))
            {
                return (keyA.tonicNote + 3) % 12 == keyB.tonicNote || (keyA.tonicNote + 9) % 12 == keyB.tonicNote;
            }
            else
            {
                return false;
            }
        }

        public static string GetKeyNotation(KeySignature key)
        {
            return Note.GetNoteName(key.tonicNote) + key.majority.ToString();
        }


        public static KeySignature AssumeKey(List<Measure> measures, List<Note> notes)
        {
            List<KeySignature> result = new List<KeySignature>();
            // 전체 마디에서 음들의 경중을 따짐.
            int[] weightedNotes = new int[12];
            for (int i = 0; i < measures.Count; i++)
            {
                for (int j = 0; j < measures[i].notes.Count; j++)
                {

                    weightedNotes[measures[i].notes[j].noteNumber % 12] += measures[i].notes[j].length;
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
            return AssumeKeyFromNotes(presentedNotes);
            /*
            // 마지막 노트는 섹션의 마지막 마디의 노트 중 마지막것이다.
            int lastNote = notes[^1].noteNumber;

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
            for (int i = 0; i < assumedKeys.Count; i++)
            {
                result.Add(assumedKeys[i]);

            }
            return result;
            */
        }

        private static float GetSimilarity(int[] keyNotes, List<int> notes)
        {
            Array.Sort(keyNotes);
            notes.Sort();
            int count = notes.Count < keyNotes.Length ? notes.Count : keyNotes.Length;
            int sameNotes = count;
            for (int i = 0; i < count; i++)
            {
                if (!keyNotes.Contains(notes[i]))
                {
                    sameNotes--;
                }
            }
            return (float)sameNotes / count;
        }

        private static KeySignature AssumeKeyFromNotes(List<int> notes)
        {
            KeySignature matchestKey = null;
            float maxMatch = 0f;
            for (int i = 0; i < notes.Count; i++)
            {
                KeySignature key = new KeySignature(notes[i], Majority.major);
                int[] keyNotes = GetKeyNotes(key);
                float similarity = GetSimilarity(keyNotes, notes);
                if (matchestKey == null || similarity > maxMatch)
                {
                    matchestKey = key;
                    maxMatch = similarity;
                }
            }
            return matchestKey;
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
                for (int i = 0; i < keyNotes.Length; i++)
                {

                    // 그 음에서 쌓아올린 다이어토닉 코드에 노트가 포함되어 있다면
                    if (GetDiatonicChordNotesFromRoot(keySignature, keyNotes[i], noteCount).Contains(note))
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
