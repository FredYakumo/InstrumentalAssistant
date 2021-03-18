using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InstrumentalAssistant.Utils;

namespace InstrumentalAssistant
{
    public class Composer : MonoBehaviour
    {
        public Action<NoteValue> onNewNote = null;
        public Action<NoteValue> onCombindNote = null;

        public float minNewNoteDeltaTime = 0.2f;
        public float minNewNotePitch = 6f;
        public float minNewNoteVolume = 10f;

        public float m_timethreshold = 0.1f;

        public int bpm
        {
            get
            {
                if (m_musicSheet.Count <= 1)
                    return 0;

                return Mathf.RoundToInt(60f * m_musicSheet.Count / (m_musicSheet[m_musicSheet.Count - 1].startTime - m_musicSheet[0].startTime));
            }
        }

        #region UnityObjectFromInspector
        [SerializeField]
        private PitchDetector m_pitchDetector;
        #endregion

        /// <summary>
        /// Tuple: (Note, duration, startTime)
        /// </summary>
        private List<NoteValue> m_musicSheet = new List<NoteValue>();

        private NoteValue m_onProcessNote = null;

        private float m_lastVailedTimePoint = 0f;


        private void Start()
        {
            onNewNote += (NoteValue n) => Debug.Log($"New note: {n.note.noteName}{n.note.octave} - duration: {n.duration}, startTime: {n.startTime}");
        }

        private bool MinConditionCheck()
        {
            if (m_pitchDetector.pitch < minNewNotePitch)
                return false;
            if (m_pitchDetector.volume < minNewNoteVolume)
                return false;
            return true;
        }

        private void FixedUpdate()
        {
            Debug.Log($"BPM: {bpm}");
            var note = m_pitchDetector.note;
            if (m_onProcessNote == null)
            {
                if (!MinConditionCheck())
                    return;
                m_lastVailedTimePoint = Time.fixedTime;
                m_onProcessNote = new NoteValue(note, 0f, Time.fixedTime);
                return;
            }

            if (note.noteName ==  m_onProcessNote.note.noteName && note.octave == m_onProcessNote.note.octave)
            {
                if (!MinConditionCheck())
                    return;
                m_lastVailedTimePoint = Time.fixedTime;
                m_onProcessNote.duration += Time.fixedDeltaTime;
            }

            // Not the same note ( on process note go to end...)
            else
            {
                if (Time.fixedTime - m_lastVailedTimePoint < m_timethreshold)
                    return;

                if (!MinConditionCheck())
                    return;

                if (m_musicSheet.Count > 0)
                {
                    for (int i = m_musicSheet.Count - 1; i >= 0; --i)
                    {
                        var noteValue = m_musicSheet[i];
                        var n = noteValue.note;

                        if (n.noteName == note.noteName && n.octave == note.octave)
                            // Find recent same note
                            // process as note continue
                            if (m_onProcessNote.startTime - (noteValue.startTime + noteValue.duration) < minNewNoteDeltaTime)
                            {
                                m_musicSheet[i].duration += m_onProcessNote.duration;
                                onCombindNote?.Invoke(m_onProcessNote);
                                m_onProcessNote = null;
                                return;
                            }
                    }
                }

                m_musicSheet.Add(m_onProcessNote);
                onNewNote?.Invoke(m_onProcessNote);
                m_onProcessNote = null;
            }
            
        }
    }
}
