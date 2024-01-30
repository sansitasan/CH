using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEditor;
using UnityEngine;

public class CustomEditorWindow : EditorWindow
{
    public enum States
    {
        None,
        Loading,
        LoadComplete
    }
    private States state;
    private Dictionary<SerializedObject, List<SerializedProperty>> Targets = new Dictionary<SerializedObject, List<SerializedProperty>>();
    private List<Editor> SO = new List<Editor>();
    private bool _bFocused;
    private static int _focusedIdx;

    private void Awake()
    {
        InitSO().Forget();
    }

    private async UniTask InitSO()
    {
        state = States.Loading;
        await TResourceManager.Instance.LoadAsyncSO();
        state = States.LoadComplete;
        
        for (int i = 1; i < 6; ++i)
        {
            var so = TResourceManager.Instance.GetScriptableObject(i);
            if (so != null)
                SO.Add(Editor.CreateEditor(so));
        }
    }

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


    [MenuItem("StageEdit/Stage1Data")]
    static void OpenStage1()
    {
        var window = GetWindow<CustomEditorWindow>();
        _focusedIdx = 0;
        window.titleContent.text = "Stage1Data";
    }

    [MenuItem("StageEdit/Stage2Data")]
    static void OpenStage2()
    {
        var window = GetWindow<CustomEditorWindow>();
        _focusedIdx = 1;
        window.titleContent.text = "Stage2Data";
    }

    [MenuItem("StageEdit/Stage3Data")]
    static void OpenStage3()
    {
        var window = GetWindow<CustomEditorWindow>();
        _focusedIdx = 2;
        window.titleContent.text = "Stage3Data";
    }

    [MenuItem("StageEdit/Stage4Data")]
    static void OpenStage4()
    {
        var window = GetWindow<CustomEditorWindow>();
        _focusedIdx = 3;
        window.titleContent.text = "Stage4Data";
    }

    [MenuItem("StageEdit/Stage5Data")]
    static void OpenStage5()
    {
        var window = GetWindow<CustomEditorWindow>();
        _focusedIdx = 4;
        window.titleContent.text = "Stage5Data";
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
        switch (state)
        {
            case States.Loading:
                DrawLoading();
                break;
            case States.LoadComplete:
                DrawSO(_focusedIdx);
                break;
        }

        //EditorGUILayout.LabelField("이동속도, 점프 파워, 점프 차지 기간, 중력 조절", EditorStyles.boldLabel);
    }

    private void DrawLoading()
    {
        EditorGUILayout.LabelField("Loading...");
    }

    private void DrawSO(int idx)
    {
        if (SO.Count > idx)
            SO[idx].OnInspectorGUI();
    }
}
