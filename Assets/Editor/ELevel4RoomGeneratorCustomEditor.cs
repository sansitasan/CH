using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(RoomGenerator))]
public class ELevel4RoomGeneratorCustomEditor : Editor
{
    RoomGenerator m_roomGenerator;

    SerializedProperty prop_roomDuration;
    SerializedProperty prop_fallingObstalcesMax;
    SerializedProperty prop_fallingObstaclesMinDistanceing;
    SerializedProperty prop_fallingObstaclesMinTimeGap;
    SerializedProperty prop_fallingObstaclesRatio;


    private void OnEnable()
    {
        m_roomGenerator = (RoomGenerator)target;

        prop_roomDuration = serializedObject.FindProperty("roomDuration");
        prop_fallingObstalcesMax = serializedObject.FindProperty("fallingObstaclesCountMax");
        prop_fallingObstaclesMinDistanceing = serializedObject.FindProperty("fallingObstaclesMinDistancing");
        prop_fallingObstaclesMinTimeGap = serializedObject.FindProperty("fallingObstclesMinTimeGap");
        prop_fallingObstaclesRatio = serializedObject.FindProperty("fallingObstacleRatio");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(prop_roomDuration, new GUIContent("방 지속시간", "각 방에 대한 정보를 저장"));
        EditorGUILayout.PropertyField(prop_fallingObstalcesMax, new GUIContent("최대 낙하장애물 숫자", "한 번에 생성될 수 있는 최대 낙하물 숫자"));
        EditorGUILayout.PropertyField(prop_fallingObstaclesMinDistanceing, new GUIContent("낙하장애물 거리", "낙하 장애물 간의 최소 거리"));
        EditorGUILayout.PropertyField(prop_fallingObstaclesMinTimeGap, new GUIContent("낙하장애물 생성 빈도", "낙하 장애물을 생성을 시도하는 최소 시간 간격"));
        EditorGUILayout.PropertyField(prop_fallingObstaclesRatio, new GUIContent("낙하장애물 생성량", "방 지속시간 대비 낙하장애물의 생성량 비율. 1일때 최대"));

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("생성", GUILayout.Height(40)))
        {
            // m_roomGenerator.GenerateNewRoomData();
            int roomNum = m_roomGenerator.roomTriggerBundle.childCount;
            string roomName = $"room_{roomNum}";
            var go = new GameObject(roomName, typeof(BoxCollider2D), typeof(PlayerEventTrigger));
            go.transform.SetParent(m_roomGenerator.roomTriggerBundle);

            Vector2 pointA = m_roomGenerator.pointA.position;
            Vector2 pointB = m_roomGenerator.pointB.position;

            Vector2 centor = Vector2.Lerp(pointA, pointB, .5f);
            go.transform.position = centor;

            var box = go.GetComponent<BoxCollider2D>();
            box.size = new Vector2(Mathf.Abs(pointA.x - pointB.x), Mathf.Abs(pointA.y - pointB.y));
            box.isTrigger = true;

            string roomID = $"level4_room_{roomNum}";

            var trigger = go.GetComponent<PlayerEventTrigger>();
            trigger.Init(roomID);

            Level4RoomRuleset data = ScriptableObject.CreateInstance<Level4RoomRuleset>();
            string filePath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(RoomGenerator.ROOM_DATA_PATH + $"{roomID}.asset");

            data.pointA = pointA;
            data.pointB = pointB;
            data.roomDuration = m_roomGenerator.roomDuration;
            data.fallingObstaclesCountMax = m_roomGenerator.fallingObstaclesCountMax;
            data.fallingObstaclesMinDistancing = m_roomGenerator.fallingObstaclesMinDistancing;
            data.fallingObtaclesGenerationTick = m_roomGenerator.fallingObstclesMinTimeGap;
            data.fallingObstacleRatio = m_roomGenerator.fallingObstacleRatio;

            AssetDatabase.CreateAsset(data, filePath);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = data;
        }
        GUILayout.EndHorizontal();
    }
}
