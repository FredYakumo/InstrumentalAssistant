using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InstrumentalAssistant.Utils
{
    public class EqualTemperamentNote
    {
        /// <summary>
        /// Default: A = 440 Hz
        /// </summary>
        public static float standardPitch = 440f;
        public static float standardA1Pitch
        {
            get
            {
                return standardPitch / 16f;
            }
        }

        public string noteName { get; private set; }

        /// <summary>
        /// Standard A octave = 4
        /// </summary>
        public int octave { get; private set; }
        public float frequency { get; set; }
        /// <summary>
        /// The deviation from the correct pitch, 
        /// positive if freq is higher then correct freq, otherwise it is negative
        /// </summary>
        public float deviationFreq { get; private set; }

        public static List<string> temperamentName { get; private set; } = new List<string>();

        public static List<float> GetOctaveFreqs(int octave = 4)
        {
            List<float> oct = new List<float>();
            for (int i = 0; i < 12; ++i)
                oct.Add(standardA1Pitch * Mathf.Pow(2, octave) * Mathf.Pow(2, i / 12f));
            return oct;
        }

        public EqualTemperamentNote(float freq)
        {
            octave = Mathf.FloorToInt(Mathf.Sqrt(Mathf.Floor(freq / standardA1Pitch)));

            // Find near
            var octs = GetOctaveFreqs(octave);
            deviationFreq = Mathf.Infinity;
            int index = 0;
            for (int i = 0; i < 12; ++i)
            {
                float d = Mathf.Abs(octs[i] - freq);
                if (d < deviationFreq)
                {
                    index = i;
                    deviationFreq = freq - octs[i];
                }
            }

            noteName = temperamentName[index];

            // Fixed neighbor octaves
            if (octave > 1)
            {
                float dPrev = standardA1Pitch * Mathf.Pow(2, octave - 1) * Mathf.Pow(2, 11f / 12f) - freq;
                float dNext = standardA1Pitch * Mathf.Pow(2, octave + 1) - freq;

                if (dPrev < dNext)
                {
                    if (Mathf.Abs(dPrev) < deviationFreq)
                    {
                        octave -= 1;
                        deviationFreq = -dPrev;
                        noteName = temperamentName[11];
                    }
                }
                else
                {
                    if (Mathf.Abs(dNext) < deviationFreq)
                    {
                        octave += 1;
                        deviationFreq = -dNext;
                        noteName = temperamentName[0];
                    }
                }
            }

        }

        static EqualTemperamentNote()
        {
            temperamentName.Add("A");
            temperamentName.Add("A#");
            temperamentName.Add("B");
            temperamentName.Add("C");
            temperamentName.Add("C#");
            temperamentName.Add("D");
            temperamentName.Add("Eb");
            temperamentName.Add("E");
            temperamentName.Add("F");
            temperamentName.Add("F#");
            temperamentName.Add("G");
            temperamentName.Add("Ab");
        }
    }
}
