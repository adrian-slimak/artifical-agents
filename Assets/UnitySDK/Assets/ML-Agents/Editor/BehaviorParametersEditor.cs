using UnityEngine;
using UnityEditor;
using Barracuda;
using MLAgents.Sensor;

namespace MLAgents
{
    /*
     This code is meant to modify the behavior of the inspector on Agent Components.
    */
    [CustomEditor(typeof(BehaviorParameters))]
    [CanEditMultipleObjects]
    public class BehaviorParametersEditor : Editor
    {
        const float k_TimeBetweenModelReloads = 2f;
        // Time since the last reload of the model
        float m_TimeSinceModelReload;

        public override void OnInspectorGUI()
        {
            var so = serializedObject;
            so.Update();

            // Drawing the Behavior Parameters
            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(so.FindProperty("m_BehaviorName"));
            EditorGUILayout.PropertyField(so.FindProperty("m_BrainParameters"), true);
            EditorGUILayout.PropertyField(so.FindProperty("m_Model"), true);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(so.FindProperty("m_InferenceDevice"), true);
            EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField(so.FindProperty("m_BehaviorType"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Heuristic"), true);
            EditorGUI.indentLevel--;
            so.ApplyModifiedProperties();
        }
    }
}
