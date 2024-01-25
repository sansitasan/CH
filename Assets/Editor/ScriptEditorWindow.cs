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

        var t = FindObjectOfType<TestScript>();
        
        if (t != null)
        {
            var so = new SerializedObject(t);
            var props = new List<SerializedProperty>()
            {
                so.FindProperty("_scriptName"),
                so.FindProperty("_speed")
            };

            Targets.TryAdd(so, props);
        }

        EditorGUILayout.LabelField("스크립트 선택", EditorStyles.boldLabel);
        foreach (var pair in Targets)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField(pair.Key.targetObject.name, EditorStyles.boldLabel);
            //들여쓰기
            ++EditorGUI.indentLevel;

            GUI.enabled = false;
            EditorGUILayout.PropertyField(pair.Value[0], GUIContent.none);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(pair.Value[1], GUIContent.none);

            --EditorGUI.indentLevel;

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            if (EditorGUI.EndChangeCheck())
            {
                pair.Key.ApplyModifiedProperties();
            }
        }

        switch (t.State)
        {
            case TestScript.TSStates.None:
                DrawInitButton(t);
                break;

            case TestScript.TSStates.Loading:
                DrawLoading();
                break;

            case TestScript.TSStates.LoadComplete:
                DrawSelectButton(t);
                DrawClearButton(t);
                break;

            case TestScript.TSStates.SelectComplete:
                DrawTestButton(t);
                DrawClearButton(t);
                break;

            case TestScript.TSStates.Complete:
                DrawClearButton(t);
                break;
        }
    }

    private void DrawInitButton(TestScript t)
    {
        if (GUILayout.Button("Initialize"))
        {
            t.InitTest();
        }
    }

    private void DrawLoading()
    {
        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();
        GUILayout.Label("Loading Asset...");

        GUILayout.EndHorizontal();
    }

    private void DrawSelectButton(TestScript t)
    {
        List<string> names = TResourceManager.Instance.GetScriptName();

        foreach (var name in names)
        {
            if (GUILayout.Button(name))
            {
                t.GetScript(name);
            }
        }
    }

    private void DrawTestButton(TestScript t)
    {
        if (GUILayout.Button("Next"))
        {
            t.Click();
        }
    }

    private void DrawClearButton(TestScript t)
    {
        if (GUILayout.Button("Clear"))
        {
            t.Clear();
        }
    }
}
