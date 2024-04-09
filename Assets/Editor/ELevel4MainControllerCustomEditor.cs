using System.Collections;
using UnityEngine;
using UnityEditor;
using PlasticPipe.PlasticProtocol.Messages;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;

//[CustomEditor(typeof(Level4MainContoller))]
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
    }
}