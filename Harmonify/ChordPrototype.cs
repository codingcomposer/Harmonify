using System;
using System.Collections.Generic;
using System.Text;

namespace Harmonify
{
    public enum EStackType
    {
        maj, // 도 미 솔
        min, // 도 미b 솔
        aug, // 도 미 솔#
        dim, // 도 미b 솔b
        sus, // 도 파 솔
        maj7, // 도 미 솔 시
        _7, // 도 미 솔 시b
        min7, // 도 미b 솔 시b
        mM7, // 도 미b 솔 시
        dim7, // 도 미b 솔b 시bb
        dimMaj7, // 도 미b 솔b 시
        m7b5, // 도 미b 솔b 시b
        aug7, // 도 미 솔# 시b
        maj7s5, // 도 미 솔# 시
        _7sus4, // 도 파 솔 시b
        maj6, // 도 미 솔 라
        min6, // 도 미b 솔 라
        maj9, // 도 미 솔 시 레
        _9, // 도 미 솔 시b 레
        s9, // 도 미 솔 시b 레#
        b9, // 도 미 솔 시b 레b
        min9, // 도 미b 솔 시b 레
        augMaj9, // 도 미 솔# 시 레
        aug9, // 도 미 솔# 시b 레
        m9b5, // 도 미b 솔b 시b 레
        mb9b5, // 도 미b 솔b 시b 레b
        dim9, // 도 미b 솔b 시bb 레
        dimb9, // 도 미b 솔b 시bb 레b
        maj11, // 도 미 솔 시 레 파
        _11, // 도 미 솔 시b 레 파
        s11, // 도 미 솔 시b 레 파#
        min11, // 도 미b 솔 시b 레 파
        augMaj11, // 도 미 솔# 시 레 파
        _11s5, // 도 미 솔# 시b 레 파
        _11b5, // 도 미 솔b 시b 레 파
        dim11, // 도 미b 솔b 시bb 레 파
        maj13, // 도 미 솔 시 레 파 라
        _13, // 도 미 솔 시b 레 파 라
        _13s11, // 도 미 솔 시b 레 파# 라
        b13, // 도 미 솔 시b 레 파 라b
        b13s11, // 도 미 솔 시b 레 파# 라b
        min13, // 도 미b 솔 시b 레 파 라
        augmaj13, // 도 미 솔# 시 레 파 라
        _13s5, // 도 미 솔# 시b 레 파 라
        _13b5, // 도 미b 솔b시b 레 파 라

    };
    public class ChordPrototype
    {
        public static List<ChordStack> chordStacks = new List<ChordStack>();
        static ChordPrototype()
        {
            foreach (EStackType eStackType in Enum.GetValues(typeof(EStackType)))
            {
                chordStacks.Add(new ChordStack(eStackType));
            }
        }

        public static ChordStack GetChordStackOf(EStackType eStackType)
        {
            for(int i = 0; i < chordStacks.Count; i++)
            {
                if (chordStacks[i].EStackType.Equals(eStackType))
                {
                    return chordStacks[i];
                }
            }
            return null;
        }

        public static string GetStackNotation(EStackType eStackType)
        {
            return eStackType switch
            {
                EStackType.maj => "",
                EStackType.min => "m",
                EStackType.aug => "aug",
                EStackType.dim => "dim",
                EStackType.sus => "sus4",
                EStackType.maj7 => "M7",
                EStackType._7 => "7",
                EStackType.min7 => "m7",
                EStackType.mM7 => "mM7",
                EStackType.dim7 => "dim7",
                EStackType.dimMaj7 => "dimM7",
                EStackType.m7b5 => "m7♭5",
                EStackType.aug7 => "aug7",
                EStackType.maj7s5 => "M7(#5)",
                EStackType._7sus4 => "7sus4",
                EStackType.maj6 => "(add6)",
                EStackType.min6 => "m(add6)",
                EStackType.maj9 => "M9",
                EStackType._9 => "9",
                EStackType.s9 => "#9",
                EStackType.b9 => "♭9",
                EStackType.min9 => "m9",
                EStackType.augMaj9 => "augM9",
                EStackType.aug9 => "aug9",
                EStackType.m9b5 => "m9(♭5)",
                EStackType.dim9 => "dim9",
                EStackType.dimb9 => "dim(♭9)",
                EStackType.maj11 => "M11",
                EStackType._11 => "11",
                EStackType.s11 => "9#11",
                EStackType.min11 => "m11",
                EStackType.augMaj11 => "augM11",
                EStackType._11s5 => "11(#5)",
                EStackType._11b5 => "11(♭5)",
                EStackType.dim11 => "dim11",
                EStackType.maj13 => "M13",
                EStackType._13 => "13",
                EStackType._13s11 => "13(#11)",
                EStackType.b13 => "♭13",
                EStackType.b13s11 => "♭13#11",
                EStackType.min13 => "m13",
                EStackType.augmaj13 => "augM13",
                EStackType._13s5 => "13(#5)",
                EStackType._13b5 => "13(♭5)",
                EStackType.mb9b5 => "m7(♭9♭5)",
                _ => eStackType.ToString(),
            };
        }
    }
}
