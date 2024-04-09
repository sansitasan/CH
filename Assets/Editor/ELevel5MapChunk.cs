using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level5MapChunk))]
public class ELevel5MapChunk : Editor
{
    Level5MapChunk m_mapChunk;
    SerializedProperty type;
    SerializedProperty prio;

    private void OnEnable()
    {
        m_mapChunk = (Level5MapChunk)target;
        type = serializedObject.FindProperty("myChuckType");
        prio = serializedObject.FindProperty("priority");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(type, new GUIContent("청크 타입", "이 청크의 타입"));
        m_mapChunk.myChuckType = (Level5MapChunk.ChunkType)type.enumValueIndex;


        EditorGUILayout.PropertyField(prio, new GUIContent("청크 생성 빈도", "이 청크의 생성 빈도"));
        m_mapChunk.priority = prio.intValue;

        EditorGUILayout.Space(30);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("코인 정렬하기", GUILayout.Height(40)))
        {
            m_mapChunk.SortCoins();
        }
        GUILayout.EndHorizontal();
    }
}
