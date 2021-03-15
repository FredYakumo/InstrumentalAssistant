using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using InstrumentalAssistant.Utils;

namespace InstrumentalAssistant
{
    public class SpectrumVisualization : MonoBehaviour
    {
        public float maxScale = 100f;
        public bool stayMode = true;

        private bool hasResult = false;

        private RectTransform[] m_elements;
        [SerializeField]
        private float m_elementInterval = 1f;

        #region UnityObjectFromInspector
        [SerializeField]
        private RectTransform m_elementPrefab;
        [SerializeField]
        private PitchDetector m_pitchDetector;
        [SerializeField]
        private Text m_outputPitchValue;
        #endregion

        private void Start()
        {
            m_elements = new RectTransform[m_pitchDetector.binSize];

            for (int i = 0; i < m_elements.Length; ++i)
            {
                RectTransform rectTransform = Instantiate(m_elementPrefab);
                rectTransform.position = transform.position + Vector3.right * i * m_elementInterval;
                rectTransform.SetParent(transform);
                rectTransform.name = $"SampleElement[{i}]";
                m_elements[i] = rectTransform;
            }
        }

        private void Update()
        {
            if (stayMode)
            {
                if (hasResult && m_pitchDetector.pitch == 0f)
                    return;
            }

            for (int i = 0; i < m_elements.Length; ++i)
            {
                m_elements[i].localScale = new Vector3(1f, m_pitchDetector.spectrums[i] * maxScale + 2, 0f);
            }

            var equal = new EqualTemperamentNote(m_pitchDetector.pitch);
            m_outputPitchValue.text = $"Note: {equal.noteName}{equal.octave} ({equal.deviationFreq.ToString("F2")})" +
                $" Freq = {m_pitchDetector.pitch.ToString("F2")} Hz";

            hasResult = true;
        }
    }
}
