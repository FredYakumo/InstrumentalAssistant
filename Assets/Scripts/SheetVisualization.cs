using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using InstrumentalAssistant.Utils;

namespace InstrumentalAssistant
{
    public class SheetVisualization : MonoBehaviour
    {
        private class SheetPosition
        {
            public Transform transform;
            public GameObject[] upLine;
            public GameObject[] downLine;

            public GameObject upLineParent;
            public GameObject downLineParent;

            public SheetPosition(Transform transform, GameObject upLineParent, GameObject[] upLine, GameObject downLineParent, GameObject[] downLine)
            {
                this.transform = transform;
                this.upLineParent = upLineParent;
                this.upLine = upLine;
                this.downLineParent = downLineParent;
                this.downLine = downLine;
            }
        }




        #region Settings
        [Range(0f, 1f)]
        [SerializeField]
        private float screenWidthPercentage = 1f;
        [Range(0f, 1f)]
        [SerializeField]
        private float noteStartPosWidthPercentage = 0.1f;
        [SerializeField]
        private int m_elementCountPerPage = 8;
        [SerializeField]
        private float m_staffGapSize = 20f;
        [SerializeField]
        private float m_staffLineThickness = 2f;
        [SerializeField]
        private float m_noteElementSize = 20f;
        #endregion

        #region UnityObjectFromInspector
        [SerializeField]
        private Composer m_composer;
        #endregion

        private UILineRenderer[] m_staffLineRenders;

        private SheetPosition[] m_positions;

        /// <summary>
        /// (NoteValue, YPos)
        /// </summary>
        private Queue<Tuple<NoteValue, float>> m_noteQueue = new Queue<Tuple<NoteValue, float>>();
        private List<UICircle> m_noteElements = new List<UICircle>();

        private float m_a1YPos = 0f;

        private Vector2 m_screenSize;

        private static Dictionary<string, float> m_noteYPos = new Dictionary<string, float>();


        private void Start()
        {
            m_screenSize = new Vector2(Screen.width, Screen.height);

            /*
                -11.5f,
                -8f,
                -4.5f,
                -1f,
                3.5f,
                7f,
                10.5f,
                13.5f
            }*/

            m_a1YPos = -12f * m_staffGapSize;

            float startPos = Screen.width * noteStartPosWidthPercentage;
            float endPos = Screen.width * screenWidthPercentage;
            float endNoteElementsPos = Screen.width * (screenWidthPercentage - noteStartPosWidthPercentage);

            m_positions = new SheetPosition[m_elementCountPerPage];

            for (int i = 0; i < m_positions.Length; ++i)
            {
                m_positions[i] = new SheetPosition(new GameObject().transform, 
                    new GameObject(), new GameObject[5], 
                    new GameObject(), new GameObject[9]);

                m_positions[i].transform.name = "position" + i.ToString();
                m_positions[i].transform.position = transform.position + (Vector3.right * (endNoteElementsPos / m_elementCountPerPage * i + startPos));
                m_positions[i].transform.SetParent(transform);


                // down additional line;

                m_positions[i].downLineParent.transform.position = m_positions[i].transform.position;
                m_positions[i].downLineParent.transform.SetParent(m_positions[i].transform);
                m_positions[i].downLineParent.name = "downLine";
                m_positions[i].downLineParent.SetActive(false);


                for (int j = 0; j < m_positions[i].downLine.Length; ++j)
                {
                    m_positions[i].downLine[j] = new GameObject();
                    var line = m_positions[i].downLine[j].AddComponent<UILineRenderer>();
                    line.Points = new Vector2[2]
                    {
                        new Vector2(-m_noteElementSize * 1f, 0),
                        new Vector2(m_noteElementSize * 1f, 0),
                    };

                    line.lineThickness = m_staffLineThickness;
                    line.name = "downLine" + (j + 1).ToString();
                    line.transform.position = m_positions[i].transform.position + Vector3.down * (m_staffGapSize * (3f + j));
                    line.transform.SetParent(m_positions[i].downLineParent.transform);
                }

                // up additional line;

                m_positions[i].upLineParent.transform.position = m_positions[i].transform.position;
                m_positions[i].upLineParent.transform.SetParent(m_positions[i].transform);
                m_positions[i].upLineParent.name = "upLine";
                m_positions[i].upLineParent.SetActive(false);

                for (int j = 0; j < m_positions[i].upLine.Length; ++j)
                {
                    m_positions[i].upLine[j] = new GameObject();
                    var line = m_positions[i].upLine[j].AddComponent<UILineRenderer>();
                    line.Points = new Vector2[2]
                    {
                        new Vector2(-m_noteElementSize * 1f, 0),
                        new Vector2(m_noteElementSize * 1f, 0),
                    };

                    line.lineThickness = m_staffLineThickness;
                    line.name = "upLine" + (j + 1).ToString();
                    line.transform.position = m_positions[i].transform.position + Vector3.up * (m_staffGapSize * (3f + j));
                    line.transform.SetParent(m_positions[i].upLineParent.transform);
                }
            }

            m_staffLineRenders = new UILineRenderer[5];
            float heightMul = -2f;
            for (int i = 0; i < 5; ++i)
            {
                m_staffLineRenders[i] = new GameObject().AddComponent<UILineRenderer>();
                m_staffLineRenders[i].transform.position = transform.position;
                m_staffLineRenders[i].name = "StaffLine" + i.ToString();
                m_staffLineRenders[i].transform.SetParent(transform);

                m_staffLineRenders[i].Points = new Vector2[2]
                {
                    new Vector2(0, m_staffGapSize * heightMul), new Vector2(endPos, m_staffGapSize * heightMul),
                };
                heightMul += 1f;

                m_staffLineRenders[i].lineThickness = m_staffLineThickness;
            }

            m_composer.onNewNote += OnNewNote;
        }

