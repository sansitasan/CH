using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level5MapGenerationRuleset))]
public class ELevel5MapGenerationRuleset : Editor
{
    Level5MapGenerationRuleset m_ruleset;

    private void OnEnable()
    {
        m_ruleset = (Level5MapGenerationRuleset)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(30);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("청크 불러오기", GUILayout.Height(40)))
        {
            m_ruleset.chuckPrefabs.Clear();

            string PATH = "Assets/Prefabs/Stage5/Chunks/";
            var search = new List<GameObject>();
            var count = Util.TryGetUnityObjectsOfTypeFromPath<GameObject>(PATH, out search);
            Transform emptyChunk = null;

            for (int i = 0; i < count; i++)
            {
                var target = search[i].GetComponent<Level5MapChunk>();
                if (target == null || target.myChuckType == Level5MapChunk.ChunkType.Base)
                    continue;
                else if (target.myChuckType == Level5MapChunk.ChunkType.Empty)
                {
                    emptyChunk = search[i].transform;
                }
                else
                {
                    m_ruleset.chuckPrefabs.Add(search[i].transform);
                }
            }
            m_ruleset.chuckPrefabs.Insert(0, emptyChunk);

            AssetDatabase.SaveAssets();
        }
        GUILayout.EndHorizontal();
    }
}
