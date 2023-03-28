using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    public class CoroutineManager : TMonoSingleton<CoroutineManager>
    {
        private Dictionary<string, Coroutine> m_AllCoroutines = new Dictionary<string, Coroutine>();

        public bool IsCoroutineRunning(string name)
        {
            return m_AllCoroutines.ContainsKey(name);
        }

        public void StartMCoroutine(string name, IEnumerator routine)
        {
            if (IsCoroutineRunning(name))
            {
                StopCoroutine(name);
            }

            m_AllCoroutines[name] = StartCoroutine(routine);
        }

        public void StopMCoroutine(string name)
        {
            if (IsCoroutineRunning(name))
            {
                StopCoroutine(m_AllCoroutines[name]);
                m_AllCoroutines.Remove(name);
            }
        }

        public void StopAllMCoroutines()
        {
            foreach (var coroutine in m_AllCoroutines.Values)
            {
                StopCoroutine(coroutine);
            }

            m_AllCoroutines.Clear();
        }

        public void PauseMCoroutine(string name)
        {
            if (IsCoroutineRunning(name))
            {
                StopCoroutine(m_AllCoroutines[name]);
            }
        }

        public void ResumeMCoroutine(string name)
        {
            if (IsCoroutineRunning(name))
            {
                StartCoroutine(name);
            }
        }
    }

}

