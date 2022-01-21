﻿using System;
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
            }
            else if (stackTypeStr.Contains("11"))
            {
                Complexity += 3f;
            }
            else if (stackTypeStr.Contains("9"))
            {
                Complexity += 2f;
            }
            else if (stackTypeStr.Contains("7"))
            {
                Complexity += 1f;
            }
            if (stackTypeStr.Contains("aug") || stackTypeStr.Contains("dim"))
            {
                Complexity += 1f;
            }
            if (stackTypeStr.Contains("s9") || stackTypeStr.Contains("b9") || stackTypeStr.Contains("s11") || stackTypeStr.Contains("b11") || stackTypeStr.Contains("s13") || stackTypeStr.Contains("b13"))
            {
                Complexity += 1f;
            }
            chordNotes.Add(0);
            chordNotes.AddRange(GetStackingNotes(eStackType));
        }

        private List<int> GetStackingNotes(EStackType eStackType)
        {
            List<int> stackingNotes = new List<int>();
            switch (eStackType)
            {
                case EStackType.maj:
                    stackingNotes.Add(Note.E);
                    stackingNotes.Add(Note.G);
                    break;
                case EStackType.min:
                    stackingNotes.Add(Note.DsEb);
                    stackingNotes.Add(Note.G);
                    break;
                case EStackType.aug:
                    stackingNotes.Add(Note.E);
                    stackingNotes.Add(Note.GsAb);
                    break;
                case EStackType.dim:
                    stackingNotes.Add(Note.DsEb);
                    stackingNotes.Add(Note.FsGb);
                    break;
                case EStackType.sus:
                    stackingNotes.Add(Note.F);
                    stackingNotes.Add(Note.G);
                    break;
                case EStackType.maj7:
                    stackingNotes.AddRange(GetStackingNotes(EStackType.maj));
                    stackingNotes.Add(Note.B);
                    break;
                case EStackType._7:
                    stackingNotes.AddRange(GetStackingNotes(EStackType.maj));
                    stackingNotes.Add(Note.AsBb);
                    break;
                case EStackType.min7:
                    stackingNotes.AddRange(GetStackingNotes(EStackType.min));
                    stackingNotes.Add(Note.AsBb);
                    break;
                case EStackType.mM7:
                    stackingNotes.AddRange(GetStackingNotes(EStackType.min));
                    stackingNotes.Add(Note.B);
                    break;
                case EStackType.dim7: // 도 미b 솔b 시bb
                    stackingNotes.AddRange(GetStackingNotes(EStackType.dim));
                    stackingNotes.Add(Note.A);
                    break;
                case EStackType.dimMaj7: // 도 미b 솔b 시
                    stackingNotes.AddRange(GetStackingNotes(EStackType.dim));
                    stackingNotes.Add(Note.B);
                    break;
                case EStackType.m7b5: // 도 미b 솔b 시b
                    stackingNotes.AddRange(GetStackingNotes(EStackType.dim));
                    stackingNotes.Add(Note.AsBb);
                    break;
                case EStackType.aug7: // 도 미 솔# 시b
                    stackingNotes.AddRange(GetStackingNotes(EStackType.aug));
                    stackingNotes.Add(Note.AsBb);
                    break;
                case EStackType.maj7s5: // 도 미 솔# 시
                    stackingNotes.AddRange(GetStackingNotes(EStackType.aug));
                    stackingNotes.Add(Note.B);
                    break;
                case EStackType._7sus4: // 도 파 솔 시b
                    stackingNotes.AddRange(GetStackingNotes(EStackType.sus));
                    stackingNotes.Add(Note.B);
                    break;
                case EStackType.maj6: // 도 미 솔 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType.maj));
                    stackingNotes.Add(Note.A);
                    break;
                case EStackType.min6: // 도 미b 솔 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType.min));
                    stackingNotes.Add(Note.A);
                    break;
                case EStackType.maj9: // 도 미 솔 시 레
                    stackingNotes.AddRange(GetStackingNotes(EStackType.maj7));
                    stackingNotes.Add(Note.D + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType._9: // 도 미 솔 시b 레
                    stackingNotes.AddRange(GetStackingNotes(EStackType._7));
                    stackingNotes.Add(Note.D + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.s9: // 도 미 솔 시b 레#
                    stackingNotes.AddRange(GetStackingNotes(EStackType._7));
                    stackingNotes.Add(Note.DsEb + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.b9: // 도 미 솔 시b 레b
                    stackingNotes.AddRange(GetStackingNotes(EStackType._7));
                    stackingNotes.Add(Note.CsDb + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.min9: // 도 미b 솔 시b 레
                    stackingNotes.AddRange(GetStackingNotes(EStackType.min7));
                    stackingNotes.Add(Note.D + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.augMaj9: // 도 미 솔# 시 레
                    stackingNotes.AddRange(GetStackingNotes(EStackType.maj7s5));
                    stackingNotes.Add(Note.D + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.aug9: // 도 미 솔# 시b 레
                    stackingNotes.AddRange(GetStackingNotes(EStackType.aug7));
                    stackingNotes.Add(Note.D + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.m9b5: // 도 미b 솔b 시b 레
                    stackingNotes.AddRange(GetStackingNotes(EStackType.m7b5));
                    stackingNotes.Add(Note.D + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.mb9b5: // 도 미b 솔b 시b 레b
                    stackingNotes.AddRange(GetStackingNotes(EStackType.m9b5));
                    stackingNotes.Add(Note.CsDb + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.dim9: // 도 미b 솔b 시bb 레
                    stackingNotes.AddRange(GetStackingNotes(EStackType.dim7));
                    stackingNotes.Add(Note.D + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.dimb9: // 도 미b 솔b 시bb 레b
                    stackingNotes.AddRange(GetStackingNotes(EStackType.dim7));
                    stackingNotes.Add(Note.CsDb + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.maj11: // 도 미 솔 시 레 파
                    stackingNotes.AddRange(GetStackingNotes(EStackType.maj9));
                    stackingNotes.Add(Note.F + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType._11: // 도 미 솔 시b 레 파
                    stackingNotes.AddRange(GetStackingNotes(EStackType._9));
                    stackingNotes.Add(Note.F + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.s11: // 도 미 솔 시b 레 파#
                    stackingNotes.AddRange(GetStackingNotes(EStackType._9));
                    stackingNotes.Add(Note.FsGb + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.min11: // 도 미b 솔 시b 레 파
                    stackingNotes.AddRange(GetStackingNotes(EStackType.min9));
                    stackingNotes.Add(Note.F + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.augMaj11: // 도 미 솔# 시 레 파
                    stackingNotes.AddRange(GetStackingNotes(EStackType.augMaj9));
                    stackingNotes.Add(Note.F + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType._11s5: // 도 미 솔# 시b 레 파
                    stackingNotes.AddRange(GetStackingNotes(EStackType.aug9));
                    stackingNotes.Add(Note.F + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType._11b5: // 도 미 솔b 시b 레 파
                    stackingNotes.Add(Note.E);
                    stackingNotes.Add(Note.FsGb);
                    stackingNotes.Add(Note.AsBb);
                    stackingNotes.Add(Note.D + Note.OCTAVE_CHROMATIC);
                    stackingNotes.Add(Note.F + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.dim11: // 도 미b 솔b 시bb 레 파
                    stackingNotes.AddRange(GetStackingNotes(EStackType.dim9));
                    stackingNotes.Add(Note.F + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.maj13: // 도 미 솔 시 레 파 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType.maj11));
                    stackingNotes.Add(Note.F + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType._13: // 도 미 솔 시b 레 파 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType._11));
                    stackingNotes.Add(Note.A + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType._13s11: // 도 미 솔 시b 레 파# 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType.s11));
                    stackingNotes.Add(Note.A + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.b13: // 도 미 솔 시b 레 파 라b
                    stackingNotes.AddRange(GetStackingNotes(EStackType._11));
                    stackingNotes.Add(Note.GsAb + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.b13s11: // 도 미 솔 시b 레 파# 라b
                    stackingNotes.AddRange(GetStackingNotes(EStackType.s11));
                    stackingNotes.Add(Note.GsAb + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.min13: // 도 미b 솔 시b 레 파 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType.min11));
                    stackingNotes.Add(Note.A + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType.augmaj13: // 도 미 솔# 시 레 파 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType.augMaj11));
                    stackingNotes.Add(Note.A + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType._13s5: // 도 미 솔# 시b 레 파 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType._11s5));
                    stackingNotes.Add(Note.A + Note.OCTAVE_CHROMATIC);
                    break;
                case EStackType._13b5: // 도 미b 솔b시b 레 파 라
                    stackingNotes.AddRange(GetStackingNotes(EStackType._11b5));
                    stackingNotes.Add(Note.A + Note.OCTAVE_CHROMATIC);
                    break;

            }
            return stackingNotes;
        }
    }
}
