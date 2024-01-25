using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScriptEditorWindow : EditorWindow
{
    private Dictionary<SerializedObject, List<SerializedProperty>> Targets = new Dictionary<SerializedObject, List<SerializedProperty>>();
    private bool _bFocused;

    private void Update()
    {
        if (!_bFocused)
        {
            foreach (var item in Targets)
            {
                item.Key.Update();
            }

            Repaint();
        }
    }

    [MenuItem("ScriptEdit/Test")]
    static void OpenScriptEdit()
    {
        var window = GetWindow<ScriptEditorWindow>();
        window.titleContent.text = "Script";
        window.Show();
    }

    private void OnFocus()
    {
        _bFocused = true;
    }

    private void OnLostFocus()
    {
        _bFocused = false;
    }

    private void OnGUI()
    {
        Targets.Clear();

        var allCustom = FindObjectOfType<TestScript>();
        
        if (allCustom != null)
        {
            var so = new SerializedObject(allCustom);
            var props = new List<SerializedProperty>()
            {
                so.FindProperty("_scriptName")
            };

            Targets.TryAdd(so, props);
        }

        EditorGUILayout.LabelField("스크립트 선택", EditorStyles.boldLabel);
        foreach (var pair in Targets)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField(pair.Key.targetObject.name, EditorStyles.boldLabel);
            ++EditorGUI.indentLevel;
            foreach (var prop in pair.Value)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(prop, GUIContent.none);
                GUI.enabled = true;
            }
            --EditorGUI.indentLevel;

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            if (EditorGUI.EndChangeCheck())
            {
                pair.Key.ApplyModifiedProperties();
            }
        }
    }
}
