using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Harmonify
{
    public class Chord
    {
        public int Root { get; private set; }
        public ChordStack ChordStack { get; private set; }
        public EChordFunction EChordFunction { get; private set; }

        public int[] chordNotes;
        public float match;
        public bool isSecondaryDominant;
        private const float CHORD_TONE_MATCH = 1f;
        private const float AVOID_NOTE_MATCH = -1f;
        private const float AVAILABLE_TENSION_MATCH = 0.6f;
        private const float NONDIATONIC_AVAILABLE_TENSION_MATCH = 0.1f;
        private const float DIATONIC_MATCH = 0.1f;
        private const float NONDIATONIC_MATCH = -3f;

        public override string ToString()
        {
            return Note.GetNoteName(Root) + ChordStack.ToString();
        }
        public Chord(ChordStack chordStack, EChordFunction eChordFunction, int root)
        {
            ChordStack = chordStack;
            EChordFunction = eChordFunction;
            Root = root;
            chordNotes = new int[ChordStack.chordNotes.Count];
            for(int i = 0; i < chordNotes.Length; i++)
            {
                chordNotes[i] = root + ChordStack.chordNotes[i];
            }
        }
        private static eMode? GetMode(KeySignature keySignature, int chordRoot)
        {
            int modeNum = 0;
            List<int> keyNotes = KeySignature.GetKeyNotes(keySignature).ToList();
            modeNum = keyNotes.IndexOf(chordRoot);
            if(modeNum == -1)
            {
                return null;
            }
            if(keySignature.majority == KeySignature.Majority.melodicMinor)
            {
                modeNum += 7;
            }
            return (eMode)modeNum;
        }

        public static float Match(KeySignature keySignature, int note, int[] chordNotes)
        {
            note %= 12;
            for(int i = 0; i < chordNotes.Length; i++)
            {
                chordNotes[i] %= 12;
            }
            eMode? modeNullable = GetMode(keySignature, chordNotes[0]);
            if(modeNullable == null)
            {
                return float.MinValue;
            }
            eMode mode = (eMode)modeNullable;
            float match = 0f;
            List<int> avoidNotes = GetAvoidNotes(keySignature.TonicNote, mode);
            List<int> nonDiatonicAvailableTensions = GetNonDiatonicAvailableTensions(keySignature.TonicNote, mode);
            List<int> availableTensions = GetAvailableTensions(keySignature.TonicNote, mode);
            // 코드음일 경우
            if (chordNotes.Contains(note))
            {
                match += CHORD_TONE_MATCH;
            }
            // 어보이드노트일 경우
            else if (avoidNotes.Contains(note))
            {
                match += AVOID_NOTE_MATCH;
            }
            // 어베일러블 텐션일 경우
            else if (availableTensions.Contains(note))
            {
                match += AVAILABLE_TENSION_MATCH;
            }
            // 논다이어토닉 어베일러블 텐션일 경우
            else if (nonDiatonicAvailableTensions.Contains(note))
            {
                match += NONDIATONIC_AVAILABLE_TENSION_MATCH;
            }
            // 이도저도 아닌 경우
            else
            {
                    int[] keyNotes = KeySignature.GetKeyNotes(keySignature);
                    if (keyNotes.ToList().Contains(note))
                    {
                        match += DIATONIC_MATCH;
                    }
                    else
                    {
                        match += NONDIATONIC_MATCH;
                    }
            }
            return match;
        }

        private static List<int> GetAvoidNotes(int keyRoot, eMode mode)
        {
            List<int> avoidNotes = new List<int>();

            switch (mode)
            {
                // 아이오니안 : F
                case eMode.ioanian:
                    avoidNotes.Add((keyRoot + Note.F) % 12);
                    break;
                // 도리안 : B
                case eMode.dorian:
                    avoidNotes.Add((keyRoot + Note.B) % 12);
                    break;
                // 프리지안 : F, C
                case eMode.phrygian:
                    avoidNotes.Add((keyRoot + Note.F) % 12);
                    avoidNotes.Add(keyRoot);
                    break;
                // 리디안 : 없음.
                // 믹솔리디안 : C
                case eMode.mixolydian:
                    avoidNotes.Add(keyRoot);
                    break;
                // 에올리안
                case eMode.aeolian:
                    avoidNotes.Add((keyRoot + Note.F) % 12);
                    break;
                // 로크리안
                case eMode.locrian:
                    avoidNotes.Add(keyRoot);
                    break;
                // 
                case eMode.ionianb3:
                    break;
                case eMode.dorianb2:
                    avoidNotes.Add((keyRoot + Note.DsEb) % 12);
                    break;
                case eMode.lydianAugmented:
                    avoidNotes.Add((keyRoot + Note.C) % 12);
                    break;
                case eMode.lydianb7:
                    break;
                case eMode.mixolydianb6:
                    break;
                case eMode.locrians2:
                    break;
            }
            return avoidNotes;
        }

        private static List<int> GetNonDiatonicAvailableTensions(int keyRoot, eMode mode)
        {
            List<int> availableTensions = new List<int>();
            switch (mode)
            {
                case eMode.mixolydian:
                    availableTensions.Add((keyRoot + Note.AsBb) % 12);
                    availableTensions.Add((keyRoot + Note.AsBb) % 12);
                    availableTensions.Add((keyRoot + Note.CsDb) % 12);
                    availableTensions.Add((keyRoot + Note.DsEb) % 12);
                    break;
                case eMode.lydianAugmented:
                    availableTensions.Add((keyRoot + Note.FsGb) % 12);
                    break;
                case eMode.lydianb7:
                    availableTensions.Add((keyRoot + Note.FsGb) % 12);
                    break;
                case eMode.locrians2:
                    availableTensions.Add((keyRoot + Note.GsAb) % 12);
                    break;
            }
            return availableTensions;
        }

        private static List<int> GetAvailableTensions(int keyRoot, eMode mode)
        {
            List<int> availableTensions = new List<int>();
            switch (mode)
            {
                // 아이오니안 : D, A
                case eMode.ioanian:
                    availableTensions.Add((keyRoot + Note.D) % 12);
                    availableTensions.Add((keyRoot + Note.A) % 12);
                    break;
                // 도리안 : E, G
                case eMode.dorian:
                    availableTensions.Add((keyRoot + Note.E) % 12);
                    availableTensions.Add((keyRoot + Note.G) % 12);
                    break;
                // 프리지안 : A
                case eMode.phrygian:
                    availableTensions.Add((keyRoot + Note.A) % 12);
                    break;
                // 리디안 : G, B, D
                case eMode.lydian:
                    availableTensions.Add((keyRoot + Note.G) % 12);
                    availableTensions.Add((keyRoot + Note.B) % 12);
                    availableTensions.Add((keyRoot + Note.D) % 12);
                    break;
                // 믹솔리디안 : A, E
                case eMode.mixolydian:
                    availableTensions.Add((keyRoot + Note.A) % 12);
                    availableTensions.Add((keyRoot + Note.E) % 12);
                    break;
                // 애올리안 : B, D
                case eMode.aeolian:
                    availableTensions.Add((keyRoot + Note.B) % 12);
                    availableTensions.Add((keyRoot + Note.D) % 12);
                    break;
                // 로크리안 : E, G
                case eMode.locrian:
                    availableTensions.Add((keyRoot + Note.E) % 12);
                    availableTensions.Add((keyRoot + Note.G) % 12);
                    break;
                case eMode.ionianb3:
                    availableTensions.Add((keyRoot + Note.D) % 12);
                    availableTensions.Add((keyRoot + Note.F) % 12);
                    availableTensions.Add((keyRoot + Note.A) % 12);
                    break;
                case eMode.dorianb2:
                    availableTensions.Add((keyRoot + Note.F) % 12);
                    availableTensions.Add((keyRoot + Note.A) % 12);
                    break;
                case eMode.lydianAugmented:
                    availableTensions.Add((keyRoot + Note.D) % 12);
                    break;
                case eMode.lydianb7:
                    availableTensions.Add((keyRoot + Note.D) % 12);
                    availableTensions.Add((keyRoot + Note.A) % 12);
                    break;
                case eMode.mixolydianb6:
                    availableTensions.Add((keyRoot + Note.D) % 12);
                    availableTensions.Add((keyRoot + Note.F) % 12);
                    break;
                case eMode.locrians2:
                    availableTensions.Add((keyRoot + Note.D) % 12);
                    availableTensions.Add((keyRoot + Note.F) % 12);
                    break;
                case eMode.superlocrian:
                    break;
            }
            return availableTensions;
        }
    }
}
