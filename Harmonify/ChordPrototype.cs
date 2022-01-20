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
    }
}
