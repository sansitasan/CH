using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEditor;
using UnityEngine;

public class CustomEditorWindow : EditorWindow
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


    [MenuItem("StageEdit/Stage1")]
    static void OpenStage1()
    {
        var window = GetWindow<CustomEditorWindow>();
        window.titleContent.text = "Stage1";
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

        var allCustom = FindObjectOfType<Stage1Model>();
        Rigidbody2D rb = allCustom?.GetComponent<Rigidbody2D>();
        if (allCustom != null)
        {
            var so = new SerializedObject(allCustom);
            var props = new List<SerializedProperty>()
            {
                so.FindProperty("_speed"),
                so.FindProperty("_chargePower"),
                so.FindProperty("_chargeTime"),
                so.FindProperty("_gravityScale")
            };

            Targets.TryAdd(so, props);
        }

        EditorGUILayout.LabelField("이동속도, 점프 파워, 점프 차지 기간, 중력 조절", EditorStyles.boldLabel);
        foreach (var pair in Targets)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField(pair.Key.targetObject.name, EditorStyles.boldLabel);
            ++EditorGUI.indentLevel;
            foreach (var prop in pair.Value)
            {
                EditorGUILayout.PropertyField(prop);
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
