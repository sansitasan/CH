using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FallingObstacleObject))]
public class ELevel4FallingObstacleCustomEditor : Editor
{
    SerializedProperty prop_MarkerDuration;
    SerializedProperty prop_FallingStartingHeight;
    SerializedProperty prop_FallingSpeed; // animation curve
    SerializedProperty prop_FallingDuration;
    SerializedProperty prop_TriggerCheckinAmount;
    SerializedProperty prop_TriggerCheckingRadius;
    SerializedProperty prop_FalledObjectDuration;
    SerializedProperty prop_FalledDisableAnim;

    private void OnEnable()
    {
        prop_MarkerDuration = serializedObject.FindProperty("marker_Duration");
        prop_FallingStartingHeight = serializedObject.FindProperty("falling_StartingHeight");
        prop_FallingSpeed = serializedObject.FindProperty("falling_Speed");
        prop_FallingDuration = serializedObject.FindProperty("falling_Duration");
        prop_TriggerCheckinAmount = serializedObject.FindProperty("falling_TriggerCheckingAmount");
        prop_TriggerCheckingRadius = serializedObject.FindProperty("falling_TriggerCheckingRadius");
        prop_FalledObjectDuration = serializedObject.FindProperty("falled_ObjectDuration");
        prop_FalledDisableAnim = serializedObject.FindProperty("falled_DisableAnimDuration");
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label("+ 떨어지기 전");        
        EditorGUI.indentLevel = 1;
        EditorGUILayout.PropertyField(prop_MarkerDuration, new GUIContent("마커 지속시간", "떨어질 위치를 표시하는 마커의 지속시간"));
        EditorGUI.indentLevel = 0;

        GUILayout.Space(15);

        GUILayout.Label("+ 떨어지는 애니메이션");
        EditorGUI.indentLevel = 1;
        EditorGUILayout.PropertyField(prop_FallingStartingHeight, new GUIContent("낙하장애물 시작 높이"));
        prop_FallingStartingHeight.floatValue = GUILayout.HorizontalSlider(prop_FallingStartingHeight.floatValue, 0, 5, GUILayout.Height(15));
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(prop_FallingSpeed, new GUIContent("시간-낙하 그래프", "총 낙하 시간 대비 낙하 거리를 그래프로 표현"));
        EditorGUILayout.PropertyField(prop_FallingDuration, new GUIContent("최종 낙하 시간", "최종 낙하까지 걸리는 시간"));
        EditorGUI.indentLevel = 0;

        GUILayout.Space(15);

        GUILayout.Label("+ 판정");
        EditorGUI.indentLevel = 1;
        EditorGUILayout.PropertyField(prop_TriggerCheckinAmount, new GUIContent("판정 시간 비율", "최종 낙하 시점으로 부터 낙하 시작 시점까지를 1로 했을 때, 플레이어를 판정할 시간 비율"));
        prop_TriggerCheckinAmount.floatValue = GUILayout.HorizontalSlider(prop_TriggerCheckinAmount.floatValue, 0, 1, GUILayout.Height(15));
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(prop_TriggerCheckingRadius, new GUIContent("판정 범위", "낙하 중심 지점으로 부터 플레이어를 탐지할 반지름"));
        EditorGUI.indentLevel = 0;

        GUILayout.Space(15);

        GUILayout.Label("+ 떨어진 후");
        EditorGUI.indentLevel = 1;
        EditorGUILayout.PropertyField(prop_FalledObjectDuration, new GUIContent("낙하장애물 지속시간", "낙하 후 장애물로서의 역할을 지속하는 시간"));
        EditorGUILayout.PropertyField(prop_FalledDisableAnim, new GUIContent("낙하장애물 페이드아웃", "장애물로서의 역할을 마치고 페이드아웃되는 시간"));
        EditorGUI.indentLevel = 0;
        GUILayout.Space(15);


        serializedObject.ApplyModifiedProperties();
    }
}