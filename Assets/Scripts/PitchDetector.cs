using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InstrumentalAssistant
{
    [RequireComponent(typeof(AudioSource))]
    public class PitchDetector : MonoBehaviour
    {
        public int sampleSize
        {
            get
            {
                return m_sampleSize;
            }
        }

        public int binSize
        {
            get
            {
                return m_binSize;
            }
        }

        public float[] samples
        {
            get
            {
                return m_samples;
            }
        }

        public float[] spectrums
        {
            get
            {
                return m_spectrum;
            }
        }

        /// <summary>
        /// Square root of samples squared sum.
        /// </summary>
        public float rms
        {
            get;
            private set;
        }

        public float volume
        {
            get;
            private set;
        }

        public float pitch
        {
            get
            {
                return m_pitch;
            }
        }

        private struct Peak
        {
            public float amplitude;
            public int index;

            public Peak(float _frequency = 0f, int _index = -1)
            {
                amplitude = _frequency;
                index = _index;
            }
        }

        private class AmpComparer : IComparer<Peak>
        {
            public int Compare(Peak a, Peak b)
            {
                return 0 - a.amplitude.CompareTo(b.amplitude);
            }
        }

        private class IndexComparer : IComparer<Peak>
        {
            public int Compare(Peak a, Peak b)
            {
                return a.index.CompareTo(b.index);
            }
        }


        [SerializeField]
        private int m_sampleSize = 512;
        [SerializeField]
        private int m_binSize = 512;
        [SerializeField]
        private float[] m_samples;
        [SerializeField]
        private float[] m_spectrum;
        [SerializeField]
        private float m_pitch;

        private float m_refValue = 0.1f;
        private float m_threshold = 0.01f;

        private int m_samplerate;

        private List<Peak> peaks = new List<Peak>();

        #region UnityObjects
        private AudioSource m_audioSource;
        #endregion

        private void Start()
        {
            m_audioSource = GetComponent<AudioSource>();
            m_samples = new float[sampleSize];
            m_spectrum = new float[binSize];
            m_samplerate = AudioSettings.outputSampleRate;
        }

        private void Update()
        {
            PitchDetection();
        }

        private void PitchDetection()
        {
            m_audioSource.GetOutputData(samples, 0);


            // Calculate sum of squared samples
            float sum = 0f;
            for (int i = 0; i < sampleSize; ++i)
                sum += samples[i];

            rms = Mathf.Sqrt(sum / sampleSize);

            volume = Mathf.Clamp(20 + Mathf.Log10(rms / m_refValue), -160f, Mathf.Infinity);

            m_audioSource.GetSpectrumData(m_spectrum, 0, FFTWindow.BlackmanHarris);

            // Find max value
            float max = 0f;
            for (int i = 0; i < binSize; ++i)
            {
                if (spectrums[i] > max && spectrums[i] > m_threshold)
                {
                    peaks.Add(new Peak(spectrums[i], i));
                }
            }

            // Sort from hightest to lowest
            if (peaks.Count > 5)
                peaks.Sort(new AmpComparer());

            float freq = 0f;
            if (peaks.Count > 0)
            {
                max = peaks[0].amplitude;
                int maxIndex = peaks[0].index;

                // Pass the index to a float variable
                freq = maxIndex;

                if (maxIndex > 0 && maxIndex < binSize - 1)
                {
                    // interpolate index using neighbours
                    float dL = spectrums[maxIndex - 1] / spectrums[maxIndex];
                    float dR = spectrums[maxIndex + 1] / spectrums[maxIndex];
                    freq += 0.5f * (dR * dR - dL * dL);
                }
            }

            // Convert index to frequency
            m_pitch = freq * (m_samplerate / 2f) / binSize;
            peaks.Clear();
        }
    }
}
