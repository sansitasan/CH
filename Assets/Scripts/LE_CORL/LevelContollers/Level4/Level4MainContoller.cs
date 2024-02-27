using Assets.Scripts.LE_CORL.LevelContollers.Level4;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;


public class Level4MainContoller : MonoBehaviour
{
    [SerializeField] Transform roomEditingPointersParent;

    [SerializeField] List<Level4RoomRuleSet> rullsetDatas;


    [ContextMenu("asd")]
    void AddNewRoom()
    {
        var ruleset = new Level4RoomRuleSet();

        if(rullsetDatas == null) rullsetDatas = new List<Level4RoomRuleSet> ();
        
        ruleset.pointEnter = roomEditingPointersParent.GetChild(0).transform.position;
        ruleset.pointA = roomEditingPointersParent.GetChild(1).transform.position;
        ruleset.pointB = roomEditingPointersParent.GetChild(2).transform.position;
        ruleset.pointExit = roomEditingPointersParent.GetChild(3).transform.position;

        rullsetDatas.Add(ruleset);  
    }

    public void AddNewRoom_Editor() => AddNewRoom();



    private void OnDrawGizmosSelected()
    {
        Vector2 pointA = roomEditingPointersParent.GetChild(1).transform.position;
        Vector2 pointB = roomEditingPointersParent.GetChild(2).transform.position;
        Vector2 aX_bY = new Vector2(pointA.x, pointB.y);
        Vector2 bX_aY = new Vector2(pointB.x, pointA.y);

        Debug.DrawLine(pointA, aX_bY, Color.red);
        Debug.DrawLine(aX_bY, pointB, Color.red);
        Debug.DrawLine(pointB, bX_aY, Color.red);
        Debug.DrawLine(bX_aY, pointA, Color.red);
    }
}