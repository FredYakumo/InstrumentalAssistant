using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace InstrumentalAssistant
{
    public class MicrophoneManager : MonoBehaviour
    {
        #region UnityObjectFromInspector
        [SerializeField]
        private AudioSource m_audioSource;
        /*
        [SerializeField]
        private AudioMixer m_masterMixer;
        */
        #endregion


        private void Start()
        {
            if (m_audioSource != null)
            {
                m_audioSource.clip = Microphone.Start(Microphone.devices[0], true, 10, AudioSettings.outputSampleRate);
                m_audioSource.loop = true;
                /*
                if (m_masterMixer != null)
                    m_masterMixer.SetFloat("masterVolume", -80f);
                */
                StartCoroutine(WaitForMicrophoneStart());
            }
        }

        private IEnumerator WaitForMicrophoneStart()
        {
            while (!(Microphone.GetPosition(null) > 0))
                yield return new WaitForFixedUpdate();
            m_audioSource.Play();
        }
    }
}
