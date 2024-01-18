using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BehaviourTree))]
public class Test : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);

        GUI.Box(position, GUIContent.none, GUI.skin.window);

        EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        ++EditorGUI.indentLevel;

        var rt = new Rect(position.x, position.y + GUIStyle.none.CalcSize(label).y + 2, position.width, 16);
        foreach(SerializedProperty prop in property)
        {
            EditorGUI.PropertyField(rt, prop);
            rt.y += 18;
        }

        GUI.color = Color.white;

        --EditorGUI.indentLevel;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int cnt = 0;
        foreach(var prop in property)
        {
            ++cnt;
        }

        return EditorGUIUtility.singleLineHeight * (cnt + 1) + 6;
    }
}
