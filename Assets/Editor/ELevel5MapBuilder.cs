using UnityEditor;
using UnityEngine;


//[CustomEditor(typeof(Level5MapBuilder))]
public class ELevel5MapBuilder : Editor
{
    Level5MapBuilder m_MapBuilder;

    bool m_showChunkSettings;

    private void OnEnable()
    {
        m_MapBuilder = (Level5MapBuilder)target;
        m_showChunkSettings = false;
        Debug.Log("asd");
    }

    public override void OnInspectorGUI()
    {
        if(EditorGUILayout.ToggleLeft("청크 설정", m_showChunkSettings))
        {
            m_showChunkSettings = true;
        }
        else
        {
            m_showChunkSettings = false;
        }
    }
}