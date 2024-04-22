using Cinemachine;
using log4net.Repository.Hierarchy;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(RoomGenerator))]
public class ELevel4RoomGeneratorCustomEditor : Editor
{
    RoomGenerator m_roomGenerator;

    SerializedProperty prop_roomDuration;
    SerializedProperty prop_MaxCount;
    SerializedProperty prop_MinDistanceing;
    SerializedProperty prop_GenerationTick;
    SerializedProperty prop_GenerationRatio;
    SerializedProperty prop_GenerationPrio;
    SerializedProperty prop_isEscapeable;

    string roomObjectNameFormat = "level4_room_{0}";

    private void OnEnable()
    {
        m_roomGenerator = (RoomGenerator)target;

        prop_roomDuration = serializedObject.FindProperty("roomDuration");
        prop_MaxCount = serializedObject.FindProperty("generationMax");
        prop_MinDistanceing = serializedObject.FindProperty("minDistancing");
        prop_GenerationTick = serializedObject.FindProperty("generationTick");
        prop_GenerationRatio = serializedObject.FindProperty("generationRatio");
        prop_GenerationPrio = serializedObject.FindProperty("generationPrio");
        prop_isEscapeable = serializedObject.FindProperty("isEscapeableRoom");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(prop_roomDuration, new GUIContent("방 지속시간", "각 방에 대한 정보를 저장"));
        m_roomGenerator.roomDuration = prop_roomDuration.intValue;
        if (prop_roomDuration.intValue <= 0)
        {
            m_roomGenerator.roomDuration = 1;
            Debug.LogWarning("방 지속시간이 너무 짧습니다: \n최솟값= 1");
        }

        EditorGUILayout.PropertyField(prop_MaxCount, new GUIContent("최대 낙하장애물 숫자", "한 번에 생성될 수 있는 최대 낙하물 숫자"));
        m_roomGenerator.generationMax = prop_MaxCount.intValue;
        if(prop_MaxCount.intValue <= 0)
        {
            m_roomGenerator.generationMax = 10;
            Debug.LogWarning("낙하 장애무르이 생성 개수가 너무 적습니다: \n최솟값= 1, 권장값= 10 이상");
        }

        EditorGUILayout.PropertyField(prop_MinDistanceing, new GUIContent("낙하장애물 거리", "낙하 장애물 간의 최소 거리"));
        m_roomGenerator.minDistancing = prop_MinDistanceing.floatValue;
        if(prop_MinDistanceing.floatValue <= 0)
            m_roomGenerator.minDistancing = .01f;

        EditorGUILayout.PropertyField(prop_GenerationTick, new GUIContent("낙하장애물 생성 간격", "낙하 장애물을 생성을 시도하는 최소 시간 간격"));
        m_roomGenerator.generationTick = prop_GenerationTick.floatValue;
        if( prop_GenerationTick.floatValue <= 0) 
            m_roomGenerator.generationTick = .1f;

        EditorGUILayout.PropertyField(prop_GenerationRatio, new GUIContent("낙하장애물 전체 생성 계수", "방 지속시간 대 낙하장애물의 생성량 비율. 1일때 최대"));
        m_roomGenerator.generationRatio = prop_GenerationRatio.animationCurveValue;

        EditorGUILayout.PropertyField(prop_GenerationPrio, new GUIContent("낙하장애물 별 생성 계수", "각 낙하 장애물 별 생성 상대 비율"));
        for (int i = 0; i < m_roomGenerator.generationPrio.Length; i++)
        {
            m_roomGenerator.generationPrio[i] = prop_GenerationPrio.GetArrayElementAtIndex(i).intValue;

        }
        EditorGUILayout.PropertyField(prop_isEscapeable, new GUIContent("탈출 가능 여부", "방을 중간에 탈출 할 수 있는지 여부"));
        m_roomGenerator.isEscapeableRoom = prop_isEscapeable.boolValue;

        EditorGUILayout.Space(30);


        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("생성", GUILayout.Height(40)))
        {
            string roomID = string.Format(roomObjectNameFormat, m_roomGenerator.roomTriggerBundle.childCount);

            // 데이터 생성
            Level4RoomRuleset data =ScriptableObject.CreateInstance<Level4RoomRuleset>();
            string filePath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(Path.Combine(RoomGenerator.ROOM_DATA_PATH, $"{roomID}.asset"));

            data.pointA = m_roomGenerator.pointA.position;
            data.pointB = m_roomGenerator.pointB.position;
            data.roomDuration = m_roomGenerator.roomDuration;
            data.generationMax = m_roomGenerator.generationMax;
            data.minDistancing = m_roomGenerator.minDistancing;
            data.generationTick = m_roomGenerator.generationTick;
            data.generationRatio = m_roomGenerator.generationRatio;
            data.generationPrio = m_roomGenerator.generationPrio;
            data.shuffleSeed = m_roomGenerator.shuffleSeed;
            data.isEscapeableRoom = m_roomGenerator.isEscapeableRoom;

            AssetDatabase.CreateAsset(data, filePath);
            AssetDatabase.SaveAssets();
            GenerateNewTriggerObject(data);


            SetDefaultValues();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = data;
        }
        GUILayout.EndHorizontal();



        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("싱크 맞추기 (데이터 기준)", "데이터 파일을 재정렬하고 그에 맞게 게임 오브젝트를 다시 생성"), GUILayout.Height(40)))
        {
            Transform parent = m_roomGenerator.roomTriggerBundle.parent;
            DestroyImmediate(m_roomGenerator.roomTriggerBundle.gameObject);
            var newBundle = new GameObject("Rooms").transform;
            newBundle.parent = parent;
            newBundle.localPosition = Vector3.zero;
            m_roomGenerator.roomTriggerBundle = newBundle;


            // var tmp = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(RoomGenerator.ROOM_DATA_PATH);
            var search = new List<Level4RoomRuleset>();
            var count = Util.TryGetUnityObjectsOfTypeFromPath<Level4RoomRuleset>(RoomGenerator.ROOM_DATA_PATH, out search);

            for (int i = 0; i < count; i++)
            {
                var target = search[i]; // as Level4RoomRuleset;
                var old = AssetDatabase.GetAssetPath(target.GetInstanceID());
                string newName = string.Format(roomObjectNameFormat, i.ToString());
                Debug.Log("old: " + old + "\n new: " + newName);

                AssetDatabase.RenameAsset(old, newName);
                GenerateNewTriggerObject(target);
            }
            AssetDatabase.SaveAssets();

            SetDefaultValues();

            Selection.activeObject = m_roomGenerator.roomTriggerBundle;
        }
        GUILayout.EndHorizontal();
    }

    void SetDefaultValues()
    {
        m_roomGenerator.roomDuration = 30;
        m_roomGenerator.generationMax = 10;
        m_roomGenerator.minDistancing = 1;
        m_roomGenerator.generationTick = .2f;
        m_roomGenerator.generationRatio = AnimationCurve.Linear(0, 0, 1, 1);
        m_roomGenerator.generationPrio = new int[8]
        {
            100, 50, 10, 5, 0, 0, 0, 0
        };
        m_roomGenerator.shuffleSeed = UnityEngine.Random.Range(-1000, 1000);
        m_roomGenerator.isEscapeableRoom = false;
    }

    void GenerateNewTriggerObject(Level4RoomRuleset data)
    {
        Debug.Log(data.name);
        string roomName = string.Format(roomObjectNameFormat, data.name.Split('_')[2]);
        var go = new GameObject(roomName, typeof(BoxCollider2D), typeof(PlayerEventTrigger), typeof(Level4RoomController));
        go.transform.SetParent(m_roomGenerator.roomTriggerBundle);

        Vector2 pointA = data.pointA;
        Vector2 pointB = data.pointB;

        Vector2 centor = Vector2.Lerp(pointA, pointB, .5f);
        go.transform.position = centor;

        var box = go.GetComponent<BoxCollider2D>();
        box.size = new Vector2(Mathf.Abs(pointA.x - pointB.x) + .5f, Mathf.Abs(pointA.y - pointB.y));
        box.isTrigger = true;

        var trigger = go.GetComponent<PlayerEventTrigger>();
        trigger.Init(roomName);

        var roomCtrl = go.GetComponent<Level4RoomController>();

        var vCam = new GameObject(roomName + " vCam").AddComponent<CinemachineVirtualCamera>();
        vCam.transform.parent = trigger.transform;
        vCam.transform.localPosition = new Vector3(0, .5f, -10);
        vCam.gameObject.SetActive(false);

        var respawnPos = new GameObject(roomName + " respawn");
        respawnPos.transform.parent = trigger.transform;
        respawnPos.transform.localPosition = new Vector3(13, 0, 0);

        roomCtrl.SetRoomCtrl(data, trigger, vCam, respawnPos.transform);
    }
}
