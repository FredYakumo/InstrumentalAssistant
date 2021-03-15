using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using InstrumentalAssistant.Utils;
using UnityEngine.UI.Extensions;

namespace InstrumentalAssistant
{
    public class PitchVisualization : MonoBehaviour
    {
        public bool stayMode = true;
        public float stayTime = 2f;

        [Range(0f, 1f)]
        public float screenWidthPercentage = 1f;

        public float maxYScale = 5f;


        private Queue<float> m_elementYPosQueue = new Queue<float>();

        private Vector2 m_screenSize = new Vector2();

        private float m_stayTimeCounter = 0f;


        #region UnityObjectFromInspector
        [SerializeField]
        private int m_elementCount = 500;
        [SerializeField]
        private PitchDetector m_pitchDetector;
        
        [SerializeField]
        private UILineRenderer m_uiLineRenderer;
        [SerializeField]
        private UILineRenderer m_correctPitchLine;
        [SerializeField]
        private float m_lineTickness = 5f;

        #endregion

        private void Start()
        {
            m_screenSize = new Vector2(Screen.width, Screen.height);
            float midHorizontalPos = Screen.width * screenWidthPercentage * 0.5f;

            m_uiLineRenderer.Points = new Vector2[m_elementCount * 2];
            for (int i = 0; i < m_elementCount * 2; ++i)
            {
                m_uiLineRenderer.Points[i] = new Vector2(midHorizontalPos * i / m_elementCount, 0f);
                m_elementYPosQueue.Enqueue(0f);
            }

            m_correctPitchLine.Points = new Vector2[2];
            m_correctPitchLine.Points[0] = new Vector2(0, 0);
            m_correctPitchLine.Points[1] = new Vector2(midHorizontalPos * 2f, 0);

            m_uiLineRenderer.lineThickness = m_lineTickness;
            m_correctPitchLine.lineThickness = m_lineTickness;
        }

        private void ResizeVisualWidth()
        {
            float midHorizontalPos = Screen.width * screenWidthPercentage * 0.5f;

            for (int i = 0; i < m_elementCount * 2; ++i)
            {
                Vector2 pos = m_uiLineRenderer.Points[i];
                pos.x = midHorizontalPos * i / m_elementCount;
                m_uiLineRenderer.Points[i] = pos;
            }

            m_correctPitchLine.Points[1] = new Vector2(midHorizontalPos * 2f, 0);

            m_uiLineRenderer.SetAllDirty();
            m_correctPitchLine.SetAllDirty();
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

            for (int i = 0; i < m_elementCount * 2; ++i)
            {
                Vector2 pos = m_uiLineRenderer.Points[i];
                pos.y = m_elementYPosQueue.ElementAt(i);
                m_uiLineRenderer.Points[i] = pos;
            }

            m_uiLineRenderer.SetAllDirty();

            m_elementYPosQueue.Dequeue();
            m_elementYPosQueue.Enqueue(m_pitchDetector.note.deviationFreq * maxYScale);
        }
    }
}
