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
    private List<Editor> SO = new List<Editor>();
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