using System;
using System.Collections.Generic;
using System.Text;

namespace Harmonify
{
    public enum ChordFunction { Tonic, SuperTonic, Mediant, SubDominant, Dominant, SubMediant, Leading}
    public class ChordFunction
    {
        public int root;

        private static List<ChordStack> tonicForMaj = new List<ChordStack>();
        private static List<ChordStack> tonicForMin = new List<ChordStack>();
        private static List<ChordStack> superTonicForMaj = new List<ChordStack>();
        private static List<ChordStack> superTonicForMin = new List<ChordStack>();
        private static List<ChordStack> mediantForMaj = new List<ChordStack>();
        private static List<ChordStack> mediantForMin = new List<ChordStack>();
        private static List<ChordStack> subDominantForMaj = new List<ChordStack>();
        private static List<ChordStack> subDominantForMin = new List<ChordStack>();
        private static List<ChordStack> dominantForMaj = new List<ChordStack>();
        private static List<ChordStack> dominantForMin = new List<ChordStack>();
        private static List<ChordStack> subMediantForMaj = new List<ChordStack>();
        private static List<ChordStack> subMediantForMin = new List<ChordStack>();
        private static List<ChordStack> leadingForMaj = new List<ChordStack>();
        private static List<ChordStack> leadingForMin = new List<ChordStack>();


        private static List<ChordStack> dimSuperTonic = new List<ChordStack>();
        static ChordFunction()
        {
            // 메이저 키
            // 토닉 
            tonicForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.maj));
            tonicForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.maj7));
            tonicForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.maj9));

            // 슈퍼토닉
            superTonicForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.min));
            superTonicForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.min7));
            superTonicForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.min9));
            superTonicForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.min11));

            // 메디안트
            mediantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.min));
            mediantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.min7));
            mediantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.min11));
            mediantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.min13));

            // 서브도미넌트
            subDominantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.maj));
            subDominantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.maj7));
            subDominantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.maj9));

            // 도미넌트
            dominantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.maj));
            dominantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType._7));
            dominantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType._9));
            dominantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.b9));
            dominantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.s9));
            dominantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.s11));
            dominantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType._11));
            dominantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType._13));
            dominantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType._13s11));

            // 서브메디안트
            subMediantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.min));
            subMediantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.min7));
            subMediantForMaj.Add(ChordPrototype.GetChordStackOf(EStackType.min9));
            
            // 리딩톤은 일단 안 넣음.


            // 마이너 키
            // 토닉
            tonicForMin.Add(ChordPrototype.GetChordStackOf(EStackType.min));
            tonicForMin.Add(ChordPrototype.GetChordStackOf(EStackType.min7));
            tonicForMin.Add(ChordPrototype.GetChordStackOf(EStackType.min9));
            // 수퍼토닉
            tonicForMin.Add(ChordPrototype.GetChordStackOf(EStackType.dim));
            tonicForMin.Add(ChordPrototype.GetChordStackOf(EStackType.m7b5));
            tonicForMin.Add(ChordPrototype.GetChordStackOf(EStackType.min));
            tonicForMin.Add(ChordPrototype.GetChordStackOf(EStackType.min7));
            tonicForMin.Add(ChordPrototype.GetChordStackOf(EStackType.min9));
            tonicForMin.Add(ChordPrototype.GetChordStackOf(EStackType.mb9b5));
            // 메디안트
            mediantForMin.Add(ChordPrototype.GetChordStackOf(EStackType.maj));
            mediantForMin.Add(ChordPrototype.GetChordStackOf(EStackType.maj7));
            mediantForMin.Add(ChordPrototype.GetChordStackOf(EStackType.maj9));
            // 서브도미넌트
            subDominantForMin.Add(ChordPrototype.GetChordStackOf(EStackType.min));
            subDominantForMin.Add(ChordPrototype.GetChordStackOf(EStackType.min7));
            subDominantForMin.Add(ChordPrototype.GetChordStackOf(EStackType.min9));
            subDominantForMin.Add(ChordPrototype.GetChordStackOf(EStackType.min11));
            subDominantForMin.Add(ChordPrototype.GetChordStackOf(EStackType.min6));
            // 도미넌트
            dominantForMin.Add(ChordPrototype.GetChordStackOf(EStackType.maj));
            dominantForMin.Add(ChordPrototype.GetChordStackOf(EStackType._7));
            dominantForMin.Add(ChordPrototype.GetChordStackOf(EStackType.b9));
            dominantForMin.Add(ChordPrototype.GetChordStackOf(EStackType.s9));
            dominantForMin.Add(ChordPrototype.GetChordStackOf(EStackType.b13));
            // 서브메디안트
            subMediantForMin.Add(ChordPrototype.GetChordStackOf(EStackType.maj));
            subMediantForMin.Add(ChordPrototype.GetChordStackOf(EStackType.maj7));
            subMediantForMin.Add(ChordPrototype.GetChordStackOf(EStackType.min9));
        }
        public List<ChordStack> GetAvailableChordStacks(ChordFunction chordFunction, bool major)
        {
            switch (chordFunction)
            {
                case ChordFunction.Tonic:
                    if (major)
                    {
                        return tonicForMaj;
                    }
                    else
                    {
                        return tonicForMin;
                    }
                case ChordFunction.SuperTonic:
                    if(major)
                    {
                        return superTonicForMaj;
                    }
                    else
                    {
                        return superTonicForMin;
                    }
                case ChordFunction.Mediant:
                    if (major)
                    {
                        return mediantForMaj;
                    }
                    else
                    {
                        return mediantForMin;
                    }
                case ChordFunction.SubDominant:
                    if (major)
                    {
                        return subDominantForMaj;
                    }
                    else
                    {
                        return subDominantForMin;
                    }
                case ChordFunction.Dominant:
                    if (major)
                    {
                        return dominantForMaj;
                    }
                    else
                    {
                        return dominantForMin;
                    }
                case ChordFunction.SubMediant:
                    if (major)
                    {
                        return subMediantForMaj;
                    }
                    else
                    {
                        return subMediantForMin;
                    }
                default:
                    return null;
            }
        }
    }
}
