using System;
using System.Collections.Generic;
using System.Text;
using System.Media;

namespace Harmonify
{
    public class PlaySound
    {
        private static SoundPlayer[] chordPlayers = new SoundPlayer[12];
        private static SoundPlayer[] melodyPlayers = new SoundPlayer[48];
        private static string directoryPath = @"C:\Users\Overclock-1\Documents\SoundFiles\";
        private static string extension = ".wav";
        private static int melodyOffset = 36;
        static PlaySound()
        {
            for (int i = 0; i < chordPlayers.Length; i++)
            {
                chordPlayers[i] = Sound(i.ToString());
            }
            for(int j = 0;j < melodyPlayers.Length; j++)
            {
                melodyPlayers[j] = Sound((melodyOffset + j).ToString());
            }
        }
        public static void PlaySection(Section section)
        {
            for(int i = 0; i < section.measures.Count; i++)
            {
                for(int j = 0; j < section.measures[i].chords.Count; j++)
                {
                    PlayChordNote(section.measures[i].chords[j].chordNotes);
                    
                }
            }
        }

        private static void PlayMelodyNote(int note)
        {
            melodyPlayers[note - melodyOffset].PlaySync();            
        }

        private static void PlayChordNote(List<int> chordNotes)
        {
            for(int i = 0; i < chordNotes.Count; i++)
            {
                chordPlayers[i].PlaySync();
            }
        }

        private static SoundPlayer Sound(string fileName)
        {
            return new SoundPlayer(directoryPath + fileName + extension);
        }
    }
}
