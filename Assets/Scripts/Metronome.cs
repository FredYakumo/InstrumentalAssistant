using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace InstrumentalAssistant
{
    [RequireComponent(typeof(AudioSource))]
    public class Metronome : MonoBehaviour
    {
        private static int MaxBpm = 500;
        [Range(1, 500)]
        public int Bpm = 60;

        public bool Playable = false;

        #region UnityObjectFromInspector
        [SerializeField]
        private Text m_beatText;
        [SerializeField]
        private Button m_playButton;
        private Text m_playButtonText;
        #endregion

        private float timeInterval
        {
            get
            {
                return 60f / Bpm;
            }
        }

        private float m_timeCounter = 0f;

        private AudioSource m_audioSource;

        public void MetronomeStart()
        {
            Playable = true;
            m_playButtonText.text = "□";
        }

        public void MetronomeEnd()
        {
            Playable = false;
            m_playButtonText.text = "▶";
        }

        public void PlayStateSwitch()
        {
            if (Playable)
                MetronomeEnd();
            else
                MetronomeStart();
        }

        public void BpmAdjust(int v)
        {
            Bpm = Mathf.Clamp(Bpm + v, 1, MaxBpm);
            m_beatText.text = $"BPM: {Bpm}";
        }

        private void Start()
        {
            m_beatText.text = $"BPM: {Bpm}";
            m_audioSource = GetComponent<AudioSource>();
            m_audioSource.loop = false;
            m_playButtonText = m_playButton.transform.Find("Text").GetComponent<Text>();
        }

        private void Update()
        {
            if (!Playable)
                return;

            while ((m_timeCounter += Time.deltaTime) >= timeInterval)
            {
                m_timeCounter = 0f;
                m_audioSource.Stop();
                m_audioSource.Play();
            }
        }
    }
}
