using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestScript))]
public class ScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TestScript t = target as TestScript;
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