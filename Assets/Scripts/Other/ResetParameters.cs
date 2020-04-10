using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;


[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class ParameterAttribute : PropertyAttribute
{
    public readonly string Name;

    public ParameterAttribute(string name) { this.Name = name; }
}


[Serializable]
public class EnvironmentParameters : ISerializationCallbackReceiver
{
    [HideInInspector]
    public Dictionary<string, float> m_DefaultEnvParams;
    Dictionary<string, float> m_CustomEnvParams;

    public EnvironmentParameters()
    {
        m_DefaultEnvParams = new Dictionary<string, float>();
        m_CustomEnvParams = new Dictionary<string, float>();
    }

    public void LoadEnvParams(object _object, string _prefix = "")
    {
        FieldInfo[] objectFields = _object.GetType().GetFields();

        foreach (FieldInfo field in objectFields)
        {
            ParameterAttribute attribute = Attribute.GetCustomAttribute(field, typeof(ParameterAttribute)) as ParameterAttribute;

            if (attribute != null)
            {
                string paramName = _prefix==""?attribute.Name:$"{_prefix}_{attribute.Name}";
                float? paramValue = this[paramName];

                if (paramValue != null)
                {
                    if (field.FieldType == typeof(bool))
                        field.SetValue(_object, paramValue > 0);
                    else if (field.FieldType == typeof(int))
                        field.SetValue(_object, (int) paramValue);
                    else if (field.FieldType == typeof(float))
                        field.SetValue(_object, paramValue);
                }
            }
        }
    }

    public float? this[string name]
    {
        set { m_DefaultEnvParams[name] = (float)value; }
        get
        {
            if (m_CustomEnvParams.ContainsKey(name))
                return m_CustomEnvParams[name];
            else
            if (m_DefaultEnvParams.ContainsKey(name))
                return m_DefaultEnvParams[name];
            //else
            //    Debug.LogWarning($"Reset parameter '{name}' not found.");
            return null;
        }
    }

    public void SetCustomParameters(Dictionary<string, float> customParameters)
    {
        m_CustomEnvParams = customParameters;
    }

    void UpdateResetParameters()
    {
        m_ResetParametersList.Clear();
        foreach (var parameter in m_DefaultEnvParams)
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
        m_DefaultEnvParams.Clear();

        for (var i = 0; i < m_ResetParametersList.Count; i++)
        {
            if (m_DefaultEnvParams.ContainsKey(m_ResetParametersList[i].key))
            {
                Debug.LogError("The ResetParameters contains the same key twice");
            }
            else
            {
                m_DefaultEnvParams.Add(m_ResetParametersList[i].key, m_ResetParametersList[i].value);
            }
        }
    }
}
