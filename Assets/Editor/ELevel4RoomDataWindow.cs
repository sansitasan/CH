using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets.Scripts.LE_CORL.LevelContollers.Level4;

public class ELevel4RoomDataWindow : EditorWindow
{
    static void Init()
    {
        var win = GetWindow<ELevel4RoomDataWindow>(typeof(Level4RoomRuleSet));
        win.Show();
    }
}
