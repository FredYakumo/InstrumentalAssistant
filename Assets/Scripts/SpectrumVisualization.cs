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
        public bool stayMode = true;
        public float stayTime = 2f;

        [Range(0f, 1f)]
        public float screenWidthPercentage = 100f;


        public float maxYScale = 3f;

        private Vector2 m_screenSize = new Vector2();

        private RectTransform[] m_elements;

        private float m_stayTimeCounter = 0f;


        #region Settings
        private int m_displaySize = 1024;
        #endregion


        #region UnityObjectFromInspector
        [SerializeField]
        private RectTransform m_elementPrefab;
        [SerializeField]
        private PitchDetector m_pitchDetector;
        #endregion

        private void Start()
        {
            m_elements = new RectTransform[m_displaySize];

            float horEndPos = Screen.width * screenWidthPercentage;

            for (int i = 0; i < m_elements.Length; ++i)
            {
                RectTransform rectTransform = Instantiate(m_elementPrefab);
                rectTransform.position = transform.position + Vector3.right * horEndPos * ((float)i / m_elements.Length);
                rectTransform.SetParent(transform);
                rectTransform.name = $"SampleElement[{i}]";
                m_elements[i] = rectTransform;
            }
        }

        private void ResizeVisualWidth()
        {
            float horEndPos = Screen.width * screenWidthPercentage;
            for (int i = 0; i < m_elements.Length; ++i)
            {
                m_elements[i].position = transform.position + Vector3.right * horEndPos * ((float)i / m_elements.Length);
            }
        }

        private void Update()
        {
            if (m_screenSize.x != Screen.width)
            {
                m_screenSize.x = Screen.width;
                ResizeVisualWidth();
            }

            if (stayMode)
            {
                if (m_pitchDetector.pitch != 0f)
                    m_stayTimeCounter = -2f;
                else
                {
                    // pitch = 0, timecounter = -2f
                    if (m_stayTimeCounter == -2f)
                    {
                        m_stayTimeCounter = 0f;
                        return;
                    }

                    if ((m_stayTimeCounter += Time.deltaTime) < stayTime)
                    {
                        return;
                    }

                    m_stayTimeCounter = -2f;
                }
            }

            for (int i = 0; i < m_elements.Length; ++i)
            {
                m_elements[i].localScale = new Vector3(1f, m_pitchDetector.spectrums[i] * maxYScale + 2, 0f);
            }

        }
    }
}
