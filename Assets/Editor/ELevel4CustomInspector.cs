using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level4MainContoller))]
public class ELevel4CustomInspector : Editor
{
    Level4MainContoller m_MainContoller;
    SerializedProperty rullset_datas;

    private void OnEnable()
    {
        m_MainContoller = (Level4MainContoller)target;
        rullset_datas = serializedObject.FindProperty("rullsetDatas");
    }

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        GUILayout.Label("-----");

        EditorGUILayout.PropertyField(rullset_datas, new GUIContent("방 데이터", "각 방에 대한 정보를 저장"));
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("생성", GUILayout.Height(40)))
        {
            m_MainContoller.AddNewRoom_Editor();
        }
        GUILayout.EndHorizontal();
    }

}
