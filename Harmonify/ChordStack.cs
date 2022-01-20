using System;
using System.Collections.Generic;
using System.Text;

namespace Harmonify
{
    public class ChordStack
    {
        public EStackType EStackType { get; private set; }
        public float Complexity { get; private set; }

        public int StackCount { get; private set; }

        public List<int> chordNotes = new List<int>();
        public ChordStack(EStackType eStackType)
        {
            EStackType = eStackType;
            string stackTypeStr = eStackType.ToString();
            Complexity = 0f;
            if (stackTypeStr.Contains("13"))
            {
                Complexity += 4f;
                StackCount = 7;
            }
            else if (stackTypeStr.Contains("11"))
            {
                Complexity += 3f;
                StackCount = 6;
            }
            else if (stackTypeStr.Contains("9"))
            {
                Complexity += 2f;
                StackCount = 5;
            }
            else if (stackTypeStr.Contains("7"))
            {
                Complexity += 1f;
                StackCount = 4;
            }
            else
            {
                StackCount = 3;
            }
            if (stackTypeStr.Contains("aug") || stackTypeStr.Contains("dim"))
            {
                Complexity += 1f;
            }
            if (stackTypeStr.Contains("s9") || stackTypeStr.Contains("b9") || stackTypeStr.Contains("s11") || stackTypeStr.Contains("b11") || stackTypeStr.Contains("s13") || stackTypeStr.Contains("b13"))
            {
                Complexity += 1f;
            }
        }

