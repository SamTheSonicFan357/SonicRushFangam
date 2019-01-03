using UnityEngine;
using UnityEditor;
using CharacterController_V2;

namespace InspectorEditor
{
    [CustomEditor(typeof(MovementController))]
    public class MovementControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_skidDecel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_MovementSmoothing"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_AirControl"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_CrouchDisableCollider"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_CrouchSpeed"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}