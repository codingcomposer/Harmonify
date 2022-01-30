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
        public int match;
        public bool isSecondaryDominant;
        private const int CHORD_TONE_MATCH = 5;
        private const int AVOID_NOTE_MATCH = -3;
        private const int AVAILABLE_TENSION_MATCH = 2;
        private const int DIATONIC_MATCH = 1;
        private const int NONDIATONIC_MATCH = 0;
        private const int MINOR_SECOND_CRASH = -3;

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

        public static int Match(KeySignature keySignature, int note, int[] chordNotes)
        {
            note %= 12;
            int mode = note - keySignature.TonicNote;
            if(KeySignature.IsMinor(keySignature.majority))
            {
                mode += 5;
            }
            int match = 0;
            if (mode < 0)
            {
                mode += 7;
            }
            List<int> avoidNotes = GetAvoidNotes(keySignature.TonicNote, mode);
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
            // 이도저도 아닌 경우
            else
            {
                if(chordNotes.Contains((note + 1) % 12) || chordNotes.Contains((note - 1) % 12))
                {
                    match += MINOR_SECOND_CRASH;
                }
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
                    avoidNotes.Add((keyRoot + Note.F) % 12);
                    break;
                // 도리안 : B
                case 1:
                    avoidNotes.Add((keyRoot + Note.B) % 12);
                    break;
                // 프리지안 : F, C
                case 2:
                    avoidNotes.Add((keyRoot + Note.F) % 12);
                    avoidNotes.Add(keyRoot);
                    break;
                // 리디안 : 없음.
                // 믹솔리디안 : C
                case 4:
                    avoidNotes.Add(keyRoot);
                    break;
                // 에올리안
                case 5:
                    avoidNotes.Add((keyRoot + Note.F) % 12);
                    break;
                // 로크리안
                case 6:
                    avoidNotes.Add(keyRoot);
                    break;
            }
            return avoidNotes;
        }

        private static List<int> GetAvailableTensions(int keyRoot, int mode)
        {
            List<int> availableTensions = new List<int>();
            switch (mode)
            {
                // 아이오니안 : D, A
                case 0:
                    availableTensions.Add((keyRoot + Note.D) % 12);
                    availableTensions.Add((keyRoot + Note.A) % 12);
                    break;
                // 도리안 : E, G
                case 1:
                    availableTensions.Add((keyRoot + Note.E) % 12);
                    availableTensions.Add((keyRoot + Note.G) % 12);
                    break;
                // 프리지안 : A
                case 2:
                    availableTensions.Add((keyRoot + Note.A) % 12);
                    break;
                // 리디안 : G, B, D
                case 3:
                    availableTensions.Add((keyRoot + Note.G) % 12);
                    availableTensions.Add((keyRoot + Note.B) % 12);
                    availableTensions.Add((keyRoot + Note.D) % 12);
                    break;
                // 믹솔리디안 : A, E
                case 4:
                    availableTensions.Add((keyRoot + Note.A) % 12);
                    availableTensions.Add((keyRoot + Note.E) % 12);
                    break;
                // 애올리안 : B, D
                case 5:
                    availableTensions.Add((keyRoot + Note.B) % 12);
                    availableTensions.Add((keyRoot + Note.D) % 12);
                    break;
                // 로크리안 : E, G
                case 6:
                    availableTensions.Add((keyRoot + Note.E) % 12);
                    availableTensions.Add((keyRoot + Note.G) % 12);
                    break;
            }
            return availableTensions;
        }
    }
}
