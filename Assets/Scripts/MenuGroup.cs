using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InstrumentalAssistant
{
    public class MenuGroup : MonoBehaviour
    {
        #region UnityObjectsFromInspector
        [SerializeField]
        private GameObject m_turnerGroup;
        [SerializeField]
        private GameObject m_metronomeGroup;
        #endregion

        public void ShowTurnerGroup(bool v)
        {
            m_turnerGroup.SetActive(v);
        }

        public void ShowMetronomeGroup(bool v)
        {
            m_metronomeGroup.SetActive(v);
        }

        private void Start()
        {
            ShowTurnerGroup(true);
            ShowMetronomeGroup(false);
        }
    }
}