        private List<int> GetStackingNotes(EStackType eStackType)
        {
            const int CsDb = 1;
            const int D = 2;
            const int DsEb = 3;
            const int E = 4;
            const int F = 5;
            const int FsGb = 6;
            const int G = 7;
            const int GsAb = 8;
            const int A = 9;
            const int AsBb = 10;
            const int B = 11;
            const int OCTAVE_CHROMATIC = 12;
            List<int> stackingNotes = new List<int>();
            switch (EStackType)
            {
                case EStackType.maj:
                    stackingNotes.Add(E);
                    stackingNotes.Add(G);
                    break;
                case EStackType.min:
                    stackingNotes.Add(DsEb);
                    stackingNotes.Add(G);
                    break;
                case EStackType.aug:
                    stackingNotes.Add(E);
                    stackingNotes.Add(GsAb);
                    break;
                case EStackType.dim:
                    stackingNotes.Add(DsEb);
                    stackingNotes.Add(FsGb);
                    break;
                case EStackType.sus:
                    stackingNotes.Add(F);
                    stackingNotes.Add(G);
                    break;
                case EStackType.maj7:
                    stackingNotes.AddRange(GetStackingNotes(EStackType.maj));
                    stackingNotes.Add(B);
                    break;
                case EStackType._7:
                    stackingNotes.AddRange(GetStackingNotes(EStackType.maj));
                    stackingNotes.Add(AsBb);
                    break;
                case EStackType.min7:
                    stackingNotes.AddRange(GetStackingNotes(EStackType.min));
                    stackingNotes.Add(AsBb);
                    break;
                case EStackType.mM7:
                    stackingNotes.AddRange(GetStackingNotes(EStackType.min));
                    stackingNotes.Add(B);
                    break;
                case EStackType.dim7: // 도 미b 솔b 시bb
                    stackingNotes.AddRange(GetStackingNotes(EStackType.dim));
                    stackingNotes.Add(A);
                    break;
                case EStackType.dimMaj7: // 도 미b 솔b 시
                    stackingNotes.AddRange(GetStackingNotes(EStackType.dim));
                    stackingNotes.Add(B);
                    break;
                case EStackType.m7b5: // 도 미b 솔b 시b
                    stackingNotes.AddRange(GetStackingNotes(EStackType.dim));
                    stackingNotes.Add(AsBb);
                    break;
                case EStackType.aug7: // 도 미 솔# 시b
                    stackingNotes.AddRange(GetStackingNotes(EStackType.aug));
                    stackingNotes.Add(AsBb);
                    break;
                case EStackType.maj7s5: // 도 미 솔# 시
                    stackingNotes.AddRange(GetStackingNotes(EStackType.aug));
                    stackingNotes.Add(B);
                    break;
                case EStackType._7sus4: // 도 파 솔 시b
                    stackingNotes.AddRange(GetStackingNotes(EStackType.sus));
                    stackingNotes.Add(B);
                    break;
                case EStackType.maj6: // 도 미 솔 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType.maj));
                    stackingNotes.Add(A);
                    break;
                case EStackType.min6: // 도 미b 솔 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType.min));
                    stackingNotes.Add(A);
                    break;
                case EStackType.maj9: // 도 미 솔 시 레
                    stackingNotes.AddRange(GetStackingNotes(EStackType.maj7));
                    stackingNotes.Add(D + OCTAVE_CHROMATIC);
                    break;
                case EStackType._9: // 도 미 솔 시b 레
                    stackingNotes.AddRange(GetStackingNotes(EStackType._7));
                    stackingNotes.Add(D + OCTAVE_CHROMATIC);
                    break;
                case EStackType.s9: // 도 미 솔 시b 레#
                    stackingNotes.AddRange(GetStackingNotes(EStackType._7));
                    stackingNotes.Add(DsEb + OCTAVE_CHROMATIC);
                    break;
                case EStackType.b9: // 도 미 솔 시b 레b
                    stackingNotes.AddRange(GetStackingNotes(EStackType._7));
                    stackingNotes.Add(CsDb + OCTAVE_CHROMATIC);
                    break;
                case EStackType.min9: // 도 미b 솔 시b 레
                    stackingNotes.AddRange(GetStackingNotes(EStackType.min7));
                    stackingNotes.Add(D + OCTAVE_CHROMATIC);
                    break;
                case EStackType.augMaj9: // 도 미 솔# 시 레
                    stackingNotes.AddRange(GetStackingNotes(EStackType.maj7s5));
                    stackingNotes.Add(D + OCTAVE_CHROMATIC);
                    break;
                case EStackType.aug9: // 도 미 솔# 시b 레
                    stackingNotes.AddRange(GetStackingNotes(EStackType.aug7));
                    stackingNotes.Add(D + OCTAVE_CHROMATIC);
                    break;
                case EStackType.m9b5: // 도 미b 솔b 시b 레
                    stackingNotes.AddRange(GetStackingNotes(EStackType.m7b5));
                    stackingNotes.Add(D + OCTAVE_CHROMATIC);
                    break;
                case EStackType.mb9b5: // 도 미b 솔b 시b 레b
                    stackingNotes.AddRange(GetStackingNotes(EStackType.m9b5));
                    stackingNotes.Add(CsDb + OCTAVE_CHROMATIC);
                    break;
                case EStackType.dim9: // 도 미b 솔b 시bb 레
                    stackingNotes.AddRange(GetStackingNotes(EStackType.dim7));
                    stackingNotes.Add(D + OCTAVE_CHROMATIC);
                    break;
                case EStackType.dimb9: // 도 미b 솔b 시bb 레b
                    stackingNotes.AddRange(GetStackingNotes(EStackType.dim7));
                    stackingNotes.Add(CsDb + OCTAVE_CHROMATIC);
                    break;
                case EStackType.maj11: // 도 미 솔 시 레 파
                    stackingNotes.AddRange(GetStackingNotes(EStackType.maj9));
                    stackingNotes.Add(F + OCTAVE_CHROMATIC);
                    break;
                case EStackType._11: // 도 미 솔 시b 레 파
                    stackingNotes.AddRange(GetStackingNotes(EStackType._9));
                    stackingNotes.Add(F + OCTAVE_CHROMATIC);
                    break;
                case EStackType.s11: // 도 미 솔 시b 레 파#
                    stackingNotes.AddRange(GetStackingNotes(EStackType._9));
                    stackingNotes.Add(FsGb + OCTAVE_CHROMATIC);
                    break;
                case EStackType.min11: // 도 미b 솔 시b 레 파
                    stackingNotes.AddRange(GetStackingNotes(EStackType.min9));
                    stackingNotes.Add(F + OCTAVE_CHROMATIC);
                    break;
                case EStackType.augMaj11: // 도 미 솔# 시 레 파
                    stackingNotes.AddRange(GetStackingNotes(EStackType.augMaj9));
                    stackingNotes.Add(F + OCTAVE_CHROMATIC);
                    break;
                case EStackType._11s5: // 도 미 솔# 시b 레 파
                    stackingNotes.AddRange(GetStackingNotes(EStackType.aug9));
                    stackingNotes.Add(F + OCTAVE_CHROMATIC);
                    break;
                case EStackType._11b5: // 도 미 솔b 시b 레 파
                    stackingNotes.Add(E);
                    stackingNotes.Add(FsGb);
                    stackingNotes.Add(AsBb);
                    stackingNotes.Add(D + OCTAVE_CHROMATIC);
                    stackingNotes.Add(F + OCTAVE_CHROMATIC);
                    break;
                case EStackType.dim11: // 도 미b 솔b 시bb 레 파
                    stackingNotes.AddRange(GetStackingNotes(EStackType.dim9));
                    stackingNotes.Add(F + OCTAVE_CHROMATIC);
                    break;
                case EStackType.maj13: // 도 미 솔 시 레 파 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType.maj11));
                    stackingNotes.Add(F + OCTAVE_CHROMATIC);
                    break;
                case EStackType._13: // 도 미 솔 시b 레 파 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType._11));
                    stackingNotes.Add(A + OCTAVE_CHROMATIC);
                    break;
                case EStackType._13s11: // 도 미 솔 시b 레 파# 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType.s11));
                    stackingNotes.Add(A + OCTAVE_CHROMATIC);
                    break;
                case EStackType._13b11: // 도 미 솔 시b 레b (파) 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType.b9));
                    stackingNotes.Add(A + OCTAVE_CHROMATIC);
                    break;
                case EStackType.b13: // 도 미 솔 시b 레 파 라b
                    stackingNotes.AddRange(GetStackingNotes(EStackType._11));
                    stackingNotes.Add(GsAb + OCTAVE_CHROMATIC);
                    break;
                case EStackType.b13s11: // 도 미 솔 시b 레 파# 라b
                    stackingNotes.AddRange(GetStackingNotes(EStackType.s11));
                    stackingNotes.Add(GsAb + OCTAVE_CHROMATIC);
                    break;
                case EStackType.min13: // 도 미b 솔 시b 레 파 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType.min11));
                    stackingNotes.Add(A + OCTAVE_CHROMATIC);
                    break;
                case EStackType.augmaj13: // 도 미 솔# 시 레 파 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType.augMaj11));
                    stackingNotes.Add(A + OCTAVE_CHROMATIC);
                    break;
                case EStackType._13s5: // 도 미 솔# 시b 레 파 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType._11s5));
                    stackingNotes.Add(A + OCTAVE_CHROMATIC);
                    break;
                case EStackType._13b5: // 도 미b 솔b시b 레 파 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType._11b5));
                    stackingNotes.Add(A + OCTAVE_CHROMATIC);
                    break;

            }
            return stackingNotes;
        }
    }
}
