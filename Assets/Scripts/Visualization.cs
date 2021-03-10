using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace InstrumentalAssistant
{
    public class Visualization : MonoBehaviour
    {
        public float maxScale = 100f;

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

            for (uint i = 0; i < m_elements.Length; ++i)
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
            for (uint i = 0; i < m_elements.Length; ++i)
            {
                m_elements[i].localScale = new Vector3(1f, m_pitchDetector.spectrums[i] * maxScale + 2, 0f);
            }

            m_outputPitchValue.text = $"Pitch = {m_pitchDetector.pitch.ToString("F2")} Hz";
        }
    }
}
