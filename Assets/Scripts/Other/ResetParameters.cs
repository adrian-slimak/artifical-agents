using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UPC
{
    [Serializable]
    public class ResetParameters : ISerializationCallbackReceiver
    {
        [HideInInspector]
        public Dictionary<string, float> m_DefaultResetParameters;
        Dictionary<string, float> m_CustomResetParameters;

        public ResetParameters()
        {
            m_DefaultResetParameters = new Dictionary<string, float>();
            m_CustomResetParameters = new Dictionary<string, float>();
        }


        public float? this[string name]
        {
            set { m_DefaultResetParameters[name] = (float)value; }
            get
            {
                if (m_CustomResetParameters.ContainsKey(name))
                    return m_CustomResetParameters[name];
                else
                if (m_DefaultResetParameters.ContainsKey(name))
                    return m_DefaultResetParameters[name];
                else
                    Debug.LogWarning($"Reset parameter '{name}' not found.");
                return null;
            }
        }

        public void SetCustomParameters(Dictionary<string, float> customParameters)
        {
            m_CustomResetParameters = customParameters;
        }

        void UpdateResetParameters()
        {
            m_ResetParametersList.Clear();
            foreach (var parameter in m_DefaultResetParameters)
            {
                m_ResetParametersList.Add(new ResetParameter { key = parameter.Key, value = parameter.Value });
            }
        }

        [FormerlySerializedAs("ResetParameters")]
        [SerializeField]
        List<ResetParameter> m_ResetParametersList = new List<ResetParameter>();

        [Serializable]
        public struct ResetParameter
        {
            public string key;
            public float value;
        }

        public void OnBeforeSerialize()
        {
            UpdateResetParameters();
        }

        public void OnAfterDeserialize()
        {
            m_DefaultResetParameters.Clear();


            for (var i = 0; i < m_ResetParametersList.Count; i++)
            {
                if (m_DefaultResetParameters.ContainsKey(m_ResetParametersList[i].key))
                {
                    Debug.LogError("The ResetParameters contains the same key twice");
                }
                else
                {
                    m_DefaultResetParameters.Add(m_ResetParametersList[i].key, m_ResetParametersList[i].value);
                }
            }
        }
    }
}