        private void Update()
        {
            if (Screen.width != m_screenSize.x)
            {
                m_screenSize.x = Screen.width;
                ResizeVisualWidth();
            }
        }

        private void OnNewNote(NoteValue noteValue)
        {
            var note = noteValue.note;
            float endPos = m_screenSize.x * screenWidthPercentage;

            m_noteQueue.Enqueue(
                new Tuple<NoteValue, float>(
                noteValue,
                    m_a1YPos + m_staffGapSize + ((note.octave - 1) * m_staffGapSize * 3.5f) + m_noteYPos[note.noteName] * m_staffGapSize / 2f
           ));

            if (m_noteQueue.Count > m_elementCountPerPage)
                m_noteQueue.Dequeue();

            

            for (int i = 0; i < m_noteQueue.Count; ++i)
            {
                if (m_noteElements.Count <= i)
                {
                    UICircle n = new GameObject().AddComponent<UICircle>();
                    n.name = "note" + i.ToString();
                    n.transform.position = m_positions[i].transform.position;
                    n.transform.SetParent(m_positions[i].transform);


                    n.GetComponent<RectTransform>().sizeDelta = new Vector2(m_noteElementSize, m_noteElementSize);

                    var rune = new GameObject().AddComponent<UILineRenderer>();
                    rune.Points = new Vector2[2]
                    {
                        new Vector2(-m_noteElementSize / 2f, 0f),
                        new Vector2(-m_noteElementSize / 2f, -m_staffGapSize * 4f),
                    };

                    rune.lineThickness = m_staffLineThickness * 1f;
                    rune.name = "rune";
                    rune.transform.position = n.transform.position;
                    rune.transform.SetParent(n.transform);


                    m_noteElements.Add(n);
                }

                var currentElement = m_noteQueue.ElementAt(i);

                var pos = m_noteElements[i].transform.position;
                pos.y = transform.position.y + currentElement.Item2;
                m_noteElements[i].transform.position = pos;

                if (pos.y > transform.position.y)
                {
                    m_positions[i].upLineParent.SetActive(true);
                    m_positions[i].downLineParent.SetActive(false);

                    foreach (var e in m_positions[i].upLine)
                        e.SetActive(e.transform.position.y <= pos.y);
                }
                else if (pos.y < transform.position.y)
                {
                    m_positions[i].downLineParent.SetActive(true);
                    m_positions[i].upLineParent.SetActive(false);

                    foreach (var e in m_positions[i].downLine)
                        e.SetActive(e.transform.position.y >= pos.y);
                }
                else
                {
                    m_positions[i].downLineParent.SetActive(false);
                    m_positions[i].upLineParent.SetActive(false);
                }

            }
        }

        private void ResizeVisualWidth()
        {
            float endPos = Screen.width * screenWidthPercentage;

            float heightMul = -2f;
            for (int i = 0; i < 5; ++i)
            {
                m_staffLineRenders[i].Points = new Vector2[2]
                {
                    new Vector2(0, m_staffGapSize * heightMul), new Vector2(endPos, m_staffGapSize * heightMul),
                };

                heightMul += 1f;

                m_staffLineRenders[i].SetAllDirty();
            }

            float startPos = Screen.width * noteStartPosWidthPercentage;
            float endNoteElementsPos = Screen.width * (screenWidthPercentage - noteStartPosWidthPercentage);

            for (int i = 0; i < m_positions.Length; ++i)
            {
                m_positions[i].transform.position = transform.position + (Vector3.right * (endNoteElementsPos / m_elementCountPerPage * i + startPos));
            }
        }

        static SheetVisualization()
        {
            m_noteYPos.Add("A", 0f);
            m_noteYPos.Add("A#", 0f);
            m_noteYPos.Add("B", 1f);
            m_noteYPos.Add("C", 2f);
            m_noteYPos.Add("C#", 2f);
            m_noteYPos.Add("D", 3f);
            m_noteYPos.Add("Eb", 3f);
            m_noteYPos.Add("E", 4f);
            m_noteYPos.Add("F", 5f);
            m_noteYPos.Add("F#", 5f);
            m_noteYPos.Add("G", 6f);
            m_noteYPos.Add("Ab", 6f);
        }
    }
}
