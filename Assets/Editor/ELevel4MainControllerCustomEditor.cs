using System.Collections;
using UnityEngine;
using UnityEditor;
using PlasticPipe.PlasticProtocol.Messages;
using Unity.VisualScripting;
using System.Collections.Generic;

[CustomEditor(typeof(Level4MainContoller))]
public class ELevel4MainControllerCustomEditor : Editor
{

    Level4MainContoller m_Level4MainController;

    SerializedProperty prop_TriggerList;
    SerializedProperty prop_RoomRulesetDatas;
    bool openTriggerList = false;
    bool openRoomRulesetDatas = false;


    private void OnEnable()
    {
        m_Level4MainController = (Level4MainContoller)target;

        prop_TriggerList = serializedObject.FindProperty("triggersInLevel");
        prop_RoomRulesetDatas = serializedObject.FindProperty("roomRulesets");
    }


    public override void OnInspectorGUI()
    {
        if (GUILayout.Toggle(openTriggerList, new GUIContent("트리거 리스트 보기")))
        {
            GUILayout.Label("씬에서 관리하는 모든 트리거 리스트. 순서대로 활성화 됨");

            GUILayout.Space(15);

            openTriggerList = true;
            for (int i = 0; i < prop_TriggerList.arraySize; i++)
            {
                EditorGUILayout.PropertyField(prop_TriggerList.GetArrayElementAtIndex(i), GUIContent.none);
                /*
                var trigger = prop_TriggerList.GetArrayElementAtIndex(i);
                trigger.stringValue()
                Debug.Log(trigger.name);
                if (trigger.name.StartsWith("room"))
                {
                    int roomNum = int.Parse(trigger.name.Split('_')[1]);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(prop_TriggerList.GetArrayElementAtIndex(i), GUIContent.none);
                    EditorGUILayout.PropertyField(prop_RoomRulesetDatas.GetArrayElementAtIndex(roomNum));
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                }
                */
            }

            GUILayout.Space(15);

            if (GUILayout.Button(new GUIContent("트리거 불러오기", "이 게임오브젝트의 자식에 있는 모든 트리거를 불러옮"), GUILayout.Height(30)))
            {
                m_Level4MainController.UpdateEventTriggerList();
            }
        }
        else
        {
            openTriggerList = false;
        }
        GUILayout.Space(15);


        if (GUILayout.Toggle(openRoomRulesetDatas, new GUIContent("Room Data 보기")))
        {
            GUILayout.Label("입력된 Room Ruleset Data");

            GUILayout.Space(15);

            openRoomRulesetDatas = true;
            for (int i = 0; i < prop_RoomRulesetDatas.arraySize; i++)
            {
                EditorGUILayout.PropertyField(prop_RoomRulesetDatas.GetArrayElementAtIndex(i), GUIContent.none);
                /*
                var trigger = prop_TriggerList.GetArrayElementAtIndex(i);
                trigger.stringValue()
                Debug.Log(trigger.name);
                if (trigger.name.StartsWith("room"))
                {
                    int roomNum = int.Parse(trigger.name.Split('_')[1]);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(prop_TriggerList.GetArrayElementAtIndex(i), GUIContent.none);
                    EditorGUILayout.PropertyField(prop_RoomRulesetDatas.GetArrayElementAtIndex(roomNum));
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                }
                */
            }

            GUILayout.Space(15);

            if (GUILayout.Button(new GUIContent("데이터 불러오기", "지정된 경로의 RoomRulesetData 들을 모두 가져옴 (지정된 리스트 = 데이터 저장 장소"), GUILayout.Height(30)))
            {
                var loaded = AssetDatabase.LoadAllAssetsAtPath(RoomGenerator.ROOM_DATA_PATH);
                var tmp = new List<Level4RoomRuleset>();
                if (loaded != null)
                {
                    foreach (var r in loaded)
                    {
                        tmp.Add(r.ConvertTo<Level4RoomRuleset>());
                    }
                }
                m_Level4MainController.ReloadRoomRulesetDatas(tmp);
            }
        }
        else
        {
            openRoomRulesetDatas = false;
        }
        // 저장
        serializedObject.ApplyModifiedProperties();
    }
}