using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace UPC
{
    /// <summary>
    /// PropertyDrawer for ResetParameters. Defines how ResetParameters are displayed in the
    /// Inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(ResetParameters))]
    public class ResetParameterDrawer : PropertyDrawer
    {
        ResetParameters m_Parameters;
        const float k_LineHeight = 17f;
        const string k_NewKeyPrefix = "Param-";
        private bool _Foldout;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            LazyInitializeParameters(property);
            if(_Foldout)
                return (m_Parameters.m_DefaultResetParameters.Count + 2) * k_LineHeight;
            return k_LineHeight;
        }

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LazyInitializeParameters(property);

            var foldoutRect = position;
            foldoutRect.width -= 2 * k_LineHeight;
            foldoutRect.height = k_LineHeight;
            EditorGUI.BeginChangeCheck();
            _Foldout = EditorGUI.Foldout(foldoutRect, _Foldout, label, true);
            if (EditorGUI.EndChangeCheck())
                EditorPrefs.SetBool(label.text, _Foldout);

            if (!_Foldout)
                return;

            position.height = k_LineHeight;
            EditorGUI.LabelField(position, label);
            position.y += k_LineHeight;
            var width = position.width / 2 - 24;
            var keyRect = new Rect(position.x + 20, position.y, width, position.height);
            var valueRect = new Rect(position.x + width + 30, position.y, width, position.height);
            DrawAddRemoveButtons(keyRect, valueRect);

            EditorGUI.BeginProperty(position, label, property);
            foreach (var parameter in m_Parameters.m_DefaultResetParameters)
            {
                var key = parameter.Key;
                var value = parameter.Value;
                keyRect.y += k_LineHeight;
                valueRect.y += k_LineHeight;
                EditorGUI.BeginChangeCheck();
                var newKey = EditorGUI.TextField(keyRect, key);
                if (EditorGUI.EndChangeCheck())
                {
                    MarkSceneAsDirty();
                    try
                    {
                        m_Parameters.m_DefaultResetParameters.Remove(key);
                        m_Parameters.m_DefaultResetParameters.Add(newKey, value);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    break;
                }

                EditorGUI.BeginChangeCheck();
                value = EditorGUI.FloatField(valueRect, value);
                if (EditorGUI.EndChangeCheck())
                {
                    MarkSceneAsDirty();
                    m_Parameters[key] = value;
                    break;
                }
            }
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Draws the Add and Remove buttons.
        /// </summary>
        /// <param name="addRect">The rectangle for the Add New button.</param>
        /// <param name="removeRect">The rectangle for the Remove Last button.</param>
        void DrawAddRemoveButtons(Rect addRect, Rect removeRect)
        {
            // This is the Add button
            if (m_Parameters.m_DefaultResetParameters.Count == 0)
            {
                addRect.width *= 2;
            }
            if (GUI.Button(addRect,
                new GUIContent("Add New", "Add a new item to the default reset parameters"),
                EditorStyles.miniButton))
            {
                MarkSceneAsDirty();
                AddParameter();
            }

            // If there are no items in the ResetParameters, Hide the Remove button
            if (m_Parameters.m_DefaultResetParameters.Count == 0)
            {
                return;
            }
            // This is the Remove button
            if (GUI.Button(removeRect,
                new GUIContent(
                    "Remove Last", "Remove the last item from the default reset parameters"),
                EditorStyles.miniButton))
            {
                MarkSceneAsDirty();
                RemoveLastParameter();
            }
        }

        /// <summary>
        /// Signals that the property has been modified and requires the scene to be saved for
        /// the changes to persist. Only works when the Editor is not playing.
        /// </summary>
        static void MarkSceneAsDirty()
        {
            if (!EditorApplication.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }

        /// <summary>
        /// Ensures that the state of the Drawer is synchronized with the property.
        /// </summary>
        /// <param name="property">The SerializedProperty of the ResetParameters
        /// to make the custom GUI for.</param>
        void LazyInitializeParameters(SerializedProperty property)
        {
            if (m_Parameters != null)
            {
                return;
            }
            var target = property.serializedObject.targetObject;
            m_Parameters = fieldInfo.GetValue(target) as ResetParameters;
            if (m_Parameters == null)
            {
                m_Parameters = new ResetParameters();
                fieldInfo.SetValue(target, m_Parameters);
            }
        }

        /// <summary>
        /// Removes the last ResetParameter from the ResetParameters
        /// </summary>
        void RemoveLastParameter()
        {
            if (m_Parameters.m_DefaultResetParameters.Count > 0)
            {
                var key = m_Parameters.m_DefaultResetParameters.Keys.ToList()[m_Parameters.m_DefaultResetParameters.Count - 1];
                m_Parameters.m_DefaultResetParameters.Remove(key);
            }
        }

        /// <summary>
        /// Adds a new ResetParameter to the ResetParameters with a default name.
        /// </summary>
        void AddParameter()
        {
            var key = k_NewKeyPrefix + m_Parameters.m_DefaultResetParameters.Count;
            var value = default(float);
            try
            {
                m_Parameters.m_DefaultResetParameters.Add(key, value);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}
